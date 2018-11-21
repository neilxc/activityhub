using System;
using System.Net;
using Application.Interfaces;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Domain;
using Infrastructure.Errors;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Infrastructure
{
    public class CloudinaryAccount : ICloudinaryAccount
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryAccount(IOptions<CloudinarySettings> cloudinaryConfig)
        {   
            var acc = new Account(
                cloudinaryConfig.Value.CloudName,
                cloudinaryConfig.Value.ApiKey,
                cloudinaryConfig.Value.ApiSecret
            );

            _cloudinary = new Cloudinary(acc);
        }

        public Photo AddPhotoForUser(IFormFile file)
        {
            var uploadResult = new ImageUploadResult();

            if (file.Length > 0)
            {
                using (var stream = file.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.Name, stream)
                    };

                    uploadResult = _cloudinary.Upload(uploadParams);
                }
            }

            var photo = new Photo
            {
                DateAdded = DateTime.Now,
                IsMain = false,
                PublicId = uploadResult.PublicId,
                Url = uploadResult.SecureUri.ToString()
            };

            return photo;
        }

        public string DeletePhotoForUser(Photo photo)
        {
            var deleteParams = new DeletionParams(photo.PublicId);

            var result = _cloudinary.Destroy(deleteParams);

            if (result.Result == "ok")
            {
                return result.Result;
            }

            throw new RestException(HttpStatusCode.BadRequest);
        }
    }
}