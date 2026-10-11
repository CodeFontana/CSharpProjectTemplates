using Microsoft.FluentUI.AspNetCore.Components;

namespace FluentUI.Features.Components;

public sealed record ThemeColor(string Name, string? Hex)
{
    public string Swatch => "#" + (Hex ?? "0F6CBD").TrimStart('#');

    public static IReadOnlyList<ThemeColor> All { get; } =
    [
        new("Default", null),
        new("Access", "#A4373A"),
        new("Booking", "#00A99D"),
        new("Exchange", "#0078D4"),
        new("Excel", "#217346"),
        new("GroupMe", "#00BCF2"),
        new("Office", "#D83B01"),
        new("OneDrive", "#0078D4"),
        new("OneNote", "#7719AA"),
        new("Outlook", "#0F6CBD"),
        new("Planner", "#31752F"),
        new("PowerApps", "#742774"),
        new("PowerBI", "#F2C811"),
        new("PowerPoint", "#B7472A"),
        new("Project", "#31752F"),
        new("Publisher", "#077568"),
        new("SharePoint", "#0078D4"),
        new("Skype", "#0078D4"),
        new("Stream", "#BC1948"),
        new("Sway", "#008272"),
        new("Teams", "#6264A7"),
        new("Visio", "#3955A3"),
        new("Windows", "#0078D4"),
        new("Word", "#2B579A"),
        new("Yammer", "#106EBE")
    ];

    public async Task ApplyAsync(IThemeService theme)
    {
        ThemeMode mode = await theme.IsDarkModeAsync() ? ThemeMode.Dark : ThemeMode.Light;

        if (Hex is null)
        {
            await theme.SetThemeAsync(ThemeColorVariant.Default);
            await theme.SetThemeAsync(mode);
            return;
        }

        await theme.SetThemeAsync(new ThemeSettings(Hex, Mode: mode));
    }
}
