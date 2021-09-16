using Newtonsoft.Json;
using Quasar.Data.V2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace QuasarDataEditor
{
    class DBStructs
    {
        /// <summary>
        /// This function will read the CSV containing all song names, 
        /// Then, it matches them with the database ID's
        /// Then, it matches them with the appropriate series.
        /// Then it outputs the whole as a CSV
        /// </summary>
        public static Game GetXML(Game game)
        {
            List<MSBTMatch> Matches = new List<MSBTMatch>();
            List<MSBTValue> MSBTRegionalValues = new List<MSBTValue>();

            //Parsing Song names and ID's
            StreamReader file =    new StreamReader(@"ui_bgm_names.csv");
            string line = "";
            while ((line = file.ReadLine()) != null)
            {
                if(line.Length > 3)
                {
                    if (line.Substring(0, 3) == "bgm")
                    {
                        MSBTValue val = new MSBTValue()
                        {
                            FileIdentifier = line.Split(',')[0],
                            RegionalName = line.Replace(line.Split(',')[0] + ",", "")
                        };

                        if (val.FileIdentifier.Split('_')[1] == "title")
                        {
                            val.FileIdentifier = val.FileIdentifier.Split('_')[2];
                            MSBTRegionalValues.Add(val);
                        }
                    }
                }
            }

            //Matching with database
            XDocument doc = XDocument.Load(@"ui_bgm_db.xml");
            XElement el = doc.Element("struct").Element("list");
            foreach (XElement item in el.Descendants("struct"))
            {
                XElement NameID = item.Element("string");
                MSBTValue val = MSBTRegionalValues.SingleOrDefault(v => v.FileIdentifier == NameID.Value);
                if(val != null)
                {
                    MSBTMatch match = new MSBTMatch()
                    {
                        us_en = val.RegionalName
                    };
                    foreach(XElement x in item.Elements("hash40"))
                    {
                        switch (x.Attribute("hash").Value)
                        {
                            default:
                                break;
                            case "record_type":
                                match.RecordingType = x.Value;
                                break;
                            case "ui_bgm_id":
                                match.Filename = x.Value.Substring(3, x.Value.Length - 3);
                                break;
                        }
                    }
                    Matches.Add(match);
                }
            }

            //Matching Series
            foreach (XElement Listerine in doc.Element("struct").Elements("list"))
            {
                string seriesName = Listerine.Attribute("hash").Value;
                if (seriesName.Substring(0, 3) == "bgm")
                {
                    foreach (XElement str in Listerine.Elements("struct"))
                    {
                        XElement fileName = str.Element("hash40");
                        MSBTMatch m = Matches.SingleOrDefault(ma => ma.Filename == fileName.Value.Substring(3, fileName.Value.Length - 3));
                        if (m != null)
                        {
                            m.Series = seriesName;
                        }
                    }
                }
            }

            //Serialising file
            JsonSerializer listerino = new JsonSerializer();
            using (StreamWriter NewFile = File.CreateText(@"NewFile.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(NewFile, Matches);
            }

            using (StreamWriter CSVOutput = new StreamWriter(@"CSVOutput.csv"))
            {
                CSVOutput.WriteLine("Song Name,File Name,Recording Type,Series Name");
                foreach (MSBTMatch ma in Matches)
                {
                    CSVOutput.WriteLine(String.Format("\"{0}\",{1},{2},{3}", ma.us_en, ma.Filename, ma.RecordingType, ma.Series));
                }
            }

            //Editing matches


            foreach (MSBTMatch m in Matches)
            {
                //If no database entry already matches this filename
                if(!game.GameElementFamilies[2].GameElements.Any(ge => ge.GameFolderName == m.Filename))
                {
                    int lastID = game.GameElementFamilies[2].GameElements.Last().ID;
                    while(game.GameElementFamilies[2].GameElements.Any(ge => ge.ID == lastID))
                    {
                        lastID++;
                    }
                    GameElement newElement = new GameElement()
                    {
                        ID = lastID,
                        FilterValue = m.Series,
                        GameFolderName = m.Filename,
                        Name = String.Format("{0} [{1}]", m.us_en, m.RecordingType)
                    };

                    game.GameElementFamilies[2].GameElements.Add(newElement);


                }
                //Replacing the name if it exists
                else
                {
                    GameElement gel = game.GameElementFamilies[2].GameElements.Single(ge => ge.GameFolderName == m.Filename);
                    gel.Name = String.Format("{0} [{1}]",m.us_en,m.RecordingType);
                    gel.FilterValue = m.Series;
                }
            }

            return game;

            /*
            XmlSerializer serializer = new XmlSerializer(typeof(@struct));

            using (FileStream fileStream = new FileStream(@"ui_bgm_db.xml", FileMode.Open))
            {
                
                @struct result = (@struct)serializer.Deserialize(fileStream);
                foreach(structListStruct item in result.list.@struct)
                {
                    Console.WriteLine(item.Items[9].ToString());
                }
            }*/
        }
    }

    public class MSBTValue
    {
        public string FileIdentifier { get; set; }
        public string RegionalName { get; set; }
    }

    public class MSBTMatch
    {
        public string us_en { get; set; }
        public string Filename { get; set; }
        public string RecordingType { get; set; }
        public string Series { get; set; }
    }
}
