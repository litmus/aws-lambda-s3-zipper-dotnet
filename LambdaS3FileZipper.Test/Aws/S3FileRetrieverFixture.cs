using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using LambdaS3FileZipper.Aws;
using LambdaS3FileZipper.Exceptions;
using LambdaS3FileZipper.Models;
using NSubstitute;
using NUnit.Framework;

namespace LambdaS3FileZipper.Test.Aws
{
	[TestFixture]
	public class S3FileRetrieverFixture
	{
		private S3FileRetriever fileRetriever;

		private IAwsS3Client s3Client;

		private string bucket;
		private string existingResource;
		private string notFoundResource;
		private string[] files;
		private CancellationToken cancellationToken;

		[SetUp]
		public void SetUp()
		{
			bucket = "test-bucket";
			existingResource = "test-resource";
			notFoundResource = "not-found";
			files = new[] {"file1", "file2"};
			cancellationToken = CancellationToken.None;

			s3Client = Substitute.For<IAwsS3Client>();
			s3Client.List(bucket, existingResource, cancellationToken).Returns(files);
			s3Client.List(bucket, notFoundResource, cancellationToken).Returns(new string[] { });
			s3Client.Download(bucket, resource: Arg.Any<string>(), destinationPath: Arg.Any<string>(), cancellationToken);
			s3Client.Download(bucket, resourceKey: Arg.Any<string>(), cancellationToken).Returns(new FileResponse("key", default));

			fileRetriever = new S3FileRetriever(s3Client);
		}

		[Test]
		public async Task RetrieveToLocalDirectory_ShouldReturnObjectPaths()
		{
			var directory = await fileRetriever.RetrieveToLocalDirectory(bucket, existingResource, cancellationToken: cancellationToken);

			Assert.IsNotNull(directory);
			await s3Client.Received(1).List(bucket, existingResource, cancellationToken);
			foreach (var file in files)
			{
				await s3Client.Received(1).Download(bucket, file, directory, cancellationToken);
			}
		}

	    [Test]
	    public async Task RetrieveToLocalDirectory_ShouldReturnEmptyCollectionWhenNoFilesAreFound()
	    {
            Assert.ThrowsAsync<ResourceNotFoundException>(() => fileRetriever.RetrieveToLocalDirectory(bucket, notFoundResource, cancellationToken: cancellationToken));

	        await s3Client.Received(1).List(bucket, notFoundResource, cancellationToken);
	        await s3Client.DidNotReceive().Download(bucket, Arg.Any<string>(), Arg.Any<string>(), cancellationToken);
	    }

        [Test]
	    public async Task RetrieveToLocalDirectory_ShouldReturnOnlyFilesMatchingExpression()
	    {
	        var resourceMatchExpression = @".*2";

	        await fileRetriever.RetrieveToLocalDirectory(bucket, existingResource, resourceMatchExpression, cancellationToken);

	        await s3Client.Received(1).List(bucket, existingResource, cancellationToken);
	        await s3Client.DidNotReceive().Download(bucket, "file1", Arg.Any<string>(), cancellationToken);
	        await s3Client.Received(1).Download(bucket, "file2", Arg.Any<string>(), cancellationToken);
        }

	    [Test]
	    public async Task RetrieveToMemory_ShouldReturnObjectPaths()
	    {
		    var fileResponses = await fileRetriever.RetrieveToMemory(bucket, existingResource, cancellationToken: cancellationToken);

		    Assert.That(fileResponses, Is.Not.Empty);
		    Assert.That(fileResponses.Count(), Is.EqualTo(2));
		    await s3Client.Received(1).List(bucket, existingResource, cancellationToken);
		    foreach (var file in files)
		    {
			    await s3Client.Received(1).Download(bucket, file, cancellationToken);
		    }
	    }

	    [Test]
	    public async Task RetrieveToMemory_ShouldReturnEmptyCollectionWhenNoFilesAreFound()
	    {
		    Assert.ThrowsAsync<ResourceNotFoundException>(() => fileRetriever.RetrieveToMemory(bucket, notFoundResource, cancellationToken: cancellationToken));

		    await s3Client.Received(1).List(bucket, notFoundResource, cancellationToken);
		    await s3Client.DidNotReceive().Download(bucket, Arg.Any<string>(), cancellationToken);
	    }

	    [Test]
	    public async Task RetrieveToMemory_ShouldReturnOnlyFilesMatchingExpression()
	    {
		    var resourceMatchExpression = @".*2";

		    var fileResponses = await fileRetriever.RetrieveToMemory(bucket, existingResource, resourceMatchExpression, cancellationToken);

		    Assert.That(fileResponses, Is.Not.Empty);
		    Assert.That(fileResponses.Count(), Is.EqualTo(1));
			await s3Client.Received(1).List(bucket, existingResource, cancellationToken);
		    await s3Client.DidNotReceive().Download(bucket, "file1", cancellationToken);
		    await s3Client.Received(1).Download(bucket, "file2", cancellationToken);
	    }
	}
}