using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KnowledgeApi.Data;
using KnowledgeApi.Models;
using KnowledgeApi.Dtos;

namespace KnowledgeApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ArticlesController : ControllerBase
    {
        private readonly KnowledgeContext _context;

        public ArticlesController(KnowledgeContext context)
        {
            _context = context;
        }

        // GET api/articles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ArticleDto>>> GetAll()
        {
            var articles = await _context.Articles
                                         .Include(a => a.ArticleTags)
                                         .ToListAsync();

            var result = articles.Select(a => new ArticleDto
            {
                Id = a.Id,
                Title = a.Title,
                Content = a.Content,
                CreatedAt = a.CreatedAt,
                Category = (int)a.Category,
                Tags = a.ArticleTags.Select(t => t.Description).ToList()
            });

            return Ok(result);
        }

        // GET api/articles/{id}
        [HttpGet("{id:Guid}")]
        public async Task<ActionResult<ArticleDto>> GetById(Guid id)
        {
            var article = await _context.Articles
                                        .Include(a => a.ArticleTags)
                                        .FirstOrDefaultAsync(a => a.Id == id);

            if (article == null)
                return NotFound();

            var result = new ArticleDto
            {
                Id = article.Id,
                Title = article.Title,
                Content = article.Content,
                CreatedAt = article.CreatedAt,
                Category = (int)article.Category,
                Tags = article.ArticleTags.Select(t => t.Description).ToList()
            };

            return Ok(result);
        }

        // POST api/articles
        [HttpPost]
        public async Task<ActionResult<ArticleDto>> Create([FromBody] CreateArticleDto dto)
        {
            var article = new Article
            {
                Id = Guid.NewGuid(),
                Title = dto.Title,
                Content = dto.Content,
                Category = (KnowledgeApi.Models.Enums.ArticleCategory)dto.Category,
                CreatedAt = DateTime.UtcNow,
                ArticleTags = dto.Tags.Select(t => new Tag
                {
                    Id = Guid.NewGuid(),
                    Description = t
                }).ToList()
            };

            _context.Articles.Add(article);
            await _context.SaveChangesAsync();

            var result = new ArticleDto
            {
                Id = article.Id,
                Title = article.Title,
                Content = article.Content,
                CreatedAt = article.CreatedAt,
                Category = (int)article.Category,
                Tags = article.ArticleTags.Select(t => t.Description).ToList()
            };

            return CreatedAtAction(nameof(GetById), new { id = article.Id }, result);
        }

        // PUT api/articles/{id}
        [HttpPut("{id:Guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateArticleDto dto)
        {
            var article = await _context.Articles
                                        .Include(a => a.ArticleTags)
                                        .FirstOrDefaultAsync(a => a.Id == id);

            if (article == null)
                return NotFound();

            article.Title = dto.Title;
            article.Content = dto.Content;
            article.Category = (KnowledgeApi.Models.Enums.ArticleCategory)dto.Category;

            // Tags neu setzen (alte löschen, neue hinzufügen)
            article.ArticleTags.Clear();
            article.ArticleTags = dto.Tags.Select(t => new Tag
            {
                Id = Guid.NewGuid(),
                Description = t,
                ArticleId = article.Id
            }).ToList();

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE api/articles/{id}
        [HttpDelete("{id:Guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var article = await _context.Articles
                                        .Include(a => a.ArticleTags)
                                        .FirstOrDefaultAsync(a => a.Id == id);

            if (article == null)
                return NotFound();

            _context.Tags.RemoveRange(article.ArticleTags);
            _context.Articles.Remove(article);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
