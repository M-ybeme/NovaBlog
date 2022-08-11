using NovaBlog.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NovaBlog.Models
{
    public class BlogPost
    {
        //primary key
        public int Id { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at most {1} characters", MinimumLength = 2)]
        public string? Title { get; set; }

        [Required]
        public string? Contnet { get; set; }

        [DataType(DataType.Date)]
        public DateTime Created { get; set; }

        [DataType(DataType.Date)]
        public DateTime? LastUpdated { get; set; }
        
        
        //A Foriegn Key
        public int CategoryId { get; set; }

        public string? Slug { get; set; }

        public string? Abstract { get; set; }

        public bool IsDeleted { get; set; }

        [DisplayName ("Published")]
        public bool IsPublished { get; set; }

        //Properties for stroing image
        public byte[]? ImageData { get; set; }
        public string? ImageType { get; set; }

        //Properties for passing file info from the form (html) to the post.
        //Also not saved in the db via [notmapped] attribute
        [NotMapped]
        public IFormFile? BlogPostImage { get; set; }


        // Navigation Properties
        public virtual Category? Category { get; set; }

        public virtual ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();

        public virtual ICollection<Tag> Tags { get; set; } = new HashSet<Tag>();


    }
}
