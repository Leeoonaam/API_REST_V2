using MimicAPI.Controllers;
using MimicAPI.Helpers;

namespace MimicAPI.Repositories.Contracts
{
    public interface IPalavraRepository
    {
        List<Palavras> ObterPalavras(PalavraURLQuery URL);

        Palavras Obter(int id);

        void Cadastrar(Palavras palavra);

        void Atualizar(Palavras palavra);

        void Deletar(int id);

    }
}
