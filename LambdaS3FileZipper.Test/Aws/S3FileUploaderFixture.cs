using System.Threading;
using System.Threading.Tasks;
using LambdaS3FileZipper.Aws;
using NSubstitute;
using NUnit.Framework;

namespace LambdaS3FileZipper.Test.Aws
{
	[TestFixture]
    public class S3FileUploaderFixture
    {
	    private S3FileUploader uploader;

	    private IAwsS3Client client;

	    private string bucket = "bucket";
	    private string resource = "resource";
	    private string compressedFile = "compressed-file";
		private string url = "s3.com/compressed-file";
	    private CancellationToken regular = new CancellationToken();

		[SetUp]
	    public void SetUp()
		{
			client = Substitute.For<IAwsS3Client>();
			client.GenerateUrl(bucket, resource).Returns(url);

			uploader = new S3FileUploader(client);
		}

	    [Test]
	    public async Task Upload_ShouldUploadLocalCompressedFile()
	    {
		    await uploader.Upload(bucket, resource, compressedFile, regular);

		    await client.Received().Upload(bucket, resource, compressedFile, regular);
	    }

	    [Test]
	    public async Task Upload_ShouldGenerateAnUrlForUploadedResource()
	    {
		    await uploader.Upload(bucket, resource, compressedFile, regular);

		    client.Received().GenerateUrl(bucket, resource);
	    }

	    [Test]
	    public async Task Upload_ShouldReturnUrl()
	    {
		    var generatedUrl = await uploader.Upload(bucket, resource, compressedFile, regular);

			Assert.That(generatedUrl, Is.EqualTo(url));
	    }

	    [Test]
	    public async Task Upload_ShouldThrowExceptionWhenCanceled()
	    {
			var canceled = new CancellationToken(canceled: true);

		    Assert.ThrowsAsync<TaskCanceledException>(() => uploader.Upload(bucket, resource, compressedFile, canceled));

		    await client.DidNotReceive().Upload(bucket, resource, compressedFile, canceled);
	    }
	}
}
