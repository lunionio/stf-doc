namespace Doc.Dominio.Entidades
{
    public class Documento: Base
    {
        public string Numero { get; set; }
        public int Tipo { get; set; }
        public virtual DocumentoTipo DocumentoTipo { get; set; }
    }
}
