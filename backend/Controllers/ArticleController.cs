using Microsoft.AspNetCore.Mvc;
using KnowledgeApi.Data;
using KnowledgeApi.DTOs;
using KnowledgeApi.Models;
using KnowledgetApi.Models.Enums;
using KnowledgeApi.Services;
using Microsoft.EntityFrameworkCore;

namespace KnowledgeApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ArticlesController : ControllerBase
    {
        private readonly IArticleService _articleService;
        private readonly ILogger<ArticlesController> _logger;

        public ArticlesController(IArticleService articleService, ILogger<ArticlesController> logger)
        {
            _articleService = articleService;
            _logger = logger;
        }

        /// <summary>
        /// Alle Artikel abrufen (neueste zuerst)
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ArticleDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ArticleDto>>> GetAll()
        {
            _logger.LogInformation("Fetching all articles");
            try
            {
                var articles = await _articleService.GetAllArticlesAsync();
                return Ok(articles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching articles");
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "Fehler beim Abrufen der Artikel" });
            }
        }

        /// <summary>
        /// Einzelnen Artikel nach ID abrufen
        /// </summary>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ArticleDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ArticleDto>> GetById(Guid id)
        {
            _logger.LogInformation("Fetching article with ID: {ArticleId}", id);
            
            if (id == Guid.Empty)
                return BadRequest(new { message = "Ungültige Artikel-ID" });

            try
            {
                var article = await _articleService.GetArticleByIdAsync(id);
                if (article == null)
                {
                    _logger.LogWarning("Article not found with ID: {ArticleId}", id);
                    return NotFound(new { message = $"Artikel mit ID '{id}' nicht gefunden" });
                }

                return Ok(article);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching article with ID: {ArticleId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Fehler beim Abrufen des Artikels" });
            }
        }

        /// <summary>
        /// Neuen Artikel erstellen
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ArticleDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ArticleDto>> Create([FromBody] CreateArticleDto createArticleDto)
        {
            _logger.LogInformation("Creating new article");

            // Validierung
            if (createArticleDto == null)
                return BadRequest(new { message = "Request Body ist erforderlich" });

            if (string.IsNullOrWhiteSpace(createArticleDto.Title))
                return BadRequest(new { message = "Titel ist erforderlich" });

            if (string.IsNullOrWhiteSpace(createArticleDto.Content))
                return BadRequest(new { message = "Inhalt ist erforderlich" });

            if (createArticleDto.Title.Length > 500)
                return BadRequest(new { message = "Titel darf maximal 500 Zeichen lang sein" });

            try
            {
                var articleDto = await _articleService.CreateArticleAsync(createArticleDto);
                _logger.LogInformation("Article created successfully with ID: {ArticleId}", articleDto.Id);
                
                return CreatedAtAction(nameof(GetById), new { id = articleDto.Id }, articleDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating article");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Fehler beim Erstellen des Artikels" });
            }
        }

        /// <summary>
        /// Artikel aktualisieren
        /// </summary>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateArticleDto updateArticleDto)
        {
            _logger.LogInformation("Updating article with ID: {ArticleId}", id);

            if (id == Guid.Empty)
                return BadRequest(new { message = "Ungültige Artikel-ID" });

            if (updateArticleDto == null)
                return BadRequest(new { message = "Request Body ist erforderlich" });

            if (string.IsNullOrWhiteSpace(updateArticleDto.Title))
                return BadRequest(new { message = "Titel ist erforderlich" });

            if (string.IsNullOrWhiteSpace(updateArticleDto.Content))
                return BadRequest(new { message = "Inhalt ist erforderlich" });

            try
            {
                var success = await _articleService.UpdateArticleAsync(id, updateArticleDto);
                if (!success)
                {
                    _logger.LogWarning("Article not found for update with ID: {ArticleId}", id);
                    return NotFound(new { message = $"Artikel mit ID '{id}' nicht gefunden" });
                }

                _logger.LogInformation("Article updated successfully with ID: {ArticleId}", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating article with ID: {ArticleId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Fehler beim Aktualisieren des Artikels" });
            }
        }

        /// <summary>
        /// Artikel löschen
        /// </summary>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(Guid id)
        {
            _logger.LogInformation("Deleting article with ID: {ArticleId}", id);

            if (id == Guid.Empty)
                return BadRequest(new { message = "Ungültige Artikel-ID" });

            try
            {
                var success = await _articleService.DeleteArticleAsync(id);
                if (!success)
                {
                    _logger.LogWarning("Article not found for deletion with ID: {ArticleId}", id);
                    return NotFound(new { message = $"Artikel mit ID '{id}' nicht gefunden" });
                }

                _logger.LogInformation("Article deleted successfully with ID: {ArticleId}", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting article with ID: {ArticleId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Fehler beim Löschen des Artikels" });
            }
        }
    }
}
