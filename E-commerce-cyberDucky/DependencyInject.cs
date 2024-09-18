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
            //services.AddCors();
            /*services.AddSingleton<GlobalExceptionMiddleware>();
            services.AddSingleton<PerformanceMiddleware>();*/
            //services.AddSingleton<Stopwatch>();
            //services.AddScoped<IUserRepo, UserRepo>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            //services.AddSingleton<ICurrentTime, CurrentTime>();
            //services.AddScoped<IClaimsService, ClaimsService>();

            services.AddScoped<IProductRepo, ProductRepo>();
            services.AddScoped<IProductService, ProductService>();
            services.AddHttpContextAccessor();


            //services.AddScoped<IZodiacProductRepo, ZodiacProductRepo>();

            //services.AddScoped<IZodiacProductService, ZodiacProductService>();

            //services.AddScoped<ICategoryRepo, CategoryRepo>();
            //services.AddScoped<ICategoryService, CategoryService>();

            //services.AddScoped<IMaterialRepo, MaterialRepo>();
            //services.AddScoped<IMaterialService, MaterialService>();
          
            //services.AddScoped<IOrderRepo, OrderRepo>();
            //services.AddScoped<IOrderService, OrderService>();

            //services.AddScoped<IZodiacRepo, ZodiacRepo>();
            //services.AddScoped<IZodiacService, ZodiacService>();

            //services.AddScoped<IImageRepo, ImageRepo>();
            //services.AddScoped<IImageService, ImageService>();

            //services.AddScoped<ICollectionRepo, CollectionsRepo>();
            //services.AddScoped<ICollectionService, CollectionService>();

            //services.AddScoped<ICollectionProductRepo, CollectionProductRepo>();


            //services.AddScoped<IUserRepo, UserRepo>();
            //services.AddScoped<IUserService, UserService>();

            return services;
        }
    }
}
