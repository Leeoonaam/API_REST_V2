using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimicAPI.Data;
using MimicAPI.Helpers;
using MimicAPI.Models;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace MimicAPI.Controllers
{
    [Route("api/palavras")] // responsavel por definir o caminho pra localizar todas as acoes do controlador palavras
    public class Palavras : ControllerBase
    {
        private readonly Context _banco;

        public Palavras(Context banco)
        {
            _banco = banco;
        }

        [Route("")] // deixar vazio porque assim que entrar no endereço: site.../api/palavras já vai obter todas as palavras sem precisar entrar no metodo
        [HttpGet]
        public ActionResult ObterTodasPalavras([FromQuery] PalavraURLQuery URL) // FromQuery: Atributo informando que os valores vem de uma querystring | PalavraURLQuery: classe objeto para os parametros 
        {
            var item = _banco.Palavras.AsQueryable(); //converte para query ao inves de usar dbset para realizar os filtros por data
            //verifica se contem valor
            if (URL.Data.HasValue)
            {
                item = item.Where(a => a.Criado > URL.Data.Value || a.Atualizado > URL.Data.Value);
            }

            if (URL.pagNumero.HasValue)
            {
                var qtdTotRegistros = item.Count();
                item = item.Skip((URL.pagNumero.Value - 1) * URL.pagRegistro.Value).Take(URL.pagRegistro.Value); // Paginacao, o skip pula e take pega

                var paginacao = new Paginacao();
                paginacao.NumeroPagina = URL.pagNumero.Value;
                paginacao.RegistroPorPagina = URL.pagRegistro.Value;
                paginacao.TotalRegistros = qtdTotRegistros;
                paginacao.TotalPaginas = (int) Math.Ceiling((double) qtdTotRegistros / URL.pagRegistro.Value); // Ceiling arredonda o valor (double)

                // resposta no cabeçalho para informar a paginação pro usuario
                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(paginacao));

                // se tentar acessar uma pagina que não existe, erro 404
                if (URL.pagNumero > paginacao.TotalPaginas)
                {
                    return NotFound();
                }
            }

            return Ok(item);
        }

        [Route("{id}")] // o id é para obter uma palavra especifica, para acessar o obter o usuario o endereço sera: site.../api/palavras/1
        [HttpGet]
        public ActionResult ObterPalavra(int id)
        {
            var obj = _banco.Palavras.Find(id);
            
            //verifica retorno do obj de busca
            if (obj == null)
                return NotFound();
                //ou
                //return StatusCode(404);
            
            return Ok(_banco.Palavras.Find(id));
        }

        [Route("")] //site.../api/palavras(POST: id, nome......)
        [HttpPost]
        public ActionResult Cadastrar([FromBody]palavra palavra)
        {
            _banco.Palavras.Add(palavra);
            _banco.SaveChanges();
            return Created($"/api/palavras/{palavra.Id}",palavra); // apos criar ele retorna encaminhando para consulta junto com o id criado
        }

        [Route("{id}")] // site.../api/palavras/1 (POST: id, nome......)
        [HttpPut]
        public ActionResult Atualizar(int id, [FromBody]palavra palavra)
        {
            // AsNoTracking().FirstOrDefault evita o erro entityframework para obter o id com mesmo valor
            // e por ter mais de um objeto dentro da consulta com o mesmo id
            var obj = _banco.Palavras.AsNoTracking().FirstOrDefault(a=>a.Id == id);

            //verifica retorno do obj de busca
            if (obj == null)
                return NotFound();

            palavra.Id = id; //caso não tenha entregado o id, força
            _banco.Palavras.Update(palavra);
            _banco.SaveChanges();
            return Ok();
        }

        [Route("{id}")] // site.../api/palavras/1 (DELETE)
        [HttpDelete]
        public ActionResult Deletar(int id)
        {
            var palavra = _banco.Palavras.Find(id);

            //verifica retorno do obj de busca
            if (palavra == null)
                return NotFound();

            palavra.Ativo = false;
            _banco.Palavras.Update(palavra);
            _banco.SaveChanges();
            return NoContent(); //status ok, porem como é uma exclusão não tem dados para apresentar, por isso retorna Nocontent
        }
    }
}
