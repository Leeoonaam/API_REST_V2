namespace MimicAPI.V1.Models.DTO
{
    // as mesmas propriedades da palabra porem contera a propriedade links para o nivel 3 de maturidade da API
    public class PalavraDTO : BaseDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public int Pontuacao { get; set; }
        public bool Ativo { get; set; }
        public DateTime Criado { get; set; }
        public DateTime? Atualizado { get; set; }
    }
}
