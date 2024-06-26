using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimicAPI.Helpers;
using MimicAPI.Models;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using MimicAPI.Repositories.Contracts;
using AutoMapper;
using MimicAPI.Models.DTO;

namespace MimicAPI.Controllers
{
    [Route("api/palavras")] // responsavel por definir o caminho pra localizar todas as acoes do controlador palavras
    public class Palavras : ControllerBase
    {
        private readonly IPalavraRepository _repository;
        private readonly IMapper _mapper; //injecao de dependencia como propriedade

        public Palavras(IPalavraRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        //[Route("")] // deixar vazio porque assim que entrar no endereço: site.../api/palavras já vai obter todas as palavras sem precisar entrar no metodo
        [HttpGet("",Name = "OBTERTUDO")]
        public ActionResult ObterTodasPalavras([FromQuery] PalavraURLQuery URL) // FromQuery: Atributo informando que os valores vem de uma querystring | PalavraURLQuery: classe objeto para os parametros 
        {
            var item = _repository.ObterPalavras(URL);

            // se tentar acessar uma pagina que não existe, erro 404
            if (item.Results.Count == 0)
                return NotFound();

            if (item.paginacao != null)
                // resposta no cabeçalho para informar a paginação pro usuario
                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(item.paginacao));
            
            //refatoração do metodo para criar os link para navegação da lista de palavras
            PaginationList<PalavraDTO> lista = CriarLinksListaPalavraDTO(URL, item);

            // retorna lista com os links adicionados
            return Ok(lista);
        }

        //Metodo para criar os link de navegação nas listas
        private PaginationList<PalavraDTO> CriarLinksListaPalavraDTO(PalavraURLQuery URL, PaginationList<palavra> item)
        {
            //conversao da paginacao da palavra para paginar palavrasDTO
            var lista = _mapper.Map<PaginationList<palavra>, PaginationList<PalavraDTO>>(item);

            //loop adiconando link para cada palavra da lista de palavras
            foreach (var palavra in lista.Results)
            {
                palavra.Links = new List<LinkDTO>(); //instancia
                palavra.Links.Add(new LinkDTO("self", Url.Link("OBTEMPALAVRA", new { id = palavra.Id }), "GET"));
            }

            //preenchimento do link no resultado na lista
            // Como na rota nãp tem parametro, a informaçao URL do fromquery, automaticamente servira como preenchimento para a querystring
            lista.Links.Add(new LinkDTO("self", Url.Link("OBTERTUDO", URL), "GET"));

            // links para paginação e possibilirar navegacao anterior e proximo
            if (item.paginacao != null)
            {
                // resposta no cabeçalho para informar a paginação pro usuario
                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(item.paginacao));

                // verifica para paginar a primeira corretamente
                if (URL.pagNumero + 1 <= item.paginacao.TotalPaginas)
                {
                    //objeto para receber os parametros da paginacao
                    var queryString = new PalavraURLQuery() { pagNumero = URL.pagNumero + 1, pagRegistro = URL.pagRegistro, Data = URL.Data };
                    lista.Links.Add(new LinkDTO("next", Url.Link("OBTERTUDO", queryString), "GET"));
                }
                //verifica para demais paginas e ter a opção de volta
                if (URL.pagNumero - 1 > 0)
                {
                    var queryString = new PalavraURLQuery() { pagNumero = URL.pagNumero - 1, pagRegistro = URL.pagRegistro, Data = URL.Data };
                    lista.Links.Add(new LinkDTO("prev", Url.Link("OBTERTUDO", queryString), "GET"));
                }
            }

            return lista;
        }

        //route removido para que o erro do parametro ID não retorno como query string no final do link
        //[Route("{id}")] // o id é para obter uma palavra especifica, para acessar o obter o usuario o endereço sera: site.../api/palavras/1
        [HttpGet("{id}",Name = "OBTEMPALAVRA")] //nomeando para ligar o dominio correto aos links | adiciona o route dentro do verbo http e passar o id
        public ActionResult ObterPalavra(int id)
        {
            var obj = _repository.Obter(id);
            
            //verifica retorno do obj de busca
            if (obj == null)
                return NotFound();
                //ou
                //return StatusCode(404);

            // copia toda informacao que vem do banco e vai converter criando o link já com o valor null
            PalavraDTO palavraDTO = _mapper.Map<palavra, PalavraDTO>(obj);
            palavraDTO.Links = new List<LinkDTO>(); // instancia para adicionar, já que o valor vem null
            //adiciona o link
            //Utiliza a Url.Link, propriedade que existe no ControllerBase, para passar a rota
            palavraDTO.Links.Add(
                new LinkDTO("self", Url.Link("OBTEMPALAVRA", new {id = palavraDTO.Id}), "GET"));
            // adicionando link para informar ao usuario que alem de consultar ele pode ter o link para acessar as funcionalidades de atualizar e deletar
            palavraDTO.Links.Add(
                new LinkDTO("update", Url.Link("ATUALIZARPALAVRA", new {id = palavraDTO.Id}), "PUT"));
            palavraDTO.Links.Add(
                new LinkDTO("delete", Url.Link("EXCLUIRPALAVRA", new { id = palavraDTO.Id }), "DELETE"));

            return Ok(palavraDTO);
        }

        [Route("")] //site.../api/palavras(POST: id, nome......)
        [HttpPost]
        public ActionResult Cadastrar([FromBody]palavra palavra)
        {
            _repository.Cadastrar(palavra);
            return Created($"/api/palavras/{palavra.Id}",palavra); // apos criar ele retorna encaminhando para consulta junto com o id criado
        }

        //[Route("{id}")] // site.../api/palavras/1 (POST: id, nome......)
        [HttpPut("{id}",Name = "ATUALIZARPALAVRA")]
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

        //[Route("{id}")] // site.../api/palavras/1 (DELETE)
        [HttpDelete("{id}",Name = "EXCLUIRPALAVRA")]
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
