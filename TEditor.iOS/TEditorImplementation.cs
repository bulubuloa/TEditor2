using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TEditor.Abstractions;
using UIKit;

namespace TEditor
{
    public class TEditorImplementation : BaseTEditor
    {
        public override Task<TEditorResponse> ShowTEditor(string html, ToolbarBuilder toolbarBuilder = null, string title = null, bool autoFocusInput = false, Dictionary<string, string> macros = null)
        {
            // TODO: HTML input must be not null
            if (string.IsNullOrEmpty(html))
                html = string.Empty;

            var taskRes = new TaskCompletionSource<TEditorResponse>();
            var tvc = new TEditorViewController();
            var builder = toolbarBuilder;
            if (toolbarBuilder == null)
                builder = new ToolbarBuilder().AddAll();

            tvc.BuildToolbar(builder);
            tvc.SetHTML(html);
            tvc.Title = title;

            // find a navigation controller
            var nav = FindNavigationController(UIApplication.SharedApplication.KeyWindow.RootViewController);

            // done button
            //var doneIcon = UIImage.FromFile("Images/fa-check@2x.png");
            var doneButton = new UIBarButtonItem(UIBarButtonSystemItem.Done, async (item, args) =>
            {
                nav?.PopViewController(true);
                taskRes.SetResult(new TEditorResponse
                {
                    IsSave = true,
                    HTML = await tvc.GetHTML()
                });
            });

            // navigation to editor html view
            tvc.NavigationItem.SetRightBarButtonItem(doneButton, true);
            nav?.PushViewController(tvc, true);
            nav.NavigationBar.Hidden = false;
            nav.NavigationBarHidden = false;

            // set result 
            return taskRes.Task;
        }

        private static UINavigationController FindNavigationController(UIViewController parrent)
        {
            var navigationController = parrent?.NavigationController;
            if (navigationController != null)
                return navigationController;

            if (parrent?.ChildViewControllers == null || !parrent.ChildViewControllers.Any())
                return null;

            foreach (var parrentChildViewController in parrent.ChildViewControllers)
            {
                if (parrentChildViewController is UINavigationController uiNavigationController)
                    return uiNavigationController;

                if (parrentChildViewController is UITabBarController tabController)
                {
                    if (tabController.SelectedViewController.NavigationController != null)
                        return tabController.SelectedViewController.NavigationController;

                    var lastSelectedTabNavigationController = tabController.SelectedViewController.ChildViewControllers?.Last()?.NavigationController;
                    if (lastSelectedTabNavigationController != null)
                        return lastSelectedTabNavigationController;
                }

                if (parrentChildViewController.PresentedViewController != null)
                {
                    var newFindNavigationController = FindNavigationController(parrentChildViewController.PresentedViewController);
                    if (newFindNavigationController == null)
                        continue;

                    return newFindNavigationController;
                }
            }

            // Not found
            return null;
        }
    }
}
