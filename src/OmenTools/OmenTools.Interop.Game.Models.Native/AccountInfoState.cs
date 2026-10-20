namespace OmenTools.Interop.Game.Models.Native;

public enum AccountInfoState
{
	Uninitialized = -2,
	ReadingCacheFile,
	CacheFileInvalid,
	UpdatedFromServer,
	CacheFileProcessed
}
