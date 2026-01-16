using System.ComponentModel.DataAnnotations;

namespace KnowledgeApi.DTOs
{
/// <summary>
    /// Artikel DTO für POST/Create-Request
    /// </summary>
    public class CreateArticleDto
    {
        [Required(ErrorMessage = "Titel ist erforderlich")]
        [StringLength(500, MinimumLength = 3, 
            ErrorMessage = "Titel muss zwischen 3 und 500 Zeichen lang sein")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Inhalt ist erforderlich")]
        [StringLength(50000, MinimumLength = 10,
            ErrorMessage = "Inhalt muss zwischen 10 und 50000 Zeichen lang sein")]
        public string Content { get; set; } = string.Empty;

        [Required(ErrorMessage = "Kategorie ist erforderlich")]
        public ArticleCategoryDto Category { get; set; }

        public List<string> Tags { get; set; } = new();
    }
}
