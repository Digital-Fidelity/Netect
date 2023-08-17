using System;
using System.Linq;
using System.Media;
using System.Net.NetworkInformation;
using System.Threading;

class Program
{
    static void Main()
    {
        Console.WriteLine("Monitoring for suspicious network traffic...");

        NetworkChange.NetworkAddressChanged += (_, __) =>
        {
            ThreadPool.QueueUserWorkItem(_ =>
            {
                CheckForSuspiciousTraffic();
            });
        };

        Console.ReadLine();
    }

    static void CheckForSuspiciousTraffic()
    {
        var interfaces = NetworkInterface.GetAllNetworkInterfaces()
            .Where(i => i.OperationalStatus == OperationalStatus.Up);

        foreach (var networkInterface in interfaces)
        {
            var statistics = networkInterface.GetIPv4Statistics();

            // Check for suspicious criteria
            if (statistics.UnicastPacketsReceived == 0)
            {
                HighlightAndAlert(networkInterface.Name);
            }
        }
    }

    static void HighlightAndAlert(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"{DateTime.Now}: Suspicious network traffic detected: {message}");
        Console.ResetColor();

        // Play alert sound
        using (var soundPlayer = new SoundPlayer(@"alarm.mp3"))
        {
            soundPlayer.Play();
        }
    }
}