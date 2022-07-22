﻿namespace WebApi.Services;

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

    public async Task<ServiceResponseModel<AuthUserModel>> RegisterAsync(string requestor, RegisterUserModel registerUser)
    {
        _logger.LogInformation($"Register new user {registerUser.Email}... [{requestor}]");
        ServiceResponseModel<AuthUserModel> serviceResponse = new();

        try
        {
            AppUser appUser = await _accountRepository.CreateAsync(registerUser);

            serviceResponse.Success = true;
            serviceResponse.Data = new AuthUserModel
            {
                Username = appUser.UserName,
                Token = await _tokenService.CreateTokenAsync(appUser)
            };
            serviceResponse.Message = $"Successfully registered user [{appUser.UserName}]";
            _logger.LogInformation(serviceResponse.Message);
        }
        catch (Exception e)
        {
            serviceResponse.Success = false;
            serviceResponse.Message = e.Message;
            _logger.LogError(e.Message);
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
            _logger.LogInformation(serviceResponse.Message);
        }
        catch (Exception e)
        {
            serviceResponse.Success = false;
            serviceResponse.Message = e.Message;
            _logger.LogError(e.Message);
        }

        return serviceResponse;
    }

    public async Task<ServiceResponseModel<AccountModel>> GetAccount(string requestor, string username)
    {
        _logger.LogInformation($"Get user account {username}... [{requestor}]");
        ServiceResponseModel<AccountModel> serviceResponse = new();

        try
        {
            AppUser appUser = await _accountRepository.GetAsync(username);

            if (appUser != null)
            {
                AccountModel appAcount = new()
                {
                    Id = appUser.Id,
                    Username = appUser.UserName,
                    Email = appUser.Email,
                    LastActive = appUser.LastActive,
                    Created = appUser.Created
                };

                serviceResponse.Success = true;
                serviceResponse.Data = appAcount;
                serviceResponse.Message = $"Successfully retrieved user [{appAcount.Username}]";
                _logger.LogInformation(serviceResponse.Message);
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
            _logger.LogError(e.Message);
        }

        return serviceResponse;
    }

    public async Task<ServiceResponseModel<bool>> DeleteAccount(string requestor, string username)
    {
        _logger.LogInformation($"Delete user account {username}... [{requestor}]");
        ServiceResponseModel<bool> serviceResponse = new();

        try
        {
            IdentityResult result = await _accountRepository.DeleteAsync(requestor, username);

            if (result.Succeeded)
            {
                serviceResponse.Success = result.Succeeded;
                serviceResponse.Data = result.Succeeded;
                serviceResponse.Message = $"Successfully deleted user [{username}] -- {result}";
                _logger.LogInformation(serviceResponse.Message);
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
            _logger.LogError(e.Message);
        }

        return serviceResponse;
    }
}
