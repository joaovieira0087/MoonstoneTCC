using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace MoonstoneTCC.Models
{
    public class Candidatura
    {
        public int Id { get; set; }

        public string UsuarioId { get; set; }
        public IdentityUser Usuario { get; set; }

        public int VagaEmpregoId { get; set; }
        public VagaEmprego Vaga { get; set; }

        [Required(ErrorMessage = "Nome é obrigatório.")]
        [StringLength(100, ErrorMessage = "Nome muito longo.")]
        public string NomeCompleto { get; set; }

        [Required(ErrorMessage = "CPF é obrigatório.")]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "CPF deve conter exatamente 11 dígitos numéricos.")]
        public string CPF { get; set; }

        [Required(ErrorMessage = "RG é obrigatório.")]
        public string RG { get; set; }

        [Required(ErrorMessage = "Endereço é obrigatório.")]
        public string Endereco { get; set; }

        [Required(ErrorMessage = "Telefone é obrigatório.")]
        [RegularExpression(@"^\(?\d{2}\)?[\s-]?\d{4,5}-?\d{4}$", ErrorMessage = "Telefone em formato inválido.")]
        public string Telefone { get; set; }
        public bool Cancelada { get; set; } = false;
        public string? MotivoCancelamento { get; set; }
        public DateTime? DataCancelamento { get; set; }
        public DateTime DataEnvio { get; set; }
        public string Status { get; set; } = "Pendente";
        public ICollection<RespostaCandidatura> Respostas { get; set; }
        public ICollection<HistoricoStatusCandidatura> HistoricoStatus { get; set; }
        public ICollection<EtapaProcessoSeletivo> Etapas { get; set; }
        public string? ComentarioInterno { get; set; }
        public string? Feedback { get; set; }
        public EntrevistaAgendada EntrevistaAgendada { get; set; }
        public ICollection<MensagemCandidatura> Mensagens { get; set; }



    }

}
