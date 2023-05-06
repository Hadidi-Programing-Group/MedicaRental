using MedicaRental.BLL.Dtos;
using MedicaRental.BLL.Helpers;
using MedicaRental.DAL.Context;
using MedicaRental.DAL.Models;
using MedicaRental.DAL.UnitOfWork;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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


    #region Raouf-Added-Methods

    public async Task<UserApprovalInfoWithIdDto?> GetClientApprovalInfoWithIdAsync(string userId)
    {
        return await _unitOfWork.Clients.FindAsync(
            predicate: c => c.Id == userId,
            selector: c => new UserApprovalInfoWithIdDto(
                c.Id,
                c.Ssn,
         SharedHelper.GetMimeFromBase64(Convert.ToBase64String(c.NationalIdImage ?? SharedHelper.CardPlaceHolder)),
            SharedHelper.GetMimeFromBase64(Convert.ToBase64String(c.UnionCardImage ?? SharedHelper.CardPlaceHolder))
                ));
    }



    public async Task<IEnumerable<UserProfileInfoWithIdDto>> GetClientsNeedingApprovalAsync()
    {
        var clients = await _unitOfWork.Clients.FindAllAsync(
            selector: c => new UserProfileInfoWithIdDto(
                c.Id,
                c.Name,
                c.User.FirstName,
                c.User.LastName,
                c.User.PhoneNumber,
                c.Address,
                c.User.Email,
                c.IsGrantedRent
            ),
            predicate: c => !c.IsGrantedRent,
            include: source => source.Include(c => c.User)
        );

        return clients;
    }
    public async Task<List<UserApprovalInfoDto>> GetAllClientsApprovalInfoAsync()
    {
        var clients = await _unitOfWork.Clients.GetAllAsync();

        var approvalInfoList = clients.Select(c => new UserApprovalInfoDto(
            c.Ssn,
            SharedHelper.GetMimeFromBase64(Convert.ToBase64String(c.NationalIdImage ?? SharedHelper.CardPlaceHolder)),
            SharedHelper.GetMimeFromBase64(Convert.ToBase64String(c.UnionCardImage ?? SharedHelper.CardPlaceHolder))
        )).ToList();

        return approvalInfoList;
    }

    public async Task<List<UserProfileInfoDto>> GetAllClientsAsync()
    {
        var clients = await _unitOfWork.Clients.GetAllAsync(
            include: source => source.Include(c => c.User));

        var clientList = clients.Select(c => new UserProfileInfoDto(
            c.Name,
            c.User.FirstName,
            c.User.LastName,
            c.User.PhoneNumber,
            c.Address,
            c.User.Email,
            c.IsGrantedRent
        )).ToList();

        return clientList;
    }


    public async Task<StatusDto> UpdateApprovalInfoRejectAsync(string userId, UpdateApprovalInfoRejectDto updateApprovalInfoDto)
    {
        var client = await _unitOfWork.Clients.FindAsync(c => c.Id == userId);
        if (client is null)
            return new StatusDto(StatusMessage: "User couldn't be found", StatusCode: System.Net.HttpStatusCode.NotFound);

        client.Ssn = updateApprovalInfoDto.NationalId;

        if (updateApprovalInfoDto.NationalImage is null)
        client.NationalIdImage = SharedHelper.RejectedImgPlaceholder;

        if (updateApprovalInfoDto.UnionImage is null)
            client.UnionCardImage = SharedHelper.RejectedImgPlaceholder;


        var update = _unitOfWork.Clients.Update(client);
        _unitOfWork.Save();

        return new StatusDto("User has been updated successully", System.Net.HttpStatusCode.OK);
    }

    #endregion

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

    public async Task<UserApprovalInfoDto?> GetClientApprovalInfoAsync(string userId)
    {
        return await _unitOfWork.Clients.FindAsync(
            predicate: c => c.Id == userId,
            selector: c => new UserApprovalInfoDto(
                c.Ssn,
                SharedHelper.GetMimeFromBase64(Convert.ToBase64String( c.NationalIdImage ?? SharedHelper.CardPlaceHolder)),
                SharedHelper.GetMimeFromBase64(Convert.ToBase64String( c.UnionCardImage ?? SharedHelper.CardPlaceHolder))
                ));
    }
    public async Task<UserProfileInfoDto?> GetClientInfoAsync(string userId)
    {
        return await _unitOfWork.Clients.FindAsync(
            predicate: c => c.Id == userId,
            include: source => source.Include(c => c.User),
            selector: c => new UserProfileInfoDto(
                c.Name,
                c.User.FirstName,
                c.User.LastName,
                c.User.PhoneNumber,
                c.Address,
                c.User.Email,
                c.IsGrantedRent
                ));
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

    public async Task<StatusDto> UpdateApprovalInfoAsync(string userId, UpdateApprovalInfoDto updateApprovalInfoDto)
    {
        var client = await _unitOfWork.Clients.FindAsync(c => c.Id == userId);
        if (client is null)
            return new StatusDto(StatusMessage: "User couldn't be found", StatusCode: System.Net.HttpStatusCode.NotFound);

        client.Ssn = updateApprovalInfoDto.NationalId;
        client.NationalIdImage = Convert.FromBase64String(updateApprovalInfoDto.NationalImage);
        client.UnionCardImage = Convert.FromBase64String(updateApprovalInfoDto.UnionImage);

        var update = _unitOfWork.Clients.Update(client);
        _unitOfWork.Save();

        return new StatusDto("User has been updated successully", System.Net.HttpStatusCode.OK);
    }

    public async Task<StatusDto> UpdateClientInfoAsync(string userId, UpdateProfileInfoDto updateProfileInfoDto)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
            return new StatusDto(StatusMessage: "User couldn't be found", StatusCode: System.Net.HttpStatusCode.NotFound);

        user.PhoneNumber = updateProfileInfoDto.PhoneNumber;
        user.FirstName = updateProfileInfoDto.FirstName;
        user.LastName = updateProfileInfoDto.LastName;
        user.Email = updateProfileInfoDto.Email;
        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            return new StatusDto(result.Errors.FirstOrDefault()?.Description ?? "User couldn't be updated", System.Net.HttpStatusCode.BadRequest);

        var client = await _unitOfWork.Clients.FindAsync(c => c.Id == userId);
        if (client is null)
            return new StatusDto(StatusMessage: "User couldn't be found", StatusCode: System.Net.HttpStatusCode.NotFound);

        client.Address = updateProfileInfoDto.Address;

        var update = _unitOfWork.Clients.Update(client);
        _unitOfWork.Save();

        return new StatusDto("User has been updated successully", System.Net.HttpStatusCode.OK);
    }

    public async Task<UserBasicInfoDto?> GetClientInfoByEmailAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if(user is null) return null;
        
        var client = await _unitOfWork.Clients.FindAsync(c => c.Id == user.Id);

        if (client is null) return null;

        return new(user.Id, user.Name, client.Ssn, user.LockoutEnd > DateTimeOffset.Now);
    }


}
