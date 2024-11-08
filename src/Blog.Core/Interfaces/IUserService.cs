using System;
using Blog.Core.DTOs.Auth;
using Microsoft.AspNetCore.Identity;

namespace Blog.Core.Interfaces;

public interface IUserService
{
    Task<(IdentityResult Result, IdentityUser User, string Token)> RegisterUserAsync(RegisterUserDto dto);
    Task<string> ValidateUserAsync(LoginUserDto dto); 


}