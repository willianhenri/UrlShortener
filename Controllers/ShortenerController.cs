using Microsoft.AspNetCore.Mvc;
using UrlShortener.Api.Dto;
using UrlShortener.Api.Services;

namespace UrlShortener.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShortenerController : ControllerBase
    {
        private readonly IUrlShorteningService _urlShorteningService;
        private readonly ILogger<ShortenerController> _logger;
        
        public ShortenerController(IUrlShorteningService urlShorteningService, ILogger<ShortenerController> logger)
        {
            _urlShorteningService = urlShorteningService;
            _logger = logger;
        }

        [HttpGet("/{shortCode}")]
        [ProducesResponseType(StatusCodes.Status302Found)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RedirectToOriginalUrl(string shortCode)
        {
            _logger.LogInformation("Tentando redirecionar para o código: {ShortCode}", shortCode);

            if (string.IsNullOrWhiteSpace(shortCode))
            {
                _logger.LogWarning("Código curto inválido ou vazio recebido.");
                return NotFound();
            }

            try
            {
                var originalUrl = await _urlShorteningService.GetOriginalUrlAndTrackClickAsync(shortCode);

                if (string.IsNullOrWhiteSpace(originalUrl))
                {
                    _logger.LogWarning("Código curto não encontrado no banco: {{ShortCode}}", shortCode);
                    return NotFound();
                }

                _logger.LogInformation("Código {ShortCode} encontrado. Redirecionando para: {OriginalUrl}", shortCode,
                    originalUrl);
                return Redirect(originalUrl);
            }

            catch(Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao tentar redirecionar para o codigo {ShortCode}", shortCode);
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um erro interno ao processar sua solicitação.");
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(object))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateShortUrl([FromBody] ShortenUrlRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var shortenedUrl = await _urlShorteningService.CreateShortenedUrlAsync(request.OriginalUrl);
                return Ok(shortenedUrl);
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar url curta");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}