using System;
using System.Linq;
using Android.App;
using Android.Widget;
using System.Collections.Generic;
using Android.Views;
using Android.Content;
using Android.Support.V7.App;
using TEditor.Abstractions;
using System.Threading.Tasks;
using TEditor.Droid;

namespace TEditor
{
    [Activity(Label = "TEditorActivity",
         WindowSoftInputMode = Android.Views.SoftInput.AdjustResize | Android.Views.SoftInput.StateVisible,
         Theme = "@style/Theme.AppCompat.NoActionBar.FullScreen")]
    public class TEditorActivity : AppCompatActivity
    {
        const int ToolbarFixHeight = 60;
        TEditorWebView _editorWebView;
        LinearLayoutDetectsSoftKeyboard _rootLayout;
        LinearLayout _toolbarLayout;
        Android.Support.V7.Widget.Toolbar _topToolBar;

        private IList<string> _macros;
        private IList<string> _macrosValues;

        public static Action<bool, string> SetOutput { get; set; }

        private static Android.Graphics.Color _keysColor = Android.Graphics.Color.ParseColor("#FAFAFA");

        protected override void OnCreate(Android.OS.Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.TEditorActivity);

            _topToolBar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.TopToolbar);

            _topToolBar.Title = CrossTEditor.PageTitle;

            SetSupportActionBar(_topToolBar);

            if (SupportActionBar != null)
            {
                SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                SupportActionBar.SetDisplayShowHomeEnabled(true);
                SupportActionBar.SetDisplayShowCustomEnabled(true);

            }

            _rootLayout = FindViewById<LinearLayoutDetectsSoftKeyboard>(Resource.Id.RootRelativeLayout);
            _editorWebView = FindViewById<TEditorWebView>(Resource.Id.EditorWebView);
            _toolbarLayout = FindViewById<LinearLayout>(Resource.Id.ToolbarLayout);

            _rootLayout.onKeyboardShown += HandleSoftKeyboardShwon;
            _editorWebView.SetOnCreateContextMenuListener(this);

            BuildToolbar();

            string htmlString = Intent.GetStringExtra("HTMLString") ?? "<p></p>";
            _editorWebView.SetHTML(htmlString);

            bool autoFocusInput = Intent.GetBooleanExtra("AutoFocusInput", false);
            _editorWebView.SetAutoFocusInput(autoFocusInput);

            _macros = Intent.GetStringArrayListExtra("macroKeys");
            _macrosValues = Intent.GetStringArrayListExtra("macroValues");
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.TopToolbarMenu, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.TitleFormatted?.ToString() == "Save")
            {
                Task<string> action = _editorWebView.GetHTML();

                action.ContinueWith(x =>
                {
                    string html = x.Result;
                    SetOutput?.Invoke(true, html);

                    Finish();
                });
            }
            else if (item.TitleFormatted?.ToString() == "Macros")
            {
                PromptMacros();
            }

            return base.OnOptionsItemSelected(item);
        }

        private void PromptMacros()
        {
            var builderSingle = new Android.App.AlertDialog.Builder(this);

            builderSingle.SetTitle("Select Macro");

            var negative = new EventHandler<DialogClickEventArgs>(
                (s, args) =>
                {

                });

            var positive = new EventHandler<DialogClickEventArgs>(
                async (s, args) =>
                {
                    if (_macrosValues != null && _macrosValues.Count > args.Which)
                    {
                        var value = _macrosValues[args.Which];
                        var currentHtml = await _editorWebView.GetHTML();
                        _editorWebView.SetHTML(currentHtml + value + "<br/>");
                    }
                });

            builderSingle.SetItems(_macros.ToArray(), positive);
            builderSingle.SetNegativeButton("Cancel", negative);

            var adialog = builderSingle.Create();

            adialog.Show();
        }

        public override bool OnSupportNavigateUp()
        {
            OnBackPressed();
            SetOutput?.Invoke(false, null);
            return true;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            _rootLayout.onKeyboardShown -= HandleSoftKeyboardShwon;
        }

        public void BuildToolbar()
        {
            ToolbarBuilder builder = TEditorImplementation.ToolbarBuilder;
            if (builder == null)
                builder = new ToolbarBuilder().AddAll();

            foreach (var item in builder)
            {
                ImageButton imagebutton = new ImageButton(this);
                imagebutton.SetBackgroundColor(_keysColor);
                //
                //
                imagebutton.Click += (sender, e) =>
                {
                    item.ClickFunc.Invoke(_editorWebView.RichTextEditor);
                };
                string imagename = item.ImagePath.Split('.')[0];
                int resourceId = (int)typeof(Resource.Drawable).GetField(imagename).GetValue(null);
                imagebutton.SetImageResource(resourceId);
                var toolbarItems = FindViewById<LinearLayout>(Resource.Id.ToolbarItemsLayout);
                toolbarItems.AddView(imagebutton);
            }
        }

        public void HandleSoftKeyboardShwon(bool shown, int newHeight)
        {
            if (shown)
            {
                _toolbarLayout.Visibility = Android.Views.ViewStates.Visible;
                int widthSpec = View.MeasureSpec.MakeMeasureSpec(0, MeasureSpecMode.Unspecified);
                int heightSpec = View.MeasureSpec.MakeMeasureSpec(0, MeasureSpecMode.Unspecified);
                _toolbarLayout.Measure(widthSpec, heightSpec);
                int toolbarHeight = _toolbarLayout.MeasuredHeight == 0 ? (int)(ToolbarFixHeight * Resources.DisplayMetrics.Density) : _toolbarLayout.MeasuredHeight;
                int topToolbarHeight = _topToolBar.MeasuredHeight == 0 ? (int)(ToolbarFixHeight * Resources.DisplayMetrics.Density) : _topToolBar.MeasuredHeight;
                int editorHeight = newHeight - toolbarHeight - topToolbarHeight;

                _editorWebView.LayoutParameters.Height = editorHeight;
                _editorWebView.LayoutParameters.Width = LinearLayout.LayoutParams.MatchParent;
                _editorWebView.RequestLayout();
            }
            else
            {
                if (newHeight != 0)
                {
                    _toolbarLayout.Visibility = Android.Views.ViewStates.Invisible;
                    _editorWebView.LayoutParameters = new LinearLayout.LayoutParams(-1, -1);

                }
            }
        }

    }
}

