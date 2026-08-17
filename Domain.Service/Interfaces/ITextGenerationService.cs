using System.Threading;
using System.Threading.Tasks;

namespace Domain.Service.Interfaces
{
    public interface ITextGenerationService
    {
        Task<string> GenerateSynopsisAsync(string title, string author, CancellationToken cancellationToken = default);
    }
}
