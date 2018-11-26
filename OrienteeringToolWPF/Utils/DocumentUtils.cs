using OrienteeringToolWPF.Model;
using OrienteeringToolWPF.Utils.Documents;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

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
            WordprocessingUtils.CreateStartingList(fullFilePath, relays);
            DocumentValidator.ValidateWordprocessingDocument(fullFilePath);
        }

        /// <summary>
        /// Creates general report as Excel document
        /// </summary>
        /// <param name="fullFilePath">Full path to file which will be created</param>
        /// <param name="relays">List of relays with competitors</param>
        /// <exception cref="NotSupportedException">
        /// Throws when there is different amount of males and females in relay
        /// </exception>
        public static void CreateReport(string fullFilePath, List<Relay> relays)
        {
            SpreadsheetUtils.CreateReport(fullFilePath, relays);
            DocumentValidator.ValidateSpreadsheetDocument(fullFilePath);
        }

        /// <summary>
        /// Creates competitors list as text file compatibile with OORG 10 format
        /// </summary>
        /// <param name="fullFilePath">Full path to file which will be created</param>
        /// <param name="relays">List of relays with competitors</param>
        /// <param name="categories">List of categories</param>
        public static void ExportCompetitors(string fullFilePath, List<Relay> relays, List<Category> categories)
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
