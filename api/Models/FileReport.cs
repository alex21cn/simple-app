using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models
{
    [Table("FileReport")]
    public class FileReport
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(255)]
        public string? Filename { get; set; }

        [MaxLength(255)]
        public string? Type { get; set; }

        public string? Path { get; set; }

        public DateTime PostedDate { get; set; }

        public DateTime LastUpdatedDate { get; set; }

        public int NumberOfLinkedNodes { get; set; }

        [MaxLength(255)]
        public string? LinkedNodes { get; set; }
    }
}
