using System.ComponentModel.DataAnnotations;

namespace KnowledgeApi.DTOs
{
    /// <summary>
    /// Article DTO for API responses
    /// </summary>
    public class ArticleDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public List<string> Tags { get; set; } = new();
        public DateTime CreatedAt { get; set; }
    }
}
