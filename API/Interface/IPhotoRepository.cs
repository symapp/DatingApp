using API.DTO;
using API.Entities;
using API.Helpers;

namespace API.Interface;

public interface IPhotoRepository
{
    Task<IEnumerable<Photo>> GetUnapprovedPhotos();

    Task<Photo> GetPhotoById(int id);

    void RemovePhoto(Photo photo);
}