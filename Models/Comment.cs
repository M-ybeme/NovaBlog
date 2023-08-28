using System.ComponentModel.DataAnnotations;

namespace NovaBlog.Models
{
    public class Comment
    {
        //Primary Key
        public int Id { get; set; }

        //Foriegn Key
        public int BlogPostId { get; set; }

        [Required]
        public string? AuthorId { get; set; }

        [DataType(DataType.Date)]
        public DateTime Created { get; set; }

        [DataType(DataType.Date)]
        public DateTime? LastUpdated { get; set; }

        public string? UpdateReason { get; set; }

        [Required]
        [StringLength(5000, ErrorMessage = "The {0} must be at least {2} and at most {1} characters", MinimumLength = 2)]
        public string? Body { get; set; }


        //Navigation Properties
        public virtual BlogPost? NovaBlog { get; set; }

        public virtual BlogUser? Author { get; set; }

    }
}