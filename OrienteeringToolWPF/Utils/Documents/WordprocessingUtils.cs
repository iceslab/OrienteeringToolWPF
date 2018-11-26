using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using OrienteeringToolWPF.Enumerations;
using OrienteeringToolWPF.Model;
using System;
using System.Collections.Generic;

namespace OrienteeringToolWPF.Utils.Documents
{
    public class WordprocessingUtils
    {
        /// <summary>
        /// Creates starting list as Word document using OpenXml
        /// </summary>
        /// <param name="fullFilePath">Full path to file which will be created</param>
        /// <param name="relays">List of relays with competitors</param>
        /// <exception cref="NotSupportedException">
        /// Throws when there is different amount of males and females in relay
        /// </exception>
        public static void CreateStartingList(string fullFilePath, List<Relay> relays)
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
                    doc.Body.Append(new Paragraph(
                        new Run(
                            new Text(relay.Name))));
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
                                tc.Append(new Paragraph(
                                    new Run(
                                        new Text(headers[cellIndex]))));
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
                                tc.Append(new Paragraph(
                                    new Run(
                                        new Text(text))));
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

        private static Table CreateEmptyTable()
        {
            var table = new Table();
            var props = new TableProperties(
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
    }
}
