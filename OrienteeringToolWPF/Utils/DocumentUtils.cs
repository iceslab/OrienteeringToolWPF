using OrienteeringToolWPF.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Word = Microsoft.Office.Interop.Word;

namespace OrienteeringToolWPF.Utils
{
    public static class DocumentUtils
    {
        /// <summary>
        /// Creates starting list as Word document
        /// </summary>
        /// <param name="fullFilePath">Full path to file which will be created</param>
        /// <param name="relays">List of relays with competitors</param>
        static void CreateStartingList(string fullFilePath, List<Relay> relays)
        {
            if (relays == null)
                throw new ArgumentNullException(nameof(relays), "Relays list cannot be null");
            
            // Create application
            var application = new Word.Application();
            application.ShowAnimation = false;
#if DEBUG
            application.Visible = true;
#else
            application.Visible = false;
#endif
            object missing = System.Reflection.Missing.Value;

            var document = application.Documents.Add(ref missing, ref missing, ref missing, ref missing);
            Word.Paragraph paragraph = document.Content.Paragraphs.Add(ref missing);
            Word.Table firstTable = document.Tables.Add(paragraph.Range, 5, 5, ref missing, ref missing);
        }
    }
}
