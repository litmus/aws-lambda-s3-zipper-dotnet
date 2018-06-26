using System.Threading;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using LambdaS3FileZipper.Aws;
using NSubstitute;
using NUnit.Framework;

namespace LambdaS3FileZipper.Test.Aws
{
	[TestFixture]
    public class AwsS3ClientFixture
	{
		private IAmazonS3 amazonS3;

		private AwsS3Client client;

		private string bucket;
		private string resource;
		private string file;
		private CancellationToken regular;

		[SetUp]
	    public void SetUp()
	    {
		    bucket = "bucket";
		    resource = "resource";
		    file = "compressed-file";

			amazonS3 = Substitute.For<IAmazonS3>();

			client = new AwsS3Client(amazonS3);
		}

		[Test]
		public async Task Upload_ShouldPutFileInS3()
		{
			await client.Upload(bucket, resource, file, regular);

			await amazonS3.Received().PutObjectAsync(
				Arg.Is<PutObjectRequest>(request => request.BucketName == bucket && request.Key == resource && request.FilePath == file),
				regular);
		}

		[Test]
		public async Task Upload_ShouldThrowOnCancellation()
		{
			var canceled = new CancellationToken(canceled: true);

			Assert.ThrowsAsync<TaskCanceledException>(() => client.Upload(bucket, resource, file, canceled));

			await amazonS3.DidNotReceive().PutObjectAsync(Arg.Any<PutObjectRequest>(), canceled);
		}
	}
}
