using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NovaBlog.Models
{
    public class Category
    {
        //Prime Key
        public int Id { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at most {1} characters", MinimumLength = 2)]
        public string? Name { get; set; }

        [StringLength(2000, ErrorMessage = "The {0} must be at least {2} and at most {1} characters", MinimumLength = 2)]
        public string? Description { get; set; }

        //Properties for stroing image
        public byte[]? ImageData { get; set; }
        public string? ImageType { get; set; }

        //Properties for passing file info from the form (html) to the post.
        //Also not saved in the db via [notmapped] attribute
        [NotMapped]
        public IFormFile? CategoryImage { get; set; }

        //Navigation Properties
        public virtual ICollection<BlogPost> BlogPosts { get; set; } = new HashSet<BlogPost>();

    }
}