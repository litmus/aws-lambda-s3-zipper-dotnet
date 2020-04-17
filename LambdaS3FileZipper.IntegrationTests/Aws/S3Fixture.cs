using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using LambdaS3FileZipper.Aws;
using LambdaS3FileZipper.IntegrationTests.Aws.Interfaces;
using LambdaS3FileZipper.IntegrationTests.Logging;
using LambdaS3FileZipper.Interfaces;
using NUnit.Framework;

namespace LambdaS3FileZipper.IntegrationTests.Aws
{
	[TestFixture]
	public abstract class S3Fixture
	{
		protected IAwsS3Client Client { get; }
		protected IS3TestEnvironment TestEnvironment { get; }
		internal ILog Log { get; }

		protected S3Fixture()
		{
			Client = new AwsS3Client(new AmazonS3Client(RegionEndpoint.USEast1));
			TestEnvironment = new EnvironmentVariableS3TestEnvironment();
			Log = LogProvider.GetCurrentClassLogger();
		}

		protected void DeleteLocalTempDirectory(string tempDirectory)
		{
			try
			{
				if (Directory.Exists(tempDirectory))
				{
					Directory.Delete(tempDirectory);
				}
			}
			catch
			{
				Log.Warn("Could not delete {Directory}", tempDirectory);
			}
		}

		protected void DeleteLocalTempFile(string tempFile)
		{
			try
			{
				if (File.Exists(tempFile))
				{
					File.Delete(tempFile);
				}
			}
			catch
			{
				Log.Warn("Could not delete {File}", tempFile);
			}
		}

		protected async Task DeleteTempS3Object(string bucket, string resourceName)
		{
			try
			{
				await Client.Delete(bucket, resourceName, CancellationToken.None);
			}
			catch
			{
				Log.Warn("Could not delete S3 object {ResourceName}", resourceName);
			}
		}

		protected void AssertFileIsValid(string filePath)
		{
			var file = new FileInfo(filePath);
			Assert.That(file.Exists, Is.True);
			Assert.That(file.Length, Is.GreaterThan(0));
		}
	}
}