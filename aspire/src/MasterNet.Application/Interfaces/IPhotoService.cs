using MasterNet.Application.Photos.GetPhoto;

namespace MasterNet.Application.Interfaces;

public interface IPhotoService
{

    Task<PhotoUploadResult> AddPhoto(byte[] file);

    Task<string> DeletePhoto(string publicId);
}