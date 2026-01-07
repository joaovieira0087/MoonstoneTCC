using FastReport;
using FastReport.Data;
using FastReport.Export.PdfSimple;
using FastReport.Web;
using Microsoft.AspNetCore.Mvc;
using MoonstoneTCC.Areas.Admin.FastReportUtils;
using MoonstoneTCC.Areas.Admin.Services;
using MoonstoneTCC.Services;

namespace MoonstoneTCC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminUsuariosReportController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnv;
        private readonly RelatorioUsuariosService _relatorioUsuariosService;
        private readonly LoggerAdminService _logger;


        public AdminUsuariosReportController(IWebHostEnvironment webHostEnv, RelatorioUsuariosService relatorioUsuariosService, LoggerAdminService logger)
        {
            _webHostEnv = webHostEnv;
            _relatorioUsuariosService = relatorioUsuariosService;
            _logger = logger;
        }


        public async Task<IActionResult> UsuariosReport()
        {
            var webReport = new WebReport();
            var mssql = new MsSqlDataConnection();
            webReport.Report.Dictionary.Connections.Add(mssql);

            webReport.Report.Load(Path.Combine(_webHostEnv.ContentRootPath, "wwwroot/reports", "UsuariosRelatorio.frx"));

            var usuarios = HelperFastReport.GetTable(await _relatorioUsuariosService.GetUsuariosAsync(), "UsuariosReport");
            webReport.Report.RegisterData(usuarios, "UsuariosReport");
            await _logger.RegistrarAcaoAsync("Acessou a visualização do relatório de usuários");

            return View(webReport);
        }

        [Route("UsuariosRelatorioPDF")]
        public async Task<IActionResult> UsuariosPDF()
        {
            var report = new WebReport();
            var mssql = new MsSqlDataConnection();
            report.Report.Dictionary.Connections.Add(mssql);

            report.Report.Load(Path.Combine(_webHostEnv.ContentRootPath, "wwwroot/reports", "UsuariosRelatorio.frx"));

            var usuarios = HelperFastReport.GetTable(await _relatorioUsuariosService.GetUsuariosAsync(), "UsuariosReport");
            report.Report.RegisterData(usuarios, "UsuariosReport");

            report.Report.Prepare();
            var stream = new MemoryStream();
            report.Report.Export(new PDFSimpleExport(), stream);
            stream.Position = 0;
            await _logger.RegistrarAcaoAsync("Gerou o relatório de usuários (PDF)");

            return new FileStreamResult(stream, "application/pdf");
        }
    }
}
