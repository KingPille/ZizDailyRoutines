namespace DailyRoutines.Manager;

internal enum InternalAPIRequestType
{
	ReportLocationInfo,
	ReportLocationInfos,
	ReportModulesUsageByCID,
	RequestZoneCoordinates,
	RequestPlayerInfoByCID,
	RequestBlacklistCredential,
	CreateBlacklist,
	CreateSharedBlacklist,
	UpdateBlacklist,
	GetBlacklist,
	RequestPremiumCredential,
	ReportCredentialCID,
	ReportCID,
	ReportChaoticBonus,
	RequestChaoticBonus,
	RequestChaoticUptime,
	RequestRecommendModules,
	RequestUsedNames,
	ReportDutyProgress,
	ReportCosmoSpecialWeather,
	GetBlacklistLogs,
	GetDailyActiveUserCount,
	RequestISPTranslator,
	ReportCECrowdsource,
	RequestCECrowdsourceSampling
}
