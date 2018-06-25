using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LambdaS3FileZipper.Interfaces;

namespace LambdaS3FileZipper.Aws
{
	public class S3FileRetriever : IFileRetriever
    {
	    private readonly IAwsS3Client s3Client;

	    public S3FileRetriever(IAwsS3Client s3Client)
	    {
		    this.s3Client = s3Client;
	    }

	    public async Task<IEnumerable<string>> Retrieve(string bucket, string resource, CancellationToken cancellationToken)
	    {
		    return await s3Client.List(bucket, resource, cancellationToken);
	    }
    }
}
