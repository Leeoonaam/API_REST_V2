using MimicAPI.V1.Models.DTO;


namespace MimicAPI.Helpers
{
    //public class PaginationList<T> : List<T> //recebe caractristica de lista
    // alterando para armazenar os registros 
    public class PaginationList<T>
    {
        // results armazenara toda a lista de palavras e seus atributos e propriedades que sera contido dentro de uma pagina só
        public List<T> Results { get; set; } = new List<T>(); // instanciando aqui para evitar o erro null ao realizar o addrange 
        public Paginacao paginacao {  get; set; }
        public List<LinkDTO> Links { get; set; } = new List<LinkDTO>();
    }
}
