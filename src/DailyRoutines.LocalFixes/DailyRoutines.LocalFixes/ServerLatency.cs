using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace DailyRoutines.LocalFixes;

public static class ServerLatency
{
	public static async Task<long> MeasureAsync(IPAddress address, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		if (IPAddress.IsLoopback(address) || address.Equals(IPAddress.Any) || address.Equals(IPAddress.IPv6Any))
		{
			return -1L;
		}
		try
		{
			using Ping ping = new Ping();
			PingReply pingReply = await ping.SendPingAsync(address, TimeSpan.FromSeconds(1L), new byte[32], new PingOptions(), cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			return (pingReply.Status == IPStatus.Success) ? pingReply.RoundtripTime : (-1);
		}
		catch (PingException)
		{
			return -1L;
		}
		catch (SocketException)
		{
			return -1L;
		}
	}
}
