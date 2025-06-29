﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;

namespace Ecosystem.Projects.Samples;

[Area(ProjectsRemoteServiceConsts.ModuleName)]
[RemoteService(Name = ProjectsRemoteServiceConsts.RemoteServiceName)]
[Route("api/Projects/sample")]
public class SampleController(ISampleAppService sampleAppService)
    : ProjectsController,
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
