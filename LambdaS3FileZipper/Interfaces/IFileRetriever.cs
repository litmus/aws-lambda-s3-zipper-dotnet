using System.Threading;
using System.Threading.Tasks;

namespace LambdaS3FileZipper.Interfaces
{
	public interface IFileRetriever
	{
		Task<string> Retrieve(string bucket, string resource, CancellationToken cancellationToken);
	}
}