using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KnowledgeApi.Data;
using KnowledgeApi.Models;

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
        public async Task<ActionResult<IEnumerable<Article>>> GetAll()
        {
            return await _context.Articles
                                 .OrderByDescending(a => a.CreatedAt)
                                 .ToListAsync();
        }

        // GET api/articles/5
        [HttpGet("{id:Guid}")]
        public async Task<ActionResult<Article>> GetById(Guid id)
        {
            var article = await _context.Articles.FindAsync(id);
            if (article == null)
                return NotFound();
            return article;
        }

        // POST api/articles
        [HttpPost]
        public async Task<ActionResult<Article>> Create([FromBody] Article article)
        {
            article.CreatedAt = DateTime.UtcNow;
            _context.Articles.Add(article);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = article.Id }, article);
        }

        // PUT api/articles/5
        [HttpPut("{id:Guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] Article updated)
        {
            var article = await _context.Articles.FindAsync(id);
            if (article == null)
                return NotFound();

            article.Title     = updated.Title;
            article.Content   = updated.Content;
            // optional: article.UpdatedAt = DateTime.UtcNow;

            _context.Entry(article).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE api/articles/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int Guid)
        {
            var article = await _context.Articles.FindAsync(Guid);
            if (article == null)
                return NotFound();

            _context.Articles.Remove(article);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
