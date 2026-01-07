using System;
using System.ComponentModel.DataAnnotations;

namespace MoonstoneTCC.Models
{
    public class ExclusaoConta
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UsuarioId { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string Motivo { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime DataHora { get; set; } = DateTime.Now;
    }
}

