namespace WebLicense.Core.Enums
{
    public static class Roles
    {
        // global administrator role
        public const long AdminId = 9223372036854775807;
        public const string Admin = "Admin";
        public const string AdminNormalized = "ADMIN";

        // local administrator role
        public const long CustomerAdminId = 9223372036854775806;
        public const string CustomerAdmin = "Customer Admin";
        public const string CustomerAdminNormalized = "CUSTOMER ADMIN";
        
        // specific customer manager role
        public const long CustomerManagerId = 9223372036854775805;
        public const string CustomerManager = "Customer Manager";
        public const string CustomerManagerNormalized = "CUSTOMER MANAGER";
        
        // customer user role
        public const long CustomerUserId = 9223372036854775804;
        public const string CustomerUser = "Customer User";
        public const string CustomerUserNormalized = "CUSTOMER USER";
    }
}