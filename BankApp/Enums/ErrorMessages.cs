namespace BankApp.Enums;

public enum ErrorMessages
{
    /// <summary>
    /// Indicates that the user's account has been locked.
    /// </summary>
    LockedAccount,

    /// <summary>
    /// Indicates that the specified user could not be found.
    /// </summary>
    UserNotFound,

    /// <summary>
    /// Indicates that the selected option is invalid.
    /// </summary>
    InvalidOption,

    /// <summary>
    /// Indicates that no bank accounts were found for the user.
    /// </summary>
    NoBankAccountFound,

    /// <summary>
    /// Not enough accounts available to make a transfer.
    /// </summary>
    NotEnoughAccounts,

    /// <summary>
    /// Indicates that the source and destination accounts are the same.
    /// </summary>
    SameSourceAndDestinationAccount
}
