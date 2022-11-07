using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PdfGenerate.Utility;

namespace PdfGenerate.Controllers;

[Route("api/pdfcreator")]
[ApiController]
public class PdfCreatorController : ControllerBase
{
        private readonly IConverter _converter;
        private readonly Setting _settings;
        
        public PdfCreatorController(IConverter converter,  IOptions<Setting> settings)
        {
            _converter = converter;
            _settings = settings.Value;
        }
        [HttpGet]
        public async Task<IActionResult> CreatePdf()
        {
            var fileName = "TransactionReports.pdf";
            
            var pdfPath = Path.Combine(
                Directory.GetCurrentDirectory(), _settings.DocumentsPath,
                fileName);
            
            var globalSettings = new GlobalSettings
            {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Portrait,
                PaperSize = PaperKind.A4,
                Margins = new MarginSettings { Top = 10 },
                DocumentTitle = "PDF Report",
                Out = pdfPath
            };
            var objectSettings = new ObjectSettings
            {
                PagesCount = true,
                HtmlContent = TemplateGenerator.GetHTMLString(),
                WebSettings = { DefaultEncoding = "utf-8", UserStyleSheet =  Path.Combine(Directory.GetCurrentDirectory(), "assets", "styles.css") },
                HeaderSettings = { FontName = "Arial", FontSize = 9, Right = "Page [page] of [toPage]", Line = true },
                FooterSettings = { FontName = "Arial", FontSize = 9, Line = true, Center = "Report Footer" }
            };
            var pdf = new HtmlToPdfDocument()
            {
                GlobalSettings = globalSettings,
                Objects = { objectSettings }
            };
            

            _converter.Convert(pdf);
            
            var memory = new MemoryStream();
            await using (var stream = new FileStream(pdfPath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }

            memory.Position = 0;
            
            return File(memory, GetContentType(pdfPath), Path.GetFileName(pdfPath));
        }
        
        private string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }
        
        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.ms-word"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsx", "application/vnd.openxmlformatsofficedocument.spreadsheetml.sheet"},
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"}
            };
        }
    }