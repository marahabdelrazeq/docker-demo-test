using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using QRCoder;
 

namespace CommonLibrary.Utilities.FileHelpers
{
    public class FileUploadUtility : IFileUploadUtility
    {
        private readonly IConfiguration _configuration;

        public FileUploadUtility(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> UploadFileAsync(IFormFile formFile, string entityName)
        {
            try
            {
                var folderPath = _configuration["AttachmentSettings:FolderPath"]! + "/" + entityName;

                var fileName = $"{Guid.NewGuid() + Path.GetExtension(formFile.FileName).ToLowerInvariant()}";

                var filePath = folderPath + "/" + fileName;

                var filePathUrl = entityName + "/" + formFile.FileName;

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                if (!File.Exists(filePathUrl))
                {
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await formFile.CopyToAsync(fileStream);

                        fileStream.Flush();
                    }

                    return entityName + "/" + fileName;
                }

                return filePathUrl;
            }
            catch (Exception ex)
            {
                throw new Exception("File upload failed", ex);
            }
        }

        public async Task<string> UploadQR(string entityName, params string[] data)
        {
            try
            {
                var folderPath = _configuration["AttachmentSettings:FolderPath"]! + "/" + entityName;

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                string fileName = Guid.NewGuid().ToString().Substring(0, 4);
                string fullPath = Path.Combine(folderPath, fileName);
                string imagePath = fullPath + ".png";

                //QRCodeGenerator qrGenerator = new QRCodeGenerator();
                //QRCodeData qrCodeData = qrGenerator.CreateQrCode(string.Join(",", data), QRCodeGenerator.ECCLevel.Q);
                //QRCode qrCode = new QRCode(qrCodeData);
                ////using (Bitmap qrCodeImage = qrCode.GetGraphic(20))
                ////{
                ////    qrCodeImage.Save(imagePath, ImageFormat.Png);
                ////}
                ///

                using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
                using (QRCodeData qrCodeData = qrGenerator.CreateQrCode(string.Join(",", data), QRCodeGenerator.ECCLevel.Q))
                using (PngByteQRCode qrCode = new PngByteQRCode(qrCodeData))
                {
                    byte[] qrCodeImage = qrCode.GetGraphic(20);

                    //save the image to a file
                    using (FileStream fs = new FileStream(imagePath, FileMode.Create))
                    {
                        await fs.WriteAsync(qrCodeImage, 0, qrCodeImage.Length);
                    }
                }




                return entityName + "/" + fileName + ".png";
            }
            catch (Exception ex)
            {
                throw new Exception("QR code generation failed", ex);
            }
        }
    }
}
