using ICSharpCode.AvalonEdit.Highlighting;
using System.IO;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using System.Resources;

namespace SCaddins.RunScript.Views
{
    public partial class RunScriptView
    {
        public RunScriptView()
        {
            InitializeComponent();
            var editor = this.bindableAvalonEditor;
            ResourceLoader.LoadHighlightingDefinition(editor);
        }
    }
}
