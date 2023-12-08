using System.Security.Claims;
using AzureBlobWebApp.BusinessLayer.DTOs;
using AzureBlobWebApp.DataLayer.Models;
using Microsoft.AspNetCore.Mvc;

namespace AzureBlobWebApp.BusinessLayer.Interfaces
{
    public interface ILoginService
    {
        TokenResponse Authenticate(string username, Claim[] claims);
        TokenResponse Authenticate([FromBody] UserCredential userCred);
        TokenResponse Refresh([FromBody] TokenResponse token);
        ResponseBase Register([FromBody] User userInfo);
    }
}
