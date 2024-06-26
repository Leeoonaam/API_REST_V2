namespace MimicAPI.Models.DTO
{
    // abstract para não instanciar a classe
    public abstract class BaseDTO
    {
        public List<LinkDTO> Links { get; set; }
    }
}
