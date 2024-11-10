using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Blog.Core.DTOs.Auth;
using Blog.Core.Interfaces;
using Blog.Data.Data;
using Blog.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Blog.Core.Services;

public class UserService : IUserService
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ApplicationDbContext _context;
    private readonly JwtSettings _jwtSettings;

    public UserService(UserManager<IdentityUser> userManager, ApplicationDbContext context, IOptions<JwtSettings> jwtSettings)
    {
        _userManager = userManager;
        _context = context;
        _jwtSettings = jwtSettings.Value;
    }
    public async Task<(IdentityResult Result, IdentityUser User)> RegisterUserAsync(RegisterUserDto dto)
    {
        var user = new IdentityUser { UserName = dto.Nome, Email = dto.Email };
        var result = await _userManager.CreateAsync(user, dto.Password);       
        if (result.Succeeded)
        {
            // Adiciona o Autor associado ao usuário criado
            var autor = new Autor
            {
                Id = user.Id,
                Nome = dto.Nome,
                Email = dto.Email
            };

            _context.Autores.Add(autor);
            await _context.SaveChangesAsync();

           
        }
        return (result, result.Succeeded ? user : null);

    }
    public async Task<string> ValidateUserAsync(LoginUserDto loginUser)
    {
        var user = await _userManager.FindByEmailAsync(loginUser.Email);
        if (user == null || !await _userManager.CheckPasswordAsync(user, loginUser.Password))
        {
            return null;
        }
        return await GerarJwt(user.Email);
    }

    private async Task<string> GerarJwt(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        var roles = await _userManager.GetRolesAsync(user);

        var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email )
            };

        // Adicionar roles como claims
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));

        }

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtSettings.Segredo);

        var token = tokenHandler.CreateToken(new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Issuer = _jwtSettings.Emissor,
            Audience = _jwtSettings.Audiencia,
            Expires = DateTime.UtcNow.AddHours(_jwtSettings.ExpiracaoHoras),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        });

        var encodedToken = tokenHandler.WriteToken(token);

        return encodedToken;
    }

}
