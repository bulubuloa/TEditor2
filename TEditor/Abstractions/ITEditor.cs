using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace TEditor.Abstractions
{
    public interface ITEditor : IDisposable
    {
        Task<TEditorResponse> ShowTEditor(string html, ToolbarBuilder toolbarBuilder = null, string title = null, bool autoFocusInput = false, Dictionary<string, string> macros = null);
    }
}
