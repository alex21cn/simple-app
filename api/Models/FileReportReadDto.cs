namespace api.Models
{
    /// <summary>
    /// DTO returned to callers for FileReport.
    /// </summary>
    public class FileReportReadDto
    {
        public int Id { get; set; }
        public string? Filename { get; set; }
        public string? Type { get; set; }
        public string? Path { get; set; }
        public DateTime PostedDate { get; set; }
        public DateTime LastUpdatedDate { get; set; }
        public int NumberOfLinkedNodes { get; set; }
        public string? LinkedNodes { get; set; }
    }
}
