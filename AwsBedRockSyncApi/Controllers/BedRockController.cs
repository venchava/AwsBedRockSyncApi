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
        private readonly ILogger<BedRockController> _logger;

        public BedRockController(IBedrockService bedrockService, ILogger<BedRockController> logger)
        {
            _bedrockService = bedrockService ?? throw new ArgumentNullException(nameof(bedrockService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost("generate-text")]
        public async Task<IActionResult> GenerateTextAsync([FromBody] TextGenerationRequest request)
        {
            _logger.LogInformation("Received text generation request with prompt length: {PromptLength}, MaxTokens: {MaxTokens}",
                request?.Prompt?.Length ?? 0, request?.MaxTokens);

            if (string.IsNullOrWhiteSpace(request?.Prompt))
            {
                _logger.LogWarning("Received empty prompt in text generation request");
                return BadRequest("Prompt cannot be empty.");
            }

            try
            {
                _logger.LogDebug("Calling Bedrock service to generate text");
                var generatedText = await _bedrockService.GenerateTextAsync(
                    request.Prompt,
                    request.MaxTokens);

                _logger.LogInformation("Successfully generated text with length: {GeneratedTextLength}",
                    generatedText?.Length ?? 0);

                return Ok(new { GeneratedText = generatedText });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while generating text. Prompt length: {PromptLength}, MaxTokens: {MaxTokens}",
                    request.Prompt.Length, request.MaxTokens);

                return StatusCode(StatusCodes.Status500InternalServerError,
                    "An error occurred while processing your request. Please try again later.");
            }
        }

    }
}
