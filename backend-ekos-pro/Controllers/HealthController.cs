using Domain.Service.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace backend_ekos_pro.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    private readonly ILogger<HealthController> _logger;

    public HealthController(ILogger<HealthController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Get()
    {
        _logger.LogInformation("Health check endpoint called");

        var response = ApiResponse<object>.SuccessResponse(
            new
            {
                Status = "Healthy",
                Timestamp = DateTime.UtcNow,
                Version = "1.0.0"
            },
            "API is running successfully");

        return Ok(response);
    }
}
