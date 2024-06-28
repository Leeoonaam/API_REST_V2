using Microsoft.AspNetCore.Mvc;

namespace MimicAPI.V2.Controllers
{
    // Endereco devido o versionamento ficara: /api/V2.0/palavras
    [ApiController] // atributo para que os controladores trabalhem ou tenham varias funcionalidades para API / Atributo obrigatorio para o versionamento da API
    [Route("api/V{version:apiVersion}/[controller]")] // responsavel por definir o caminho pra localizar todas as acoes do controlador palavras
    [ApiVersion("2.0")] // definindo a versão 
    public class Palavras : ControllerBase
    {
        [HttpGet("", Name = "OBTERTUDO")]
        public string ObterTodasPalavras()
        {
            return "Versão 2.0";
        }
    }
}
