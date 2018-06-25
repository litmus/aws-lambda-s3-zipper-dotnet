using System.Threading.Tasks;
using LambdaS3FileZipper.Models;

namespace LambdaS3FileZipper.Interfaces
{
	public interface IHandler
	{
		Task<Response> Handle(Request request);
	}
}