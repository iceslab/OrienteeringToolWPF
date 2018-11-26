using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using OrienteeringToolWPF.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OrienteeringToolWPF.Utils.Documents
{
    class SpreadsheetUtils
    {
        /// <summary>
        /// Creates starting list as Word document
        /// </summary>
        /// <param name="fullFilePath">Full path to file which will be created</param>
        /// <param name="relays">List of relays with competitors</param>
        /// <exception cref="NotSupportedException">
        /// Throws when there is different amount of males and females in relay
        /// </exception>
        public static void CreateReport(string fullFilePath, List<Relay> relays)
        {
            using (SpreadsheetDocument spreadsheetDocument =
                SpreadsheetDocument.Create(fullFilePath, SpreadsheetDocumentType.Workbook, true))
            {
                // Add a WorkbookPart to the document.
                var workbookpart = CreateEmptyWorkbookPart(spreadsheetDocument);

                // Classify competition
                ClassificationUtils.ClassifyAll(relays);
                // Get list of all competitors only
                var competitors = new List<Competitor>();
                var relaysDict = new Dictionary<long, string>();
                foreach (var relay in relays)
                {
                    competitors.AddRange(relay.Competitors);
                    relaysDict.Add((long)relay.Id, relay.Name);
                }
                competitors.Sort();

                InsertHeader(spreadsheetDocument, 0);
                uint row = 1;
                foreach (var c in competitors)
                {
                    InsertCompetitor(spreadsheetDocument, c, row++, relaysDict);
                }

                // Save
                workbookpart.Workbook.Save();
            }
        }

        private static WorkbookPart CreateEmptyWorkbookPart(SpreadsheetDocument spreadsheetDocument)
        {
            // Add a WorkbookPart to the document.
            var workbookpart = spreadsheetDocument.AddWorkbookPart();
            workbookpart.Workbook = new Workbook();

            // Add a WorksheetPart to the WorkbookPart.
            var worksheetPart = workbookpart.AddNewPart<WorksheetPart>();
            worksheetPart.Worksheet = new Worksheet(new SheetData());

            // Add Sheets to the Workbook.
            var sheets = spreadsheetDocument.WorkbookPart.Workbook.
                AppendChild(new Sheets());

            // Append a new worksheet and associate it with the workbook.
            var sheet = new Sheet()
            {
                Id = spreadsheetDocument.WorkbookPart.
                GetIdOfPart(worksheetPart),
                SheetId = 1,
                Name = "Podsumowanie zawodow"
            };
            sheets.Append(sheet);

            // Shared strings for inserting text in cells
            spreadsheetDocument.WorkbookPart.AddNewPart<SharedStringTablePart>();

            // Styles for timestamp format
            WorkbookStylesPart sp = spreadsheetDocument.WorkbookPart.AddNewPart<WorkbookStylesPart>();
            sp.Stylesheet = new Stylesheet
            {
                NumberingFormats = new NumberingFormats()
                //CellFormats = new CellFormats()
            };

            //save the changes to the style sheet part   
            sp.Stylesheet.Save();

            return workbookpart;
        }

        private static void InsertHeader(SpreadsheetDocument spreadsheetDocument, int points)
        {
            string[] headers = { "Lp.", "Imię i nazwisko", "Szkoła", "SI", "CZAS" };
            uint i = 0;
            for (; i < headers.Length + points; i++)
            {
                // Insert headers
                if (i < headers.Length)
                {
                    InsertSharedStringCell(spreadsheetDocument, 0, i, headers[i]);
                }
                // Insert space for points
                else if (i < headers.Length + points - 1)
                {
                    InsertSharedStringCell(spreadsheetDocument, 0, i, (i - headers.Length).ToString());
                }
                // Last point is finish
                else
                {
                    InsertSharedStringCell(spreadsheetDocument, 0, i, "FINISH");
                }
            }
        }

        private static void InsertCompetitor(
            SpreadsheetDocument spreadsheetDocument,
            Competitor competitor,
            uint row,
            Dictionary<long, string> relaysDict)
        {
            uint column = 0;
            InsertNumberCell(spreadsheetDocument, row, column++, row);
            InsertSharedStringCell(spreadsheetDocument, row, column++, competitor.Name);

            string relayName = "";
            relaysDict.TryGetValue(competitor.RelayId, out relayName);
            InsertSharedStringCell(spreadsheetDocument, row, column++, relayName);
            InsertNumberCell(spreadsheetDocument, row, column++, (long)competitor.Chip);
            InsertTimestampCell(spreadsheetDocument, row, column++, competitor.Result.RunningTime);
        }

        private static void InsertSharedStringCell(
            SpreadsheetDocument spreadsheetDocument,
            uint row,
            uint column,
            string value)
        {
            // Insert the text into the SharedStringTablePart.
            int index = InsertSharedStringItem(
                value,
                spreadsheetDocument.WorkbookPart.GetPartsOfType<SharedStringTablePart>().First());

            // Insert cell into the worksheet.
            Cell cell = InsertCellInWorksheet(
                ColumnNumberToString(column),
                row + 1,
                spreadsheetDocument.WorkbookPart.WorksheetParts.First());

            // Set the value of cell
            cell.CellValue = new CellValue(index.ToString());
            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        }

        private static void InsertNumberCell(
            SpreadsheetDocument spreadsheetDocument,
            uint row,
            uint column,
            long value)
        {
            // Insert cell into the worksheet.
            Cell cell = InsertCellInWorksheet(
                ColumnNumberToString(column),
                row + 1,
                spreadsheetDocument.WorkbookPart.WorksheetParts.First());

            // Set the value of cell
            cell.CellValue = new CellValue(value.ToString());
            cell.DataType = new EnumValue<CellValues>(CellValues.Number);
        }

        private static void InsertTimestampCell(
            SpreadsheetDocument spreadsheetDocument,
            uint row,
            uint column,
            long value)
        {
            // Insert cell into the worksheet.
            Cell cell = InsertCellInWorksheet(
                ColumnNumberToString(column),
                row + 1,
                spreadsheetDocument.WorkbookPart.WorksheetParts.First());

            // Set the value of cell
            cell.CellValue = new CellValue(value.ToString());
            cell.DataType = new EnumValue<CellValues>(CellValues.Date);
            cell.StyleIndex = GetTimestampStyleIndex(spreadsheetDocument);
        }

        private static uint GetTimestampStyleIndex(SpreadsheetDocument spreadsheetDocument)
        {
            // get the stylesheet from the current sheet    
            var stylesheet = spreadsheetDocument.WorkbookPart.WorkbookStylesPart.Stylesheet;
            // cell formats are stored in the stylesheet's NumberingFormats
            var numberingFormats = stylesheet.NumberingFormats;

            const string dateFormatCode = "[m]:ss";
            // first check if we find an existing NumberingFormat with the desired formatcode
            var dateFormat = numberingFormats.OfType<NumberingFormat>().FirstOrDefault(format => format.FormatCode == dateFormatCode);
            // if not: create it
            if (dateFormat == null)
            {
                dateFormat = new NumberingFormat
                {
                    NumberFormatId = UInt32Value.FromUInt32(164),  // Built-in number formats are numbered 0 - 163. Custom formats must start at 164.
                    FormatCode = StringValue.FromString(dateFormatCode)
                };
                numberingFormats.AppendChild(dateFormat);
                // we have to increase the count attribute manually ?!?
                numberingFormats.Count = Convert.ToUInt32(numberingFormats.Count());
                // save the new NumberFormat in the stylesheet
                stylesheet.Save();
            }
            // get the (1-based) index of the dateformat
            return Convert.ToUInt32(numberingFormats.ToList().IndexOf(dateFormat) + 1);
        }

        private static string ColumnNumberToString(uint column)
        {
            string[] letters = {
                "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M",
                "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "Y", "Z"
            };

            string retVal = "";
            do
            {
                uint modulo = column % (uint)letters.Length;
                column /= (uint)letters.Length;
                retVal = letters[modulo] + retVal;
            }
            while (column > 0);

            return retVal;
        }

        // Given a column name, a row index, and a WorksheetPart, inserts a cell into the worksheet. 
        // If the cell already exists, returns it. 
        private static Cell InsertCellInWorksheet(string columnName, uint rowIndex, WorksheetPart worksheetPart)
        {
            var worksheet = worksheetPart.Worksheet;
            var sheetData = worksheet.GetFirstChild<SheetData>();
            string cellReference = columnName + rowIndex;

            // If the worksheet does not contain a row with the specified row index, insert one.
            Row row;
            if (sheetData.Elements<Row>().Where(r => r.RowIndex == rowIndex).Count() != 0)
            {
                row = sheetData.Elements<Row>().Where(r => r.RowIndex == rowIndex).First();
            }
            else
            {
                row = new Row() { RowIndex = rowIndex };
                sheetData.Append(row);
            }

            // If there is not a cell with the specified column name, insert one.  
            if (row.Elements<Cell>().Where(c => c.CellReference.Value == columnName + rowIndex).Count() > 0)
            {
                return row.Elements<Cell>().Where(c => c.CellReference.Value == cellReference).First();
            }
            else
            {
                // Cells must be in sequential order according to CellReference. Determine where to insert the new cell.
                Cell refCell = null;
                foreach (var cell in row.Elements<Cell>())
                {
                    if (cell.CellReference.Value.Length == cellReference.Length)
                    {
                        if (string.Compare(cell.CellReference.Value, cellReference, true) > 0)
                        {
                            refCell = cell;
                            break;
                        }
                    }
                }

                var newCell = new Cell() { CellReference = cellReference };
                row.InsertBefore(newCell, refCell);

                worksheet.Save();
                return newCell;
            }
        }

        // Given text and a SharedStringTablePart, creates a SharedStringItem with the specified text 
        // and inserts it into the SharedStringTablePart. If the item already exists, returns its index.
        private static int InsertSharedStringItem(string text, SharedStringTablePart shareStringPart)
        {
            // If the part does not contain a SharedStringTable, create one.
            if (shareStringPart.SharedStringTable == null)
            {
                shareStringPart.SharedStringTable = new SharedStringTable();
            }

            int i = 0;

            // Iterate through all the items in the SharedStringTable. If the text already exists, return its index.
            foreach (SharedStringItem item in shareStringPart.SharedStringTable.Elements<SharedStringItem>())
            {
                if (item.InnerText == text)
                {
                    return i;
                }

                i++;
            }

            // The text does not exist in the part. Create the SharedStringItem and return its index.
            shareStringPart.SharedStringTable.AppendChild(new SharedStringItem(new Text(text)));
            shareStringPart.SharedStringTable.Save();

            return i;
        }
    }
}
