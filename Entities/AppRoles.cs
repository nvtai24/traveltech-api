namespace TravelTechApi.Common.Constants
{
    /// <summary>
    /// Application role constants
    /// </summary>
    public static class AppRoles
    {
        /// <summary>
        /// Administrator role - full system access
        /// </summary>
        public const string Admin = "Admin";

        /// <summary>
        /// Regular user role - basic access
        /// </summary>
        public const string User = "User";

        public const string Moderator = "Moderator";


        /// <summary>
        /// Get all available roles
        /// </summary>
        public static string[] GetAllRoles()
        {
            return new[] { Admin, User, Moderator };
        }
    }
}
