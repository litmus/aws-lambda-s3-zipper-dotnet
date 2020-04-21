using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LambdaS3FileZipper.IntegrationTests.Extensions;
using LambdaS3FileZipper.IntegrationTests.Testing;
using NUnit.Framework;
using FileAssert = LambdaS3FileZipper.IntegrationTests.Testing.FileAssert;

namespace LambdaS3FileZipper.IntegrationTests
{
	[TestFixture]
	public class FileZipperFixture
	{
		private FileZipper fileZipper;

		private string zipFilePath;
		private string sourceDirectoryName;
		private string sourceDirectoryPath;

		private CancellationToken cancellationToken;

		[SetUp]
		public void SetUp()
		{
			sourceDirectoryName = Guid.NewGuid().ToString();
			sourceDirectoryPath = Path.Combine(Path.GetTempPath(), sourceDirectoryName);
			Directory.CreateDirectory(sourceDirectoryPath);

			fileZipper = new FileZipper();

			cancellationToken = CancellationToken.None;
		}

		[TearDown]
		public void TearDown()
		{
			FileTool.TryDeleteFile(zipFilePath);
			FileTool.TryDeleteDirectory(sourceDirectoryPath);
		}

		[Test]
		public async Task Compress_ShouldZipFolder()
		{
			var textFilePath = Path.Combine(sourceDirectoryPath, "compress.txt");
			await File.WriteAllTextAsync(textFilePath, "Compress test.");

			zipFilePath = await fileZipper.Compress(sourceDirectoryPath);
			Debugger.Break();

			FileAssert.Exists(zipFilePath);
			FileAssert.ZipHasFiles(zipFilePath, expectedFileCount: 1);
		}

		[Test]
		public async Task Compress_WithFlatEnabled_ShouldZipFolder()
		{
			await FileTool.CreateTempTextFile(sourceDirectoryPath, "compress.txt", fileContent: "compress");

			var subDirectoryPath = Path.Combine(sourceDirectoryPath, Guid.NewGuid().ToString());
			Directory.CreateDirectory(subDirectoryPath);
			await FileTool.CreateTempTextFile(subDirectoryPath, "compress-sub.txt", fileContent: "compress-sub");

			zipFilePath = await fileZipper.Compress(sourceDirectoryPath, flat: true);
			Debugger.Break();

			FileAssert.Exists(zipFilePath);
			FileAssert.ZipHasFiles(zipFilePath, expectedFileCount: 2);
		}

		[Test]
		public async Task Compress_ShouldZipFilesInMemory()
		{
			var zipFileKey = $"{sourceDirectoryName}.zip";
			zipFilePath = Path.Combine(Path.GetTempPath(), zipFileKey);

			var tempFileNames = new[] {"text-file-0.txt", "text-file-1.txt"};
			var fileResponses = await Task.WhenAll(tempFileNames
				.Select(fileName => FileTool.CreateTempTextFile(sourceDirectoryPath, fileName, fileContent: fileName))
				.ToArray());

			var zipFileResponse = await fileZipper.Compress(zipFileKey, fileResponses, cancellationToken);

			await zipFileResponse.WriteTo(zipFilePath);
			Console.WriteLine("Created zipFilePath at {0}", zipFilePath);
			Debugger.Break();

			FileAssert.HasContent(zipFilePath);
		}
	}
}