﻿using OrienteeringToolWPF.Enumerations;
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
        /// <exception cref="NotSupportedException">
        /// Throws when there is different amount of males and females in relay
        /// </exception>
        public static void CreateStartingList(string fullFilePath, List<Relay> relays)
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
            object endOfDoc = "\\endofdoc";
            Word.Paragraph paragraph = null;
            Word.Range range = null;
            Word.Table table = null;
            object rangePara = null;
            // Add document
            var document = application.Documents.Add(ref missing, ref missing, ref missing, ref missing);

            foreach (var relay in relays)
            {
                // Breaks page only if not first page
                ((Word.Range)rangePara)?.InsertBreak(Word.WdBreakType.wdPageBreak);

                paragraph = document.Content.Paragraphs.Add(ref missing);
                paragraph.Range.Text = relay.Name;
                range = document.Bookmarks.get_Item(ref endOfDoc).Range;

                var males = Competitor.ExtractGender(relay.Competitors, Gender.MALE);
                var females = Competitor.ExtractGender(relay.Competitors, Gender.FEMALE);

                if (males.Count != females.Count)
                    throw new NotSupportedException("Different amount of males and females in relay is illegal");

                var halfCompetitorsCount = relay.Competitors.Count / 2;
                table = document.Tables.Add(
                   range,
                   halfCompetitorsCount + 1,
                   5,
                   ref missing,
                   ref missing);
                table.Borders.Enable = 1;

                foreach (Word.Row row in table.Rows)
                {
                    // TODO: Change to resources
                    string[] headers = { "Kolejność startu", "Imię i nazwisko", "Chip", "Imię i nazwisko", "Chip" };

                    foreach (Word.Cell cell in row.Cells)
                    {
                        if (cell.ColumnIndex == 3)
                        {
                            // Thicker cell border
                            cell.Borders[Word.WdBorderType.wdBorderRight].LineWidth =
                                Word.WdLineWidth.wdLineWidth225pt;
                        }

                        // Set headers for table
                        if (cell.RowIndex == 1)
                        {
                            cell.Range.Text = headers[cell.ColumnIndex - 1];
                        }
                        else
                        {
                            var text = "";
                            switch (cell.ColumnIndex)
                            {
                                case 1:
                                    text = string.Format("{0}.", cell.RowIndex - 1);
                                    break;
                                case 2:
                                    text = females[cell.RowIndex - 2].Name;
                                    break;
                                case 3:
                                    text = females[cell.RowIndex - 2].Chip.ToString();
                                    break;
                                case 4:
                                    text = males[cell.RowIndex - 2].Name;
                                    break;
                                case 5:
                                    text = males[cell.RowIndex - 2].Chip.ToString();
                                    break;
                            }
                            cell.Range.Text = text;
                        }
                    }
                }

                // Add paragraph
                rangePara = document.Bookmarks.get_Item(ref endOfDoc).Range;
                //paragraph = document.Content.Paragraphs.Add(ref missing);

                //paragraph.Range.Text = Environment.NewLine;
            }

        }
    }
}
