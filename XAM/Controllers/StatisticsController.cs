using Microsoft.AspNetCore.Mvc;
using XAM.Models;
using Castle.DynamicProxy;
using XAM.Interceptors;

namespace XAM.Controllers;

public class StatisticsController : Controller
{
    private readonly XamDbContext _context;

    public StatisticsController(XamDbContext context)
    {
        _context = context;
    }

    public IActionResult Statistics()
    {
        return View();
    }

    public IActionResult FetchStatistics(StatisticsCompiler statisticsCompiler, TimeTakenInterceptor timeTakenInterceptor, ProxyGenerator proxyGenerator)
    {
        DataHolder _dataHolder = _context.GetDataHolder();
        try
        {
            IStatisticsCompiler notInterceptedCompiler = statisticsCompiler;
            IStatisticsCompiler interceptedCompiler = proxyGenerator.CreateInterfaceProxyWithTarget(notInterceptedCompiler, timeTakenInterceptor);

            var result = interceptedCompiler.CompileStatistics(_dataHolder);
            return Json(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while fetching statistics: {ex.Message}");
        }
    }
}