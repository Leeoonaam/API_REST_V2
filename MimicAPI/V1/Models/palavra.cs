using System.ComponentModel.DataAnnotations;

namespace MimicAPI.V1.Models
{
    public class palavra
    {
        public int Id { get; set; }
        [Required] //campo obrigatorio
        public string Nome { get; set; }
        [Required] //campo obrigatorio
        public int Pontuacao { get; set; }
        public bool Ativo { get; set; }
        public DateTime Criado { get; set; }
        public DateTime? Atualizado { get; set; }
    }
}
