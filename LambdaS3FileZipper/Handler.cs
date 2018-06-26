using System.Threading.Tasks;
using LambdaS3FileZipper.Interfaces;
using LambdaS3FileZipper.Logging;
using LambdaS3FileZipper.Models;

namespace LambdaS3FileZipper
{
	public class Handler : IHandler
	{
		private readonly IFileRetriever fileRetriever;
		private readonly IFileZipper fileZipper;
		private readonly IFileUploader fileUploader;
		private readonly ILog log;

		public Handler(IFileRetriever fileRetriever, IFileZipper fileZipper, IFileUploader fileUploader)
		{
			this.fileRetriever = fileRetriever;
			this.fileZipper = fileZipper;
			this.fileUploader = fileUploader;
			this.log = LogProvider.GetCurrentClassLogger();
		}

		public async Task<Response> Handle(Request request)
		{
			var directory = await fileRetriever.Retrieve(request.OriginBucketName, request.OriginResourceName);
			log.Debug("Retrieved files from {Bucket}:{Resource}", request.OriginBucketName, request.OriginResourceName);

			var compressedFileName = await fileZipper.Compress(directory);

			var url = await fileUploader.Upload(request.DestinationBucketName, request.DestinationResourceName, compressedFileName);

			return new Response(url);
		}
	}
}
