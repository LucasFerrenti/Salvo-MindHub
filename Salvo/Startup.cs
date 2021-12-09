using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Salvo.Models;
using Salvo.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;

namespace Salvo
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            //injeccion de dependencia salvocontext
            services.AddDbContext<SalvoContext>(opt => opt.UseSqlServer(Configuration.GetConnectionString("SalvoDB"),
                                                o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)));
            //injeccion réposotorio game
            services.AddScoped<IGameRepository, GameRepository>();
            //injeccion réposotorio gameplayer
            services.AddScoped<IGamePlayerRepository, GamePlayerRepository>();
            //injeccion réposotorio player
            services.AddScoped<IPlayerRepository, PlayerRepository>();
            //injeccion repositorio score
            services.AddScoped<IScoreRepository, ScoreRepository>();
            //Authentication
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(10);
                    options.LoginPath = new PathString("/index.html");
                });
            //Authorization
            services.AddAuthorization(options =>
            {
                options.AddPolicy("PlayerOnly", policy => policy.RequireClaim("Player"));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=games}/{ action = Get}");
            });
        }
    }
}