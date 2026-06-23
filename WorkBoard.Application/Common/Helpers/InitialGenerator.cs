namespace WorkBoard.Application.Common.Helpers;

public static class InitialGenerator
{
    public static string Generate(string? fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
        {
            return "??";
        }

        var parts = fullName.Split(
            ' ', 
            StringSplitOptions.RemoveEmptyEntries);

        return parts.Length > 1
            ? $"{parts[0][0]}{parts[1][0]}".ToUpper()
            : parts[0][0].ToString().ToUpper();
    }
}
