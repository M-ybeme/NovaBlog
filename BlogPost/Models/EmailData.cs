using System.ComponentModel.DataAnnotations;

namespace NovaBlog.Models
{
    public class EmailData
    {
        [Required]
        public string Subject { get; set; } = "";
        [Required]
        public string Body { get; set; } = "";

        public int? BlogUserId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        public string? UserEmail { get; set; }

    }
}
