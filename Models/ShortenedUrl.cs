using System.ComponentModel.DataAnnotations;

namespace UrlShortener.Api.Models;

public class ShortenedUrl
{
    [Key] 
    public Guid Id { get; set; } 

    [Required] 
    public string OriginalUrl { get; set; } = string.Empty; 

    [Required]
    public string ShortCode { get; set; } = string.Empty; 

    [Required]
    public string ShortUrl { get; set; } = string.Empty; 

    public DateTime CreatedAtUtc { get; set; } 

    public int ClickCount { get; set; } = 0;

    public ShortenedUrl()
    {
        
        Id = Guid.NewGuid();
        CreatedAtUtc = DateTime.UtcNow; 
    }
}