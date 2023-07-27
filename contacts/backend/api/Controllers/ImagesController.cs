using Microsoft.AspNetCore.Mvc;

namespace Contacts.Api.Controllers;

[ApiController]
[ApiExplorerSettings(IgnoreApi = true)]
[Route("api/images")]
public class ImagesController : ControllerBase
{
    [HttpGet]
    public IActionResult GetImages()
    {
        throw new NotImplementedException("Not implemented yet");
    }
}