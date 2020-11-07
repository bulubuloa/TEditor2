using System;
using TEditor.Abstractions;
using TEditor.Droid;
using Xamarin.Forms;

[assembly: Dependency(typeof(TEditorService))]
namespace TEditor.Droid
{
    public class TEditorService : ITEditorService
    {
        public ITEditor GetTEditor()
        {
            return new TEditorImplementation();
        }
    }
}
