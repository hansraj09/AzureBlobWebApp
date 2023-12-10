using System.Security.Claims;
using AzureBlobWebApp.BusinessLayer.DTOs;
using AzureBlobWebApp.DataLayer.DTOs;
using AzureBlobWebApp.DataLayer.Models;
using Microsoft.AspNetCore.Mvc;

namespace AzureBlobWebApp.BusinessLayer.Interfaces
{
    public interface ILoginService
    {
        TokenResponse Authenticate(string username, Claim[] claims);
        TokenResponse Authenticate([FromBody] CUserCredential userCred);
        TokenResponse Refresh([FromBody] TokenResponse token);
        ResponseBase Register([FromBody] User userInfo);

        // this is a temporary method to test the authorization functionality
        // REMOVE LATER
        IEnumerable<string> GetAllUsers();
    }
}
