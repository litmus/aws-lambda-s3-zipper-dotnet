using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LambdaS3FileZipper.IntegrationTests.Extensions;
using LambdaS3FileZipper.IntegrationTests.Logging;
using LambdaS3FileZipper.IntegrationTests.Testing;
using LambdaS3FileZipper.Models;
using NUnit.Framework;
using FileAssert = LambdaS3FileZipper.IntegrationTests.Testing.FileAssert;

namespace LambdaS3FileZipper.IntegrationTests
{
	[TestFixture]
	public class FileZipperFixture
	{
		private ILog log;

		private FileZipper fileZipper;

		private string zipFilePath;
		private string sourceDirectoryName;
		private string sourceDirectoryPath;
		private CancellationToken cancellationToken;

		[OneTimeSetUp]
		public void OneTimeSetUp()
		{
			ConsoleLog.EnableSerilog();

			log = LogProvider.GetCurrentClassLogger();
		}

		[SetUp]
		public void SetUp()
		{
			sourceDirectoryName = Guid.NewGuid().ToString();
			sourceDirectoryPath = Path.Combine(Path.GetTempPath(), sourceDirectoryName);
			cancellationToken = CancellationToken.None;

			Directory.CreateDirectory(sourceDirectoryPath);

			fileZipper = new FileZipper();
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
			var subDirectoryName = Guid.NewGuid().ToString();
			var fileResponses = await CreateTempTextFiles(subDirectoryName, "text-file-0.txt", "text-file-1.txt");

			var zipFileResponse = await fileZipper.Compress(zipFileKey, fileResponses, cancellationToken: cancellationToken);

			zipFilePath = Path.Combine(Path.GetTempPath(), zipFileKey);
			await zipFileResponse.WriteTo(zipFilePath);
			log.Debug("Wrote ZIP files to {ZipFilePath}", zipFilePath);
			Debugger.Break();

			FileAssert.ZipHasFiles(zipFilePath, expectedFileCount: 2);
			FileAssert.ZipHasFilesWithName(zipFilePath, entryFileName: $"{subDirectoryName}/text-file-0.txt");
		}

		[Test]
		public async Task Compress_WithFlatEnabled_ShouldZipFilesInMemory()
		{
			var zipFileKey = $"{sourceDirectoryName}.zip";
			var subDirectoryName = Guid.NewGuid().ToString();
			var fileResponses = await CreateTempTextFiles(subDirectoryName, "text-file-0.txt", "text-file-1.txt");

			var zipFileResponse = await fileZipper.Compress(zipFileKey, fileResponses, flat: true, cancellationToken);

			zipFilePath = Path.Combine(Path.GetTempPath(), zipFileKey);
			await zipFileResponse.WriteTo(zipFilePath);
			log.Debug("Wrote ZIP files to {ZipFilePath}", zipFilePath);
			Debugger.Break();

			FileAssert.ZipHasFiles(zipFilePath, expectedFileCount: 2);
			FileAssert.ZipHasFilesWithName(zipFilePath, entryFileName: "text-file-0.txt");
		}

		private async Task<IEnumerable<FileResponse>> CreateTempTextFiles(string subDirectoryName, params string[] fileNames)
		{
			// Create sub-directory to validate flat structure of resulting compressed file
			FileTool.CreateDirectoryIn(sourceDirectoryPath, subDirectoryName);

			var tempFileNames = fileNames.Select(fileName => $"{subDirectoryName}/{fileName}");
			var fileResponses = await Task.WhenAll(tempFileNames
				.Select(fileName => FileTool.CreateTempTextFile(sourceDirectoryPath, fileName, fileContent: fileName))
				.ToArray());
			return fileResponses;
		}
	}
}