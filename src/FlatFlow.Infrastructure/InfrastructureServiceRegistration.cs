using FlatFlow.Application.Contracts.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlatFlow.Infrastructure
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<Persistence.FlatFlowDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // Register repositories - Implementation of repositories will be added later
            // services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            //services.AddScoped<IChoreRepository, ChoreRepository>();
            //services.AddScoped<IFlatRepository, FlatRepository>();
            //services.AddScoped<INoteRepository, NoteRepository>();
            //services.AddScoped<IPaymentRepository, PaymentRepository>();
            //services.AddScoped<ITenantRepository, TenantRepository>();

            return services;
        }
    }
}
