using Microsoft.EntityFrameworkCore;

namespace PostManagement.Models
{
    public class PostManagementDbContext : DbContext
    {
        public PostManagementDbContext(DbContextOptions<PostManagementDbContext> options)
            : base(options) { }

        public virtual DbSet<AppUsers> AppUsers { get; set; }
        public virtual DbSet<Posts> Posts { get; set; }
        public virtual DbSet<PostCategories> PostCategories { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AppUsers>()
           .HasKey(c => c.UserId);

            modelBuilder.Entity<AppUsers>()
                .Property(c => c.UserId)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Posts>()
                .HasKey(o => o.PostId);

            modelBuilder.Entity<Posts>()
                .Property(o => o.PostId)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Posts>()
                .HasOne(c => c.Users)
                .WithMany(o => o.Posts)
                .HasForeignKey(o => o.AuthorId);

            modelBuilder.Entity<PostCategories>()
                .HasKey(c => c.CategoryId);

            modelBuilder.Entity<PostCategories>()
                .Property(c => c.CategoryId)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Posts>()
                .HasOne(c => c.PostCategories)
                .WithMany(o => o.Posts)
                .HasForeignKey(o => o.CatergoryId);

            modelBuilder.Entity<Posts>()
                .HasIndex(p => p.AuthorId)
                .IsUnique(false); // This ensures the index is not unique

            modelBuilder.Entity<Posts>()
                .HasIndex(p => p.CatergoryId)
                .IsUnique(false); // This ensures the index is not unique
        }
    }
}
