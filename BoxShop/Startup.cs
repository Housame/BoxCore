using AutoMapper;
using BoxShop.Models;
using BoxShop.Models.Entities;
using BoxShop.Models.SuperAdmin;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace BoxShop
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            var connString = @"Data Source=boxstoresrv.database.windows.net;Initial Catalog=boxstoredb;Integrated Security=False;User ID=PuppetMaster;Password=!Silencio-&65?!;Connect Timeout=15;Encrypt=True;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";


            services.AddDbContext<BoxShopContext>(
               options => options.UseSqlServer(connString));

            services.AddDbContext<IdentityDbContext>(
               options => options.UseSqlServer(connString));

            services.AddIdentity<IdentityUser, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Cookies.ApplicationCookie.LoginPath = "/AdminClient/signin";
            })
                .AddEntityFrameworkStores<IdentityDbContext>()
                .AddDefaultTokenProviders();

            //This is where most of the properties are mapped between Models and ViewModels.
            Mapper.Initialize((config) =>
            {
                config.CreateMap<ProductAddVM, Product>().ForSourceMember(p => p.Id, opt => opt.Ignore());
                config.CreateMap<Product, ProductAddVM>();
                config.CreateMap<ProductShowVM, Product>();
                config.CreateMap<Product, ProductShowVM>();
                config.CreateMap<User, UserDisplayVM>();
                config.CreateMap<UserDisplayVM, User>();
                config.CreateMap<Product, ProductDisplayVM>();
                config.CreateMap<User, AdminsShowVM>();
                config.CreateMap<AdminsShowVM, User>();
                config.CreateMap<User, UserShowVM>();
                config.CreateMap<UserShowVM, User>();
                config.CreateMap<RegisterBoxVM, Box>();
                config.CreateMap<Box, RegisterBoxVM>();

                config.CreateMap<AddOrderVM, Order>()
                .ForMember(p => p.DateOfPurchase, x => x.UseValue(DateTime.Today));
                config.CreateMap<OrderItemVM, OrderItem>()
                .ForMember(p => p.DateOfPurchase, x => x.UseValue(DateTime.Today))
                .ForSourceMember(p => p.OrderId, opt => opt.Ignore());

                config.CreateMap<User, UserOrdersVM>();
                config.CreateMap<Order, OrderVM>();
                config.CreateMap<OrderItem, OrderItemVM>();
            });

            services.AddMvc();
            services.AddSession();
            services.AddMemoryCache();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app.UseSession();
            app.UseDeveloperExceptionPage();
            app.UseStaticFiles();
            app.UseIdentity();
            app.UseMvcWithDefaultRoute();
        }
    }
}
