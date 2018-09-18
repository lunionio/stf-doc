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
    public class DocumentoLogCross : ICross<DocumentoLog>
    {
        private DocumentoLogRepository _logRepository;
        private SegurancaService _service;

        public DocumentoLogCross(DocumentoLogRepository repository, SegurancaService service)
        {
            _logRepository = repository;
            _service = service;
        }

        public async Task DeleteAsync(DocumentoLog entity, string token)
        {
            try
            {
                await _service.ValidateTokenAsync(token);
                _logRepository.Remove(entity);
            }
            catch(InvalidTokenException e)
            {
                throw e;
            }
            catch(Exception e)
            {
                throw new LogException("Não foi possível deletar o registro de log.", e);
            }
        }

        public async Task<IEnumerable<DocumentoLog>> GetAllAsync(string token)
        {
            try
            {
                await _service.ValidateTokenAsync(token);
                var logs = _logRepository.GetAll();
                return logs;
            }
            catch(InvalidTokenException e)
            {
                throw e;
            }
            catch(Exception e)
            {
                throw new LogException("Não foi possível listar os registros de log.", e);
            }
        }

        public async Task<DocumentoLog> GetByIdAsync(int entityId, int idCliente, string token)
        {
            try
            {
                await _service.ValidateTokenAsync(token);
                var log = _logRepository.GetList(l => l.IdCliente.Equals(idCliente)
                    && l.ID.Equals(entityId)).SingleOrDefault();
                return log;
            }
            catch(InvalidTokenException e)
            {
                throw e;
            }
            catch(Exception e)
            {
                throw new LogException("Não foi possível recuperar o registro de log.", e);
            }
        }

        public async Task<DocumentoLog> SaveAsync(DocumentoLog entity, string token)
        {
            try
            {
                await _service.ValidateTokenAsync(token);
                entity.DataCriacao = DateTime.UtcNow;
                entity.DataEdicao = DateTime.UtcNow;
                entity.ID = _logRepository.Add(entity);
                return entity;
            }
            catch(InvalidTokenException e)
            {
                throw e;
            }
            catch(Exception e)
            {
                throw new LogException("Não foi possível salvar o registro de log.", e);
            }
        }

        public async Task<DocumentoLog> UpdateAsync(DocumentoLog entity, string token)
        {
            try
            {
                await _service.ValidateTokenAsync(token);
                entity.DataEdicao = DateTime.UtcNow;
                _logRepository.Update(entity);
                return entity;
            }
            catch(InvalidTokenException e)
            {
                throw e;
            }
            catch(Exception e)
            {
                throw new LogException("Não foi possível atualizar o registro de log.", e);
            }
        }
    }
}