using Domain;
using Microsoft.AspNetCore.Http;

namespace Application.Interfaces
{
    public interface ICloudinaryAccount
    {
        Photo AddPhotoForUser(IFormFile photo);
        string DeletePhotoForUser(Photo photo);
    }
}