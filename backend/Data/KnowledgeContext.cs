using Microsoft.EntityFrameworkCore;
using KnowledgeApi.Models;

namespace KnowledgeApi.Data
{
    public class KnowledgeContext : DbContext
    {
        public KnowledgeContext(DbContextOptions<KnowledgeContext> options) : base(options) { }

        public DbSet<Article> Articles { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Tag> ArticleTags { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Tag>()
                .HasKey(at => new { at.ArticleId, at.Id });
        }
    }
}
