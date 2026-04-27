using MudBlazor;

namespace MudBlazorServerUI.Features.Theme;

public static class AppTheme
{
    private static readonly string[] s_sansSerifStack =
    [
        "Inter",
        "system-ui",
        "-apple-system",
        "Segoe UI",
        "Roboto",
        "Helvetica Neue",
        "Arial",
        "sans-serif"
    ];

    private static readonly string[] s_monoStack =
    [
        "JetBrains Mono",
        "ui-monospace",
        "SFMono-Regular",
        "Menlo",
        "Consolas",
        "monospace"
    ];

    public static readonly MudTheme Default = new()
    {
        PaletteLight = BuildLightPalette(),
        PaletteDark = BuildDarkPalette(),
        Typography = BuildTypography(),
        LayoutProperties = new LayoutProperties
        {
            DefaultBorderRadius = "10px",
            DrawerWidthLeft = "260px",
            AppbarHeight = "64px"
        }
    };

    private static PaletteLight BuildLightPalette() => new()
    {
        Black = "#0F172A",
        White = "#FFFFFF",

        Primary = "#4F46E5",
        PrimaryContrastText = "#FFFFFF",
        PrimaryDarken = "#4338CA",
        PrimaryLighten = "#818CF8",

        Secondary = "#0EA5E9",
        SecondaryContrastText = "#FFFFFF",
        SecondaryDarken = "#0284C7",
        SecondaryLighten = "#38BDF8",

        Tertiary = "#10B981",
        TertiaryContrastText = "#FFFFFF",
        TertiaryDarken = "#059669",
        TertiaryLighten = "#34D399",

        Info = "#3B82F6",
        InfoContrastText = "#FFFFFF",
        InfoDarken = "#2563EB",
        InfoLighten = "#60A5FA",

        Success = "#10B981",
        SuccessContrastText = "#FFFFFF",
        SuccessDarken = "#059669",
        SuccessLighten = "#34D399",

        Warning = "#F59E0B",
        WarningContrastText = "#1F2937",
        WarningDarken = "#D97706",
        WarningLighten = "#FBBF24",

        Error = "#EF4444",
        ErrorContrastText = "#FFFFFF",
        ErrorDarken = "#DC2626",
        ErrorLighten = "#F87171",

        Dark = "#0F172A",
        DarkContrastText = "#F8FAFC",
        DarkDarken = "#020617",
        DarkLighten = "#1E293B",

        Background = "#EEF2F7",
        BackgroundGray = "#E2E8F0",
        Surface = "#FFFFFF",

        AppbarBackground = "#FFFFFF",
        AppbarText = "#0F172A",

        DrawerBackground = "#FFFFFF",
        DrawerText = "#1F2937",
        DrawerIcon = "#64748B",

        TextPrimary = "#0F172A",
        TextSecondary = "#475569",
        TextDisabled = "#94A3B8",

        ActionDefault = "#475569",
        ActionDisabled = "#CBD5E1",
        ActionDisabledBackground = "#E2E8F0",

        LinesDefault = "#CBD5E1",
        LinesInputs = "#94A3B8",
        TableLines = "#E2E8F0",
        TableStriped = "#F8FAFC",
        TableHover = "#F1F5F9",

        Divider = "#94A3B8",
        DividerLight = "#CBD5E1",

        GrayDefault = "#64748B",
        GrayLight = "#94A3B8",
        GrayLighter = "#E2E8F0",
        GrayDark = "#475569",
        GrayDarker = "#1E293B",

        OverlayDark = "rgba(15, 23, 42, 0.6)",
        OverlayLight = "rgba(255, 255, 255, 0.6)",

        HoverOpacity = 0.06,
        RippleOpacity = 0.10,
        RippleOpacitySecondary = 0.20
    };

    private static PaletteDark BuildDarkPalette() => new()
    {
        Black = "#1C2128",
        White = "#CDD9E5",

        Primary = "#539BF5",
        PrimaryContrastText = "#CDD9E5",
        PrimaryDarken = "#316DCA",
        PrimaryLighten = "#6CB6FF",

        Secondary = "#6CB6FF",
        SecondaryContrastText = "#1C2128",
        SecondaryDarken = "#539BF5",
        SecondaryLighten = "#96D0FF",

        Tertiary = "#57AB5A",
        TertiaryContrastText = "#CDD9E5",
        TertiaryDarken = "#347D39",
        TertiaryLighten = "#7EBA80",

        Info = "#539BF5",
        InfoContrastText = "#CDD9E5",
        InfoDarken = "#316DCA",
        InfoLighten = "#6CB6FF",

        Success = "#57AB5A",
        SuccessContrastText = "#CDD9E5",
        SuccessDarken = "#347D39",
        SuccessLighten = "#7EBA80",

        Warning = "#C69026",
        WarningContrastText = "#1C2128",
        WarningDarken = "#966600",
        WarningLighten = "#DAAA3F",

        Error = "#E5534B",
        ErrorContrastText = "#CDD9E5",
        ErrorDarken = "#C93C37",
        ErrorLighten = "#F47067",

        Dark = "#1C2128",
        DarkContrastText = "#ADBAC7",
        DarkDarken = "#13171D",
        DarkLighten = "#2D333B",

        Background = "#22272E",
        BackgroundGray = "#2D333B",
        Surface = "#2D333B",

        AppbarBackground = "#1C2128",
        AppbarText = "#ADBAC7",

        DrawerBackground = "#1C2128",
        DrawerText = "#ADBAC7",
        DrawerIcon = "#768390",

        TextPrimary = "#ADBAC7",
        TextSecondary = "#768390",
        TextDisabled = "#636E7B",

        ActionDefault = "#ADBAC7",
        ActionDisabled = "#636E7B",
        ActionDisabledBackground = "#373E47",

        LinesDefault = "#444C56",
        LinesInputs = "#444C56",
        TableLines = "#373E47",
        TableStriped = "#2D333B",
        TableHover = "#373E47",

        Divider = "#373E47",
        DividerLight = "#2D333B",

        GrayDefault = "#768390",
        GrayLight = "#636E7B",
        GrayLighter = "#444C56",
        GrayDark = "#ADBAC7",
        GrayDarker = "#CDD9E5",

        OverlayDark = "rgba(13, 17, 23, 0.7)",
        OverlayLight = "rgba(34, 39, 46, 0.6)",

        HoverOpacity = 0.08,
        RippleOpacity = 0.12,
        RippleOpacitySecondary = 0.22
    };

    private static Typography BuildTypography() => new()
    {
        Default = new DefaultTypography
        {
            FontFamily = s_sansSerifStack,
            FontSize = "0.9rem",
            FontWeight = "400",
            LineHeight = "1.55",
            LetterSpacing = "normal"
        },
        H1 = new H1Typography
        {
            FontFamily = s_sansSerifStack,
            FontSize = "3rem",
            FontWeight = "700",
            LineHeight = "1.1",
            LetterSpacing = "-0.025em"
        },
        H2 = new H2Typography
        {
            FontFamily = s_sansSerifStack,
            FontSize = "2.25rem",
            FontWeight = "700",
            LineHeight = "1.15",
            LetterSpacing = "-0.02em"
        },
        H3 = new H3Typography
        {
            FontFamily = s_sansSerifStack,
            FontSize = "1.75rem",
            FontWeight = "700",
            LineHeight = "1.2",
            LetterSpacing = "-0.015em"
        },
        H4 = new H4Typography
        {
            FontFamily = s_sansSerifStack,
            FontSize = "1.375rem",
            FontWeight = "600",
            LineHeight = "1.3",
            LetterSpacing = "-0.01em"
        },
        H5 = new H5Typography
        {
            FontFamily = s_sansSerifStack,
            FontSize = "1.125rem",
            FontWeight = "600",
            LineHeight = "1.4",
            LetterSpacing = "-0.005em"
        },
        H6 = new H6Typography
        {
            FontFamily = s_sansSerifStack,
            FontSize = "1rem",
            FontWeight = "600",
            LineHeight = "1.45",
            LetterSpacing = "normal"
        },
        Subtitle1 = new Subtitle1Typography
        {
            FontFamily = s_sansSerifStack,
            FontSize = "0.95rem",
            FontWeight = "500",
            LineHeight = "1.5",
            LetterSpacing = "normal"
        },
        Subtitle2 = new Subtitle2Typography
        {
            FontFamily = s_sansSerifStack,
            FontSize = "0.85rem",
            FontWeight = "500",
            LineHeight = "1.5",
            LetterSpacing = "normal"
        },
        Body1 = new Body1Typography
        {
            FontFamily = s_sansSerifStack,
            FontSize = "0.95rem",
            FontWeight = "400",
            LineHeight = "1.6",
            LetterSpacing = "normal"
        },
        Body2 = new Body2Typography
        {
            FontFamily = s_sansSerifStack,
            FontSize = "0.85rem",
            FontWeight = "400",
            LineHeight = "1.55",
            LetterSpacing = "normal"
        },
        Button = new ButtonTypography
        {
            FontFamily = s_sansSerifStack,
            FontSize = "0.85rem",
            FontWeight = "600",
            LineHeight = "1.75",
            LetterSpacing = "0.01em",
            TextTransform = "none"
        },
        Caption = new CaptionTypography
        {
            FontFamily = s_sansSerifStack,
            FontSize = "0.75rem",
            FontWeight = "400",
            LineHeight = "1.4",
            LetterSpacing = "0.02em"
        },
        Overline = new OverlineTypography
        {
            FontFamily = s_sansSerifStack,
            FontSize = "0.7rem",
            FontWeight = "600",
            LineHeight = "1.5",
            LetterSpacing = "0.12em",
            TextTransform = "uppercase"
        }
    };
}
