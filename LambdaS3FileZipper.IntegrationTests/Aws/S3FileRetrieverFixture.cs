﻿using System.Diagnostics;
using System.IO;
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
		public async Task RetrieveToLocalDirectory_ShouldDownloadAllFiles()
		{
			var resourceExpressionPattern = ".*png$";
			var directoryPath = await fileRetriever.RetrieveToLocalDirectory(TestEnvironment.IntegrationTestBucket, TestEnvironment.IntegrationTestResourceName, resourceExpressionPattern);

			var directory = new DirectoryInfo(directoryPath);
			Debugger.Break();
			Assert.That(directory.Exists);
			Assert.That(directory.GetFiles(), Is.Not.Empty);

			DeleteLocalTempDirectory(directoryPath);
		}
	}
}