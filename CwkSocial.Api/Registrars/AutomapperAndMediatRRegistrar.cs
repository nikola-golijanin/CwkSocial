using CwkSocial.Application;
using MediatR;

namespace CwkSocial.Api.Registrars;

public class AutomapperAndMediatRRegistrar : IWebApplicationBuilderRegistrar
{
    public void RegisterServices(WebApplicationBuilder builder)
    {
        builder.Services.AddAutoMapper(typeof(Program), typeof(AssemblyReference));
        builder.Services.AddMediatR(typeof(AssemblyReference));
    }
}