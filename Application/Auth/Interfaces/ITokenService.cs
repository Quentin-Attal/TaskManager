using Application.Auth.Models;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Auth.Interfaces
{
    public interface ITokenService
    {
        string CreateAccessToken(AppUser user);
        RefreshTokenDescriptor CreateRefreshToken();
        string HashRefreshToken(string refreshTokenPlain);
        string GetAllRefreshTokenByUserId(Guid userId);
    }

}
