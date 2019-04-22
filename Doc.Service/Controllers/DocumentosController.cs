using Doc.Dominio.Entidades;
using Doc.Infra.Cross;
using Doc.Infra.Cross.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Doc.Service.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class DocumentosController : Controller
    {
        private DocumentoCross _dContext;
        private DocumentoLogCross _logContext;

        public DocumentosController([FromServices]DocumentoCross documentoCross, [FromServices]DocumentoLogCross logCross)
        {
            _dContext = documentoCross;
            _logContext = logCross;
        }

        [HttpPost("{token}")]
        public async Task<IActionResult> SaveAsync([FromRoute]string token, [FromBody]Documento documento)
        {
            try
            {
                var result = _dContext.GetByNumero(documento.Numero);

                if(result != null && result.ID > 0)
                {
                    return Ok(false);
                }

                var doc = await _dContext.SaveAsync(documento, token);
                return Ok(doc);
            }
            catch (InvalidTokenException e)
            {
                return StatusCode(401, $"{ e.Message } { e.InnerException.Message }");
            }
            catch (DocumentoException e)
            {
                return StatusCode(400, $"{ e.Message } { e.InnerException.Message }");
            }
            catch (Exception e)
            {
                return StatusCode(500, "Ocorreu um erro ao tentar salvar o documento recebido. Entre em contato com o suporte.");
            }
        }

        [HttpPut("{ipUsuario}/{token}")]
        public async Task<IActionResult> UpdateAsync([FromRoute]string token, [FromBody]Documento documento, [FromRoute]string ipUsuario)
        {
            try
            {
                var doc = await _dContext.UpdateAsync(documento, token);
                var dLog = new DocumentoLog(ipUsuario, $"Documento { documento.ID } foi alterado pelo usuário { documento.UsuarioEdicao }. Status atual do documento: { documento.Status }.", documento.UsuarioEdicao, documento.IdCliente)
                {
                    UsuarioEdicao = documento.UsuarioEdicao,
                    Ativo = true,
                    Nome = "UPDATE",
                    Status = 1,
                };
                var log = await _logContext.SaveAsync(dLog, token);

                return Ok("Documento atualizado com sucesso.");
            }
            catch (InvalidTokenException e)
            {
                return StatusCode(401, $"{ e.Message } { e.InnerException.Message }");
            }
            catch (DocumentoException e)
            {
                return StatusCode(400, $"{ e.Message } { e.InnerException.Message }");
            }
            catch (LogException e)
            {
                return StatusCode(400, $"{ e.Message } { e.InnerException.Message }");
            }
            catch (Exception e)
            {
                return StatusCode(500, "Ocorreu um erro ao tentar salvar o documento recebido. Entre em contato com o suporte.");
            }
        }

        [HttpPost("{ipUsuario}/{token}")]
        public async Task<IActionResult> DeleteAsync([FromRoute]string token, [FromBody]Documento documento, [FromRoute]string ipUsuario)
        {
            try
            {
                await _dContext.DeleteAsync(documento, token);
                var dLog = new DocumentoLog(ipUsuario, $"Documento { documento.ID } foi alterado pelo usuário { documento.UsuarioEdicao }. Status atual do documento: { documento.Status }.", documento.UsuarioEdicao, documento.IdCliente)
                {
                    UsuarioEdicao = documento.UsuarioEdicao,
                    Ativo = true,
                    Nome = "DELETE",
                    Status = 1,
                };

                var log = await _logContext.SaveAsync(dLog, token);

                return Ok("Documento deletado com sucesso.");

            }
            catch (InvalidTokenException e)
            {
                return StatusCode(401, $"{ e.Message } { e.InnerException.Message }");
            }
            catch (DocumentoException e)
            {
                return StatusCode(400, $"{ e.Message } { e.InnerException.Message }");
            }
            catch(LogException e)
            {
                return StatusCode(400, $"{ e.Message } { e.InnerException.Message }");
            }
            catch (Exception e)
            {
                return StatusCode(500, "Ocorreu um erro ao tentar deletar o documento. Entre em contato com o suporte.");
            }
        }

        [HttpGet("{idCliente:int}/{idDocumento:int}/{token}")]
        public async Task<IActionResult> GetByIdAsync([FromRoute]int idCliente, [FromRoute]int idDocumento, [FromRoute]string token)
        {
            try
            {
                var documento = await _dContext.GetByIdAsync(idDocumento, idCliente, token);
                return Ok(documento);
            }
            catch (InvalidTokenException e)
            {
                return StatusCode(401, $"{ e.Message } { e.InnerException.Message }");
            }
            catch (DocumentoException e)
            {
                return StatusCode(400, $"{ e.Message } { e.InnerException.Message }");
            }
            catch (Exception e)
            {
                return StatusCode(500, "Ocorreu um erro ao buscar documento. Entre em contato com o suporte.");
            }
        }

        [HttpGet("{idCliente:int}/{token}")]
        public async Task<IActionResult> GetByClienteIdAsync([FromRoute]int idCliente, [FromRoute]string token)
        {
            try
            {
                var documento = await _dContext.GetByClienteAsync(idCliente, token);
                return Ok(documento);
            }
            catch (InvalidTokenException e)
            {
                return StatusCode(401, $"{ e.Message } { e.InnerException.Message }");
            }
            catch (DocumentoException e)
            {
                return StatusCode(400, $"{ e.Message } { e.InnerException.Message }");
            }
            catch (Exception e)
            {
                return StatusCode(500, "Ocorreu um erro ao buscar documento. Entre em contato com o suporte.");
            }
        }

        [HttpGet("{token}")]
        public async Task<IActionResult> GetAllAsync([FromRoute]string token)
        {
            try
            {
                var documentos = await _dContext.GetAllAsync(token);
                return Ok(documentos);
            }
            catch (InvalidTokenException e)
            {
                return StatusCode(401, $"{ e.Message } { e.InnerException.Message }");
            }
            catch (DocumentoException e)
            {
                return StatusCode(400, $"{ e.Message } { e.InnerException.Message }");
            }
            catch (Exception e)
            {
                return StatusCode(500, "Ocorreu um erro ao buscar os documentos. Entre em contato com o suporte.");
            }
        }

        [HttpGet("Validate/{codigoExterno:int}/{idCliente:int}/{token}")]
        public async Task<IActionResult> ValidateAsync([FromRoute]int codigoExterno, [FromRoute]int idCliente, [FromRoute]string token)
        {
            try
            {
                await _dContext.ValidateAsync(codigoExterno, idCliente, token);
                return Ok(true);
            }
            catch (InvalidTokenException e)
            {
                return new ObjectResult(false);
            }
            catch (DocumentoException e)
            {
                return new ObjectResult(false);
            }
            catch (Exception e)
            {
                return new ObjectResult(false);
            }
        }

        [HttpGet("GetByCodigo/{codigoExterno:int}/{idCliente:int}/{token}")]
        public async Task<IActionResult> GetByCodigoExternoAsync([FromRoute]int codigoExterno, [FromRoute]int idCliente, [FromRoute]string token)
        {
            try
            {
                var documentos = await _dContext.GetByCodigoExterno(codigoExterno, idCliente, token);
                return Ok(documentos);
            }
            catch (InvalidTokenException e)
            {
                return StatusCode(401, $"{ e.Message } { e.InnerException.Message }");
            }
            catch (DocumentoException e)
            {
                return StatusCode(400, $"{ e.Message } { e.InnerException.Message }");
            }
            catch (Exception e)
            {
                return StatusCode(500, "Ocorreu um erro ao buscar os documentos. Entre em contato com o suporte.");
            }
        }

        [HttpGet("GetAllStatus/{token}")]
        public async Task<IActionResult> GetDocumentoStatuses([FromRoute]string token)
        {
            try
            {
                var statuses = await _dContext.GetDocumentoStatusesAsync(token);
                return Ok(statuses);
            }
            catch (InvalidTokenException e)
            {
                return StatusCode(401, $"{ e.Message } { e.InnerException.Message }");
            }
            catch (DocumentoException e)
            {
                return StatusCode(400, $"{ e.Message } { e.InnerException.Message }");
            }
            catch (Exception e)
            {
                return StatusCode(500, "Ocorreu um erro ao buscar os statuses. Entre em contato com o suporte.");
            }
        }

        [HttpPost("UpdateDocumentos/{token}")]
        public async Task<IActionResult> UpdateDocumentos([FromRoute]string token, [FromBody]IEnumerable<Documento> documentos)
        {
            try
            {
                await _dContext.SaveAsync(documentos, token);
                return Ok("Documentos atualizados com sucesso.");
            }
            catch (InvalidTokenException e)
            {
                return StatusCode(401, $"{ e.Message } { e.InnerException.Message }");
            }
            catch (DocumentoException e)
            {
                return StatusCode(400, $"{ e.Message } { e.InnerException.Message }");
            }
            catch (Exception e)
            {
                return StatusCode(500, "Ocorreu um erro ao tentar atualizar os documentos recebidos. Entre em contato com o suporte.");
            }
        }

        [HttpGet("Tipos/{token}")]
        public async Task<IActionResult> GetAllTiposAsync([FromRoute]string token)
        {
            try
            {
                var tipos = await _dContext.GetTiposAsync(token);
                return Ok(tipos);
            }
            catch (InvalidTokenException e)
            {
                return StatusCode(401, $"{ e.Message } { e.InnerException.Message }");
            }
            catch (ServiceException e)
            {
                return StatusCode(401, $"{ e.Message } { e.InnerException.Message }");
            }
            catch (DocumentoException e)
            {
                return StatusCode(400, $"{ e.Message } { e.InnerException.Message }");
            }
            catch (Exception e)
            {
                return StatusCode(500, "Ocorreu um erro ao buscar os documentos. Entre em contato com o suporte.");
            }
        }
    }
}