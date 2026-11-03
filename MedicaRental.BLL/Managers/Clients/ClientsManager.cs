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
    private readonly IAccountsManager _accountsManager;
    private readonly UserManager<AppUser> _userManager;

    public ClientsManager(IUnitOfWork unitOfWork,
        IAccountsManager accountsManager,
        UserManager<AppUser> userManager
        )
    {
        _unitOfWork = unitOfWork;
        _accountsManager = accountsManager;
        _userManager = userManager;
    }

    public async Task<StatusDto> ApproveUserAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
            return new StatusDto(
                StatusMessage: $"User {email} coudn't be found",
                StatusCode: System.Net.HttpStatusCode.NotFound
                );
        var client = await _unitOfWork.Clients.FindAsync(c => c.Id == user.Id);
        if (client is null)
            return new StatusDto(
                StatusMessage: $"User {email} is not a client and can't be approved to rent",
                StatusCode: System.Net.HttpStatusCode.NotFound
                );

        client.IsGrantedRent = true;
        var isUpdated = _unitOfWork.Clients.Update(client);
        if (!isUpdated)
            return new StatusDto(
                StatusMessage: $"User {email} coudn't be updated",
                StatusCode: System.Net.HttpStatusCode.BadRequest
                );

        try
        {
            _unitOfWork.Save();
            return new StatusDto(
                StatusMessage: $"User {email} is approved to rent",
                StatusCode: System.Net.HttpStatusCode.OK
                );
        }
        catch
        {
            return new StatusDto(
                StatusMessage: $"User {email} coudn't be updated",
                StatusCode: System.Net.HttpStatusCode.BadRequest
                );
        }
    }

    public async Task<ClientRegisterStatusDto> RegisterNewUserAsync(ClientRegisterInfoDto clientRegisterInfoDto)
    {
        var baseUserRegisterStatus = await _accountsManager.RegisterNewUserAsync(clientRegisterInfoDto.BaseUserRegisterInfo);
        if (!baseUserRegisterStatus.isCreated)
            return new ClientRegisterStatusDto
            (
                isCreated: false,
                RegisterMessage: baseUserRegisterStatus.RegisterMessage
            );

        var newUser = baseUserRegisterStatus.NewUser;
        if (newUser is null)
            return new ClientRegisterStatusDto
            (
                isCreated: false,
                RegisterMessage: "Couldn't Create User"
            );

        try
        {

            var newClient = new Client
            {
                Id = newUser.Id,
                Ssn = clientRegisterInfoDto.SSN,
                Address = clientRegisterInfoDto.Address,
                IsGrantedRent = false,
                NationalIdImage = Convert.FromBase64String(clientRegisterInfoDto.NationalIdImage),
                UnionCardImage = Convert.FromBase64String(clientRegisterInfoDto.UnionCardImage),
            };

            await _unitOfWork.Clients.AddAsync(newClient);
            _unitOfWork.Save();
        }
        catch
        {
            await _accountsManager.DeleteAsync(newUser);
            return new ClientRegisterStatusDto
            (
                isCreated: false,
                RegisterMessage: "National ID is already registered"
            );

        }

        return new ClientRegisterStatusDto
        (
            isCreated: true,
            RegisterMessage: "Account Created Successfully"
        );
    }
}
