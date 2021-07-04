using GoodNight.Service.Storage.Interface;
using Microsoft.AspNetCore.Mvc;

namespace GoodNight.Service.Api.Controller
{
  [ApiController]
  [Route("api/v1/status")]
  public class StatusController : ControllerBase
  {
    private IStore store;

    public StatusController(IStore store)
    {
      this.store = store;
    }

    [HttpGet]
    public ActionResult<object> Get()
    {
      var proc = System.Diagnostics.Process.GetCurrentProcess();
      proc.Refresh();
      return new {
        status = "Yup, we're good.",
        startTime = proc.StartTime,
        totalProcTime = proc.TotalProcessorTime,
        privateMemory = proc.WorkingSet64 / 1048576.0,
        storeStatus = store.GetStatus()
      };
    }
  }
}
