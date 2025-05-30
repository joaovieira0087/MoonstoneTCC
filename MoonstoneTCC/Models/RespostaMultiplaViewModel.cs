public class RespostaMultiplaViewModel
{
    public int ComunicadoId { get; set; }
    public List<RespostaItem> Respostas { get; set; } = new List<RespostaItem>();
}

public class RespostaItem
{
    public int PerguntaId { get; set; }
    public string Texto { get; set; }
}
