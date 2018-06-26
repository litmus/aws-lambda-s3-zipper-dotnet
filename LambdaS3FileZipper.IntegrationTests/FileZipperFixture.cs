using System;
using System.IO;
using System.Threading.Tasks;
using LambdaS3FileZipper.IntegrationTests.Logging;
using NUnit.Framework;

namespace LambdaS3FileZipper.IntegrationTests
{
	[TestFixture]
	public class FileZipperFixture
	{
		private readonly FileZipper fileZipper;

		public FileZipperFixture()
		{
			fileZipper = new FileZipper();
		}

		[Test]
		public async Task Compress_ShouldZipFolder()
		{
			var tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
			Directory.CreateDirectory(tempDirectory);

			var tempFile = Path.Combine(tempDirectory, "compress.txt");
			await File.WriteAllTextAsync(tempFile, "Compress test.");

			try
			{
				var zipFile = await fileZipper.Compress(tempDirectory);

				Assert.True(File.Exists(zipFile));
			}
			finally
			{
				if (Directory.Exists(tempDirectory))
				{
					Directory.Delete(tempDirectory, recursive: true);
				}
		
				if (File.Exists(tempFile))
				{
					File.Delete(tempFile);
				}
			}
		}
	}
}