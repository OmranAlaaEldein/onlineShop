using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shopping.Models.Const
{
    public static class ValidConst
    {
        public static int MaxLenghtNames = 20;
        public static string ErrorUserName = "User Name is required";
        public static string ErrorPassword = "Password is required";
        public static string ErrorUserExist = "User already exists!";
        public static string ErrorUserCreationFailed= "User creation failed! Please check user details and try again.";
        public static string UserCreation = "User created successfully!";
        public static string ErrorUserNotExist = "User Not exists";

        public static string ErrorIdNotVAlid = "Invalid id";
        public static string ErrorBradNotExist = "brad is not exist";
        public static string ErrorDeleteCategory = "delete all category first";
        public static string ErrorNotFoundCategory = "Not Found category";
        
        public static string ErrorNotFoundproduct = "product Not found";
        public static string ErrorDeleteProducts = "delete all Products first";

        public static string ErrorDeleteItems="delete all Items first";
        public static string ErrorNotFoundItem="Item Not found";


        public static string ErrorNotFoundOrder = "Order Not found";
    }

    public static class PathConst
    {
        public const string Brade = "Uploads/Brades";
        public const string Item = "Uploads/items";
    }

    public static class AuthConst
    {
        public const string Invalid = "Invalid authentication request";
        public const string InvalidPayload = "Invalid payload";
        
        public const string ErrorTokens = "Invalid tokens";
        public const string ErrorConfirmPassword = "Invalid confirm password";
        public const string ErrorInvalidpassword = "Invalid password";
        public const string ErrorTokenExpired = "We cannot refresh this since the token has not expired";
        public const string ErrorRefreshToken = "refresh token doesnt exist";
        public const string ErrorRelogin = "token has expired, user needs to relogin";
        public const string ErrorTokenUsed = "token has been used";
        public const string ErrorTokenMatch = "the token doenst mateched the saved token";
        public const string ErrorTokenRevoked = "Token is Revoked";
        
    }
    public static class UserRoles
    {
        public const string Admin = "Admin";
        public const string User = "User";
    }
}
