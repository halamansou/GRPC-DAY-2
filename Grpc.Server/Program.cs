using GRPC.Server.Handler;
using GRPC.Server.Services;
using Microsoft.AspNetCore.Authentication;

namespace GRPC.Server
{
    public class Program
    {
        const string ApiKeySchemeName = "X-Api-Key";
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddGrpc();
            builder.Services.AddScoped<IApiKeyAuthenticationService, ApiKeyAuthenticationService>();
            builder.Services.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = Consts.ApiKeySchemeName;
            }).AddScheme<AuthenticationSchemeOptions, ApiKeyAuthonticationHandler>(Consts.ApiKeySchemeName, configureOptions => { });
            builder.Services.AddAuthorization();

            var app = builder.Build();


            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapGrpcService<InventroyService>();

            app.Run();
        }
    }
}