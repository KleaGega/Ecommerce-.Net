namespace MVCProject.Handlers
{
    using Microsoft.AspNetCore.Identity;
        public static class PasswordHashHandler
        {
            private static readonly PasswordHasher<string> _passwordHasher = new PasswordHasher<string>();

            /// <summary>
            /// Hash the password for storing in the database
            /// </summary>
            public static string HashPassword(string password)
            {
                return _passwordHasher.HashPassword(null, password);
            }

            /// <summary>
            /// Verify the entered password with the hashed password
            /// </summary>
            public static bool VerifyPassword(string enteredPassword, string hashedPassword)
            {
                var result = _passwordHasher.VerifyHashedPassword(null, hashedPassword, enteredPassword);
                return result == PasswordVerificationResult.Success || result == PasswordVerificationResult.SuccessRehashNeeded;
            }
        }
    }


