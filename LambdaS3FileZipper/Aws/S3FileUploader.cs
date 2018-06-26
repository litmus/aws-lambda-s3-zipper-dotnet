using System;
using System.Threading;
using System.Threading.Tasks;
using LambdaS3FileZipper.Interfaces;

namespace LambdaS3FileZipper.Aws
{
    public class S3FileUploader : IFileUploader
    {
	    private readonly IAwsS3Client client;

		public S3FileUploader(IAwsS3Client client)
		{
			this.client = client;
		}

	    public async Task<string> Upload(string bucketName, string resourceName, string compressedFileName, CancellationToken token)
	    {
			token.ThrowIfCancellationRequested();

		    await client.Upload(bucketName, resourceName, compressedFileName, token);
		    return client.GenerateUrl(bucketName, resourceName);
	    }
    }
}
