using System.Security.Cryptography;

namespace CommonLibrary.Utilities.StringHelper
{
    public static class PasswordGenerator
    {
        /// <summary>
        /// Generates a cryptographically secure random password with the specified length.
        /// </summary>
        /// <param name="length">The length of the password to generate.</param>
        /// <returns>A secure random password.</returns>
        public static string GenerateRandomPassword(int length)
        {
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()-_=+";

            using var rng = RandomNumberGenerator.Create();
            var bytes = new byte[length];
            rng.GetBytes(bytes);

            char[] chars = new char[length];
            for (int i = 0; i < length; i++)
            {
                chars[i] = validChars[bytes[i] % validChars.Length];
            }

            return new string(chars);
        }
    }
}
