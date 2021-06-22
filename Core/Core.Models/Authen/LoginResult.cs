using System;
namespace Core.Models
{
    public enum LogCategory : int
	{
		LogonSuccess = 1,
		LogonFail = 2,
		Logout = 3,
		ChangePassword = 4,
		ResetPassword = 5,
		Enable = 6,
		Disable = 7,
		LockedPassword = 8,
		UnlockPassword = 9,
		MenuAccess = 10,
		ForgetPassword = 11,
		UnlockUser = 12,
		Active = 13,
		Inactive = 14,
		WorkStationMismatch = 15
	}
	public enum LoginResultType
	{
		Success = 0,
		UserNotFound = 100,
		PasswordIncorrect = 101,
		UserLock = 102,
		UserDisable = 103,
		TemporarilySuspended = 104,
		InActive = 105,
		UserExpired = 200,
		PasswordExpired = 201,
		ForceChagePassword = 203,
		PasswordWillExpire = 204,

		IsWarningPasswordExpire = 300,
		Operationfailed = 900
	}
	public class LoginResult
	{
		public LoginResultType ResultCode { get; set; }
		public string ResutlMessage { get; set; }
		public string ResutlMessage2 { get; set; }
		public bool IsWarningPasswordExpired { get; set; }
		public bool IsPasswordExpired { get; set; }
		public bool IsForceChagePassword { get; set; }
		public bool IsDisable { get; set; }
		public bool IsInactive { get; set; }
		public bool IsLock { get; set; }
		public bool IsIncorrect { get; set; }
		public bool IsSuccess { get; set; }

		public LoginResult()
		{
			IsDisable = false;
			IsInactive = false;
			IsLock = false;
			IsIncorrect = false;
			IsSuccess = false;
		}
	}
}
