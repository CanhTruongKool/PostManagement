using System.ComponentModel.DataAnnotations;

namespace PostManagement.Models
{
    public class Posts
    {
        [Key]
        public int PostId { get; set; }
        [Required]
        public int AuthorId { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public int PublishStatus { get; set; }
        public int CatergoryId { get; set; }

        // Navigation property
        public AppUsers? Users { get; set; }

        public PostCategories? PostCategories { get; set; }
    }
}
