using System.ComponentModel.DataAnnotations;

namespace PostManagement.Models
{
    public class PostCategories
    {
        [Key]
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        // Navigation property
        public IEnumerable<Posts>? Posts { get; set; }
    }
}
