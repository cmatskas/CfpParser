using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace CfpParser
{
    public class Parser
    {
        string filepath;
        string title;
        string location;
        string startDate;
        string endDate;
        string closingDate;
        string url;
        string confYear;
        Dictionary<string, Cfp> CfpCollection;
        string outputpath;
        bool testexisting;
        string existingfolder;
        string tags;
        List<string> existingFileNames;

        public Parser(string filePath, string outputPath, bool checkIfExists, string existingFolder)
        {
            filepath = filePath;
            outputpath = outputPath;
            testexisting = checkIfExists;
            existingfolder = existingFolder;
            CfpCollection = new Dictionary<string, Cfp>();
            existingFileNames = new List<string>();
            LoadExistingFileNames();
        }

        public void ParseFile()
        {
            int lineGroupCount = 6;
            int lineSkip = 0;

            var lines = File.ReadAllLines(filepath);
            while (lineSkip <= lines.Length)
            {
                var conference = lines.Skip(lineSkip).Take(lineGroupCount).ToArray();

                GetConferenceTitleAndLocation(conference[0]);
                GetClosingDate(conference[1]);
                GetConferenceDates(conference[3]);
                url = conference[4];
                tags = conference[5];
                var filename = GetFileName();
                if (!CfpCollection.Keys.Contains(filename))
                {
                    CfpCollection.Add(filename, CreateCfp());
                }

                lineSkip += 7;
            }
        }

        public void WriteOutput()
        {
            if(!Directory.Exists(outputpath))
            {
                Directory.CreateDirectory(outputpath);
            }

            foreach (var key in CfpCollection.Keys)
            {
                if (testexisting && CfpFileExists(key))
                {
                    continue;
                }

                var cfp = CfpCollection[key];
                File.WriteAllText(Path.Combine(outputpath, key), JsonConvert.SerializeObject(cfp));
            }
        }

        public bool CfpFileExists(string filename)
        {
            return existingFileNames.Contains(filename);
        }

        private void LoadExistingFileNames()
        {
            if (testexisting)
            {
                existingFileNames = Directory.EnumerateFiles(existingfolder).ToList();
            }
        }

        private Cfp CreateCfp()
        {
            return new Cfp
            {
                name = title,
                lang = "en",
                location = location,
                callForPapersEnd = closingDate,
                conferenceEnd = endDate,
                conferenceStart = startDate,
                url = url,
                tags = tags
            };
        }

        private void GetConferenceDates(string line)
        {
            if (string.IsNullOrEmpty(line) || line == "Conference Dates:")
            {
                startDate = string.Empty;
                endDate = string.Empty;
                confYear = DateTime.Now.Year.ToString();
                return;
            }

            var partArray = line.Replace("Conference Dates: ", "").Replace(",", "").Split(' ');
            string[] parts = partArray.Where(t => !t.Equals("to")).ToArray();

            startDate = new DateTime(int.Parse(parts[2]), DateTime.ParseExact(parts[0], "MMMM", CultureInfo.CurrentCulture).Month, int.Parse(parts[1])).ToString("yyyy-MM-dd");
            endDate = parts.Length == 6
                    ? new DateTime(int.Parse(parts[5]), DateTime.ParseExact(parts[3], "MMMM", CultureInfo.CurrentCulture).Month, int.Parse(parts[4])).ToString("yyyy-MM-dd")
                    : endDate = startDate;

            confYear = parts[2];
        }

        private void GetConferenceTitleAndLocation(string line)
        {
            var parts = line.Split('-');
            title = parts[0].Trim();
            location = parts[1].Trim();
        }

        private void GetClosingDate(string line)
        {
            closingDate = string.Empty;
            var dateString = line.Substring(8);
            if (!string.IsNullOrEmpty(dateString) && !dateString.Equals("Unknown", StringComparison.InvariantCultureIgnoreCase))
            {
                DateTime result;
                var convertionSuccessful = DateTime.TryParse(dateString, new CultureInfo("en-US"), DateTimeStyles.None, out result);
                if (convertionSuccessful)
                {
                    closingDate = result.ToString("yyyy-MM-dd");
                    return;
                }

                var parts = dateString.Split(' ');

                var month = DateTime.ParseExact(parts[0], "MMMM", CultureInfo.CurrentCulture).Month.ToString();
                month = month.Length == 1 ? "0" + month : month; 
                var date = parts[1].Replace(",", "");
                date = date.Length == 1 ? "0" + date : date;
                var year = parts[2];
                closingDate = year + "-" + month + "-" + date;
            }
        }

        private string GetFileName()
        {
            var cleanTitle = title.Replace(":", "").Replace("Call For Papers", "").Replace("&", "And").Replace(".", "").Trim();
            var filename = cleanTitle.Contains(confYear) ? cleanTitle + ".json" : cleanTitle + confYear + ".json";
            return filename.Replace(" ", "");
        }
    }
}
