using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using LambdaS3FileZipper.Aws;
using NUnit.Framework;

namespace LambdaS3FileZipper.IntegrationTests.Aws
{
	[TestFixture]
	public class S3FileUploaderFixture : S3Fixture
	{
		private readonly S3FileUploader fileUploader;

		public S3FileUploaderFixture()
		{
			fileUploader = new S3FileUploader(Client);
		}

		[Test]
		public async Task Upload_ShouldSaveResource()
		{
			var localTestFile = Path.Combine(Path.GetTempPath(), "uploadTest.txt");
			await File.WriteAllTextAsync(localTestFile, "upload test", CancellationToken.None);

			var result = await fileUploader.Upload(TestEnvironment.TestBucket, "uploadTest.txt", localTestFile,
				CancellationToken.None);

			Assert.NotNull(result);
			Assert.True(Uri.TryCreate(result, UriKind.Absolute, out var uri));
		}
	}
}