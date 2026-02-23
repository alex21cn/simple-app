using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using api.Models;
using api.Services;

namespace api.Controllers
{
    [ApiController]
    //[Route("[controller]")]
    [Authorize]
    /// <summary>
    /// CRUD operations for FileReport.
    /// </summary>
    public class FileReportController : ControllerBase
    {
        private readonly FileReportRepository _repo;
        private readonly ILogger<FileReportController> _logger;

        public FileReportController(FileReportRepository repo, ILogger<FileReportController> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        /// <summary>
        /// Insert a new FileReport.
        /// </summary>
        /// <param name="dto">FileReport to create. Id is ignored.</param>
        /// <returns>The created FileReport with Id populated.</returns>
        [HttpPost("insert")]
        [Authorize(Roles = "hdcs_admin")]
        [ProducesResponseType(typeof(Models.FileReportReadDto), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Insert([FromBody] Models.FileReportCreateDto dto, CancellationToken cancellationToken)
        {
            if (dto == null) return BadRequest("Missing body");

            try
            {
                var entity = new Models.FileReport
                {
                    Filename = dto.Filename,
                    Type = dto.Type,
                    Path = dto.Path,
                    PostedDate = dto.PostedDate,
                    LastUpdatedDate = dto.LastUpdatedDate,
                    NumberOfLinkedNodes = dto.NumberOfLinkedNodes,
                    LinkedNodes = dto.LinkedNodes
                };

                var created = await _repo.InsertAsync(entity, cancellationToken);

                var read = new Models.FileReportReadDto
                {
                    Id = created.Id,
                    Filename = created.Filename,
                    Type = created.Type,
                    Path = created.Path,
                    PostedDate = created.PostedDate,
                    LastUpdatedDate = created.LastUpdatedDate,
                    NumberOfLinkedNodes = created.NumberOfLinkedNodes,
                    LinkedNodes = created.LinkedNodes
                };

                return Ok(read);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Insert failed");
                return Problem(ex.Message);
            }
        }

        /// <summary>
        /// Update an existing FileReport by Id.
        /// </summary>
        /// <param name="id">Id of the FileReport to update.</param>
        /// <param name="dto">Updated FileReport values.</param>
        [HttpPut("update/{id:int}")]
        [Authorize(Roles = "hdcs_admin")]
        [ProducesResponseType(typeof(Models.FileReportReadDto), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update(int id, [FromBody] Models.FileReportUpdateDto dto, CancellationToken cancellationToken)
        {
            if (dto == null) return BadRequest("Missing body");

            try
            {
                var entity = new Models.FileReport
                {
                    Filename = dto.Filename,
                    Type = dto.Type,
                    Path = dto.Path,
                    PostedDate = dto.PostedDate,
                    LastUpdatedDate = dto.LastUpdatedDate,
                    NumberOfLinkedNodes = dto.NumberOfLinkedNodes,
                    LinkedNodes = dto.LinkedNodes
                };

                var updated = await _repo.UpdateAsync(id, entity, cancellationToken);
                if (updated == null) return NotFound();

                var read = new Models.FileReportReadDto
                {
                    Id = updated.Id,
                    Filename = updated.Filename,
                    Type = updated.Type,
                    Path = updated.Path,
                    PostedDate = updated.PostedDate,
                    LastUpdatedDate = updated.LastUpdatedDate,
                    NumberOfLinkedNodes = updated.NumberOfLinkedNodes,
                    LinkedNodes = updated.LinkedNodes
                };

                return Ok(read);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Update failed");
                return Problem(ex.Message);
            }
        }

        /// <summary>
        /// Delete a FileReport by Id.
        /// </summary>
        [HttpDelete("delete/{id:int}")]
        [Authorize(Roles = "hdcs_admin")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            try
            {
                var ok = await _repo.DeleteAsync(id, cancellationToken);
                if (!ok) return NotFound();
                return Ok(new { deleted = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Delete failed");
                return Problem(ex.Message);
            }
        }

        /// <summary>
        /// Load FileReport records with pagination.
        /// </summary>
        /// <param name="pageSize">Number of records per page (default 100).</param>
        /// <param name="pageIndex">Zero-based page index (default 0).</param>
        [HttpGet("load")]
        [ProducesResponseType(typeof(Models.PagedResult<Models.FileReport>), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Load([FromQuery] int pageSize = 100, [FromQuery] int pageIndex = 0, CancellationToken cancellationToken = default)
        {
            if (pageSize <= 0) return BadRequest("pageSize must be > 0");
            if (pageIndex < 0) return BadRequest("pageIndex must be >= 0");

            try
            {
                var result = await _repo.GetPagedAsync(pageIndex, pageSize, cancellationToken);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Load failed");
                return Problem(ex.Message);
            }
        }
    }
}
