namespace MimicAPI.Helpers
{
    public class PaginationList<T> : List<T> //recebe caractristica de lista
    {
        public Paginacao paginacao {  get; set; }

    }
}
