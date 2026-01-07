using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using MoonstoneTCC.Models;


public class ComentarioCurtida
{
    public int Id { get; set; }

    [Required]
    public int ComentarioId { get; set; }

    [Required]
    public string UsuarioId { get; set; }

    [ForeignKey("ComentarioId")]
    public ComentarioJogo Comentario { get; set; }

    [ForeignKey("UsuarioId")]
    public IdentityUser Usuario { get; set; }
}
