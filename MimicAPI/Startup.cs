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
using AutoMapper;
using MimicAPI.Helpers;
using MimicAPI.V1.Repositories;
using MimicAPI.V1.Repositories.Contracts;
using Microsoft.AspNetCore.Mvc.Versioning;

namespace MimicAPI
{
    public class Startup
    {
        //servicos
        public void ConfigureServices(IServiceCollection services)
        {
            // Configurar o automapper / ele mapea as propriedades de um objeto para o outro e ele copia esses valores,
            // isso em classes que não tem relacionamneto entre elas mas tem propriedades em comum
            //Criando o arquivo de configuração
            var config = new MapperConfiguration(cfg => {
                cfg.AddProfile(new DTOMapperProfile());
            });
            IMapper mapper = config.CreateMapper();
            services.AddSingleton(mapper); //uma instancia pra todas aplicação

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
            //adiciona configuração do repositorio 
            services.AddScoped<IPalavraRepository, PalavraRepository>(); 
            // adiciona configuração de versionamento da api
            services.AddApiVersioning(cfg =>
            {
                cfg.ReportApiVersions = true; // retorna no cabeçalho uma lista de versão disponiveis para que o usuario possa migrar de acordo com sua documentação
                cfg.ApiVersionReader = new HeaderApiVersionReader("api-version"); // habilita a possibilidade de realizar a consulta utilizando o cabeçalho
                cfg.AssumeDefaultVersionWhenUnspecified = true; // configuração para direciona o usuario para uma versão padrão, quando nao especifica na URL
                cfg.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0); // definindo a versão padrão
            }
                ); 
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