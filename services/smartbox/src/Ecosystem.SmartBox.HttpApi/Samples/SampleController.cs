using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;

namespace Ecosystem.SmartBox.Samples;

[Area(SmartBoxRemoteServiceConsts.ModuleName)]
[RemoteService(Name = SmartBoxRemoteServiceConsts.RemoteServiceName)]
[Route("api/SmartBox/sample")]
public class SampleController(ISampleAppService sampleAppService)
    : SmartBoxController,
        ISampleAppService
{
    private readonly ISampleAppService _sampleAppService = sampleAppService;

    [HttpGet]
    public Task<SampleDto> GetAsync()
    {
        return _sampleAppService.GetAsync();
    }

    [HttpGet]
    [Route("authorized")]
    [Authorize]
    public Task<SampleDto> GetAuthorizedAsync()
    {
        return _sampleAppService.GetAsync();
    }
}
