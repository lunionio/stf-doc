using Doc.Dominio.Entidades;
using Doc.Infra.Cross.Exceptions;
using Doc.Infra.Cross.Generics;
using Doc.Repositorio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Doc.Infra.Cross
{
    public class DocumentoCross : ICross<Documento>
    {
        private DocumentoRepository _repository;
        private DocumentoStatusRepository _sRepository;
        private SegurancaService _service;

        public DocumentoCross(DocumentoRepository repository, 
            SegurancaService service, DocumentoStatusRepository sRepository)
        {
            _repository = repository;
            _service = service;
            _sRepository = sRepository;
        }

        public async Task DeleteAsync(Documento entity, string token)
        {
            try
            {
                await _service.ValidateTokenAsync(token);
                entity.Status = 9;
                entity.Ativo = false;
                entity.DataEdicao = DateTime.UtcNow;
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

        public async Task<IEnumerable<Documento>> GetAllAsync(string token)
        {
            try
            {
                await _service.ValidateTokenAsync(token);
                var result = _repository.GetList(d => d.Status != 9);
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
                var document = _repository.GetList(d => d.ID.Equals(entityId) && d.IdCliente.Equals(idCliente) && d.Status != 9).SingleOrDefault();
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

                switch (entity.ID)
                {
                    case 0:
                        entity.DataCriacao = DateTime.UtcNow;
                        entity.DataEdicao = DateTime.UtcNow;
                        entity.DocumentoStatusID = 1; //Pendente
                        entity.Ativo = true;

                        var dId = _repository.Add(entity);
                        break;
                    default:
                        entity.DataEdicao = DateTime.UtcNow;
                        _repository.Update(entity);
                        break;
                }

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
                var documentos = _repository.GetList(d => d.IdCliente.Equals(idCliente) && d.Status != 9);
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

        public async Task ValidateAsync(int codigoExterno, int idCliente, string token)
        {
            try
            {
                await _service.ValidateTokenAsync(token);
                var documentos = _repository.GetList(d => d.IdCliente.Equals(idCliente) 
                                    && d.CodigoExterno.Equals(codigoExterno));

                var statuses = _sRepository.GetAll();

                foreach (var documento in documentos)
                {
                    if (documento.Requerido)
                    {
                        var status = statuses.FirstOrDefault(s => s.ID.Equals(documento.DocumentoStatusID));

                        if (status == null)
                        {
                            throw new DocumentoException("Não foram encontrados os status dos documentos.");
                        }

                        if (status.ID == 1)
                        {
                            throw new DocumentoException("Há documentos pendentes.");
                        }

                        if (status.ID == 3)
                        {
                            throw new DocumentoException("Há documentos reprovados.");
                        }
                    }
                }
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
