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
            InsertHeader(worksheet, maxSteps);

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
            //row.Cell(column++).Value = competitor.Result.RunningTime;
            row.Cell(column++).Value = tsc.Convert(
                competitor.Result.RunningTime,
                null,
                Properties.Resources.TimestampFormat,
                null);


            foreach (var s in rs)
            {
                var punchesWithCode = competitor.Punches.Where(p => p.Code == s.Code).ToList();
                long delta = 0;
                if (punchesWithCode.Count == 1)
                {
                    delta = punchesWithCode[0].DeltaPrevious;
                }

                row.Cell(column++).Value = dtc.Convert(
                        delta,
                        null,
                        Properties.Resources.DeltaTimeFormat,
                        null);
            }
        }
    }
}
