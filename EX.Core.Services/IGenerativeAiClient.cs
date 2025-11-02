using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EX.Core.Services
{
    public interface IGenerativeAiClient
    {
        Task<string> GenerateAsync(
            IList<(string role, string content)> messages,
            double temperature,
            int maxTokens,
            CancellationToken cancellationToken = default);
    }
}