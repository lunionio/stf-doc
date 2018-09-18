namespace Doc.Dominio.Entidades
{
    public class DocumentoLog: Base
    {
        public string Ip { get; set; }

        public DocumentoLog(string ip, string descricao, int usuarioCriacao, int idCliente)
        {
            Ip = ip;
            Descricao = descricao;
            UsuarioCriacao = usuarioCriacao;
            IdCliente = idCliente;
        }
    }
}
