﻿using WebApi.SharedLibrary.Identity.Models;
using WebApi.SharedLibrary.Models;

namespace WebApi.Interfaces;

public interface IAccountService
{
    Task<ServiceResponseModel<AccountModel>> GetAccountAsync(string requestor, string username);
    Task<ServiceResponseModel<List<AccountModel>>> GetAccountsAsync(string requestor);
    Task<ServiceResponseModel<AuthUserModel>> RegisterAsync(string requestor, RegisterUserModel registerUser);
    Task<ServiceResponseModel<AuthUserModel>> LoginAsync(LoginUserModel loginUser);
    Task<ServiceResponseModel<bool>> UpdateAccountAsync(string requestor, AccountUpdateModel updateAccount);
    Task<ServiceResponseModel<bool>> DeleteAccountAsync(string requestor, string username);
}
