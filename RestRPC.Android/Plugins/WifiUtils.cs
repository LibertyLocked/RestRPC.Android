using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Net.Wifi;

using RestRPC.Framework.Plugins;

namespace RestRPC.Android.Plugins
{
    class WifiUtils : Plugin
    {
        WifiManager wifiMan;

        public WifiUtils()
        {
            wifiMan = (WifiManager)Application.Context.GetSystemService(Context.WifiService);
        }

        public override object Respond(object[] args)
        {
            if (args.Length > 0 && args[0] != null)
            {
                if (args[0].Equals("ssid"))
                {
                    return wifiMan.ConnectionInfo.SSID;
                }
                else if (args[0].Equals("scan"))
                {
                    return wifiMan.StartScan();
                }
                else if (args[0].Equals("scanresults"))
                {
                    var wResults = new WScanResult[wifiMan.ScanResults.Count];

                    // Format the scan results
                    for (int i = 0; i < wifiMan.ScanResults.Count; i++)
                    {
                        var r = wifiMan.ScanResults[i];
                        var wsr = new WScanResult();
                        wsr.Bssid = r.Bssid;
                        wsr.Ssid = r.Ssid;
                        wsr.Capabilities = r.Capabilities;
                        wsr.ChannelWidth = r.ChannelWidth;
                        wsr.Frequency = r.Frequency;
                        wsr.Level = r.Level;
                        wsr.TSFTimestamp = r.Timestamp;
                        wResults[i] = wsr;
                    }

                    return wResults;
                }
            }


            return null;
        }

        struct WScanResult
        {
            public string Bssid;
            public string Ssid;
            public string Capabilities;
            public int ChannelWidth;
            public int Frequency;
            public int Level;
            public long TSFTimestamp;
        }
    }
}
