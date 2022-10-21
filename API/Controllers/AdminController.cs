using API.DTO;
using API.Entities;
using API.Helpers;
using API.Interface;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AdminController : BaseApiController
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IPhotoService _photoService;

    public AdminController(UserManager<AppUser> userManager, IUnitOfWork unitOfWork, IMapper mapper,
        IPhotoService photoService)
    {
        _userManager = userManager;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _photoService = photoService;
    }

    [Authorize(Policy = "RequireAdminRole")]
    [HttpGet("users-with-roles")]
    public async Task<ActionResult> GetUsersWithRoles()
    {
        var users = await _userManager.Users
            .Include(r => r.UserRoles)
            .ThenInclude(r => r.Role)
            .OrderBy(u => u.UserName)
            .Select(u => new
            {
                u.Id,
                Username = u.UserName,
                Roles = u.UserRoles.Select(r => r.Role.Name).ToList()
            })
            .ToListAsync();

        return Ok(users);
    }

    [HttpPost("edit-roles/{username}")]
    public async Task<ActionResult> EditRoles(string username, [FromQuery] string roles)
    {
        var selectedRoles = roles.Split(",").ToArray();

        var user = await _userManager.FindByNameAsync(username);
        if (user == null) return NotFound();

        var userRoles = await _userManager.GetRolesAsync(user);

        var result = await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));
        if (!result.Succeeded) return BadRequest("Failed to add to roles");

        result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(userRoles));
        if (!result.Succeeded) return BadRequest("Failed to remove from roles");

        return Ok(await _userManager.GetRolesAsync(user));
    }
    
    [Authorize(Policy = "ModeratePhotoRole")]
    [HttpGet("photos-to-moderate")]
    public async Task<ActionResult<IEnumerable<PhotoForApprovalDto>>> GetPhotosForApproval()
    {
        var photos = await _unitOfWork.PhotoRepository.GetUnapprovedPhotos();
        return Ok(_mapper.Map<IEnumerable<PhotoForApprovalDto>>(photos));
    }

    [Authorize(Policy = "ModeratePhotoRole")]
    [HttpPost("approve-photo/{photoId}")]
    public async Task<ActionResult> ApprovePhoto(int photoId)
    {
        var photo = await _unitOfWork.PhotoRepository.GetPhotoById(photoId);

        if (photo == null)
            return NotFound();

        if (photo.IsApproved)
            return BadRequest("This photo is already approved");

        
        var user = await _unitOfWork.UserRepository.GetUserByPhotoId(photoId);
        if (user == null) return BadRequest("This photo isn't owned by anyone");
        
        var approvedPhotos = user.Photos.Where(p => p.IsApproved);
        if (!approvedPhotos.Any()) 
            photo.IsMain = true;
        
        photo.IsApproved = true;

        if (await _unitOfWork.Complete())
            return Ok();

        return BadRequest("Could not approve photo");
    }

    [Authorize(Policy = "ModeratePhotoRole")]
    [HttpPost("reject-photo/{id}")]
    public async Task<ActionResult> RejectPhoto(int id)
    {
        var photo = await _unitOfWork.PhotoRepository.GetPhotoById(id);

        if (photo == null)
            return NotFound();
        
        if (photo.PublicId != null)
        {
            var deletionResult = await _photoService.DeletePhotoAsync(photo.PublicId);
            if (deletionResult.Error != null) return BadRequest(deletionResult.Error.Message);
        }
        
        _unitOfWork.PhotoRepository.RemovePhoto(photo);

        if (await _unitOfWork.Complete())
            return Ok();

        return BadRequest("Could not approve photo");
    }

}