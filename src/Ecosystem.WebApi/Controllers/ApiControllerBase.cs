using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Ecosystem.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class ApiControllerBase : ControllerBase
{
    protected readonly ISender Sender;

    protected ApiControllerBase(ISender sender)
    {
        Sender = sender;
    }
}
