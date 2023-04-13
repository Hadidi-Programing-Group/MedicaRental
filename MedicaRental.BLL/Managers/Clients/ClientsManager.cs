using MedicaRental.BLL.Dtos;
using MedicaRental.DAL.Context;
using MedicaRental.DAL.Models;
using MedicaRental.DAL.UnitOfWork;
using Microsoft.AspNetCore.Identity;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MedicaRental.BLL.Managers;

public class ClientsManager : IClientsManager
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<AppUser> _userManager;

    public ClientsManager(IUnitOfWork unitOfWork, UserManager<AppUser> userManager)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
    }

    public async Task<RegisterStatusDto> RegisterNewUserAsync(RegisterInfoDto registerInfoDto)
    {
        var newUser = new AppUser
        {
            FirstName = registerInfoDto.FirstName,
            LastName = registerInfoDto.LastName,
            Email = registerInfoDto.Email,
            UserName = registerInfoDto.Email,
            PhoneNumber = registerInfoDto.PhoneNumber,
        };

        var registerUserResult = await _userManager.CreateAsync(newUser, registerInfoDto.Password);
        if (!registerUserResult.Succeeded)
            return new RegisterStatusDto
            (
                isCreated: false,
                RegisterMessage: registerUserResult.Errors.First().Description
            );

        var ClaimsList = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, newUser.Id),
            new Claim(ClaimTypes.Email, newUser.Email),
            new Claim(ClaimTypes.GivenName, newUser.FirstName),
            new Claim(ClaimTypes.Surname, newUser.LastName),
            new Claim(ClaimTypes.Role, UserRoles.Client.ToString()),
        };

        var addingClaimsResult = await _userManager.AddClaimsAsync(newUser, ClaimsList);
        if (!addingClaimsResult.Succeeded)
            return new RegisterStatusDto
            (
                isCreated: false,
                RegisterMessage: addingClaimsResult.Errors.First().Description
            );

        try
        {

            var newClient = new Client
            {
                Id = newUser.Id,
                Ssn = registerInfoDto.SSN,
                Address = registerInfoDto.Address,
                IsGrantedRent = false,
                NationalIdImage = Convert.FromBase64String(registerInfoDto.NationalIdImage),
                UnionCardImage = Convert.FromBase64String(registerInfoDto.UnionCardImage),
            };

            _unitOfWork.Clients.Add(newClient);
            _unitOfWork.Save();
        }
        catch (Exception e)
        {
            await _userManager.DeleteAsync(newUser);
            return new RegisterStatusDto
            (
                isCreated: false,
                RegisterMessage: e.Message
            );

        }

        return new RegisterStatusDto
        (
            isCreated: true,
            RegisterMessage: "Account Created Successfully"
        );
    }
}
