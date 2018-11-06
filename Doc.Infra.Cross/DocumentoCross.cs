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
        private DocStatusObservacoesRepository _dObsRep;
        private SegurancaService _service;
        private DocumentoTipoRepository _tRepository;

        public DocumentoCross(DocumentoRepository repository, SegurancaService service, 
            DocumentoStatusRepository sRepository, DocumentoTipoRepository tRepository, DocStatusObservacoesRepository dObsRep)
        {
            _repository = repository;
            _service = service;
            _sRepository = sRepository;
            _tRepository = tRepository;
            _dObsRep = dObsRep;
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
                        entity = await UpdateAsync(entity, token);
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

                if(entity.StatusObservacoes != null && entity.StatusObservacoes.ID > 0)
                {
                    _dObsRep.Update(entity.StatusObservacoes);
                }
                else if(entity.StatusObservacoes != null)
                {
                    var id = _dObsRep.Add(entity.StatusObservacoes);
                    entity.StatusObservacoes.ID = id;
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

        public async Task<IEnumerable<Documento>> GetByClienteAsync(int idCliente, string token)
        {
            try
            {
                await _service.ValidateTokenAsync(token);
                var documentos = _repository.GetList(d => d.IdCliente.Equals(idCliente) && d.Status != 9);
                var docIds = documentos.Select(d => d.ID);

                var observacoes = _dObsRep.GetList(o => docIds.Contains(o.DocID));

                foreach (var item in documentos)
                {
                    item.StatusObservacoes = observacoes.SingleOrDefault(o => o.DocID.Equals(item.ID));
                }

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

        public async Task<IEnumerable<Documento>> GetByCodigoExterno(int codigoExterno, int idCliente, string token)
        {
            try
            {
                await _service.ValidateTokenAsync(token);
                var documentos = _repository.GetList(d => d.IdCliente.Equals(idCliente)
                                    && d.CodigoExterno.Equals(codigoExterno));

                var docIds = documentos.Select(d => d.ID);

                var docTipos = documentos.Select(d => d.Tipo);

                var tipos = _tRepository.GetList(t => docTipos.Contains(t.ID));
                var statuses = _sRepository.GetAll();

                foreach (var item in documentos)
                {
                    item.DocumentoTipo = tipos.Where(t => t.ID.Equals(item.Tipo)).SingleOrDefault();
                    item.DocumentoStatus = statuses.FirstOrDefault(s => s.ID.Equals(item.DocumentoStatusID));
                    item.StatusObservacoes = _dObsRep.GetSingle(o => docIds.Contains(o.DocID));
                }

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

        public async Task<IEnumerable<DocumentoStatus>> GetDocumentoStatusesAsync(string token)
        {
            try
            {
                await _service.ValidateTokenAsync(token);

                var statuses = _sRepository.GetAll();
                return statuses;
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

        public async Task SaveAsync(IEnumerable<Documento> entities, string token)
        {
            try
            {
                await _service.ValidateTokenAsync(token);
                
                foreach (var entity in entities)
                {
                    entity.DataEdicao = DateTime.UtcNow;
                    _repository.Update(entity);

                    if (entity.StatusObservacoes != null && entity.StatusObservacoes.ID > 0)
                    {
                        _dObsRep.Update(entity.StatusObservacoes);
                    }
                    else if (entity.StatusObservacoes != null)
                    {
                        var id = _dObsRep.Add(entity.StatusObservacoes);
                        entity.StatusObservacoes.ID = id;
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
