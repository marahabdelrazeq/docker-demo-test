using Microsoft.AspNetCore.Http;

namespace CommonLibrary.Utilities.FileHelpers
{
    public interface IFileUploadUtility
    {
        Task<string> UploadFileAsync(IFormFile formFile, string entityName);
        Task<string> UploadQR(string entityName, params string[] data);
    }
}
