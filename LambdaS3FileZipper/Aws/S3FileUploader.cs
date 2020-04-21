using System.Threading;
using System.Threading.Tasks;
using LambdaS3FileZipper.Interfaces;
using LambdaS3FileZipper.Models;

namespace LambdaS3FileZipper.Aws
{
    public class S3FileUploader : IFileUploader
    {
	    private readonly IAwsS3Client client;

		public S3FileUploader(IAwsS3Client client)
		{
			this.client = client;
		}

	    public async Task<string> Upload(string bucket, string fileKey, string compressedFileName, CancellationToken cancellationToken = default)
	    {
			cancellationToken.ThrowIfCancellationRequested();

		    await client.Upload(bucket, fileKey, compressedFileName, cancellationToken);
		    return client.GenerateUrl(bucket, fileKey);
	    }

	    public async Task<string> Upload(string bucket, string fileKey, FileResponse fileResponse, CancellationToken cancellationToken = default)
	    {
		    cancellationToken.ThrowIfCancellationRequested();

		    await client.Upload(bucket, fileKey, fileResponse, cancellationToken);
		    return client.GenerateUrl(bucket, fileKey);
	    }
	}
}
