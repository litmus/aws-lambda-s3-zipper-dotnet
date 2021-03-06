﻿using System;
using Amazon.Lambda.Core;

namespace LambdaS3FileZipper.App
{
	internal class InternalLambdaContext : ILambdaContext
	{
		public InternalLambdaContext(string awsRequestId)
		{
			AwsRequestId = awsRequestId;
		}

		public string AwsRequestId { get; }
		public IClientContext ClientContext { get; }
		public string FunctionName { get; }
		public string FunctionVersion { get; }
		public ICognitoIdentity Identity { get; }
		public string InvokedFunctionArn { get; }
		public ILambdaLogger Logger { get; }
		public string LogGroupName { get; }
		public string LogStreamName { get; }
		public int MemoryLimitInMB { get; }
		public TimeSpan RemainingTime { get; }
	}
}
