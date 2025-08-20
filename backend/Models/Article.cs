using System.ComponentModel.DataAnnotations.Schema;
using KnowledgeApi.Models.Enums;


namespace KnowledgeApi.Models
{
    public class Article
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = "";
        public string Content { get; set; } = "";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ArticleCategory Category { get; set; }

        // Navigation property zu relationalen Tags
        public List<Tag> ArticleTags { get; set; } = new();
        
        // Bequeme List<string> für Code / UI
        [NotMapped]
        public List<string> Tags
        {
            get => ArticleTags.Select(t => t.Description).ToList();
            set
            {
                ArticleTags.Clear();
                if (value != null)
                {
                    ArticleTags.AddRange(value.Select(description => new Tag { Description = description }));
                }
            }
        }
    }
}

