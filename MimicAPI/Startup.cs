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
using Microsoft.OpenApi.Models;
using MimicAPI.Helpers.Swagger;
using Microsoft.Extensions.PlatformAbstractions;

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
                //cfg.ApiVersionReader = new HeaderApiVersionReader("api-version"); // habilita a possibilidade de realizar a consulta utilizando o cabeçalho
                cfg.AssumeDefaultVersionWhenUnspecified = true; // configuração para direciona o usuario para uma versão padrão, quando nao especifica na URL
                cfg.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0); // definindo a versão padrão
                cfg.ApiVersionReader = new MediaTypeApiVersionReader();
            });

            // configuração do swagger
            services.AddSwaggerGen(c =>
            {
                // configuração para resolver o conflito de URL ou rotas, vai pegar o primeiro e deconsiderar os demais
                c.ResolveConflictingActions(ApiDescription => ApiDescription.First());
                //primeira versão | classe info para informações da API de sua escolha
                c.SwaggerDoc("v2.0", new OpenApiInfo { Title = "API - Rest com ASP.NET Core - V2.0", Version = "v2.0" });
                c.SwaggerDoc("v1.1", new OpenApiInfo { Title = "API - Rest com ASP.NET Core - V1.1", Version = "v1.1" });
                c.SwaggerDoc("v1.0", new OpenApiInfo { Title = "API - Rest com ASP.NET Core - V1.0", Version = "v1.0" });

                //adicionando configuração para apresentar os comentarios na documentação swagger
                //var CaminhoProjeto = PlatformServices.Default.Application.ApplicationBasePath; //biblioteca para capturar o caminho do projeto
                //var NomeProjeto = $"{PlatformServices.Default.Application.ApplicationName}.xml";
                //var CaminhoArquivoXMLComentario = Path.Combine(CaminhoProjeto, NomeProjeto);
                //c.IncludeXmlComments(CaminhoArquivoXMLComentario);

                // Define um predicado que determina se uma API específica (apiDesc) deve ser incluída em um documento de API (docName).
                c.DocInclusionPredicate((docName, apiDesc) =>
                {
                    var actionApiVersionModel = apiDesc.ActionDescriptor?.GetApiVersion();
                    // significaria que esta ação não é versionada e deve ser incluída em todos os lugares
                    if (actionApiVersionModel == null)
                    {
                        return true;
                    }
                    if (actionApiVersionModel.DeclaredApiVersions.Any())
                    {
                        return actionApiVersionModel.DeclaredApiVersions.Any(V => $"v{V.ToString()}" == docName);
                    }
                    return actionApiVersionModel.ImplementedApiVersions.Any(V => $"v{V.ToString()}" == docName);
                });

                //filtragem por versão no swagger
                c.OperationFilter<ApiVersionOperationFilter>();

            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            // Configurações para ambiente de desenvolvimento
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                // Habilitar o Swagger UI e o endpoint JSON para a documentação gerada
                app.UseSwagger(); //cria um arquivo no caminho: /swagger/v1/swagger.json
                //configuração da interface grafica
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v2.0/swagger.json", "V2.0");
                    c.SwaggerEndpoint("/swagger/v1.1/swagger.json", "V1.1");
                    c.SwaggerEndpoint("/swagger/v1.0/swagger.json", "V1.0");
                });

                // Redirecionar para a página do Swagger ao acessar a raiz da aplicação
                app.Use(async (context, next) =>
                {
                    if (context.Request.Path == "/")
                    {
                        context.Response.Redirect("/swagger");
                    }
                    else
                    {
                        await next();
                    }
                });
            }

            //ao usar esse midway apresenta as mensagens erros para o usuario
            app.UseStatusCodePages();

            app.UseMvc();
        }
    }
}