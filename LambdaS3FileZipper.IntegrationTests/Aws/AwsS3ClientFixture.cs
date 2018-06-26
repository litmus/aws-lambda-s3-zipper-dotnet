using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Amazon;
using LambdaS3FileZipper.Aws;
using LambdaS3FileZipper.IntegrationTests.Aws.Interfaces;
using LambdaS3FileZipper.IntegrationTests.Logging;
using NUnit.Framework;

namespace LambdaS3FileZipper.IntegrationTests.Aws
{
	[TestFixture]
	public class AwsS3ClientFixture
	{
		private static readonly AwsS3Client client = new AwsS3Client(RegionEndpoint.USEast1);
		private readonly IS3TestEnvironment testEnvironment;
		private readonly ILog log;

		public AwsS3ClientFixture()
		{
			testEnvironment = new EnvironmentVariableS3TestEnvironment();
			log = LogProvider.GetCurrentClassLogger();
		}

		[Test]
		public async Task List_ShouldReturnObjects()
		{
			var result = await client.List(testEnvironment.TestBucket, "", CancellationToken.None);
		}

		[Test]
		public async Task Download_ShouldRetrieveObject()
		{
			var localPath = await client.Download(testEnvironment.TestBucket, testEnvironment.TestObject, Path.GetTempPath(), CancellationToken.None);

			Assert.True(File.Exists(localPath));

			try
			{
				File.Delete(localPath);
			}
			catch
			{
				log.Warn("Could not delete {File}", localPath);
			}
		}
	}
}