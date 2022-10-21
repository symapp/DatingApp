﻿using System.Security.Claims;
using API.DTO;
using API.Entities;
using API.Helpers;

namespace API.Interface;

public interface IUserRepository
{
    void Update(AppUser user);
    

    Task<IEnumerable<AppUser>> GetUsersAsync();

    Task<AppUser> GetUserByIdAsync(int id);

    Task<AppUser> GetUserByUsernameAsync(string username);

    Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams);

    Task<MemberDto> GetMemberAsync(string username, ClaimsPrincipal currentUser);

    Task<string> GetUserGender(string username);

    Task<AppUser> GetUserByPhotoId(int photoId);
}