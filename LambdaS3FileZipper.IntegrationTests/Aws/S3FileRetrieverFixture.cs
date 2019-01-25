using System.IO;
using System.Threading;
using System.Threading.Tasks;
using LambdaS3FileZipper.Aws;
using NUnit.Framework;

namespace LambdaS3FileZipper.IntegrationTests.Aws
{
	[TestFixture]
	public class S3FileRetrieverFixture : S3Fixture
	{
		private readonly S3FileRetriever fileRetriever;

		public S3FileRetrieverFixture()
		{
			fileRetriever = new S3FileRetriever(Client);
		}

		[Test]
		public async Task Retrieve_ShouldDownloadAllFiles()
		{
			var directory = await fileRetriever.Retrieve(TestEnvironment.IntegrationTestBucket, "test.png", resourceExpressionPattern: ".*");

			Assert.IsTrue(Directory.Exists(directory));

			DeleteLocalTempDirectory(directory);
		}
	}
}