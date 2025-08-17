namespace KnowledgeApi.Models
{
    public class ArticleTag
    {
        public Guid Id { get; set; }
        public string Description { get; set; } = "";

        // Foreign Key
        public Guid ArticleId { get; set; }
        public Article? Article { get; set; }
    }
}