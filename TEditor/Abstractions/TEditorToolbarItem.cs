using System;
namespace TEditor.Abstractions
{
    public class TEditorToolbarItem
    {
        public string ImagePath { get; set; }

        public string Label { get; set; }

        public Func<ITEditorAPI, string> ClickFunc { get; set; }

    }
}
