using System.ComponentModel.DataAnnotations.Schema;

namespace Doc.Dominio.Entidades
{
    public class Documento: Base
    {
        public byte[] Conteudo { get; set; }
        public string Numero { get; set; }
        public int Tipo { get; set; }
        [NotMapped]
        public DocumentoTipo DocumentoTipo { get; set; }
    }
}
