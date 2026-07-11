namespace CommonLibrary.Enums
{
#pragma warning disable
    public enum Status
    {
        // Success Codes
        Success = 200,
        ProcessedSuccess = 0,
        EmailSended = 11,

        // Client Error Codes (4xx)
        BadRequest = 400,
        NotFound = 404,
        UnprocessableEntity = 422,
        Unauthorized = 401,
        Forbidden = 403,
        Existing = 711,
        Duplicate = 16,
        CannotDelete = 27,
        InvalidQuantity = 14,
        InvalidExportType = 15,
        InvalidOldPassword = 28,
        YouAreNotAuthorized = 100,
        YouAreNotAuthorizedToAccessThisResource = 101,

        // Authentication Errors
        UsernamePasswordIncorrect = 709,
        InvalidStatusCode = 710,
        TokenExpired = 102,
        TokenExpireFailure = 4,
        UserNotAuthorizeFailure = 3,

        // User Registration Errors
        UserNotRegistered = 1,
        UserAlreadyRegistered = 2,
        RoleAlreadyRegistered = 6,
        NameExists = 7,
        CompanyAlreadyRegistered = 8,

        // Specific User-Related Errors
        EmailExists = 704,
        MobileNumberExists = 706,
        UserNameExists = 707,
        PhoneNotExists = 10,
        NoAccountsRegistered = 12,
        AccountNotConfirmed = 13,
        UserCannotBeDeleted = 31,
        UserIsBlocked = 708,

        // General Errors
        InternalServerError = 500,
        Exception = 705,
        DataNotFound = 5,
        GeneralError = 99,
        InvalidUsernameOrPassword = 9,
    }
}
