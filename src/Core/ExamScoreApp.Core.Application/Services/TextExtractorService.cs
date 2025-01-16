using System;
using System.IO;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;

namespace ExamScoreApp.Core.Application.Services
{
    public class TextExtractorService
    {
        public string ExtractTextFromPdf(string pdfFilePath)
        {
            using (PdfReader pdfReader = new PdfReader(pdfFilePath))
            using (PdfDocument pdfDocument = new PdfDocument(pdfReader))
            {
                StringWriter stringWriter = new StringWriter();
                for (int page = 1; page <= pdfDocument.GetNumberOfPages(); page++)
                {
                    ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
                    string pageContent = PdfTextExtractor.GetTextFromPage(pdfDocument.GetPage(page), strategy);
                    stringWriter.Write(pageContent);
                }
                return stringWriter.ToString();
            }
        }

        public void SaveTextToFile(string text, string txtFilePath)
        {
            File.WriteAllText(txtFilePath, text);
        }

        public int GetCharacterCount(string text)
        {
            return text.Length;
        }

        public void ExtractAndSaveText(string pdfFilePath, string txtFilePath)
        {
            string extractedText = ExtractTextFromPdf(pdfFilePath);
            SaveTextToFile(extractedText, txtFilePath);
            int characterCount = GetCharacterCount(extractedText);
            Console.WriteLine($"Character Count: {characterCount}");
        }
    }
}