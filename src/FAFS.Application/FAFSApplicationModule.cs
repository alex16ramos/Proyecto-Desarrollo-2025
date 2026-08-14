using FAFS.Application.Contracts.Destinations;
using FAFS.Destinations;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Account;
using Volo.Abp.AutoMapper;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.SettingManagement;

namespace FAFS;

[DependsOn(
    typeof(FAFSDomainModule),
    typeof(FAFSApplicationContractsModule),
    typeof(AbpPermissionManagementApplicationModule),
    typeof(AbpFeatureManagementApplicationModule),
    typeof(AbpIdentityApplicationModule),
    typeof(AbpAccountApplicationModule),
    typeof(AbpSettingManagementApplicationModule)
    )]
public class FAFSApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<FAFSApplicationModule>();
        });

        // Configuración de clientes HTTP para APIs externas
        context.Services.AddHttpClient("GeoDbClient");
        context.Services.AddHttpClient("TicketmasterClient");

        // Vincular la configuración del archivo appsettings.json (sección ExternalApis)
        context.Services.AddOptions<ExternalApisOptions>()
            .BindConfiguration("ExternalApis");

        // Registrar servicios de integración externa
        context.Services.AddTransient<ICitySearchService, GeoDbCitySearchService>();
        context.Services.AddTransient<ITicketmasterService, TicketmasterService>();
    }
}
