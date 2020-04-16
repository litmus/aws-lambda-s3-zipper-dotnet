using System.Threading;
using System.Threading.Tasks;
using LambdaS3FileZipper.Models;

namespace LambdaS3FileZipper.Interfaces
{
	public interface IFileRetriever
	{
		Task<string> RetrieveToLocalDirectory(string bucket, string resource, string resourceExpressionPattern = default, CancellationToken cancellationToken = default);
		Task<FileResponse[]> RetrieveToMemory(string bucket, string fileKey, string resourceExpressionPattern = default, CancellationToken cancellationToken = default);
	}
}