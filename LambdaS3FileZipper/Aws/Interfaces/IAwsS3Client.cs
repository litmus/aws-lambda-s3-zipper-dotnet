using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LambdaS3FileZipper
{
    public interface IAwsS3Client
    {
	    Task<IEnumerable<string>> List(string bucketName, string resource, CancellationToken cancellationToken);
	    Task Upload(string bucketName, string resourceName, string destinationPath, CancellationToken cancellationToken);
    }
}