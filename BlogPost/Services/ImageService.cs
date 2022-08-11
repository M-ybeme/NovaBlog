using NovaBlog.Services.Interfaces;

namespace NovaBlog.Services
{
    public class ImageService : IImageService
    {
      
        public string ConvertByteArrayToFile(byte[] fileData, string extension)
        {
            //ToDo: Blog Customizations

           
            try
            {
                string ImageBase64Data = Convert.ToBase64String(fileData);
                return string.Format($"data:{extension};base64,{ImageBase64Data}");
            }
            catch (Exception)
            {
                throw;
            }            
        }

        public async Task<byte[]> ConvertFileToByteArrayAsync(IFormFile file)
        {
            try
            {
                using MemoryStream memoryStream = new();
                await file.CopyToAsync(memoryStream);
                byte[] byteFile = memoryStream.ToArray();

                return byteFile;
            }
            catch (Exception)
            {
                throw;
            }            
        }
    }
}
