﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;

namespace Ecosystem.IdentityService.Samples;

[Area(IdentityServiceRemoteServiceConsts.ModuleName)]
[RemoteService(Name = IdentityServiceRemoteServiceConsts.RemoteServiceName)]
[Route("api/IdentityService/sample")]
public class SampleController(ISampleAppService sampleAppService)
    : IdentityServiceController,
        ISampleAppService
{
    private readonly ISampleAppService _sampleAppService = sampleAppService;

    [HttpGet]
    public async Task<SampleDto> GetAsync()
    {
        return await _sampleAppService.GetAsync();
    }

    [HttpGet]
    [Route("authorized")]
    [Authorize]
    public async Task<SampleDto> GetAuthorizedAsync()
    {
        return await _sampleAppService.GetAsync();
    }
}
