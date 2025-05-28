using AwsBedRockSyncApi.Interfaces;
using AwsBedRockSyncApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AwsBedRockSyncApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BedRockController : ControllerBase
    {
        private readonly IBedrockService _bedrockService;

        public BedRockController( IBedrockService bedrockService)
        {
           _bedrockService = bedrockService ?? throw new ArgumentNullException(nameof(bedrockService));
        }

        [HttpPost("generate-text")]
        public async Task<IActionResult> GenerateTextAsync([FromBody] TextGenerationRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Prompt))
            {
                return BadRequest("Prompt cannot be empty.");
            }
            try
            {
                var generatedText = await _bedrockService.GenerateTextAsync(
                    request.Prompt,
                    request.MaxTokens);
                return Ok( new { GeneratedText = generatedText });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error generating text: {ex.Message}");
            }
        }

    }
}
