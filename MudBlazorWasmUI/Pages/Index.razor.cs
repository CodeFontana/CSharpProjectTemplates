﻿using Microsoft.AspNetCore.Components;

namespace MudBlazorWasmUI.Pages;

public partial class Index
{
    [Inject] NavigationManager NavManager { get; set; }

    private void OnLogin()
    {
        NavManager.NavigateTo("/login");
    }

    private void OnRegister()
    {
        NavManager.NavigateTo("/register");
    }
}
