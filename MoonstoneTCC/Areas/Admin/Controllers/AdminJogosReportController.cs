using FastReport.Data;
using FastReport.Export.PdfSimple;
using FastReport.Web;
using MoonstoneTCC.Areas.Admin.FastReportUtils;
using MoonstoneTCC.Areas.Admin.Servicos;
using Microsoft.AspNetCore.Mvc;


namespace MoonstoneTCC.Areas.Admin.Controllers;

[Area("Admin")]
public class AdminJogosReportController : Controller
{
    private readonly IWebHostEnvironment _webHostEnv;
    private readonly RelatorioJogosService _relatorioJogosService;

    public AdminJogosReportController(IWebHostEnvironment webHostEnv,
        RelatorioJogosService relatorioJogosService)
    {
        _webHostEnv = webHostEnv;
        _relatorioJogosService = relatorioJogosService;
    }
    public async Task<ActionResult> JogosCategoriaReport()
    {
        var webReport = new WebReport();
        var mssqlDataConnection = new MsSqlDataConnection();

        webReport.Report.Dictionary.AddChild(mssqlDataConnection);

        webReport.Report.Load(Path.Combine(_webHostEnv.ContentRootPath, "wwwroot/reports",
                                           "JogosCategorias.frx"));

        var jogos = HelperFastReport.GetTable(await _relatorioJogosService.GetJogosReport(), "JogosReport");
        var categorias = HelperFastReport.GetTable(await _relatorioJogosService.GetCategoriasReport(), "CategoriasReport");

        webReport.Report.RegisterData(jogos, "JogoReport");
        webReport.Report.RegisterData(categorias, "CategoriasReport");
        return View(webReport);
    }

    [Route("JogosCategoriaPDF")]
    public async Task<ActionResult> JogosCategoriaPDF()
    {
        var webReport = new WebReport();
        var mssqlDataConnection = new MsSqlDataConnection();

        webReport.Report.Dictionary.AddChild(mssqlDataConnection);

        webReport.Report.Load(Path.Combine(_webHostEnv.ContentRootPath, "wwwroot/reports",
                                           "JogosCategorias.frx"));

        var jogos = HelperFastReport.GetTable(await _relatorioJogosService.GetJogosReport(), "JogosReport");
        var categorias = HelperFastReport.GetTable(await _relatorioJogosService.GetCategoriasReport(), "CategoriasReport");

        webReport.Report.RegisterData(jogos, "JogoReport");
        webReport.Report.RegisterData(categorias, "CategoriasReport");

        webReport.Report.Prepare();

        Stream stream = new MemoryStream();

        webReport.Report.Export(new PDFSimpleExport(), stream);
        stream.Position = 0;

        //return File(stream, "application/zip", "JogosCategoria.pdf");
        return new FileStreamResult(stream, "application/pdf");
    }
}
