namespace Doc.Dominio.Entidades
{
    public class Documento: Base
    {
        public byte[] Conteudo { get; set; }
        public string Numero { get; set; }
        public int Tipo { get; set; }
        public virtual DocumentoTipo DocumentoTipo { get; set; }
    }
}
