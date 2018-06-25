using System.Threading.Tasks;
using LambdaS3FileZipper.Interfaces;
using LambdaS3FileZipper.Models;

namespace LambdaS3FileZipper
{
    public class Handler : IHandler
    {
	    private readonly IFileRetriever fileRetriever;
	    private readonly IFileZipper fileZipper;
	    private readonly IFileUploader fileUploader;

		public Handler(IFileRetriever fileRetriever, IFileZipper fileZipper, IFileUploader fileUploader)
		{
			this.fileRetriever = fileRetriever;
			this.fileZipper = fileZipper;
			this.fileUploader = fileUploader;
		}

	    public async Task<Response> Handle(Request request)
	    {
		    var files = await fileRetriever.Retrieve(request.OriginBucketName, request.OriginResourceName);

		    var compressedFileName = await fileZipper.Compress(files);

		    var url = await fileUploader.Upload(request.DestinationBucketName, request.DestinationResourceName, compressedFileName);

			return new Response(url);
	    }
    }
}
