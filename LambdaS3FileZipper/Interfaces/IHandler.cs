using LambdaS3FileZipper.Models;

namespace LambdaS3FileZipper.Interfaces
{
	public interface IHandler
	{
		Response Handle(Request request);
	}
}