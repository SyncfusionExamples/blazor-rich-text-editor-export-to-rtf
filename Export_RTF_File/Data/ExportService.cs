using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Syncfusion.Pdf;
using Syncfusion.HtmlConverter;

namespace Export_RTF_File.Data
{
    public class ExportService
    {
        public MemoryStream ExportAsPdf(string content)
        {
            try
            {
                // Initialize the HTML to PDF converter
                HtmlToPdfConverter htmlConverter = new HtmlToPdfConverter();

                WebKitConverterSettings settings = new WebKitConverterSettings();

                // Used to load resources before convert
                // Map your local path here
                string baseUrl = @"C:/Users/TempKaruna/source/repos/RTEtoPDF/wwwroot/images";

                // Set WebKit path
                // Map your local path installed location here
                settings.WebKitPath = @"C:/Program Files (x86)/Syncfusion/HTMLConverter/18.1.0.52/QtBinariesDotNetCore";

                // Set additional delay; units in milliseconds;
                settings.AdditionalDelay = 3000;

                // Assign WebKit settings to HTML converter
                htmlConverter.ConverterSettings = settings;

                // Convert HTML string to PDF
                PdfDocument document = htmlConverter.Convert(content, baseUrl);

                // Save the document into stream.
                MemoryStream stream = new MemoryStream();

                document.Save(stream);

                stream.Position = 0;

                // Close the document.
                document.Close(true);

                return stream;
            }
            catch
            {
                return new MemoryStream();
            }
        }
    }
}
