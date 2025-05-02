using Microsoft.EntityFrameworkCore;
using UrlShortener.Api.Data;
using UrlShortener.Api.Models;

namespace UrlShortener.Api.Services;

public class UrlShorteningService : IUrlShorteningService
{
    private readonly AppDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly Random _random = new Random();
    
    public UrlShorteningService(AppDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<string?> GetOriginalUrlAndTrackClickAsync(string shortCode)
    {
        var shortenedUrlEntity = await _dbContext.ShortenedUrls
            .FirstOrDefaultAsync(s => s.ShortCode == shortCode);

        if (shortenedUrlEntity == null)
        {
            return null;
        }
        
        shortenedUrlEntity.ClickCount++;

        try
        {
            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            Console.WriteLine($"Erro de concorrência ao atualizar ClickCount para {shortCode}: {ex.Message}");
        }
        
        return shortenedUrlEntity.OriginalUrl;
    }

    public async Task<ShortenedUrl> CreateShortenedUrlAsync(string originalUrl)
    {
        //validação da url
        if (!Uri.TryCreate(originalUrl, UriKind.Absolute, out var uri))
        {
            throw new ArgumentException("A URL fornecida não é valida", nameof(originalUrl));
        }

        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        const int codeLength = 7;
        string shortCode;
        bool codeExists;
        
        //construindo a url curta
        do
        {
            shortCode = new string(Enumerable.Repeat(chars, codeLength)
                .Select(s => s[_random.Next(s.Length)]).ToArray());
            codeExists = _dbContext.ShortenedUrls.Any(u => u.ShortCode == shortCode);
        } while (codeExists);
        
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
        {
            throw new InvalidOperationException("Não foi possível acessar o HttpContext.");
        }

        var scheme = httpContext.Request.Scheme;
        var host = httpContext.Request.Host;
        var shortUrl = $"{scheme}://{host}/{shortCode}";

        var shortenedUrl = new ShortenedUrl
        {
            OriginalUrl = originalUrl,
            ShortCode = shortCode,
            ShortUrl = shortUrl
        };
        
        await _dbContext.ShortenedUrls.AddAsync(shortenedUrl);
        await _dbContext.SaveChangesAsync();
        
        return shortenedUrl;
    }
}