namespace KnowledgeApi.Dtos
{
    public class CreateArticleDto
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public int Category { get; set; }
        public List<string> Tags { get; set; } = new();
    }
}
