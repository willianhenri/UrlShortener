using UrlShortener.Api.Models;

namespace UrlShortener.Api.Services;

public interface IUrlShorteningService
{
    Task<ShortenedUrl> CreateShortenedUrlAsync(string originalUrl);
    Task<string?> GetOriginalUrlAndTrackClickAsync(string shortCode);
}