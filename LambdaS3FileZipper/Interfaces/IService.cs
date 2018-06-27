using System.Threading.Tasks;
using LambdaS3FileZipper.Models;

namespace LambdaS3FileZipper.Interfaces
{
    public interface IService
    {
	    Task<Response> Process(Request request);
    }
}
