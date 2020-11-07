using System;
using TEditor.Abstractions;
using TEditor.iOS;
using Xamarin.Forms;

[assembly: Dependency(typeof(TEditorService))]
namespace TEditor.iOS
{
    public class TEditorService : ITEditorService
    {
        public ITEditor GetTEditor()
        {
            return new TEditorImplementation();
        }
    }
}