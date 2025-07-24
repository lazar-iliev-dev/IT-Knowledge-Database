using Microsoft.EntityFrameworkCore;
using KnowledgeApi.Models;

namespace KnowledgeApi.Data
{
    public class KnowledgeContext : DbContext
    {
        public KnowledgeContext(DbContextOptions<KnowledgeContext> options) : base(options) {}

        public DbSet<Article> Articles { get; set; }
    }
}
