using System;
using LambdaS3FileZipper.IntegrationTests.Aws.Interfaces;

namespace LambdaS3FileZipper.IntegrationTests.Aws
{
    public class EnvironmentVariableS3TestEnvironment : IS3TestEnvironment
    {
	    private const string TestBucketEnvironmentVariable = "S3TestBucket";
	    private const string TestObjectEnvironmentVariable = "S3TestObject";

	    private string testBucket;
		public string TestBucket
	    {
			get
		    {
			    if (testBucket == null)
			    {
				    testBucket = Environment.GetEnvironmentVariable(TestBucketEnvironmentVariable);

				    if (testBucket == null)
				    {
						throw new Exception($"missing required environment variable {TestBucketEnvironmentVariable}");
				    }
			    }

				return testBucket;
			}
	    }

	    private string testObject;
	    public string TestObject
		{
		    get
		    {
			    if (testObject == null)
			    {
				    testObject = Environment.GetEnvironmentVariable(TestObjectEnvironmentVariable);

				    if (testObject == null)
				    {
					    throw new Exception($"missing required environment variable {TestObjectEnvironmentVariable}");
				    }
			    }

			    return testObject;
		    }
	    }
	}
}
