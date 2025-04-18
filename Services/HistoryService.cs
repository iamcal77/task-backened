using MyWebAPI.Models;
using System.Text.Json;

namespace MyWebAPI.Services
{
    public class HistoryService
    {
        private const string FilePath = "history.json";

        public async Task SaveQueryAsync(QueryResponse response)
        {
            var existing = await GetAllQueriesAsync();

            // Assign unique ID
            response.Id = existing.Any() ? existing.Max(q => q.Id) + 1 : 1;
            existing.Add(response);

            await WriteToFileAsync(existing);
        }

        public async Task<List<QueryResponse>> GetAllQueriesAsync()
        {
            if (!File.Exists(FilePath))
                return new List<QueryResponse>();

            var json = await File.ReadAllTextAsync(FilePath);
            return JsonSerializer.Deserialize<List<QueryResponse>>(json) ?? new List<QueryResponse>();
        }

        public async Task<QueryResponse?> GetQueryByIdAsync(int id)
        {
            var queries = await GetAllQueriesAsync();
            return queries.FirstOrDefault(q => q.Id == id);
        }

        public async Task UpdateQueryAsync(QueryResponse updated)
        {
            var queries = await GetAllQueriesAsync();
            var index = queries.FindIndex(q => q.Id == updated.Id);
            if (index == -1) return;

            queries[index] = updated;
            await WriteToFileAsync(queries);
        }

        public async Task<bool> DeleteQueryAsync(int id)
        {
            var queries = await GetAllQueriesAsync();
            var removed = queries.RemoveAll(q => q.Id == id) > 0;

            if (removed)
                await WriteToFileAsync(queries);

            return removed;
        }


        private async Task WriteToFileAsync(List<QueryResponse> queries)
        {
            var json = JsonSerializer.Serialize(queries, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(FilePath, json);
        }
    }
}
