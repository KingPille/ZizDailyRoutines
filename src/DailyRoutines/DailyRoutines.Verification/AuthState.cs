using DailyRoutines.Common.Manager.Abstractions;
using QyFnmXyVXpvh4aoXixb;
using Sem6j6yhpQ5uEW1MJxL;
using hYVydiyFN238b8srlVt;
using hcXTsMLIXdeaKdE7xb7;

namespace DailyRoutines.Verification;

public static class AuthState
{
	public static bool IsConnected => ManagerBase<ILbWvcLd7xPWOTvckYC>.Instance().GE5LYllDIN<sTVhMSySSmLd9sTlJde>()?.IsLive ?? false;

	public static bool IsAuth
	{
		get
		{
			xuH6tCybRrvw1o0SxGV xuH6tCybRrvw1o0SxGV2 = ManagerBase<ILbWvcLd7xPWOTvckYC>.Instance().GE5LYllDIN<xuH6tCybRrvw1o0SxGV>();
			if (xuH6tCybRrvw1o0SxGV2 == null || !xuH6tCybRrvw1o0SxGV2.IsAllowToPassAuth)
			{
				return ManagerBase<ILbWvcLd7xPWOTvckYC>.Instance().GE5LYllDIN<sTVhMSySSmLd9sTlJde>()?.IsValid ?? false;
			}
			return true;
		}
	}

	public static bool IsPremium => ManagerBase<ILbWvcLd7xPWOTvckYC>.Instance().GE5LYllDIN<S3DJJ3yPZjLumXepvvO>()?.IsPremium ?? false;

	public static bool IsBlacklisted => false;
}
