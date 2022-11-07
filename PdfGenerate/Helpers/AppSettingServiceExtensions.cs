namespace PdfGenerate.Helpers;

public static class AppSettingServiceExtensions
{
      public static IServiceCollection AddAppSettingsService(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<Setting>(options =>
            {
                options.DocumentsPath = config.GetSection("AppSettings:DocumentsPath").Value;
            });
            return services;
        }
}