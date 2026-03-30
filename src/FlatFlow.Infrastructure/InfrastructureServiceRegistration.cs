using FlatFlow.Application.Contracts.Persistence;
using FlatFlow.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FlatFlow.Infrastructure
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<Persistence.FlatFlowDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IChoreRepository, ChoreRepository>();
            services.AddScoped<IFlatRepository, FlatRepository>();
            services.AddScoped<INoteRepository, NoteRepository>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();
            services.AddScoped<ITenantRepository, TenantRepository>();

            return services;
        }
    }
}
