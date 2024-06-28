namespace MimicAPI.V1.Models.DTO
{
    // abstract para não instanciar a classe
    public abstract class BaseDTO
    {
        public List<LinkDTO> Links { get; set; } = new List<LinkDTO>(); //instanciando aqui para não ficar instanciando toda vez no codigo
    }
}
