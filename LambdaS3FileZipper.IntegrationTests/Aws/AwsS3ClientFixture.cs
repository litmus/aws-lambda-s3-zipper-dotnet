using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace LambdaS3FileZipper.IntegrationTests.Aws
{
	[TestFixture]
	public class AwsS3ClientFixture : S3Fixture
	{
		[Test]
		public async Task List_ShouldReturnObjects()
		{
			var result = await Client.List(TestEnvironment.IntegrationTestBucket, "", CancellationToken.None);
			Assert.True(result.Any());
		}

		[Test]
		public async Task Download_ShouldRetrieveObject()
		{
			var localPath = await Client.Download(TestEnvironment.IntegrationTestBucket, TestEnvironment.IntegrationTestResourceName, Path.GetTempPath(), CancellationToken.None);

			Assert.True(File.Exists(localPath));

			DeleteLocalTempFile(localPath);
		}

		[Test]
		public async Task Upload_ShouldSaveFile()
		{
			const string testFileName = "uploadTest.txt";
			var localTestFile = Path.Combine(Path.GetTempPath(), testFileName);

			try
			{
				await File.WriteAllTextAsync(localTestFile, "upload test", CancellationToken.None);

				await Client.Upload(TestEnvironment.IntegrationTestBucket, testFileName, localTestFile, CancellationToken.None);
			}
			finally
			{
				DeleteLocalTempFile(localTestFile);
				await DeleteTempS3Object(TestEnvironment.IntegrationTestBucket, testFileName);
			}
		}

		[Test]
		public async Task Delete_ShouldRemoveObject()
		{
			const string testFileName = "uploadTest.txt";
			var localTestFile = Path.Combine(Path.GetTempPath(), testFileName);

			try
			{
				await File.WriteAllTextAsync(localTestFile, "upload test", CancellationToken.None);

				await Client.Upload(TestEnvironment.IntegrationTestBucket, testFileName, localTestFile, CancellationToken.None);

				Assert.DoesNotThrowAsync(() => Client.Delete(TestEnvironment.IntegrationTestBucket, testFileName, CancellationToken.None));

				var objects = await Client.List(TestEnvironment.IntegrationTestBucket, "", CancellationToken.None);
				Assert.False(objects.Contains(testFileName));
			}
			finally
			{
				DeleteLocalTempFile(localTestFile);
				await DeleteTempS3Object(TestEnvironment.IntegrationTestBucket, testFileName);
			}
		}
	}
}