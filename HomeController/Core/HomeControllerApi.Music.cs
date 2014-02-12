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
        public static Task<IEnumerable<PandoraRadioStation>> GetPandoraStations()
        {
            var tcs = new TaskCompletionSource<IEnumerable<PandoraRadioStation>>();

            WebClient wc = new WebClient();
            wc.DownloadStringCompleted += (s, args) =>
            {
                if (args.Error == null)
                {
                    var json = JArray.Parse(args.Result);
                    tcs.TrySetResult(from obj in json
                                     select new PandoraRadioStation()
                                     {
                                         Name = obj["name"].ToString(),
                                         Id = obj["id"].ToString()
                                     });
                }
                else
                {
                    tcs.TrySetException(new Exception("Could not retrieve the list of pandora stations"));
                }
            };
            wc.DownloadStringAsync(new Uri(BaseUrl + "music/pandora/stations/pierreca@live.com", UriKind.Absolute));

            return tcs.Task;
        }

        public static Task<bool> Play(PandoraRadioStation station)
        {
            var tcs = new TaskCompletionSource<bool>();

            WebClient wc = new WebClient();
            wc.DownloadStringCompleted += (s, args) =>
            {
                if (args.Error == null)
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
                else
                {
                    tcs.TrySetException(new Exception("Could not play the station"));
                }
            };
            wc.DownloadStringAsync(new Uri(BaseUrl + "music/pandora/play/" + station.Id, UriKind.Absolute));

            return tcs.Task;
        }

        public static Task<RadioSong> GetCurrentSong()
        {
            var tcs = new TaskCompletionSource<RadioSong>();

            WebClient wc = new WebClient();
            wc.DownloadStringCompleted += (s, args) =>
            {
                if (args.Error == null)
                {
                    try
                    {
                        var json = JObject.Parse(args.Result);
                        var currentSong = new RadioSong();
                        currentSong.Album = json["album"].ToString();
                        currentSong.AlbumArt = json["album_art"].ToString();
                        currentSong.Artist = json["artist"].ToString();
                        currentSong.Title = json["title"].ToString();
                        tcs.TrySetResult(currentSong);
                    }
                    catch (Exception ex)
                    {
                        tcs.TrySetException(new Exception("Could not parse JSON response"));
                    }
                }
                else
                {
                    tcs.TrySetException(new Exception("Could not get current song"));
                }
            };
            wc.DownloadStringAsync(new Uri(BaseUrl + "music/nowplaying?nocache=" + Environment.TickCount.ToString(), UriKind.Absolute));

            return tcs.Task;
        }


        public static Task<bool> Pause()
        {
            var tcs = new TaskCompletionSource<bool>();

            WebClient wc = new WebClient();
            wc.DownloadStringCompleted += (s, args) =>
            {
                if (args.Error == null)
                {
                    if (string.Compare(args.Result, "OK\r\n", StringComparison.InvariantCultureIgnoreCase) == 0)
                    {
                        tcs.TrySetResult(true);
                    }
                    else
                    {
                        tcs.TrySetResult(false);
                    }
                }
                else
                {
                    tcs.TrySetException(new Exception("Could not pause"));
                }
            };
            wc.DownloadStringAsync(new Uri(BaseUrl + "music/pause?nocache=" + Environment.TickCount.ToString(), UriKind.Absolute));

            return tcs.Task;
        }
    }
}
