using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LambdaS3FileZipper
{
    public interface IAwsS3Client
    {
	    Task<IEnumerable<string>> List(string bucketName, string resource, CancellationToken cancellationToken);
	    Task<string> Download(string bucketName, string resource, string destinationPath, CancellationToken cancellationToken);
	    Task Upload(string bucketName, string resourceName, string filePath, CancellationToken cancellationToken);
	    Task Delete(string bucketName, string resourceName, CancellationToken cancellationToken);
	    string GenerateUrl(string bucketName, string resourceName);
    }
}