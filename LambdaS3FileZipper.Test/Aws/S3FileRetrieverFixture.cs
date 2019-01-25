using System.Threading;
using System.Threading.Tasks;
using LambdaS3FileZipper.Aws;
using LambdaS3FileZipper.Exceptions;
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
		private string[] testFiles = new[] { "file1", "file2" };

		[SetUp]
		public void SetUp()
		{
			s3Client = Substitute.For<IAwsS3Client>();
			s3Client.List(testBucket, testResource, Arg.Any<CancellationToken>()).Returns(testFiles);
			s3Client.Download(testBucket, Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
			fileRetriever = new S3FileRetriever(s3Client);
		}

		[Test]
		public async Task List_ShouldReturnObjectPaths()
		{
			var directory = await fileRetriever.Retrieve(testBucket, testResource, cancellationToken: CancellationToken.None);

			Assert.IsNotNull(directory);
			await s3Client.Received().List(testBucket, testResource, Arg.Any<CancellationToken>());

			foreach (var file in testFiles)
			{
				await s3Client.Received().Download(testBucket, file, directory, Arg.Any<CancellationToken>());
			}
		}

	    [Test]
	    public async Task List_ShouldReturnEmptyCollectionWhenNoFilesAreFound()
	    {
	        testResource = "not-found";

            Assert.ThrowsAsync<ResourceNotFoundException>(() => fileRetriever.Retrieve(testBucket, testResource));

	        await s3Client.Received().List(testBucket, testResource, Arg.Any<CancellationToken>());
	        await s3Client.DidNotReceive().Download(testBucket, Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
	    }

        [Test]
	    public async Task List_ShouldReturnEmptyCollectionWhenNoFilesMatchExpression()
	    {
	        var resourceMatchExpression = @"^f$";

	        Assert.ThrowsAsync<ResourceNotFoundException>(() => fileRetriever.Retrieve(testBucket, testResource, resourceMatchExpression));

	        await s3Client.Received().List(testBucket, testResource, Arg.Any<CancellationToken>());
	        await s3Client.DidNotReceive().Download(testBucket, Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
        }
	}
}