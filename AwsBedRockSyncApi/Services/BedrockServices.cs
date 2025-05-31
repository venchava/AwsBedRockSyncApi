using Amazon.BedrockRuntime;
using Amazon.BedrockRuntime.Model;
using AwsBedRockSyncApi.Interfaces;
using Microsoft.AspNetCore.Components.Forms;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;

namespace AwsBedRockSyncApi.Services
{
    public class BedrockServices : IBedrockService
    {
        private readonly AmazonBedrockRuntimeClient _bedrockClient;
        private readonly ILogger<BedrockServices> _logger;
        private readonly string _titantextModelid = "amazon.titan-text-express-v1";


        public BedrockServices(IConfiguration configuration, ILogger<BedrockServices> logger)
        {
            _logger = logger;
            _bedrockClient = new AmazonBedrockRuntimeClient(
                configuration["AWS:AccessKey"],
                configuration["AWS:SecretKey"],
                 Amazon.RegionEndpoint.GetBySystemName(configuration["Aws:Region"] ?? "us-east-1"));
        }



        public async Task<string> GenerateTextAsync(string prompt, int maxTokens = 400)
        {
            try
            {
                _logger.LogInformation("Generating text for prompt: {Prompt} with maxTokens: {MaxTokens}", prompt, maxTokens);

                var requestBody = new
                {
                    InputText = prompt,
                    textGenerationConfig = new
                    {
                        MaxTokens = maxTokens,
                        temparature = 0.7,
                        topP = 0.9,
                        StopSequences = new string[] { }
                    },
                };

                var jsonrequest = JsonSerializer.Serialize(requestBody);
                var request = new InvokeModelRequest
                {
                    ModelId = _titantextModelid,
                    Body = new MemoryStream(Encoding.UTF8.GetBytes(jsonrequest)),
                    ContentType = "application/json"
                };

                _logger.LogDebug("Sending request to Bedrock with ModelId: {ModelId}", _titantextModelid);
                var response = await _bedrockClient.InvokeModelAsync(request);

                using var reader = new StreamReader(response.Body);
                var responseBody = await reader.ReadToEndAsync();
                var responseObject = JsonSerializer.Deserialize<JsonElement>(responseBody);

                var generatedText = responseObject.GetProperty("results").EnumerateArray().First()
                                    .GetProperty("outputText").GetString();

                _logger.LogInformation("Successfully generated text response");
                return generatedText ?? string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating text for prompt: {Prompt}", prompt);
                throw;
            }

        }

    }



}
