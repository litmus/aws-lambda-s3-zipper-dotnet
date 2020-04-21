using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using LambdaS3FileZipper.Aws;
using LambdaS3FileZipper.Models;
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
		private string localFile;
		private FileResponse fileResponse;
		private CancellationToken cancellationToken;

		[SetUp]
		public void SetUp()
		{
			bucket = "bucket";
			resource = "resource";
			localFile = "compressed-file";

			fileResponse = new FileResponse(localFile, contentStream: new MemoryStream());

			cancellationToken = CancellationToken.None;

			amazonS3 = Substitute.For<IAmazonS3>();

			client = new AwsS3Client(amazonS3);
		}

		[Test]
		public async Task Upload_ShouldPutFileInS3()
		{
			await client.Upload(bucket, resource, localFile, cancellationToken);

			await amazonS3.Received().PutObjectAsync(
				Arg.Is<PutObjectRequest>(request => request.BucketName == bucket && request.Key == resource && request.FilePath == localFile),
				cancellationToken);
		}

		[Test]
		public async Task Upload_ShouldThrowOnCancellation()
		{
			var canceled = new CancellationToken(canceled: true);

			Assert.ThrowsAsync<TaskCanceledException>(() => client.Upload(bucket, resource, localFile, canceled));

			await amazonS3.DidNotReceive().PutObjectAsync(Arg.Any<PutObjectRequest>(), canceled);
		}

		[Test]
		public async Task Upload_WithInMemoryFile_ShouldPutFileInS3()
		{
			await client.Upload(bucket, resource, fileResponse, cancellationToken);

			await amazonS3.Received().PutObjectAsync(
				Arg.Is<PutObjectRequest>(request => request.BucketName == bucket && request.Key == resource && request.InputStream == fileResponse.ContentStream),
				cancellationToken);
		}

		[Test]
		public async Task Upload_WithInMemoryFile_ShouldThrowOnCancellation()
		{
			var canceled = new CancellationToken(canceled: true);

			Assert.ThrowsAsync<TaskCanceledException>(() => client.Upload(bucket, resource, fileResponse, canceled));

			await amazonS3.DidNotReceive().PutObjectAsync(Arg.Any<PutObjectRequest>(), canceled);
		}

		[Test]
		public void GenerateUrl_ShouldGetPreSignedUrl()
		{
			client.GenerateUrl(bucket, resource);

			amazonS3.Received().GetPreSignedURL(
				Arg.Is<GetPreSignedUrlRequest>(request => request.BucketName == bucket && request.Key == resource));
		}
	}
}
