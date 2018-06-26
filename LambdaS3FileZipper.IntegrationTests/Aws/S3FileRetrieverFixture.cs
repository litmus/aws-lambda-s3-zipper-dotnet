using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using LambdaS3FileZipper.Aws;
using LambdaS3FileZipper.IntegrationTests.Aws;
using LambdaS3FileZipper.IntegrationTests.Aws.Interfaces;
using LambdaS3FileZipper.IntegrationTests.Logging;
using NUnit.Framework;

namespace LambdaS3FileZipper.IntegrationTests
{
	[TestFixture]
	public class S3FileRetrieverFixture
	{
		private readonly S3FileRetriever fileRetriever;
		private readonly IS3TestEnvironment testEnvironment;
		private readonly ILog log;

		public S3FileRetrieverFixture()
		{
			var s3Client = new AwsS3Client(new AmazonS3Client(RegionEndpoint.USEast1));
			fileRetriever = new S3FileRetriever(s3Client);
			testEnvironment = new EnvironmentVariableS3TestEnvironment();
			log = LogProvider.GetCurrentClassLogger();
		}

		[Test]
		public async Task Retrieve_ShouldDownloadAllFiles()
		{
			var directory = await fileRetriever.Retrieve(testEnvironment.TestBucket, "", CancellationToken.None);

			Assert.IsTrue(Directory.Exists(directory));

			try
			{
				File.Delete(directory);
			}
			catch
			{
				log.Warn("Could not delete {Directory}", directory);
			}
		}
	}
}