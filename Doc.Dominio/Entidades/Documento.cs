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
        public int DocStatusObsID { get; set; }

        [NotMapped]
        public DocumentoTipo DocumentoTipo { get; set; }

        [NotMapped]
        public DocumentoStatus DocumentoStatus { get; set; }

        [NotMapped]
        public DocStatusObservacoes StatusObservacoes { get; set; }
    }
}
