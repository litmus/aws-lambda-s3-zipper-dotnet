using System.Threading;
using System.Threading.Tasks;

namespace LambdaS3FileZipper.Interfaces
{
	public interface IFileRetriever
	{
		Task<string> RetrieveToLocalDirectory(string bucket, string resource, string resourceExpressionPattern = default, CancellationToken cancellationToken = default);
	}
}