using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using TestePowerEmbedded.Models;
using TestePowerEmbedded.Services;
using TestePowerEmbedded.ViewModels;

namespace TestePowerEmbedded.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly PowerBiServiceApi _powerBiServiceApi;
    private readonly IConfiguration _configuration;
    
    public HomeController(IConfiguration configuration, ILogger<HomeController> logger, PowerBiServiceApi powerBiServiceApi)
    {
        _logger = logger;
        _powerBiServiceApi = powerBiServiceApi;
        _configuration = configuration;
    }

    [AllowAnonymous]
    public IActionResult Index()
    {
        return View();
    }
    
    [AuthorizeForScopes(Scopes = ["user.read"])]
    public async Task<ActionResult<EmbeddedReportViewModel>> Report()
    {
        var workspaceId = new Guid(_configuration["PowerBi:WorkspaceId"]);
        var reportId = new Guid(_configuration["PowerBi:ReportId"]);
        
        var report = await _powerBiServiceApi.GetReport(workspaceId, reportId);
        
        return View(report);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
