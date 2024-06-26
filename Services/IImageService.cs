﻿/*
 * Blog Project
 * An ASP.NET MVC Blog
 * Based on Coder Foundry Blog series
 * 
 * Mark Ambrocio 2022
 * https://github.com/markanator/csharp_AmbroBlogProject
 */
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
