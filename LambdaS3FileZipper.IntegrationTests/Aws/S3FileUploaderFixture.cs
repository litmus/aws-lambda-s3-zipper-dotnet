using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using LambdaS3FileZipper.Aws;
using LambdaS3FileZipper.IntegrationTests.Aws.Interfaces;
using NUnit.Framework;

namespace LambdaS3FileZipper.IntegrationTests.Aws
{
	[TestFixture]
    public class S3FileUploaderFixture
	{
		private readonly S3FileUploader fileUploader;
		private readonly IS3TestEnvironment testEnvironment;

		public S3FileUploaderFixture()
		{
			var s3Client = new AwsS3Client(new AmazonS3Client(RegionEndpoint.USEast1));
			fileUploader = new S3FileUploader(s3Client);
			testEnvironment = new EnvironmentVariableS3TestEnvironment();
		}

	    [Test]
	    public async Task Upload_ShouldSaveResource()
	    {
		    var localTestFile = Path.Combine(Path.GetTempPath(), "uploadTest.txt");
		    await File.WriteAllTextAsync(localTestFile, "upload test", CancellationToken.None);

		    var result = await fileUploader.Upload(testEnvironment.TestBucket, "uploadTest.txt", localTestFile, CancellationToken.None);

			Assert.NotNull(result);
		    Assert.True(Uri.TryCreate(result, UriKind.Absolute, out var uri));
	    }
    }
}
