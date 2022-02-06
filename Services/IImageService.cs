using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace AmbroBlogProject.Services
{
    public interface IImageService
    {
        Task<byte[]> EncodeImageAsync(IFormFile file);  // file uploads
        Task<byte[]> EncodeImageAsync(string fileName); // static img paths

        string DecodeImage(byte[] data, string contentType); // display actual image
        string ContentType(IFormFile file); // get contentType of the given file
        int Size(IFormFile file); //
    }
}
