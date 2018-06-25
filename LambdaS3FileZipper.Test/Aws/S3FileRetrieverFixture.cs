using System.Threading;
using System.Threading.Tasks;
using LambdaS3FileZipper.Aws;
using NSubstitute;
using NUnit.Framework;

namespace LambdaS3FileZipper.Test.Aws
{
	[TestFixture]
	public class S3FileRetrieverFixture
	{
		private S3FileRetriever fileRetriever;

		private IAwsS3Client s3Client;

		[SetUp]
		public void SetUp()
		{
			s3Client = Substitute.For<IAwsS3Client>();

			fileRetriever = new S3FileRetriever(s3Client);
		}

		[Test]
		public async Task List_ShouldReturnObjectPaths()
		{
			s3Client.List(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
				.Returns(new [] { "file1", "file2" });

			var list = fileRetriever.Retrieve("bucket", "resource", CancellationToken.None);

			Assert.IsNotNull(list);
		}
	}
}