using System.IO;
using System.Threading;
using System.Threading.Tasks;
using LambdaS3FileZipper.Aws;
using LambdaS3FileZipper.Interfaces;
using LambdaS3FileZipper.Models;
using NSubstitute;
using NUnit.Framework;

namespace LambdaS3FileZipper.Test.Aws
{
	[TestFixture]
    public class S3FileUploaderFixture
    {
	    private S3FileUploader uploader;

	    private IAwsS3Client client;

	    private string bucket;
	    private string resource;
	    private string filePath;
	    private FileResponse fileResponse;
		private string url;
		private CancellationToken cancellationToken;

		[SetUp]
	    public void SetUp()
		{
			bucket = "bucket";
			resource = "resource";
			filePath = "file";
			url = "s3.com/file";
			fileResponse = new FileResponse("file", contentStream: new MemoryStream());
			cancellationToken = CancellationToken.None;

			client = Substitute.For<IAwsS3Client>();
			client.GenerateUrl(bucket, resource).Returns(url);

			uploader = new S3FileUploader(client);
		}

	    [Test]
	    public async Task Upload_ShouldUploadLocalCompressedFile()
	    {
		    await uploader.Upload(bucket, resource, filePath, cancellationToken);

		    await client.Received().Upload(bucket, resource, filePath, cancellationToken);
	    }

	    [Test]
	    public async Task Upload_ShouldGenerateAnUrlForUploadedResource()
	    {
		    await uploader.Upload(bucket, resource, filePath, cancellationToken);

		    client.Received().GenerateUrl(bucket, resource);
	    }

	    [Test]
	    public async Task Upload_ShouldReturnUrl()
	    {
		    var generatedUrl = await uploader.Upload(bucket, resource, filePath, cancellationToken);

			Assert.That(generatedUrl, Is.EqualTo(url));
	    }

	    [Test]
	    public async Task Upload_ShouldThrowExceptionWhenCanceled()
	    {
			var canceled = new CancellationToken(canceled: true);

		    Assert.ThrowsAsync<TaskCanceledException>(() => uploader.Upload(bucket, resource, filePath, canceled));

		    await client.DidNotReceive().Upload(bucket, resource, filePath, canceled);
	    }

	    [Test]
	    public async Task Upload_WithInMemoryFile_ShouldUploadLocalCompressedFile()
	    {
		    await uploader.Upload(bucket, resource, fileResponse, cancellationToken);

		    await client.Received().Upload(bucket, resource, fileResponse, cancellationToken);
	    }

	    [Test]
	    public async Task Upload_WithInMemoryFile_ShouldGenerateAnUrlForUploadedResource()
	    {
		    await uploader.Upload(bucket, resource, fileResponse, cancellationToken);

		    client.Received().GenerateUrl(bucket, resource);
	    }

	    [Test]
	    public async Task Upload_WithInMemoryFile_ShouldReturnUrl()
	    {
		    var generatedUrl = await uploader.Upload(bucket, resource, fileResponse, cancellationToken);

		    Assert.That(generatedUrl, Is.EqualTo(url));
	    }

	    [Test]
	    public async Task Upload_WithInMemoryFile_ShouldThrowExceptionWhenCanceled()
	    {
		    cancellationToken = new CancellationToken(canceled: true);

		    Assert.ThrowsAsync<TaskCanceledException>(() => uploader.Upload(bucket, resource, fileResponse, cancellationToken));

		    await client.DidNotReceive().Upload(bucket, resource, fileResponse, cancellationToken);
	    }
	}
}
