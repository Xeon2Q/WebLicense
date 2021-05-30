namespace WebLicense.Core.Enums
{
    public static class Roles
    {
        // global administrator role
        public const int AdminId = 1;
        public const string Admin = "Admin";
        public const string AdminNormalized = "ADMIN";

        // local administrator role
        public const int CustomerAdminId = 100;
        public const string CustomerAdmin = "Customer Admin";
        public const string CustomerAdminNormalized = "CUSTOMER ADMIN";
        
        // specific customer manager role
        public const int CustomerManagerId = 200;
        public const string CustomerManager = "Customer Manager";
        public const string CustomerManagerNormalized = "CUSTOMER MANAGER";
        
        // customer user role
        public const int CustomerUserId = 300;
        public const string CustomerUser = "Customer User";
        public const string CustomerUserNormalized = "CUSTOMER USER";
    }
}