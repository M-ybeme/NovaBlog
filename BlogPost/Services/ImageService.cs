using NovaBlog.Services.Interfaces;

namespace NovaBlog.Services
{
    public class ImageService : IImageService
    {
        //ToDo: Blog Customizations

        //points to file location of images
        private readonly string _defaultBlogPostImageSrc = "/img/Placeholder.png";
        private readonly string _deafaultUserImageSrc = "/img/DefaultUserIamge.png";
        private readonly string _defaultCategoryImg = "/img/DefaultCategoryImage.png";
        public string ConvertByteArrayToFile(byte[] fileData, string extension, int imageType)
        {

            if (fileData == null || fileData.Length == 0 )
            {
                
                switch (imageType)
                {
                    //numbers match defaultIamge in enums 
                    //BlogUser Image
                    case 1:  return _deafaultUserImageSrc;
                    //BlogPost Image
                    case 2: return _defaultBlogPostImageSrc;
                    // Category Image
                    case 3: return _defaultCategoryImg;
                }
            }

           
            try
            {
                string ImageBase64Data = Convert.ToBase64String(fileData!);
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
