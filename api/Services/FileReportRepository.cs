using api.Data;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Services
{
    public class FileReportRepository
    {
        private readonly AppDbContext _db;

        public FileReportRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<FileReport> InsertAsync(FileReport entity, CancellationToken cancellationToken = default)
        {
            entity.PostedDate = entity.PostedDate == default ? DateTime.UtcNow : entity.PostedDate;
            entity.LastUpdatedDate = entity.LastUpdatedDate == default ? DateTime.UtcNow : entity.LastUpdatedDate;
            _db.FileReports.Add(entity);
            await _db.SaveChangesAsync(cancellationToken);
            return entity;
        }

        public async Task<FileReport?> UpdateAsync(int id, FileReport updated, CancellationToken cancellationToken = default)
        {
            var existing = await _db.FileReports.FindAsync(new object[] { id }, cancellationToken);
            if (existing == null) return null;

            // Update fields
            existing.Filename = updated.Filename;
            existing.Type = updated.Type;
            existing.Path = updated.Path;
            existing.LastUpdatedDate = updated.LastUpdatedDate == default ? DateTime.UtcNow : updated.LastUpdatedDate;
            existing.PostedDate = updated.PostedDate;
            existing.NumberOfLinkedNodes = updated.NumberOfLinkedNodes;
            existing.LinkedNodes = updated.LinkedNodes;

            await _db.SaveChangesAsync(cancellationToken);
            return existing;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var existing = await _db.FileReports.FindAsync(new object[] { id }, cancellationToken);
            if (existing == null) return false;
            _db.FileReports.Remove(existing);
            await _db.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<PagedResult<FileReport>> GetPagedAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default)
        {
            if (pageIndex < 0) pageIndex = 0;
            if (pageSize <= 0) pageSize = 100;

            var query = _db.FileReports.AsNoTracking().OrderBy(e => e.Id);
            var total = await query.CountAsync(cancellationToken);
            var items = await query.Skip(pageIndex * pageSize).Take(pageSize).ToListAsync(cancellationToken);

            return new PagedResult<FileReport>
            {
                Items = items,
                TotalCount = total,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }
    }
}
