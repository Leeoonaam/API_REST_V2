using MimicAPI.Controllers;
using MimicAPI.Helpers;
using MimicAPI.Repositories.Contracts;

namespace MimicAPI.Repositories
{
    public class PalavraRepository : IPalavraRepository // implementar a interface
    {
        //construtor para receber o context (banco)
        public PalavraRepository() 
        {

        }


        public List<Palavras> ObterPalavras(PalavraURLQuery URL)
        {
            throw new NotImplementedException();
        }

        public Palavras Obter(int id)
        {
            throw new NotImplementedException();
        }

        public void Cadastrar(Palavras palavra)
        {
            throw new NotImplementedException();
        }

        public void Atualizar(Palavras palavra)
        {
            throw new NotImplementedException();
        }

        
        public void Deletar(int id)
        {
            throw new NotImplementedException();
        }

    }
}
