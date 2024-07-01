using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimicAPI.Helpers;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using AutoMapper;
using MimicAPI.V1.Models;
using MimicAPI.V1.Models.DTO;
using MimicAPI.V1.Repositories.Contracts;

namespace MimicAPI.V1.Controllers
{
    // Endereco devido o versionamento ficara: /api/V1.0/palavras
    [ApiController] // atributo para que os controladores trabalhem ou tenham varias funcionalidades para API / Atributo obrigatorio para o versionamento da API
    [Route("api/V{version:apiVersion}/[controller]")] // responsavel por definir o caminho pra localizar todas as acoes do controlador palavras | [controller]: para não ficar colocando o nome do controlador, automaticamente usando assim, ele preenche corretamente já que foi adicionado o atributo acima
    [ApiVersion("1.0", Deprecated = true)] // definindo a versão que pode utilizar esse controlador | dessa forma apresenta no cabeçalho a versão suportada e a versão que tem que ser migrada | versão fica absoleta, ou seja, o usuario tera que migrar pra v2.
    [ApiVersion("1.1")] // dessa forma tambem define que contem outra versão. Ex: foi realizado uma melhoria
    [Produces("application/json")]
    public class Palavras : ControllerBase
    {
        private readonly IPalavraRepository _repository;
        private readonly IMapper _mapper; //injecao de dependencia como propriedade

        public Palavras(IPalavraRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// Operação que pega do banco de dados todas as palavras existentes.
        /// </summary>
        /// <param name="URL">Filtro de Pesquisa</param>
        /// <returns>Listagem de palavras</returns>
        // dessa forma estou indicando que esse metodo funciona nas duas versões, tanto na primeira (1.0) quanto na de melhoria (1.0)
        [MapToApiVersion("1.0")]
        [MapToApiVersion("1.1")] // aqui estou mapeando esse metodo ou função para a versão 1.1 que seria uma melhoria da 1.0
        //[Route("")] // deixar vazio porque assim que entrar no endereço: site.../api/palavras já vai obter todas as palavras sem precisar entrar no metodo
        [HttpGet("", Name = "OBTERTUDO")]
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

        /// <summary>
        /// Operação que pega uma unica palavra da base de dados
        /// </summary>
        /// <param name="id">codigo identificador</param>
        /// <returns>Um objeto de palavra</returns>
        [MapToApiVersion("1.0")]
        [MapToApiVersion("1.1")]
        //route removido para que o erro do parametro ID não retorno como query string no final do link
        //[Route("{id}")] // o id é para obter uma palavra especifica, para acessar o obter o usuario o endereço sera: site.../api/palavras/1
        [HttpGet("{id}", Name = "OBTEMPALAVRA")] //nomeando para ligar o dominio correto aos links | adiciona o route dentro do verbo http e passar o id
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
            //palavraDTO.Links = new List<LinkDTO>(); // instancia para adicionar, já que o valor vem null
            //adiciona o link
            //Utiliza a Url.Link, propriedade que existe no ControllerBase, para passar a rota
            palavraDTO.Links.Add(
                new LinkDTO("self", Url.Link("OBTEMPALAVRA", new { id = palavraDTO.Id }), "GET"));
            // adicionando link para informar ao usuario que alem de consultar ele pode ter o link para acessar as funcionalidades de atualizar e deletar
            palavraDTO.Links.Add(
                new LinkDTO("update", Url.Link("ATUALIZARPALAVRA", new { id = palavraDTO.Id }), "PUT"));
            palavraDTO.Links.Add(
                new LinkDTO("delete", Url.Link("EXCLUIRPALAVRA", new { id = palavraDTO.Id }), "DELETE"));

            return Ok(palavraDTO);
        }

        /// <summary>
        /// Operação que realiza o cadastro da palavra
        /// </summary>
        /// <param name="palavra">Um objeto da palavra</param>
        /// <returns>Um objeto palavra com o seu ID</returns>
        [MapToApiVersion("1.0")]
        [MapToApiVersion("1.1")]
        [Route("")] //site.../api/palavras(POST: id, nome......)
        [HttpPost]
        public ActionResult Cadastrar([FromBody] palavra palavra)
        {
            //valida se o obj é null
            if (palavra == null)
                return BadRequest();

            // valida os dados obrigatorios | utilizando o modelstate porque já valida seu objeto (palavra) de entrada no asp dot net
            // !(negacao): verifica se é um objeto valido
            if (!ModelState.IsValid)
                return UnprocessableEntity(ModelState); //metodo do erro 422

            palavra.Ativo = true;
            palavra.Criado = DateTime.Now;

            _repository.Cadastrar(palavra);

            //mapeamento
            PalavraDTO palavraDTO = _mapper.Map<palavra, PalavraDTO>(palavra);
            palavraDTO.Links.Add(
                new LinkDTO("self", Url.Link("OBTEMPALAVRA", new { id = palavraDTO.Id }), "GET"));


            return Created($"/api/palavras/{palavra.Id}", palavra); // apos criar ele retorna encaminhando para consulta junto com o id criado
        }

        /// <summary>
        /// Operação que realiza a substituição de dados de uma palavra especifica
        /// </summary>
        /// <param name="id">codigo identificador da palavra a ser alterada</param>
        /// <param name="palavra">objeto palavra com dados</param>
        /// <returns></returns>
        [MapToApiVersion("1.0")]
        [MapToApiVersion("1.1")]
        //[Route("{id}")] // site.../api/palavras/1 (POST: id, nome......)
        [HttpPut("{id}", Name = "ATUALIZARPALAVRA")]
        public ActionResult Atualizar(int id, [FromBody] palavra palavra)
        {
            var obj = _repository.Obter(id);

            //verifica retorno do obj de busca no banco para depois validar os dados
            if (obj == null)
                return NotFound();

            //valida se o obj é null
            if (palavra == null)
                return BadRequest();

            // valida os dados obrigatorios | utilizando o modelstate porque já valida seu objeto (palavra) de entrada no asp dot net
            // !(negacao): verifica se é um objeto valido
            if (!ModelState.IsValid)
                return UnprocessableEntity(ModelState); //metodo do erro 422

            palavra.Id = id; //caso não tenha entregado o id, força
            //validações para manter o que vem do banco e atualiza somente a data atualizado para atual
            palavra.Ativo = obj.Ativo;
            palavra.Criado = obj.Criado;
            palavra.Atualizado = DateTime.Now;

            _repository.Atualizar(palavra);

            PalavraDTO palavraDTO = _mapper.Map<palavra, PalavraDTO>(palavra);
            palavraDTO.Links.Add(
                new LinkDTO("self", Url.Link("OBTEMPALAVRA", new { id = palavraDTO.Id }), "GET"));

            return Ok();
        }

        /// <summary>
        /// Operação que desativa uma palavra do sistema
        /// </summary>
        /// <param name="id">codigo identificador</param>
        /// <returns></returns>
        [MapToApiVersion("1.1")]
        //[Route("{id}")] // site.../api/palavras/1 (DELETE)
        [HttpDelete("{id}", Name = "EXCLUIRPALAVRA")]
        public ActionResult Deletar(int id)
        {
            var palavra = _repository.Obter(id);

            //verifica retorno do obj de busca
            if (palavra == null)
                return NotFound();

            _repository.Deletar(id);

            return NoContent(); //status ok, porem como é uma exclusão não tem dados para apresentar, por isso retorna Nocontent
        }


        /// <summary>
        /// Metodo para criar os link de navegação nas listas
        /// </summary>
        /// <param name="URL"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        [MapToApiVersion("1.0")]
        [MapToApiVersion("1.1")]
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
    }
}
