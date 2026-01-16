using KnowledgeApi.Data;
using KnowledgeApi.DTOs;
using KnowledgeApi.Models;
using Microsoft.EntityFrameworkCore;

namespace KnowledgeApi.Services
{
    public interface IArticleService
    {
        Task<IEnumerable<ArticleDto>> GetAllArticlesAsync();
        Task<ArticleDto?> GetArticleByIdAsync(Guid id);
        Task<ArticleDto> CreateArticleAsync(CreateArticleDto createArticleDto);
        Task<bool> UpdateArticleAsync(Guid id, UpdateArticleDto updateArticleDto);
        Task<bool> DeleteArticleAsync(Guid id);
    }
}