using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using Windows.Storage;

namespace HomeController
{
    public static class ScenarioCacheHelper
    {
        private const string ScenarioCacheFile = "scenarios.xml";

        public async static Task<List<Scenario>> LoadOrCreateScenarios()
        {
            List<Scenario> results = null;

            var cacheFile = await OpenOrCreateCacheFile();

            using (Stream s = await cacheFile.OpenStreamForReadAsync())
            {
                var serializer = new XmlSerializer(typeof(List<Scenario>));

                try
                {
                    results = (List<Scenario>)serializer.Deserialize(s);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Error deserializing the scenarios file");
                    Debug.WriteLine(ex.Message);

                    results = new List<Scenario>();
                }
            }

            return results;
        }

        public async static Task<Scenario> LoadScenario(string scenarioName)
        {
            var scenarios = await LoadOrCreateScenarios();

            var candidates = from s in scenarios
                             where string.Compare(s.Name, scenarioName, StringComparison.InvariantCultureIgnoreCase) == 0
                             select s;

            if (candidates.Count() > 0)
                return candidates.First();
            else
                return null;
        }

        public async static Task SaveScenario(Scenario scenario)
        {
            var scenarios = await LoadOrCreateScenarios();

            var candidates = from s in scenarios
                                where string.Compare(s.Name, scenario.Name, StringComparison.InvariantCultureIgnoreCase) == 0
                                select s;

            if (candidates.Count() != 0)
                scenarios.Remove(candidates.First());

            scenarios.Add(scenario);

            await ClearCacheFile();
            var cacheFile = await OpenOrCreateCacheFile();

            using (Stream s = await cacheFile.OpenStreamForWriteAsync())
            {
                var serializer = new XmlSerializer(typeof(List<Scenario>));
                serializer.Serialize(s, scenarios);
            }
        }

        public async static Task DeleteScenario(string scenarioName)
        {
            var scenarios = await LoadOrCreateScenarios();

            var candidates = from s in scenarios
                             where string.Compare(s.Name, scenarioName, StringComparison.InvariantCultureIgnoreCase) == 0
                             select s;

            if (candidates.Count() != 0)
                scenarios.Remove(candidates.First());

            await ClearCacheFile();
            var cacheFile = await OpenOrCreateCacheFile();

            using (Stream s = await cacheFile.OpenStreamForWriteAsync())
            {
                var serializer = new XmlSerializer(typeof(List<Scenario>));
                serializer.Serialize(s, scenarios);
            }   
        }

        private async static Task ClearCacheFile()
        {
            StorageFolder folder = ApplicationData.Current.LocalFolder;

            try
            {
                var file = await folder.GetFileAsync(ScenarioCacheFile);
                await file.DeleteAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error clearing scenario cache");
                Debug.WriteLine(ex);
            }
        }


        private async static Task<StorageFile> OpenOrCreateCacheFile()
        {
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            StorageFile cacheFile = null;

            var fileOpened = false;
            var fileNotFound = false;

            while (!fileOpened)
            {
                if (fileNotFound)
                {
                    await folder.CreateFileAsync(ScenarioCacheFile);
                }

                try
                {
                    cacheFile = await folder.GetFileAsync(ScenarioCacheFile);
                    fileOpened = true;
                }
                catch (FileNotFoundException fnfex)
                {
                    if (!fileNotFound)
                        fileNotFound = true;
                    else
                        throw fnfex;
                }
            }

            return cacheFile;
        }
    }
}
