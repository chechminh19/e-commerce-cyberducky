using Application;
using Application.IRepository;
using Application.IService;
using Application.Service;



//using Application.IRepositories;
//using Application.IService;
//using Application.Services;
using Infrastructure;
using Infrastructure.Repo;
using Microsoft.AspNetCore.Authentication;


//using Infrastructure.Repositories;
using System.Diagnostics;

namespace ZodiacJewelryWebApI
{
    public static class DependencyInject
    {
        public static IServiceCollection AddWebAPIService(this IServiceCollection services)
        {
            services.AddControllers().AddJsonOptions (option=> option.JsonSerializerOptions.PropertyNamingPolicy=System.Text.Json.JsonNamingPolicy.KebabCaseLower);
            /*services.AddFluentValidation();*/ 
            
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.AddHealthChecks();

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IAuthenService, AuthenService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IImageService, ImageService>();

            services.AddScoped<IProductRepo, ProductRepo>();
            services.AddScoped<IOrderRepo, OrderRepo>();
            services.AddScoped<IImageRepo, ImageRepo>();
            services.AddScoped<IUserRepo, UserRepo>();
            
            services.AddHttpContextAccessor();        
            return services;
        }
    }
}
