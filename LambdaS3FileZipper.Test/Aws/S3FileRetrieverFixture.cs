using System.Threading;
using System.Threading.Tasks;
using LambdaS3FileZipper.Aws;
using NSubstitute;
using NUnit.Framework;

namespace LambdaS3FileZipper.Test.Aws
{
	[TestFixture]
	public class S3FileRetrieverFixture
	{
		private S3FileRetriever fileRetriever;

		private IAwsS3Client s3Client;
		private string testBucket = "test-bucket";
		private string testResource = "test-resource";

		[SetUp]
		public void SetUp()
		{
			s3Client = Substitute.For<IAwsS3Client>();
			s3Client.List(testBucket, testResource, Arg.Any<CancellationToken>()).Returns(new [] { "file1", "file2" });
			fileRetriever = new S3FileRetriever(s3Client);
		}

		[Test]
		public async Task List_ShouldReturnObjectPaths()
		{
			var list = fileRetriever.Retrieve(testBucket, testResource, CancellationToken.None);

			Assert.IsNotNull(list);
			await s3Client.Received().List(testBucket, testResource, Arg.Any<CancellationToken>());
		}
	}
}