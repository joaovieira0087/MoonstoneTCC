using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using MoonstoneTCC.Models;

namespace MoonstoneTCC.ViewModels
{
    public class ResumoDiaViewModel
    {
        public DateTime DataSelecionada { get; set; }
        public List<IdentityUser> Cadastros { get; set; }
        public List<Pedido> Pedidos { get; set; }
        public List<PerguntaUsuario> Perguntas { get; set; }
        public List<AcaoAdmin> Edicoes { get; set; }
    }
}

