namespace KnowledgeApi.Dtos
{
    public class ArticleDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public int Category { get; set; }
        public List<string> Tags { get; set; } = new();
    }
}
