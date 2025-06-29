using Ecosystem.Administration;
using Volo.Abp.Application;
using Volo.Abp.Authorization;
using Volo.Abp.Modularity;
using Volo.Abp.TenantManagement;

namespace Ecosystem.SaaS;

[DependsOn(typeof(AdministrationApplicationContractsModule))]
[DependsOn(typeof(SaaSDomainSharedModule))]
[DependsOn(typeof(AbpDddApplicationContractsModule))]
[DependsOn(typeof(AbpAuthorizationModule))]
[DependsOn(typeof(AbpTenantManagementApplicationContractsModule))]
public class SaaSApplicationContractsModule : AbpModule { }
