using System;
using System.IO;
using Lumina.Data;
using OmenTools.Localization.Abstractions;

namespace OmenTools.Localization;

public sealed class FileLocalizationSource(string directoryPath) : ILocalizationSource, IDisposable
{
	private readonly string directoryPath = Path.GetFullPath(directoryPath);

	private FileSystemWatcher? watcher;

	private EventHandler<LocalizationSourceChangedEventArgs>? resourceChanged;

	public bool SupportsChangeNotifications => true;

	public event EventHandler<LocalizationSourceChangedEventArgs>? ResourceChanged
	{
		add
		{
			resourceChanged = (EventHandler<LocalizationSourceChangedEventArgs>)Delegate.Combine(resourceChanged, value);
			EnsureWatcher();
		}
		remove
		{
			resourceChanged = (EventHandler<LocalizationSourceChangedEventArgs>)Delegate.Remove(resourceChanged, value);
			if (resourceChanged == null)
			{
				DisposeWatcher();
			}
		}
	}

	public bool Exists(Language language, string resourceName)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(resourceName, "resourceName");
		return File.Exists(GetFilePath(resourceName));
	}

	public Stream? OpenRead(Language language, string resourceName)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(resourceName, "resourceName");
		string filePath = GetFilePath(resourceName);
		if (!File.Exists(filePath))
		{
			return null;
		}
		return new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
	}

	public void Dispose()
	{
		DisposeWatcher();
	}

	private string GetFilePath(string resourceName)
	{
		return Path.Join(directoryPath, resourceName);
	}

	private void EnsureWatcher()
	{
		if (watcher == null)
		{
			Directory.CreateDirectory(directoryPath);
			watcher = new FileSystemWatcher(directoryPath)
			{
				Filter = "*.*",
				IncludeSubdirectories = false,
				NotifyFilter = (NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.CreationTime),
				EnableRaisingEvents = true
			};
			watcher.Changed += OnWatcherChanged;
			watcher.Created += OnWatcherChanged;
			watcher.Renamed += OnWatcherChanged;
			watcher.Deleted += OnWatcherChanged;
		}
	}

	private void DisposeWatcher()
	{
		if (watcher != null)
		{
			watcher.Changed -= OnWatcherChanged;
			watcher.Created -= OnWatcherChanged;
			watcher.Renamed -= OnWatcherChanged;
			watcher.Deleted -= OnWatcherChanged;
			watcher.Dispose();
			watcher = null;
		}
	}

	private void OnWatcherChanged(object sender, FileSystemEventArgs e)
	{
		resourceChanged?.Invoke(this, new LocalizationSourceChangedEventArgs(e.Name ?? string.Empty, e.ChangeType));
	}
}
