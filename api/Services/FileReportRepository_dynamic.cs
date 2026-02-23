using Microsoft.Data.SqlClient;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Data;

namespace api.Services
{
    public class FileReportRepositoryDynamic
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        private static readonly Regex _columnNameRegex = new("^[A-Za-z0-9_]+$");

        public FileReportRepositoryDynamic(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Missing DefaultConnection");
        }

        private static void ValidateColumnNames(IEnumerable<string> columns)
        {
            foreach (var c in columns)
            {
                if (!_columnNameRegex.IsMatch(c))
                    throw new ArgumentException($"Invalid column name: {c}");
            }
        }

        private static object JsonElementToObject(JsonElement el)
        {
            return el.ValueKind switch
            {
                JsonValueKind.Number => el.TryGetInt64(out var l) ? (object)l : el.GetDouble(),
                JsonValueKind.String => el.GetString() ?? string.Empty,
                JsonValueKind.True => true,
                JsonValueKind.False => false,
                JsonValueKind.Null => DBNull.Value,
                _ => el.GetRawText()
            };
        }

        public async Task<int> InsertAsync(Dictionary<string, JsonElement> columns, CancellationToken cancellationToken = default)
        {
            if (columns == null || columns.Count == 0) throw new ArgumentException("No columns provided for insert");

            ValidateColumnNames(columns.Keys);

            var colNames = string.Join(", ", columns.Keys);
            var paramNames = string.Join(", ", columns.Keys.Select((c, i) => "@p" + i));

            var sql = $"INSERT INTO [FileReport] ({colNames}) VALUES ({paramNames});";

            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(sql, conn);

            int i = 0;
            foreach (var kv in columns)
            {
                var value = JsonElementToObject(kv.Value);
                cmd.Parameters.AddWithValue("@p" + i, value ?? DBNull.Value);
                i++;
            }

            await conn.OpenAsync(cancellationToken);
            var rows = await cmd.ExecuteNonQueryAsync(cancellationToken);
            return rows;
        }

        public async Task<int> UpdateAsync(string keyColumn, object keyValue, Dictionary<string, JsonElement> columns, CancellationToken cancellationToken = default)
        {
            if (columns == null || columns.Count == 0) throw new ArgumentException("No columns provided for update");
            if (string.IsNullOrWhiteSpace(keyColumn)) throw new ArgumentException("Key column required");

            ValidateColumnNames(columns.Keys.Append(keyColumn));

            var setClause = string.Join(", ", columns.Keys.Select((c, i) => $"[{c}] = @p{i}"));
            var sql = $"UPDATE [FileReport] SET {setClause} WHERE [{keyColumn}] = @key;";

            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(sql, conn);

            int i = 0;
            foreach (var kv in columns)
            {
                var value = JsonElementToObject(kv.Value);
                cmd.Parameters.AddWithValue("@p" + i, value ?? DBNull.Value);
                i++;
            }

            cmd.Parameters.AddWithValue("@key", keyValue ?? DBNull.Value);

            await conn.OpenAsync(cancellationToken);
            var rows = await cmd.ExecuteNonQueryAsync(cancellationToken);
            return rows;
        }

        public async Task<int> DeleteAsync(string keyColumn, object keyValue, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(keyColumn)) throw new ArgumentException("Key column required for delete");

            ValidateColumnNames(new[] { keyColumn });

            var sql = $"DELETE FROM [FileReport] WHERE [{keyColumn}] = @key;";

            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@key", keyValue ?? DBNull.Value);

            await conn.OpenAsync(cancellationToken);
            var rows = await cmd.ExecuteNonQueryAsync(cancellationToken);
            return rows;
        }
    }
}
