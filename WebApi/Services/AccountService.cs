using Microsoft.AspNetCore.Identity;
using WebApi.IdentityLibrary.Entities;
using WebApi.IdentityLibrary.Identity;
using WebApi.SharedLibrary.Identity.Models;
using WebApi.SharedLibrary.Models;

namespace WebApi.Services;

public class AccountService : IAccountService
{
    private readonly ILogger<AccountService> _logger;
    private readonly IAccountRepository _accountRepository;
    private readonly ITokenService _tokenService;

    public AccountService(ILogger<AccountService> logger,
                          IAccountRepository accountRepository,
                          ITokenService tokenService)
    {
        _logger = logger;
        _accountRepository = accountRepository;
        _tokenService = tokenService;
    }

    public async Task<ServiceResponseModel<AuthUserModel>> RegisterAsync(RegisterUserModel registerUser)
    {
        _logger.LogInformation("Register new user {Email}...", registerUser.Email);
        ServiceResponseModel<AuthUserModel> serviceResponse = new();

        try
        {
            AppUser appUser = await _accountRepository.CreateAccountAsync(registerUser);

            serviceResponse.Success = true;
            serviceResponse.Data = new AuthUserModel
            {
                Username = appUser.UserName,
                Token = await _tokenService.CreateTokenAsync(appUser)
            };
            serviceResponse.Message = $"Successfully registered user [{appUser.UserName}]";
            _logger.LogInformation("Successfully registered user [{username}]", appUser.UserName);
        }
        catch (Exception e)
        {
            serviceResponse.Success = false;
            serviceResponse.Message = e.Message;
            _logger.LogError("{errorMessage}", e.Message);
        }

        return serviceResponse;
    }

    public async Task<ServiceResponseModel<AuthUserModel>> LoginAsync(LoginUserModel loginUser)
    {
        ServiceResponseModel<AuthUserModel> serviceResponse = new();

        try
        {
            AppUser appUser = await _accountRepository.LoginAsync(loginUser);

            serviceResponse.Success = true;
            serviceResponse.Data = new AuthUserModel
            {
                Username = appUser.Email,
                Token = await _tokenService.CreateTokenAsync(appUser)
            };
            serviceResponse.Message = $"Successfully authenticated user [{appUser.UserName}]";
            _logger.LogInformation("Successfully authenticated user [{username}]", appUser.UserName);
        }
        catch (Exception e)
        {
            serviceResponse.Success = false;
            serviceResponse.Message = e.Message;
            _logger.LogError("{errorMessage}", e.Message);
        }

        return serviceResponse;
    }

    public async Task<ServiceResponseModel<AccountModel>> GetAccountAsync(string requestor, string username)
    {
        _logger.LogInformation("Get user account {username}... [{requestor}]", username, requestor);
        ServiceResponseModel<AccountModel> serviceResponse = new();

        try
        {
            AppUser? appUser = await _accountRepository.GetAccountAsync(username);

            if (appUser is not null)
            {
                AccountModel appAcount = new()
                {
                    Id = appUser.Id,
                    Username = appUser.UserName,
                    Email = appUser.Email,
                    Created = appUser.Created,
                    LastActive = appUser.LastActive
                };

                serviceResponse.Success = true;
                serviceResponse.Data = appAcount;
                serviceResponse.Message = $"Successfully retrieved user [{appAcount.Username}]";
                _logger.LogInformation("Successfully retrieved user [{username}]", appAcount.Username);
            }
            else
            {
                throw new Exception("Username not found");
            }
        }
        catch (Exception e)
        {
            serviceResponse.Success = false;
            serviceResponse.Message = e.Message;
            _logger.LogError("{errorMessage}", e.Message);
        }

        return serviceResponse;
    }

    public async Task<ServiceResponseModel<List<AccountModel>>> GetAccountsAsync(string requestor)
    {
        _logger.LogInformation("Get user accounts... [{requestor}]", requestor);
        ServiceResponseModel<List<AccountModel>> serviceResponse = new();

        try
        {
            List<AppUser> appUsers = await _accountRepository.GetAccountsAsync();

            if (appUsers != null)
            {
                List<AccountModel> appAcount = [];

                foreach (AppUser appUser in appUsers)
                {
                    appAcount.Add(new AccountModel()
                    {
                        Id = appUser.Id,
                        Username = appUser.UserName,
                        Email = appUser.Email,
                        Created = appUser.Created,
                        LastActive = appUser.LastActive
                    });
                }

                serviceResponse.Success = true;
                serviceResponse.Data = appAcount;
                serviceResponse.Message = $"Successfully retrieved users [Count={appAcount.Count}]";
                _logger.LogInformation("Successfully retrieved users [Count={count}]", appAcount.Count);
            }
            else
            {
                throw new Exception("No users found");
            }
        }
        catch (Exception e)
        {
            serviceResponse.Success = false;
            serviceResponse.Message = e.Message;
            _logger.LogError("{errorMessage}", e.Message);
        }

        return serviceResponse;
    }

    public async Task<ServiceResponseModel<bool>> UpdateAccountAsync(string requestor, AccountUpdateModel updateAccount)
    {
        _logger.LogInformation(
            "Update user account {id}/{username}... [{requestor}]",
            updateAccount.Id, updateAccount.UserName, requestor);

        ServiceResponseModel<bool> serviceResponse = new();

        try
        {
            await _accountRepository.UpdateAccountAsync(updateAccount);

            serviceResponse.Success = true;
            serviceResponse.Data = true;
            serviceResponse.Message = $"Successfully updated user [{updateAccount.UserName}]";
            _logger.LogInformation("Successfully updated user [{username}]", updateAccount.UserName);
        }
        catch (Exception e)
        {
            serviceResponse.Success = false;
            serviceResponse.Message = e.Message;
            _logger.LogError("{errorMessage}", e.Message);
        }

        return serviceResponse;
    }

    public async Task<ServiceResponseModel<bool>> DeleteAccountAsync(string requestor, string username)
    {
        _logger.LogInformation("Delete user account {username}... [{requestor}]", username, requestor);
        ServiceResponseModel<bool> serviceResponse = new();

        try
        {
            IdentityResult result = await _accountRepository.DeleteAccountAsync(requestor, username);

            if (result.Succeeded)
            {
                serviceResponse.Success = result.Succeeded;
                serviceResponse.Data = result.Succeeded;
                serviceResponse.Message = $"Successfully deleted user [{username}] -- {result}";
                _logger.LogInformation("Successfully deleted user [{username}] -- {result}", username, result);
            }
            else
            {
                throw new Exception($"Failed to delete user [{username}] -- {result}");
            }
        }
        catch (Exception e)
        {
            serviceResponse.Success = false;
            serviceResponse.Message = e.Message;
            _logger.LogError("{errorMessage}", e.Message);
        }

        return serviceResponse;
    }
}
