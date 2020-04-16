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
		private string[] testFiles = { "file1", "file2" };
		private CancellationToken cancellationToken;

		[SetUp]
		public void SetUp()
		{
			cancellationToken = CancellationToken.None;

			s3Client = Substitute.For<IAwsS3Client>();
			s3Client.List(testBucket, testResource, Arg.Any<CancellationToken>()).Returns(testFiles);
			s3Client.Download(testBucket, Arg.Any<string>(), Arg.Any<string>(), cancellationToken);

			fileRetriever = new S3FileRetriever(s3Client);
		}

		[Test]
		public async Task RetrieveToLocalDirectory_ShouldReturnObjectPaths()
		{
			var directory = await fileRetriever.RetrieveToLocalDirectory(testBucket, testResource, cancellationToken: cancellationToken);

			Assert.IsNotNull(directory);
			await s3Client.Received(1).List(testBucket, testResource, cancellationToken);
			foreach (var file in testFiles)
			{
				await s3Client.Received(1).Download(testBucket, file, directory, cancellationToken);
			}
		}

	    [Test]
	    public async Task RetrieveToLocalDirectory_ShouldReturnEmptyCollectionWhenNoFilesAreFound()
	    {
	        testResource = "not-found";

            Assert.ThrowsAsync<ResourceNotFoundException>(() => fileRetriever.RetrieveToLocalDirectory(testBucket, testResource, cancellationToken: cancellationToken));

	        await s3Client.Received(1).List(testBucket, testResource, cancellationToken);
	        await s3Client.DidNotReceive().Download(testBucket, Arg.Any<string>(), Arg.Any<string>(), cancellationToken);
	    }

        [Test]
	    public async Task RetrieveToLocalDirectory_ShouldReturnOnlyFilesMatchingExpression()
	    {
	        var resourceMatchExpression = @".*2";

	        await fileRetriever.RetrieveToLocalDirectory(testBucket, testResource, resourceMatchExpression, cancellationToken);

	        await s3Client.Received(1).List(testBucket, testResource, cancellationToken);
	        await s3Client.DidNotReceive().Download(testBucket, "file1", Arg.Any<string>(), cancellationToken);
	        await s3Client.Received(1).Download(testBucket, "file2", Arg.Any<string>(), cancellationToken);
        }
	}
}