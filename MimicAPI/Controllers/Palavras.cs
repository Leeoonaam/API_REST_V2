using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimicAPI.Helpers;
using MimicAPI.Models;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using MimicAPI.Repositories.Contracts;

namespace MimicAPI.Controllers
{
    [Route("api/palavras")] // responsavel por definir o caminho pra localizar todas as acoes do controlador palavras
    public class Palavras : ControllerBase
    {
        private readonly IPalavraRepository _repository;

        public Palavras(IPalavraRepository repository)
        {
            _repository = repository;
        }

        [Route("")] // deixar vazio porque assim que entrar no endereço: site.../api/palavras já vai obter todas as palavras sem precisar entrar no metodo
        [HttpGet]
        public ActionResult ObterTodasPalavras([FromQuery] PalavraURLQuery URL) // FromQuery: Atributo informando que os valores vem de uma querystring | PalavraURLQuery: classe objeto para os parametros 
        {
            var item = _repository.ObterPalavras(URL);

            // se tentar acessar uma pagina que não existe, erro 404
            if (URL.pagNumero > item.paginacao.TotalPaginas)
            {
                return NotFound();
            }
            // resposta no cabeçalho para informar a paginação pro usuario
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(item.paginacao));

            // retorna o item com todos os elementos listados
            return Ok(item.ToList());
        }

        [Route("{id}")] // o id é para obter uma palavra especifica, para acessar o obter o usuario o endereço sera: site.../api/palavras/1
        [HttpGet]
        public ActionResult ObterPalavra(int id)
        {
            var obj = _repository.Obter(id);
            
            //verifica retorno do obj de busca
            if (obj == null)
                return NotFound();
                //ou
                //return StatusCode(404);
            
            return Ok(obj);
        }

        [Route("")] //site.../api/palavras(POST: id, nome......)
        [HttpPost]
        public ActionResult Cadastrar([FromBody]palavra palavra)
        {
            _repository.Cadastrar(palavra);
            return Created($"/api/palavras/{palavra.Id}",palavra); // apos criar ele retorna encaminhando para consulta junto com o id criado
        }

        [Route("{id}")] // site.../api/palavras/1 (POST: id, nome......)
        [HttpPut]
        public ActionResult Atualizar(int id, [FromBody]palavra palavra)
        {
            var obj = _repository.Obter(id);

            //verifica retorno do obj de busca
            if (obj == null)
                return NotFound();

            palavra.Id = id; //caso não tenha entregado o id, força
            _repository.Atualizar(palavra);
            return Ok();
        }

        [Route("{id}")] // site.../api/palavras/1 (DELETE)
        [HttpDelete]
        public ActionResult Deletar(int id)
        {
            var palavra = _repository.Obter(id);

            //verifica retorno do obj de busca
            if (palavra == null)
                return NotFound();

            _repository.Deletar(id);
            
            return NoContent(); //status ok, porem como é uma exclusão não tem dados para apresentar, por isso retorna Nocontent
        }
    }
}
