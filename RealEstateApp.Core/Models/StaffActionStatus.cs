namespace RealEstateApp.Core.Models;

public enum StaffActionStatus
{
    Success,
    NotFound,

    CannotModifySelf,

    LastActiveAdmin,

    PasswordUpdateFailed
}
