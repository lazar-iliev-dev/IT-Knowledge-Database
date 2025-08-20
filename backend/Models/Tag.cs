namespace KnowledgeApi.Models
{
    /// <summary>
    /// Represents a tag associated with an article, including its description and reference to the related article.
    /// </summary>
    public class Tag
    {
        public Guid Id { get; set; }
        public string Description { get; set; } = "";
        
        // Foreign Key
        public Guid ArticleId { get; set; }
        public Article? Article { get; set; }
    }
}