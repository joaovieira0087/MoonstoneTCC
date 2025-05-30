using System.ComponentModel.DataAnnotations;

public enum TipoComunicado
{
    [Display(Name = "Comunicado")]
    Comunicado = 0,

    [Display(Name = "Enquete")]
    Enquete = 1,

    [Display(Name = "Pergunta + Resposta")]
    Pergunta = 2
}

public class Comunicado
{
    [Key]
    public int Id { get; set; }

    [Required]
    public TipoComunicado Tipo { get; set; }

    [Required]
    [StringLength(200)]
    public string Titulo { get; set; }

    [Required]
    [StringLength(2000)]
    public string Mensagem { get; set; }

    public string? OpcoesEnquete { get; set; } // Ex: "PlayStation|Xbox|PC"
    public DateTime DataCriacao { get; set; } = DateTime.Now;
}
