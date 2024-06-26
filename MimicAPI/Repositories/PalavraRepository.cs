using Microsoft.EntityFrameworkCore;
using MimicAPI.Data;
using MimicAPI.Helpers;
using MimicAPI.Models;
using MimicAPI.Repositories.Contracts;
using Newtonsoft.Json;

namespace MimicAPI.Repositories
{
    public class PalavraRepository : IPalavraRepository // implementar a interface
    {
        private readonly Context _banco; //somente leitura
        //construtor para receber o context (banco)
        public PalavraRepository(Context banco) 
        {
            _banco = banco;
        }
        
        public PaginationList<palavra> ObterPalavras(PalavraURLQuery URL)
        {
            //instancia para utilizar o metodo para trabalhar com a classe paginação interna
            var lista = new PaginationList<palavra>();

            // AsNoTracking().FirstOrDefault evita o erro entityframework para obter o id com mesmo valor
            // e por ter mais de um objeto dentro da consulta com o mesmo id
            var item = _banco.Palavras.AsNoTracking().AsQueryable(); //converte para query ao inves de usar dbset para realizar os filtros por data
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
                paginacao.TotalPaginas = (int)Math.Ceiling((double)qtdTotRegistros / URL.pagRegistro.Value); // Ceiling arredonda o valor (double)

                lista.paginacao = paginacao;
            }

            lista.AddRange(item.ToList());

            return lista;
        }

        public palavra Obter(int id)
        {
            // AsNoTracking().FirstOrDefault evita o erro entityframework para obter o id com mesmo valor
            // e por ter mais de um objeto dentro da consulta com o mesmo id
            return _banco.Palavras.AsNoTracking().FirstOrDefault(a => a.Id == id);
        }

        public void Cadastrar(palavra palavra)
        {
            _banco.Palavras.Add(palavra);
            _banco.SaveChanges();
        }

        public void Atualizar(palavra palavra)
        {
            _banco.Palavras.Update(palavra);
            _banco.SaveChanges();
        }

        public void Deletar(int id)
        {
            var palavra = Obter(id);
            palavra.Ativo = false;
            _banco.Palavras.Update(palavra);
            _banco.SaveChanges();
        }

    }
}
