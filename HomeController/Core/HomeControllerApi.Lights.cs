using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HomeController.Core
{
    public partial class HomeControllerApi
    {
        private static string BaseUrl = "http://" + SettingsHelper.ServerIP + ":5000/";

        public static Task<IEnumerable<LightSwitch>> GetSwitches()
        {
            var tcs = new TaskCompletionSource<IEnumerable<LightSwitch>>();

            WebClient wc = new WebClient();
            wc.DownloadStringCompleted += (s, args) =>
            {
                if (args.Error == null)
                {
                    var json = JArray.Parse(args.Result);
                    tcs.TrySetResult(from obj in json
                                     select new LightSwitch()
                                     {
                                         Name = obj["name"].ToString(),
                                         State = ((int)(obj["state"]) == 1) ? true : false
                                     });
                }
                else
                {
                    tcs.TrySetException(new HomeControllerApiException("Could not retrieve the list of switches"));
                }
            };
            wc.DownloadStringAsync(new Uri(BaseUrl + "wemo/list?nocache=" + Environment.TickCount.ToString(), UriKind.Absolute));
            
            return tcs.Task;
        }

        public static Task<bool> FlipSwitch(LightSwitch ls)
        {
            return HomeControllerApi.SetSwitch(ls, !ls.State);
        }

        public static Task<bool> SetSwitch(LightSwitch ls, bool on)
        {
            var urlBuilder = new StringBuilder(BaseUrl + "wemo/");
            var action = on ? "on/" : "off/";
            urlBuilder.Append(action);
            urlBuilder.Append(ls.Name);
            urlBuilder.Append("?nocache=" + Environment.TickCount.ToString());

            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

            WebClient wc = new WebClient();
            wc.DownloadStringCompleted += (s, args) =>
            {
                if (args.Error != null)
                {
                    tcs.TrySetResult(false);
                }
                else
                {
                    if (string.Compare(args.Result, "OK", StringComparison.InvariantCultureIgnoreCase) == 0)
                    {
                        tcs.TrySetResult(true);
                    }
                    else
                    {
                        tcs.TrySetResult(false);
                    }
                }
            };
            wc.DownloadStringAsync(new Uri(urlBuilder.ToString(), UriKind.Absolute));

            return tcs.Task;
        }
    }
}
