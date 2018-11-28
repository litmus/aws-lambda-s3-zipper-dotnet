namespace LambdaS3FileZipper.IntegrationTests.Aws.Interfaces
{
	public interface IS3TestEnvironment
	{
		string IntegrationTestBucket { get; }
		string IntegrationTestResourceName { get; }

		string OriginTestBucket { get; }
		string OriginTestResourceName { get; }
		string DestinationTestBucket { get; }
	}
}