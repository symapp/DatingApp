using API.DTO;
using API.Entities;
using API.Helpers;
using API.Interface;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class PhotoRepository : IPhotoRepository
{
    private readonly DataContext _context;
    private readonly IMapper _mapper;

    public PhotoRepository(DataContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<Photo>> GetUnapprovedPhotos()
    {
        var photoForApproval = await _context.Photos
            .IgnoreQueryFilters()
            .Where(p => !p.IsApproved)
            .ToListAsync();
        return photoForApproval;
    }

    public async Task<Photo> GetPhotoById(int id)
    {
        var photo = await _context.Photos.IgnoreQueryFilters().FirstOrDefaultAsync(p => p.Id == id);
        
        return _mapper.Map<Photo>(photo);
    }

    public void RemovePhoto(Photo photo)
    {
        _context.Photos.Remove(photo);
    }
}