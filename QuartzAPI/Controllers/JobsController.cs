using Microsoft.AspNetCore.Mvc;
using QuartzAPI.Models;
using QuartzAPI.Services;
using QuartzAPI.Utilities;

namespace QuartzAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JobsController : ControllerBase
{
    private readonly IQuartzJobService _quartzJobService;
    private readonly ILogger<JobsController> _logger;

    public JobsController(IQuartzJobService quartzJobService, ILogger<JobsController> logger)
    {
        _quartzJobService = quartzJobService;
        _logger = logger;
    }

    [HttpPost("create-email-job")]
    public async Task<ActionResult<ApiResponse<JobDto>>> CreateEmailJob([FromBody] EmailJobRequest request)
    {
        try
        {
            var job = JobFactory.CreateJob(
                    request.JobName,
                    request.Description,
                    JobType.SendEmail,
                    request.CronExpression,
                    new { request.ToAddress, request.Subject, request.Body, request.IsHtml }
                );

            await _quartzJobService.CreateJobAsync(job);

            return Ok(new ApiResponse<JobDto>
            {
                Success = true,
                Message = "Email job created successfully",
                Data = job
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error creating email job: {ex.Message}");
            return BadRequest(new ApiResponse<JobDto>
            {
                Success = false,
                Message = $"Error creating job: {ex.Message}"
            });
        }
    }

    [HttpPost("create-http-api-job")]
    public async Task<ActionResult<ApiResponse<JobDto>>> CreateHttpApiJob([FromBody] HttpApiJobRequest request)
    {
        try
        {
            var job = JobFactory.CreateJob(
                  request.JobName,
                  request.Description,
                  JobType.CallHttpApi,
                  request.CronExpression,
                  new { request.Url, request.HttpMethod, request.Headers, request.RequestBody }
              );

            await _quartzJobService.CreateJobAsync(job);

            return Ok(new ApiResponse<JobDto>
            {
                Success = true,
                Message = "HTTP API job created successfully",
                Data = job
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error creating HTTP API job: {ex.Message}");
            return BadRequest(new ApiResponse<JobDto>
            {
                Success = false,
                Message = $"Error creating job: {ex.Message}"
            });
        }
    }

    [HttpGet("{jobId}")]
    public async Task<ActionResult<ApiResponse<JobDto>>> GetJob(string jobId)
    {
        try
        {
            var job = await _quartzJobService.GetJobAsync(jobId);

            if (job == null)
            {
                return NotFound(new ApiResponse<JobDto>
                {
                    Success = false,
                    Message = "Job not found"
                });
            }

            return Ok(new ApiResponse<JobDto>
            {
                Success = true,
                Message = "Job retrieved successfully",
                Data = job
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting job: {ex.Message}");
            return BadRequest(new ApiResponse<JobDto>
            {
                Success = false,
                Message = $"Error retrieving job: {ex.Message}"
            });
        }
    }

    [HttpGet("all")]
    public async Task<ActionResult<ApiResponse<IEnumerable<JobDto>>>> GetAllJobs()
    {
        try
        {
            var jobs = await _quartzJobService.GetAllJobsAsync();

            return Ok(new ApiResponse<IEnumerable<JobDto>>
            {
                Success = true,
                Message = "Jobs retrieved successfully",
                Data = jobs
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting all jobs: {ex.Message}");
            return BadRequest(new ApiResponse<IEnumerable<JobDto>>
            {
                Success = false,
                Message = $"Error retrieving jobs: {ex.Message}"
            });
        }
    }

    [HttpPut("update")]
    public async Task<ActionResult<ApiResponse<JobDto>>> UpdateJob([FromBody] JobDto job)
    {
        try
        {
            await _quartzJobService.UpdateJobAsync(job);

            return Ok(new ApiResponse<string>
            {
                Success = true,
                Message = "Job updated successfully",
                Data = job.Id
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error updating job: {ex.Message}");
            return BadRequest(new ApiResponse<JobDto>
            {
                Success = false,
                Message = $"Error updating job: {ex.Message}"
            });
        }
    }

    [HttpDelete("{jobId}")]
    public async Task<ActionResult<ApiResponse<string>>> DeleteJob(string jobId)
    {
        try
        {
            await _quartzJobService.DeleteJobAsync(jobId);

            return Ok(new ApiResponse<string>
            {
                Success = true,
                Message = "Job deleted successfully",
                Data = jobId
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error deleting job: {ex.Message}");
            return BadRequest(new ApiResponse<string>
            {
                Success = false,
                Message = $"Error deleting job: {ex.Message}"
            });
        }
    }

    [HttpPost("{jobId}/pause")]
    public async Task<ActionResult<ApiResponse<string>>> PauseJob(string jobId)
    {
        try
        {
            await _quartzJobService.PauseJobAsync(jobId);

            return Ok(new ApiResponse<string>
            {
                Success = true,
                Message = "Job paused successfully",
                Data = jobId
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error pausing job: {ex.Message}");
            return BadRequest(new ApiResponse<string>
            {
                Success = false,
                Message = $"Error pausing job: {ex.Message}"
            });
        }
    }

    [HttpPost("{jobId}/resume")]
    public async Task<ActionResult<ApiResponse<string>>> ResumeJob(string jobId)
    {
        try
        {
            await _quartzJobService.ResumeJobAsync(jobId);

            return Ok(new ApiResponse<string>
            {
                Success = true,
                Message = "Job resumed successfully",
                Data = jobId
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error resuming job: {ex.Message}");
            return BadRequest(new ApiResponse<string>
            {
                Success = false,
                Message = $"Error resuming job: {ex.Message}"
            });
        }
    }

    [HttpGet("api-call-history")]
    public async Task<ActionResult<List<APICallHistoryDto>>> GetApiCallHistory()
    {
        try
        {
            var histories = await _quartzJobService.GetApiCallHistoriesAsync();
            return Ok(histories);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting API call history: {ex.Message}");
            return BadRequest(new { message = $"Error retrieving API call history: {ex.Message}" });
        }
    }
}




