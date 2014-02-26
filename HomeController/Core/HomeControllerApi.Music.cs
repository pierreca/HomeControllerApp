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

        public static Task<int> GetVolume()
        {
            var tcs = new TaskCompletionSource<int>();

            WebClient wc = new WebClient();
            wc.DownloadStringCompleted += (s, args) =>
            {
                if(args.Error == null)
                {
                    try
                    {
                        var volume = int.Parse(args.Result);
                        tcs.TrySetResult(volume);
                    }
                    catch(Exception)
                    {
                        tcs.TrySetException(new Exception("Could not parse volume"));
                    }
                }
                else
                {
                    tcs.TrySetException(new Exception("Could not get volume"));
                }
            };
            wc.DownloadStringAsync(new Uri(BaseUrl + "music/volume?nocache=" + Environment.TickCount.ToString(), UriKind.Absolute));

            return tcs.Task;
        }

        public static Task<bool> SetVolume(int volume)
        {
            // TODO: badly needs refactoring and exception handling
            var tcs = new TaskCompletionSource<bool>();

            var req = WebRequest.CreateHttp(BaseUrl + "music/volume");
            req.ContentType = "application/json";
            req.Method = "POST";

            var result = req.BeginGetRequestStream(
                state =>
                {
                    var r = state.AsyncState as HttpWebRequest;
                    using (var str = r.EndGetRequestStream(state))
                    {
                        byte[] data = Encoding.UTF8.GetBytes("{ \"volume\" : " + volume.ToString() + " }");
                        str.Write(data, 0, data.Length);
                        str.Flush();
                    }

                    var result2 = req.BeginGetResponse(
                        state2 =>
                        {
                            var r2 = state2.AsyncState as HttpWebRequest;
                            var response = r2.EndGetResponse(state2);
                            tcs.TrySetResult(true);
                        },
                        r);
                },
                req);

            return tcs.Task;
        }
    }
}
