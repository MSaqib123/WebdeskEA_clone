using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebdeskEA.RazerServices;

public interface IPdfGeneratorService
{
    /// <summary>
    /// Generates a PDF as a FileResult from any Razor view (by path or name) with any model object.
    /// </summary>
    /// <param name="viewPathOrName">The .cshtml view path or name to render.</param>
    /// <param name="model">The model passed to that view.</param>
    /// <param name="outputFilename">The filename for the downloaded PDF file.</param>
    /// <returns>A FileResult containing the PDF data.</returns>
    Task<FileResult> GeneratePdfFileAsync(string viewPathOrName, object model, string outputFilename);
}

public class PdfGeneratorService : IPdfGeneratorService
{
    private readonly IRazorRenderer _razorRenderer;
    private readonly IConverter _converter;

    public PdfGeneratorService(
        IRazorRenderer razorRenderer,
        IConverter converter)
    {
        _razorRenderer = razorRenderer;
        _converter = converter;
    }

    public async Task<FileResult> GeneratePdfFileAsync(string viewPathOrName, object model, string outputFilename)
    {
        // 1. Render the Razor view into HTML
        var htmlContent = await _razorRenderer.RenderViewToStringAsync(viewPathOrName, model);

        // 2. Setup the HTML to PDF document
        var doc = new HtmlToPdfDocument
        {
            GlobalSettings = new GlobalSettings
            {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Portrait,
                PaperSize = PaperKind.A4,
                DocumentTitle = outputFilename
            },
            Objects = {
                new ObjectSettings
                {
                    PagesCount = true,
                    HtmlContent = htmlContent,
                    WebSettings =
                    {
                        DefaultEncoding = "utf-8",
                        // If your CSS/Images are local:
                        //EnableLocalFileAccess = true
                    }
                }
            }
        };

        // 3. Convert the document to PDF in memory
        var pdfData = _converter.Convert(doc);

        // 4. Return as a FileResult
        return new FileContentResult(pdfData, "application/pdf")
        {
            FileDownloadName = outputFilename
        };
    }
}
