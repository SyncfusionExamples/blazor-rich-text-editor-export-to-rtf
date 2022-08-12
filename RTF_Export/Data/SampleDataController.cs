using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using Syncfusion.EJ.Export;
using System.Net.Http.Headers;
using Syncfusion.DocIO;
using Syncfusion.DocIO.DLS;

namespace RTF_Export.Data
{
    [Route("api/[controller]")]
    [ApiController]
    public class SampleDataController : ControllerBase
    {
        private IWebHostEnvironment hostingEnv;

        public SampleDataController(IWebHostEnvironment env)
        {
            this.hostingEnv = env;
        }


        [HttpPost]
        [Route("Import")]
        public string Import(IList<IFormFile> UploadFiles)
        {
            string HtmlString = string.Empty;
            if (UploadFiles != null)
            {
                foreach (var file in UploadFiles)
                {
                    string filename = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    filename = hostingEnv.WebRootPath + "\\files" + $@"\{filename}";
                    using (FileStream fs = System.IO.File.Create(filename))
                    {
                        file.CopyTo(fs);
                        fs.Flush();
                    }
                    using (var mStream = new MemoryStream())
                    {
                        new WordDocument(file.OpenReadStream(), FormatType.Rtf).Save(mStream, FormatType.Html);
                        mStream.Position = 0;
                        HtmlString = new StreamReader(mStream).ReadToEnd();
                    };
                    HtmlString = ExtractBodyContent(HtmlString);
                    var str = HtmlString.Replace("\r\n", "");
                    Response.Headers.Add("rteValue", str);
                }
            }
            return HtmlString;
        }

        public string ExtractBodyContent(string html)
        {
            if (html.Contains("<html") && html.Contains("<body"))
            {
                return html.Remove(0, html.IndexOf("<body>") + 6).Replace("</body></html>", "");
            }
            return html;
        }


        [HttpPost]
        [Route("ExportToRtf")]
        public FileStreamResult ExportToRtf(string value)
        {
            string htmlText = Request.Headers["value"][0];
            WordDocument document = GetDocument(htmlText);
            //Saves the Word document to MemoryStream
            MemoryStream stream = new MemoryStream();
            document.Save(stream, FormatType.Rtf);
            stream.Position = 0;
            FileStream outputStream = new FileStream("Sample.rtf", FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
            document.Save(outputStream, FormatType.Rtf);
            document.Close();
            outputStream.Flush();
            outputStream.Dispose();
            //Download Word document in the browser 
            return File(stream, "application/msword", "Sample.rtf");
        }


        public WordDocument GetDocument(string htmlText)
        {
            WordDocument document = null;
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream, System.Text.Encoding.Default);
            htmlText = htmlText.Replace("\"", "'");
            XmlConversion XmlText = new XmlConversion(htmlText);
            XhtmlConversion XhtmlText = new XhtmlConversion(XmlText);
            writer.Write(XhtmlText.ToString());
            writer.Flush();
            stream.Position = 0;
            document = new WordDocument(stream, FormatType.Html, XHTMLValidationType.None);
            return document;
        }

        [HttpPost]
        [Route("Save")]
        public void Save(IList<IFormFile> UploadFiles)
        {
            try
            {
                foreach (var file in UploadFiles)
                {
                    if (UploadFiles != null)
                    {
                        string path = hostingEnv.ContentRootPath + "\\wwwroot\\Images";
                        string filename = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                        if (!System.IO.Directory.Exists(path))
                        {
                            System.IO.Directory.CreateDirectory(path);
                        }

                        //To save the image in the sever side 
                        filename = path + $@"\{filename}";

                        if (!System.IO.File.Exists(filename))
                        {
                            using (FileStream fs = System.IO.File.Create(filename))
                            {
                                file.CopyTo(fs);
                                fs.Flush();
                            }
                        }
                    }
                }
            }
            catch
            {
            }
        }
    }

   

}
