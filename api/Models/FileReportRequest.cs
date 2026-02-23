using System.Text.Json;
using System.Collections.Generic;

namespace api.Models
{
    public class FileReportRequest
    {
        // Flexible dictionary of column -> value
        public Dictionary<string, JsonElement> Columns { get; set; } = new();

        // Optional key column name for update/delete (default: "Id")
        public string? KeyColumn { get; set; }
    }
}
