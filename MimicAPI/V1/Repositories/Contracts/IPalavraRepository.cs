using MimicAPI.Helpers;
using MimicAPI.V1.Models;

namespace MimicAPI.V1.Repositories.Contracts
{
    public interface IPalavraRepository
    {
        PaginationList<palavra> ObterPalavras(PalavraURLQuery URL);

        palavra Obter(int id);

        void Cadastrar(palavra palavra);

        void Atualizar(palavra palavra);

        void Deletar(int id);

    }
}
