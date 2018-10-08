using System.ComponentModel.DataAnnotations.Schema;

namespace Doc.Dominio.Entidades
{
    public class Documento: Base
    {
        public byte[] Arquivo { get; set; }
        public string Numero { get; set; }
        public int Tipo { get; set; }
        public bool Requerido { get; set; }
        public int CodigoExterno { get; set; }
        public int DocumentoStatusID { get; set; }

        [NotMapped]
        public DocumentoTipo DocumentoTipo { get; set; }
    }
}
