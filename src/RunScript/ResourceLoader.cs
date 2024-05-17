using System.IO;
using SCaddins.RunScript.Views;

namespace SCaddins.RunScript
{
   public static class ResourceLoader
    {
        public static void LoadHighlightingDefinition(BindableAvalonEditor editor)
        {
            using (var stream = new MemoryStream(SCaddins.Properties.Resources.Lua))
            {
                using (var reader = new System.Xml.XmlTextReader(stream))
                {
                    editor.SyntaxHighlighting =
                        ICSharpCode.AvalonEdit.Highlighting.Xshd.HighlightingLoader.Load(
                            reader,
                            ICSharpCode.AvalonEdit.Highlighting.HighlightingManager.Instance);
                }
            }
        }
    }
}
