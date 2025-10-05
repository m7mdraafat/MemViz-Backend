using Microsoft.AspNetCore.Mvc;

namespace MemViz.Api.Controllers;

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
        
        return Ok(new
        {
            Status = "Healthy",
            Timestamp = DateTime.UtcNow,
            Version = "1.0.0",
            Service = "MemViz API"
        });
    }

    [HttpGet("ready")]
    public IActionResult Ready()
    {
        // Add checks for database, external services, etc.
        return Ok(new
        {
            Status = "Ready",
            Timestamp = DateTime.UtcNow
        });
    }

    [HttpGet("live")]
    public IActionResult Live()
    {
        return Ok(new
        {
            Status = "Live",
            Timestamp = DateTime.UtcNow
        });
    }
}