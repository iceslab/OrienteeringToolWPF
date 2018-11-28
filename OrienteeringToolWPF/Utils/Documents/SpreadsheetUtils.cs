using ClosedXML.Excel;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using OrienteeringToolWPF.DAO;
using OrienteeringToolWPF.Model;
using OrienteeringToolWPF.Utils.Converters;
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
            var workbook = new XLWorkbook();
            var worksheet = workbook.AddWorksheet("Podsumowanie zawodow");

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
            competitors.Sort((l, r) => l.Result.RunningTime.CompareTo(r.Result.RunningTime));

            int row = 2;
            int maxSteps = 0;
            foreach (var c in competitors)
            {
                InsertCompetitor(worksheet, c, row++, relaysDict, ref maxSteps);
            }
            maxSteps++; // Count finish punch
            InsertHeader(worksheet, maxSteps);

            var minimums = new List<long>(maxSteps);
            var maximums = new List<long>(maxSteps);
            CalculateExtremums(worksheet, ref minimums, ref maximums, competitors.Count);
            ColourResults(worksheet, minimums, maximums, competitors.Count);

            workbook.SaveAs(fullFilePath);
        }

        private static void InsertHeader(IXLWorksheet ws, int points)
        {
            string[] headers = { "Lp.", "Imię i nazwisko", "Szkoła", "SI", "CZAS" };
            var row = ws.Row(1);

            for (int i = 1; i <= headers.Length + points; i++)
            {
                // Insert headers
                if (i <= headers.Length)
                {
                    row.Cell(i).Value = headers[i - 1];
                }
                // Insert space for points
                else if (i <= headers.Length + points - 1)
                {
                    row.Cell(i).Value = (i - headers.Length).ToString();
                }
                // Last point is finish
                else
                {
                    row.Cell(i).Value = "FINISH";
                }
            }
        }

        private static void InsertCompetitor(
            IXLWorksheet ws,
            Competitor competitor,
            int rowIdx,
            Dictionary<long, string> relaysDict,
            ref int maxSteps)
        {
            var tsc = new TimestampConverter();
            var dtc = new DeltaTimeConverter();
            var rs = RouteStepsHelper.RouteStepsWhereChip((long)competitor.Chip);
            if (rs.Count > maxSteps)
                maxSteps = rs.Count;

            var row = ws.Row(rowIdx);
            int column = 1;
            row.Cell(column++).Value = rowIdx - 1;
            row.Cell(column++).Value = competitor.Name;

            string relayName = "";
            relaysDict.TryGetValue(competitor.RelayId, out relayName);
            row.Cell(column++).Value = relayName;
            row.Cell(column++).Value = (long)competitor.Chip;

            row.Cell(column++).Value = competitor.Result.RunningTime;

            foreach (var s in rs)
            {
                var punchesWithCode = competitor.Punches.Where(p => p.Code == s.Code).ToList();
                long delta = 0;
                if (punchesWithCode.Count >= 1)
                {
                    delta = punchesWithCode[0].DeltaPrevious;
                }

                row.Cell(column++).Value = delta;
            }

            row.Cell(column++).Value = competitor.Result.FinishTime - competitor.Punches.Last().Timestamp;
            if (competitor.WrongCollections > 0)
            {
                row.Cell(column++).Value = competitor.WrongCollections;
            }
        }

        private static void CalculateExtremums(
            IXLWorksheet worksheet,
            ref List<long> minimums,
            ref List<long> maximums,
            int competitorsCount)
        {
            int maxSteps = minimums.Capacity;
            int rowOffset = 2;
            int columnOffset = 6;
            for (int column = 0; column < maxSteps; column++)
            {
                maximums.Add(long.MinValue);
                minimums.Add(long.MaxValue);
                for (int row = 0; row < competitorsCount; row++)
                {
                    var value = Convert.ToInt64((double)worksheet.Cell(row + rowOffset, column + columnOffset).Value);
                    if (value != 0 && value > maximums[column])
                    {
                        maximums[column] = value;
                    }

                    if (value != 0 && value < minimums[column])
                    {
                        minimums[column] = value;
                    }
                }
            }
        }

        private static void ColourResults(
            IXLWorksheet worksheet,
            List<long> minimums,
            List<long> maximums,
            int competitorsCount)
        {
            var tsc = new TimestampConverter();
            var dtc = new DeltaTimeConverter();

            int maxSteps = minimums.Count;
            int rowOffset = 2;
            int columnOffset = 5;
            int runningTimeColumnIdx = 5;

            // Color worst times in red
            for (int i = 0; i < 3; i++)
            {
                worksheet.Cell(rowOffset - 1 + competitorsCount - i, runningTimeColumnIdx).Style.Fill.BackgroundColor = XLColor.Red;
            }

            // Color best times in green
            for (int i = 0; i < 3; i++)
            {
                worksheet.Cell(rowOffset + i, runningTimeColumnIdx).Style.Fill.BackgroundColor = XLColor.Green;
            }

            // Determine if worst or best and then color respectively
            for (int column = 0; column < maxSteps + 1; column++)
            {
                for (int row = 0; row < competitorsCount; row++)
                {
                    var cell = worksheet.Cell(row + rowOffset, column + columnOffset);
                    var value = Convert.ToInt64((double)cell.Value);
                    if (column == 0)
                    {
                        cell.Value = tsc.Convert(
                            value,
                            null,
                            Properties.Resources.TimestampFormat,
                            null);
                    }
                    else
                    {
                        var best = minimums[column - 1];
                        var worst = maximums[column - 1];
                        if (value == worst)
                        {
                            cell.Style.Fill.BackgroundColor = XLColor.Red;
                        }

                        if (value == best)
                        {
                            cell.Style.Fill.BackgroundColor = XLColor.Green;
                        }

                        if (value == 0)
                        {
                            cell.Style.Fill.BackgroundColor = XLColor.Blue;
                        }

                        cell.Value = dtc.Convert(
                            value,
                            null,
                            Properties.Resources.DeltaTimeFormat,
                            null);
                    }
                }
            }
        }
    }
}
