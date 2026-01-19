using KnowledgeApi.Data;
using KnowledgeApi.DTOs;
using KnowledgeApi.Models;
using KnowledgeApi.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace KnowledgeApi.Services
{
    public class ArticleService : IArticleService
    {
        private readonly KnowledgeContext _context;
        private readonly ILogger<ArticleService> _logger;

        public ArticleService(KnowledgeContext context, ILogger<ArticleService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<ArticleDto>> GetAllArticlesAsync()
        {
            try
            {
                var articles = await _context.Articles
                    .Include(a => a.ArticleTags)
                    .OrderByDescending(a => a.CreatedAt)
                    .AsNoTracking()
                    .ToListAsync();

                return articles.Select(MapToDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all articles");
                throw;
            }
        }

        public async Task<ArticleDto?> GetArticleByIdAsync(Guid id)
        {
            try
            {
                var article = await _context.Articles
                    .Include(a => a.ArticleTags)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(a => a.Id == id);

                return article != null ? MapToDto(article) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching article by ID: {ArticleId}", id);
                throw;
            }
        }

        public async Task<ArticleDto> CreateArticleAsync(CreateArticleDto createArticleDto)
        {
            try
            {
                var article = new Article
                {
                    Id = Guid.NewGuid(),
                    Title = createArticleDto.Title.Trim(),
                    Content = createArticleDto.Content.Trim(),
                    Category = (Models.Enums.ArticleCategory)createArticleDto.Category,
                    CreatedAt = DateTime.UtcNow
                };

                // Tags hinzufügen falls vorhanden
                if (createArticleDto.Tags != null && createArticleDto.Tags.Any())
                {
                    foreach (var tagDescription in createArticleDto.Tags.Distinct())
                    {
                        if (!string.IsNullOrWhiteSpace(tagDescription))
                        {
                            article.ArticleTags.Add(new ArticleTags
                            {
                                Id = Guid.NewGuid(),
                                Description = tagDescription.Trim(),
                                ArticleId = article.Id
                            });
                        }
                    }
                }

                _context.Articles.Add(article);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Article created successfully with ID: {ArticleId}", article.Id);
                return MapToDto(article);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating article");
                throw;
            }
        }

        public async Task<bool> UpdateArticleAsync(Guid id, UpdateArticleDto updateArticleDto)
        {
            try
            {
                var article = await _context.Articles
                    .Include(a => a.ArticleTags)
                    .FirstOrDefaultAsync(a => a.Id == id);

                if (article == null)
                    return false;

                article.Title = updateArticleDto.Title.Trim();
                article.Content = updateArticleDto.Content.Trim();
                article.Category = (Models.Enums.ArticleCategory)updateArticleDto.Category;

                // Tags aktualisieren
                if (updateArticleDto.Tags != null)
                {
                    article.ArticleTags.Clear();

                    foreach (var tagDescription in updateArticleDto.Tags.Distinct())
                    {
                        if (!string.IsNullOrWhiteSpace(tagDescription))
                        {
                            article.ArticleTags.Add(new ArticleTags
                            {
                                Id = Guid.NewGuid(),
                                Description = tagDescription.Trim(),
                                ArticleId = article.Id
                            });
                        }
                    }
                }

                _context.Articles.Update(article);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Article updated successfully with ID: {ArticleId}", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating article with ID: {ArticleId}", id);
                throw;
            }
        }

        public async Task<bool> DeleteArticleAsync(Guid id)
        {
            try
            {
                var article = await _context.Articles.FindAsync(id);
                if (article == null)
                    return false;

                _context.Articles.Remove(article);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Article deleted successfully with ID: {ArticleId}", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting article with ID: {ArticleId}", id);
                throw;
            }
        }

        private static ArticleDto MapToDto(Article article) => new()
        {
            Id = article.Id,
            Title = article.Title,
            Content = article.Content,
            Category = article.Category.ToString(),
            Tags = article.ArticleTags.Select(t => t.Description).ToList(),
            CreatedAt = article.CreatedAt
        };
    }
}
