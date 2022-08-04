namespace BlazorWasmUI.Shared;

public partial class MainLayout
{
    [Inject] AuthenticationStateProvider AuthStateProvider { get; set; }
    [Inject] IAuthenticationService AuthService { get; set; }
    [Inject] IWebAssemblyHostEnvironment HostEnv { get; set; }
    [Inject] NavigationManager NavMan { get; set; }
    [Inject] ISnackbar Snackbar { get; set; }
    [Inject] ILocalStorageService LocalStorage { get; set; }

    private MudTheme _currentTheme = new();
    private bool _drawerOpen = true;
    private LoginUserModel _loginUser = new();
    private InputType _passwordInput = InputType.Password;
    private string _passwordInputIcon = Icons.Material.Filled.VisibilityOff;
    private bool _showPassword = false;

    protected override async Task OnInitializedAsync()
    {
        string theme = await LocalStorage.GetItemAsync<string>("Theme");

        if (string.IsNullOrWhiteSpace(theme)
            || string.Equals(theme, "Dark", System.StringComparison.CurrentCultureIgnoreCase))
        {
            _currentTheme = darkTheme;
        }
        else
        {
            _currentTheme = lightTheme;
        }
    }

    private async Task HandleLoginAsync()
    {
        if (string.IsNullOrEmpty(_loginUser.Username))
        {
            Snackbar.Add("Please enter your username", Severity.Info);
            return;
        }
        else if (string.IsNullOrEmpty(_loginUser.Password))
        {
            Snackbar.Add("Please enter your password", Severity.Info);
            return;
        }

        ServiceResponseModel<AuthUserModel> authResult = await AuthService.LoginAsync(_loginUser);
        _loginUser = new();

        if (authResult.Success)
        {
            NavMan.NavigateTo("/");
        }
        else
        {
            Snackbar.Add($"Login failed: {authResult.Message}", Severity.Error);
        }
    }

    private async Task HandleLogoutAsync()
    {
        NavMan.NavigateTo("/");
        await AuthService.LogoutAsync();
    }

    private void ToggleShowPassword()
    {
        if (_showPassword)
        {
            _showPassword = false;
            _passwordInputIcon = Icons.Material.Filled.VisibilityOff;
            _passwordInput = InputType.Password;
        }
        else
        {
            _showPassword = true;
            _passwordInputIcon = Icons.Material.Filled.Visibility;
            _passwordInput = InputType.Text;
        }
    }

    private async Task ToggleThemeAsync()
    {
        if (_currentTheme == lightTheme)
        {
            _currentTheme = darkTheme;
            await LocalStorage.SetItemAsync("Theme", "Dark");
        }
        else
        {
            _currentTheme = lightTheme;
            await LocalStorage.SetItemAsync("Theme", "Light");
        }
    }

    private MudTheme lightTheme = new()
    {
        Typography = new()
        {
            Default = new()
            {
                FontFamily = new[] { "system-ui", "-apple-system", "Segoe UI", "Roboto", "Helvetica Neue", "Noto Sans", "Liberation Sans", "Arial", "sans-serif", "Apple Color Emoji", "Segoe UI Emoji", "Segoe UI Symbol", "Noto Color Emoji" }
            }
        },
        Palette = new()
        {
            Black = "#000000FF",
            White = "#FFFFFFFF",

            Primary = "#2c3e50",
            PrimaryDarken = "#253444",
            PrimaryLighten = "#3F5973",
            PrimaryContrastText = "#ffffffff",

            Secondary = "#4E545A",
            SecondaryDarken = "#383C40",
            SecondaryLighten = "#5A6067",
            SecondaryContrastText = "#FFFFFFFF",

            Tertiary = "#3D5E77",
            TertiaryDarken = "#2B4254",
            TertiaryLighten = "#456985",
            TertiaryContrastText = "#FFFFFFFF",

            Info = "#3498db",
            InfoDarken = "#0c80df",
            InfoLighten = "#47a7f5",
            InfoContrastText = "#ffffffff",

            Success = "#00c853ff",
            SuccessDarken = "#00a344",
            SuccessLighten = "#00eb62",
            SuccessContrastText = "#ffffffff",

            Warning = "#f39c12",
            WarningDarken = "#d68100",
            WarningLighten = "#ffa724",
            WarningContrastText = "#ffffffff",

            Error = "#e74c3c",
            ErrorDarken = "#f21c0d",
            ErrorLighten = "#f66055",
            ErrorContrastText = "#ffffffff",

            Dark = "#27272f",
            DarkDarken = "#222229",
            DarkLighten = "#434350",
            DarkContrastText = "#FFFFFFFF",

            TextPrimary = "#424242ff",
            TextSecondary = "#00000089",
            TextDisabled = "#00000060",

            ActionDefault = "#00000089",
            ActionDisabled = "#00000042",
            ActionDisabledBackground = "#0000001E",

            Background = "#C8C8C8ff",
            BackgroundGrey = "#F5F5F5FF",

            Surface = "#FFFFFFFF",

            DrawerBackground = "#FFFFFFFF",
            DrawerText = "#424242FF",
            DrawerIcon = "#F1F1F1FF",

            AppbarBackground = "#594ae2ff",
            AppbarText = "#FFFFFFFF",

            LinesDefault = "#0000001E",
            LinesInputs = "#BDBDBDFF",

            TableLines = "#E0E0E0FF",
            TableStriped = "#00000005",
            TableHover = "#0000000A",

            Divider = "#E0E0E0FF",
            DividerLight = "#000000CC",

            HoverOpacity = 0.06,

            GrayDefault = "#95A5A6",
            GrayLight = "#B4BCC2",
            GrayLighter = "#ECF0F1",
            GrayDark = "#7B8A8B",
            GrayDarker = "#343A40",

            OverlayDark = "rgba(33,33,33,0.4980392156862745)",
            OverlayLight = "rgba(255,255,255,0.4980392156862745)"
        }
    };

    private MudTheme darkTheme = new()
    {
        Typography = new()
        {
            Default = new()
            {
                FontFamily = new[] { "system-ui", "-apple-system", "Segoe UI", "Roboto", "Helvetica Neue", "Noto Sans", "Liberation Sans", "Arial", "sans-serif", "Apple Color Emoji", "Segoe UI Emoji", "Segoe UI Symbol", "Noto Color Emoji" }
            }
        },
        Palette = new()
        {
            Black = "#27272F",
            White = "#FFFFFFFF",

            Primary = "#2c3e50",
            PrimaryDarken = "#253444",
            PrimaryLighten = "#3F5973",
            PrimaryContrastText = "#FFFFFFFF",

            Secondary = "#4E545A",
            SecondaryDarken = "#383C40",
            SecondaryLighten = "#5A6067",
            SecondaryContrastText = "#FFFFFFFF",

            Tertiary = "#3D5E77",
            TertiaryDarken = "#2B4254",
            TertiaryLighten = "#456985",
            TertiaryContrastText = "#FFFFFFFF",

            Info = "#2196f3ff",
            InfoDarken = "#0c80df",
            InfoLighten = "#47a7f5",
            InfoContrastText = "#FFFFFFFF",

            Success = "#00c853ff",
            SuccessDarken = "#00a344",
            SuccessLighten = "#00eb62",
            SuccessContrastText = "#FFFFFFFF",

            Warning = "#ff9800ff",
            WarningDarken = "#d68100",
            WarningLighten = "#ffa724",
            WarningContrastText = "#FFFFFFFF",

            Error = "#f44336ff",
            ErrorDarken = "#f21c0d",
            ErrorLighten = "#f66055",
            ErrorContrastText = "#FFFFFFFF",

            Dark = "#27272f",
            DarkDarken = "#222229",
            DarkLighten = "#434350",
            DarkContrastText = "#FFFFFFFF",

            TextPrimary = "rgba(255,255,255, 0.70)",
            TextSecondary = "rgba(255,255,255, 0.50)",
            TextDisabled = "rgba(255,255,255, 0.2)",

            ActionDefault = "#ADADB1",
            ActionDisabled = "rgba(255,255,255, 0.26)",
            ActionDisabledBackground = "rgba(255,255,255, 0.12)",

            Background = "#1a1a27ff",
            BackgroundGrey = "#151521FF",

            Surface = "#1E1E2DFF",

            DrawerBackground = "#1A1A27FF",
            DrawerText = "#92929FFF",
            DrawerIcon = "#92929FFF",

            AppbarBackground = "#1a1a27cc",
            AppbarText = "#92929FFF",

            LinesDefault = "#33323EFF",
            LinesInputs = "#BDBDBDFF",

            TableLines = "#33323EFF",
            TableStriped = "#00000005",
            TableHover = "#0000000A",

            Divider = "#292838FF",
            DividerLight = "#000000CC",

            HoverOpacity = 0.06,

            GrayDefault = "#9E9E9E",
            GrayLight = "#2A2833",
            GrayLighter = "#1E1E2D",
            GrayDark = "#757575",
            GrayDarker = "#616161",

            OverlayDark = "rgba(33,33,33,0.4980392156862745)",
            OverlayLight = "#1e1e2d80"
        }
    };
}
