using System.ComponentModel.DataAnnotations;

namespace GeneralLabSolutions.Domain.Entities;

public class MensagemChat
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Usuario { get; set; } = "Você";

    [Required]
    public string Conteudo { get; set; } = string.Empty; // sem limitação aqui

    public DateTime DataHora { get; set; } = DateTime.Now;
    public bool EhRespostaIA { get; set; }
}
