using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using System.Data;
using System.Windows;

namespace GrpcServer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Task.Run(() =>
            {
                var builder = WebApplication.CreateBuilder();
                builder.Services.AddGrpc();
                var app = builder.Build();
                app.MapGrpcService<TestBiz>();
                app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
                app.Run();
            });
        }
    }

}
