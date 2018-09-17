using Doc.Dominio.Entidades;
using Doc.Infra.Cross;
using Doc.Infra.Cross.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Doc.Service.Controllers
{
    [Produces("application/json")]
    [Route("api/Documentos")]
    public class DocumentosController : Controller
    {
        private DocumentoCross _dContext;

        public DocumentosController([FromServices]DocumentoCross cross)
        {
            _dContext = cross;
        }

        [HttpPost("{token}")]
        public async Task<IActionResult> SaveAsync([FromRoute]string token, [FromBody]Documento documento)
        {
            try
            {
                var doc = await _dContext.SaveAsync(documento, token);
                return Ok("Documento salvo com sucesso.");
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

        [HttpPut("{token}")]
        public async Task<IActionResult> UpdateAsync([FromRoute]string token, [FromBody]Documento documento)
        {
            try
            {
                //Salvar na tabela de Log...

                var doc = await _dContext.UpdateAsync(documento, token);
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
            catch (Exception e)
            {
                return StatusCode(500, "Ocorreu um erro ao tentar salvar o documento recebido. Entre em contato com o suporte.");
            }
        }

        [HttpDelete("{token}")]
        public async Task<IActionResult> DeleteAsync([FromRoute]string token, [FromBody]Documento documento)
        {
            try
            {
                await _dContext.DeleteAsync(documento, token);
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

        [HttpGet("{idCliente:int}/{token}")]
        public async Task<IActionResult> GetAllAsync([FromRoute]int idCliente, [FromRoute]string token)
        {
            try
            {
                var documentos = await _dContext.GetAllAsync(idCliente, token);
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
    }
}