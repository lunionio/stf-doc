using Doc.Dominio.Entidades;
using Doc.Infra.Cross.Exceptions;
using Doc.Infra.Cross.Generics;
using Doc.Repositorio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doc.Infra.Cross
{
    public class DocumentoCross : ICross<Documento>
    {
        private DocumentoRepository _repository;
        private SegurancaService _service;

        public DocumentoCross(DocumentoRepository repository, SegurancaService service)
        {
            _repository = repository;
            _service = service;
        }

        public async Task DeleteAsync(Documento entity, string token)
        {
            try
            {
                await _service.ValidateTokenAsync(token);
                entity.Status = 9;
                entity.Ativo = false;
                _repository.Update(entity);
            }
            catch (InvalidTokenException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw new DocumentoException("Não foi desabilitar o documento.", e);
            }
        }

        public async Task<IEnumerable<Documento>> GetAllAsync(int idCliente, string token)
        {
            try
            {
                await _service.ValidateTokenAsync(token);
                var result = _repository.GetList(d => d.IdCliente.Equals(idCliente));
                return result;
            }
            catch (InvalidTokenException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw new DocumentoException("Não foi possível recuperar a lista de documentos.", e);
            }
        }

        public async Task<Documento> GetByIdAsync(int entityId, int idCliente, string token)
        {
            try
            {
                await _service.ValidateTokenAsync(token);
                var document = _repository.GetList(d => d.ID.Equals(entityId) && d.IdCliente.Equals(idCliente)).SingleOrDefault();
                return document;
            }
            catch (InvalidTokenException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw new DocumentoException("Não foi possível recuperar o documento solicitado.", e);
            }
        }

        public async Task<Documento> SaveAsync(Documento entity, string token)
        {
            try
            {
                await _service.ValidateTokenAsync(token);
                entity.DataCriacao = DateTime.UtcNow;
                entity.DataEdicao = DateTime.UtcNow;
                entity.Ativo = true;

                var dId = _repository.Add(entity);

                return entity;
            }
            catch (InvalidTokenException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw new DocumentoException("Não foi possível completar a operação.", e);
            }
        }

        public async Task<Documento> UpdateAsync(Documento entity, string token)
        {
            try
            {
                await _service.ValidateTokenAsync(token);
                entity.DataEdicao = DateTime.UtcNow;
                _repository.Update(entity);

                return entity;
            }
            catch (InvalidTokenException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw new DocumentoException("Não foi possível completar a operação.", e);
            }
        }

        public async Task<IEnumerable<Documento>> GetByClienteAsync(int idCliente, string token)
        {
            try
            {
                await _service.ValidateTokenAsync(token);
                var documentos = _repository.GetList(d => d.IdCliente.Equals(idCliente));
                return documentos;
            }
            catch (InvalidTokenException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw new DocumentoException("Não foi possível completar a operação.", e);
            }
        }
    }
}
