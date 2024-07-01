using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MimicAPI.Helpers.Swagger
{
    public class ApiVersionOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // Obtém a versão da API para a ação atual do endpoint.
            var actionApiVersionModel = context.ApiDescription.ActionDescriptor?.GetApiVersion();
            // Se a versão da API for nula, retorna sem fazer nada.
            if (actionApiVersionModel == null)
            {
                return;
            }
            // Se houver versões de API declaradas, adiciona essas versões às respostas.
            if (actionApiVersionModel.DeclaredApiVersions.Any())
            {
                // Chama o método auxiliar para adicionar as versões às respostas.
                AddApiVersionToResponses(operation, actionApiVersionModel.DeclaredApiVersions);
            }
            else
            {
                // Ordena as versões de API implementadas em ordem decrescente e adiciona às respostas.
                var orderedImplementedVersions = actionApiVersionModel.ImplementedApiVersions.OrderByDescending(v => v);
                AddApiVersionToResponses(operation, orderedImplementedVersions);
            }
        }

        // Método auxiliar para adicionar versões de API às respostas.
        private void AddApiVersionToResponses(OpenApiOperation operation, IEnumerable<ApiVersion> apiVersions)
        {
            // Itera sobre cada resposta na operação.
            foreach (var response in operation.Responses)
            {
                // Obtém uma lista dos tipos de conteúdo (content types) da resposta.
                var contentTypes = response.Value.Content.Keys.ToList();
                // Itera sobre cada tipo de conteúdo.
                foreach (var contentType in contentTypes)
                {
                    // Obtém o media type associado ao tipo de conteúdo.
                    var mediaType = response.Value.Content[contentType];
                    // Para cada versão da API, adiciona uma nova entrada no conteúdo com a versão anexada ao tipo de conteúdo.
                    foreach (var version in apiVersions)
                    {
                        // Adiciona uma nova chave ao dicionário de conteúdo com o tipo de conteúdo e a versão da API.
                        response.Value.Content[$"{contentType};v={version}"] = mediaType;
                    }
                }
            }
        }
    }
}
