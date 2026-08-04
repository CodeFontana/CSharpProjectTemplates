namespace BlazorUI.Models;

/// <summary>
/// One suggestion for <c>BSInputAutocomplete</c>. <paramref name="Value"/> is what lands in the input
/// when the option is picked; <paramref name="Label"/> is what the menu shows, so it can carry context
/// the raw value lacks, such as "New York / nyc".
/// </summary>
public sealed record AutocompleteOption(string Value, string Label);
