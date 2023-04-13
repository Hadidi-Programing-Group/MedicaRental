﻿using MedicaRental.BLL.Dtos;
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

    public ClientsManager(IUnitOfWork unitOfWork,
        IAccountsManager accountsManager
        )
    {
        _unitOfWork = unitOfWork;
        _accountsManager = accountsManager;
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

            _unitOfWork.Clients.Add(newClient);
            _unitOfWork.Save();
        }
        catch (Exception e)
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
