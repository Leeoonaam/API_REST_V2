using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MimicAPI.Data;
using IHostingEnvironment = Microsoft.Extensions.Hosting.IHostingEnvironment;

namespace MimicAPI
{
    public class Startup
    {
        //servicos
        public void ConfigureServices(IServiceCollection services)
        {
            // Desativa o roteamento de endpoint
            services.AddControllersWithViews(options =>
            {
                options.EnableEndpointRouting = false; 
            }); services.AddControllersWithViews();

            //banco de dados configurado para salvar na pasta data do projeto
            services.AddDbContext<Context>(opt => {
                opt.UseSqlite("Data Source=Data\\Mimic.db");
            });

            //mvc
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            //ao usar esse midway apresenta as mensagens erros para o usuario
            app.UseStatusCodePages();

            app.UseMvc();
        }
    }
}