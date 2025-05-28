namespace AwsBedRockSyncApi.Models
{
    public class TextGenerationRequest
    {
        public string Prompt { get; set; }
        public int MaxTokens { get; set; } = 400; 
    }
}
