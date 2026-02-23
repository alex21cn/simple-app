using System.ComponentModel.DataAnnotations;

namespace api.Models
{
    /// <summary>
    /// DTO for creating a FileReport.
    /// </summary>
    public class FileReportCreateDto
    {
        [MaxLength(255)]
        public string? Filename { get; set; }

        [MaxLength(255)]
        public string? Type { get; set; }

        public string? Path { get; set; }

        [Required]
        public DateTime PostedDate { get; set; }

        [Required]
        public DateTime LastUpdatedDate { get; set; }

        [Range(0, int.MaxValue)]
        public int NumberOfLinkedNodes { get; set; } = 0;

        [MaxLength(255)]
        public string? LinkedNodes { get; set; }
    }
}
