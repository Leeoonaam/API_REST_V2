namespace MimicAPI.Models.DTO
{
    public class LinkDTO
    {
        public string Rel { get; set; } // legenda do link
        public string Href { get; set; } // endereço
        public string Method { get; set; } //metodos

        //construtor que recebera as informações
        public LinkDTO(string rel, string href, string method)
        {
            Rel = rel;
            Href = href;
            Method = method;
        }
    }
}
