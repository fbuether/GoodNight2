using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoodNight.Service.Domain.Write;
using GoodNight.Service.Store;

namespace GoodNight.Service.Api.Write
{
  [ApiController]
  [Route("api/v1/write/scenes")]
  public class WriteSceneController : ControllerBase
  {
    private IRepository<Scene> repos;

    public WriteSceneController(IRepository<Scene> repos)
    {
      this.repos = repos;
    }


    private async Task<string> GetBody()
    {
      var body = HttpContext.Request.Body;
      using (var reader = new StreamReader(body, Encoding.UTF8))
      {
        return await reader.ReadToEndAsync();
      }
    }


    [HttpGet]
    public ActionResult<IImmutableList<Scene>> Get()
    {
      System.Console.WriteLine($"repos: {repos}");

      var s = this.repos.Get()
        .Where(s => s.Name == "FirstScene")
        .FirstOrDefault();

      return new StatusCodeResult(StatusCodes.Status418ImATeapot);
    }

    [HttpPost]
    public async Task<ActionResult> Create()
    {
      var content = await GetBody();
      System.Console.WriteLine($"content:  {content}");

      return new StatusCodeResult(StatusCodes.Status417ExpectationFailed);
    }

    [HttpPut("{urlname}")]
    public ActionResult Update(string urlname)
    {
      

      return new StatusCodeResult(StatusCodes.Status416RequestedRangeNotSatisfiable);
    }

    [HttpDelete("{urlname}")]
    public ActionResult Delete(string urlname)
    {
      return new StatusCodeResult(StatusCodes.Status414RequestUriTooLong);
    }
  }
}
