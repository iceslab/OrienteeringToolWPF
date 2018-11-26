using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Validation;
using OrienteeringToolWPF.Windows.Forms;
using System;

namespace OrienteeringToolWPF.Utils.Documents
{
    static class DocumentValidator
    {
        public static void ValidateWordprocessingDocument(string filepath)
        {
            using (var wordprocessingDocument = WordprocessingDocument.Open(filepath, true))
            {
                ValidateDocument(wordprocessingDocument);
            }
        }

        public static void ValidateSpreadsheetDocument(string filepath)
        {
            using (var spreadsheetDocument = SpreadsheetDocument.Open(filepath, true))
            {
                ValidateDocument(spreadsheetDocument);
            }
        }

        private static void ValidateDocument(OpenXmlPackage package)
        {
            try
            {
                var err = new ErrorList();
                OpenXmlValidator validator = new OpenXmlValidator();
                int count = 0;
                foreach (ValidationErrorInfo error in
                    validator.Validate(package))
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
        }
    }
}
