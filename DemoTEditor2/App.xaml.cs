
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using TEditor;
using TEditor.Abstractions;

namespace DemoTEditor2
{
    public class TEditorHtmlView : StackLayout
    {
        //create bindable property, html
        public string Html { get; set; }
        WebView _displayWebView;
        public TEditorHtmlView()
        {
            this.Orientation = StackOrientation.Vertical;
            this.Children.Add(new Button
            {
                Text = "HTML Editor",
                HeightRequest = 100,
                Command = new Command(async (obj) =>
                {
                    await ShowTEditor();
                })
            });
            _displayWebView = new WebView() { HeightRequest = 500 };
            this.Children.Add(_displayWebView);
        }

        async Task ShowTEditor()
        {
            TEditorResponse response = await CrossTEditor.Current.ShowTEditor("<!-- This is an HTML comment --><p>This is a test of the <strong style=\"font-size:20px\">TEditor</strong> by <a title=\"XAM consulting\" href=\"http://www.xam-consulting.com\">XAM consulting</a></p>");
            if (response.IsSave)
            {
                if (response.HTML != null)
                {
                    _displayWebView.Source = new HtmlWebViewSource() { Html = response.HTML };
                }
            }
        }
    }

   public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            this.MainPage = new NavigationPage(new ContentPage { Content = new TEditorHtmlView(), BackgroundColor = Color.White });
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
