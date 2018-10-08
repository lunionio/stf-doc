using System;

namespace Doc.Service.Models
{
    public class BaseViewModel
    {
        public int ID { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime DateAlteracao { get; set; }
        public int UsuarioCriacao { get; set; }
        public int UsuarioEdicao { get; set; }
        public bool Ativo { get; set; }
        public int Status { get; set; }
        public int idCliente { get; set; }
    }
}