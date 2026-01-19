namespace KnowledgeApi.Models
{
    public class ArticleTags
    {
        public Guid Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public Guid ArticleId { get; set; }
        public Article? Article { get; set; }
    }
}
