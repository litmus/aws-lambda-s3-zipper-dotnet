using System.Threading;
using System.Threading.Tasks;

namespace LambdaS3FileZipper.Interfaces
{
	public interface IFileUploader
	{
		Task<string> Upload(string bucketName, string resourceName, string compressedFileName, CancellationToken token);

	}
}