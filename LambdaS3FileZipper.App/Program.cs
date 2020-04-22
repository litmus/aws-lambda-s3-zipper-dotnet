using System.Threading.Tasks;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using LambdaS3FileZipper.Aws;
using LambdaS3FileZipper.Models;
using Serilog;

namespace LambdaS3FileZipper.App
{
    internal class Program
    {
		internal static async Task Main()
        {
			// Enable Serilog Console logging
			Log.Logger = new LoggerConfiguration().MinimumLevel.Verbose().WriteTo.Console().CreateLogger();

			// Todo: Fill in credential, request details
			var accessKey = "access-key";
			var secretKey = "secret-key";
			var endpoint = RegionEndpoint.USEast1;
			var requestGuid = "guid";
			var request = new Request
			{
				OriginBucketName = "origin-bucket",
				OriginResourceName = $"{requestGuid}/results",
				DestinationBucketName = "target-bucket",
				DestinationResourceName = $"{requestGuid}.zip",
				OriginResourceExpressionPattern = @".*.png$",
			};

			var client = new AwsS3Client(new AmazonS3Client(new BasicAWSCredentials(accessKey, secretKey), endpoint));
			var service = new Service(new S3FileRetriever(client), new FileZipper(), new S3FileUploader(client));
	        var handler = new Handler(service);
			var context = new InternalLambdaContext(awsRequestId: requestGuid);

			await handler.Handle(request, context);
		}
    }
}
