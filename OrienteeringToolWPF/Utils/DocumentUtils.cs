#define USE_OPENXML

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Validation;
using DocumentFormat.OpenXml.Wordprocessing;
using OrienteeringToolWPF.Enumerations;
using OrienteeringToolWPF.Model;
using OrienteeringToolWPF.Windows.Forms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
        /// <param name="obj">Represents control which called this method (used for getting window)</param>
        /// <exception cref="NotSupportedException">
        /// Throws when there is different amount of males and females in relay
        /// </exception>
        public static void CreateStartingList(string fullFilePath, List<Relay> relays)
        {
#if USE_OPENXML
            CreateStartingListOpenXml(fullFilePath, relays);
            //ValidateWordDocument(fullFilePath);
#else
            CreateStartingListMS(fullFilePath, relays);
#endif
        }

        /// <summary>
        /// Creates starting list as Word document using OpenXml
        /// </summary>
        /// <param name="fullFilePath">Full path to file which will be created</param>
        /// <param name="relays">List of relays with competitors</param>
        /// <param name="obj">Represents control which called this method (used for getting window)</param>
        /// <exception cref="NotSupportedException">
        /// Throws when there is different amount of males and females in relay
        /// </exception>
        private static void CreateStartingListOpenXml(string fullFilePath, List<Relay> relays)
        {
            // Create Document
            using (WordprocessingDocument wordDocument =
                WordprocessingDocument.Create(fullFilePath, WordprocessingDocumentType.Document, true))
            {
                MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();
                mainPart.Document = new Document
                {
                    Body = new Body()
                };

                var doc = wordDocument.MainDocumentPart.Document;

                // Process all relays
                foreach (var relay in relays)
                {
                    // Add paragraph for relay
                    doc.Body.Append(new Paragraph(new Run(new Text(relay.Name))));
                    var males = Competitor.ExtractGender(relay.Competitors, Gender.MALE);
                    var females = Competitor.ExtractGender(relay.Competitors, Gender.FEMALE);

                    males.Sort(Competitor.CompareById);
                    females.Sort(Competitor.CompareById);

                    if (males.Count != females.Count || males.Count <= 0 || females.Count <= 0)
                        throw new NotSupportedException("Different or less than zero amount of males and females in relay");
                    var halfCompetitorsCount = relay.Competitors.Count / 2;
                    var table = CreateEmptyTable();


                    // TODO: Change to resources
                    string[] headers = { "Kolejność startu", "Imię i nazwisko", "Chip", "Imię i nazwisko", "Chip" };

                    for (var rowIndex = 0; rowIndex < halfCompetitorsCount + 1; rowIndex++)
                    {
                        var tr = new TableRow();
                        for (var cellIndex = 0; cellIndex < 5; cellIndex++)
                        {
                            var tc = new TableCell();

                            // Assume you want columns that are automatically sized.
                            tc.Append(new TableCellProperties(
                                new TableCellWidth { Type = TableWidthUnitValues.Auto }));

                            if (cellIndex == 3)
                            {
                                // Thicker cell border
                                //tc.Append(new TableCellProperties(
                                //    new TableCellBorders(
                                //        new LeftBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 18 })));
                            }

                            // Set headers for table
                            if (rowIndex == 0)
                            {
                                tc.Append(new Paragraph(new Run(new Text(headers[cellIndex]))));
                            }
                            else if (rowIndex > 0)
                            {
                                var text = "";
                                switch (cellIndex)
                                {
                                    case 0:
                                        text = string.Format("{0}.", rowIndex);
                                        break;
                                    case 1:
                                        text = females[rowIndex - 1].Name;
                                        break;
                                    case 2:
                                        text = females[rowIndex - 1].Chip.ToString();
                                        break;
                                    case 3:
                                        text = males[rowIndex - 1].Name;
                                        break;
                                    case 4:
                                        text = males[rowIndex - 1].Chip.ToString();
                                        break;
                                }
                                tc.Append(new Paragraph(new Run(new Text(text))));
                            }
                            tr.Append(tc);
                        }
                        table.Append(tr);
                    }
                    doc.Body.Append(table);
                    doc.Save();
                }
            }
        }

        /// <summary>
        /// Creates starting list as Word document using Interop objects
        /// </summary>
        /// <param name="fullFilePath">Full path to file which will be created</param>
        /// <param name="relays">List of relays with competitors</param>
        /// <param name="obj">Represents control which called this method (used for getting window)</param>
        /// <exception cref="NotSupportedException">
        /// Throws when there is different amount of males and females in relay
        /// </exception>
        private static void CreateStartingListMS(string fullFilePath, List<Relay> relays)
        {
            if (relays == null)
                throw new ArgumentNullException(nameof(relays), "Relays list cannot be null");

            // Create application
            var application = new Word.Application();
            if (application == null)
                throw new NotSupportedException("Cannot start Microsoft Office Word. Is it installed?");

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

                males.Sort(Competitor.CompareById);
                females.Sort(Competitor.CompareById);

                if (males.Count != females.Count || males.Count <= 0 || females.Count <= 0)
                    throw new NotSupportedException("Different or less than zero amount of males and females in relay");

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

                        //MessageBox.Show(string.Format($"{cell.RowIndex}, female: {females.Count}, male: {males.Count}"));
                        // Set headers for table
                        if (cell.RowIndex == 1)
                        {
                            cell.Range.Text = headers[cell.ColumnIndex - 1];
                        }
                        else if (cell.RowIndex > 1)
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
            }

            document.SaveAs2(fullFilePath);
#if !DEBUG
            document.Close(ref missing, ref missing, ref missing);
            document = null;
            application.Quit(ref missing, ref missing, ref missing);
            application = null;
#endif
        }

        public static void ValidateWordDocument(string filepath)
        {
            using (WordprocessingDocument wordprocessingDocument =
            WordprocessingDocument.Open(filepath, true))
            {
                try
                {
                    var err = new ErrorList();
                    OpenXmlValidator validator = new OpenXmlValidator();
                    int count = 0;
                    foreach (ValidationErrorInfo error in
                        validator.Validate(wordprocessingDocument))
                    {
                        count++;
                        err.Add(count + ": Error", count.ToString());
                        err.Add(count + ": Description: ", error.Description);
                        err.Add(count + ": ErrorType: ", error.ErrorType.ToString());
                        err.Add(count + ": Node: ", error.Node.ToString());
                        err.Add(count + ": Path: ", error.Path.XPath);
                        err.Add(count + ": Part: ", error.Part.Uri.ToString());
                        err.Add(count + ": -------------------------------------------", "");
                        Console.WriteLine("Error " + count);
                        Console.WriteLine("Description: " + error.Description);
                        Console.WriteLine("ErrorType: " + error.ErrorType);
                        Console.WriteLine("Node: " + error.Node);
                        Console.WriteLine("Path: " + error.Path.XPath);
                        Console.WriteLine("Part: " + error.Part.Uri);
                        Console.WriteLine("-------------------------------------------");
                    }

                    if (err.HasErrors())
                    {
                        MessageUtils.ShowValidatorErrors(null, err);
                    }
                    Console.WriteLine("count={0}", count);
                }

                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                wordprocessingDocument.Close();
            }
        }

        private static Table CreateEmptyTable()
        {
            Table table = new Table();
            TableProperties props = new TableProperties(
                            new TableBorders(
                                new TopBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 8 },
                                new BottomBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 8 },
                                new LeftBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 8 },
                                new InsideVerticalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 8 },
                                new RightBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 8 },
                                new InsideHorizontalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 8 }
                               ));
            table.AppendChild(props);
            var grid = new TableGrid(
                new GridColumn(),
                new GridColumn(),
                new GridColumn(),
                new GridColumn(),
                new GridColumn());

            table.Append(grid);
            return table;
        }
        /// <summary>
        /// Creates competitors list as text file compatibile with OORG 10 format
        /// </summary>
        /// <param name="fullFilePath">Full path to file which will be created</param>
        /// <param name="relays">List of relays with competitors</param>
        /// <param name="categories">List of categories</param>
        public static void ExportCompetitors(string fullFilePath, List<Relay> relays, List<Model.Category> categories)
        {
            using (var outputFile = new StreamWriter(fullFilePath))
            {
                var first = true;
                foreach (var relay in relays)
                {
                    foreach (var competitor in relay.Competitors)
                    {
                        var newline = first ? "" : "\n";
                        string category = categories.Find(cat => cat.Id == competitor.Category)?.Name ?? "";
                        category = category.ToUpper().PadRight(9);
                        var name = competitor.Name.PadRight(24);
                        var club = relay.Name.ToUpper().PadRight(3);
                        var empty = "".PadRight(15);

                        category = category.Substring(0, 9);
                        name = name.Substring(0, 24);
                        club = club.Substring(0, 3);

                        var line = string.Format($"{newline}{category} {name} {club} {empty}");
                        outputFile.Write(line);
                        first = false;
                    }
                }
            }

            Process.Start(fullFilePath);
        }
    }
}
