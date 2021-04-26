
using Microsoft.AspNetCore.Mvc;

namespace GoodNight.Service.Api
{
  [ApiController]
  [Route("api/v1/status")]
  public class StatusController : ControllerBase
  {
    [HttpGet]
    public ActionResult<object> Get()
    {
      var proc = System.Diagnostics.Process.GetCurrentProcess();
      proc.Refresh();

      return new {
        status = "Yup, we're good.",
        machine = proc.MachineName,
        privateMemory = proc.WorkingSet64 / 1048576.0
      };
    }
  }
}
