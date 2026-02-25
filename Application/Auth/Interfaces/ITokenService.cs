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
        RefreshTokenDescriptor CreateRefreshToken(DateTime utcNow);
        string HashRefreshToken(string refreshTokenPlain);
    }

}
