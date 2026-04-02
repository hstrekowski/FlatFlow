using FlatFlow.Api.Authorization;
using FlatFlow.Api.Middleware;
using FlatFlow.Api.Services;
using FlatFlow.Application;
using FlatFlow.Application.Contracts.Identity;
using FlatFlow.Infrastructure;
using Microsoft.AspNetCore.Authorization;

namespace FlatFlow.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddApplicationServices();
            builder.Services.AddInfrastructureServices(builder.Configuration);
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
            builder.Services.AddScoped<IAuthorizationHandler, FlatAuthorizationHandler>();
            builder.Services.AddAuthorizationBuilder()
                .AddPolicy("FlatMember", policy => policy.AddRequirements(new FlatMemberRequirement()))
                .AddPolicy("FlatOwner", policy => policy.AddRequirements(new FlatOwnerRequirement()));
            builder.Services.AddControllers();
            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
            builder.Services.AddOpenApi();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseExceptionHandler(_ => { });
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
