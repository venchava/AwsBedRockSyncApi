namespace AwsBedRockSyncApi.Interfaces
{
    public interface IBedrockService
    {
        Task<string> GenerateTextAsync(string prompt, int maxTokens = 400);
    }
}
