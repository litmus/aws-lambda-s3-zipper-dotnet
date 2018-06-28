namespace LambdaS3FileZipper.IntegrationTests.Aws.Interfaces
{
	public interface IS3TestEnvironment
	{
		string TestBucket { get; }
		string TestObject { get; }

		string OriginTestBucket { get; }
		string OriginTestResourceName { get; }
		string DestinationTestBucket { get; }
	}
}