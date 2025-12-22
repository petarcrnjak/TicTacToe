namespace TicTacToe.Authorization.Validators;

internal static class AuthValidator
{
    public static void ValidateCredentials(string? username, string? password, string paramName)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Username and password are required.", paramName);
    }
}