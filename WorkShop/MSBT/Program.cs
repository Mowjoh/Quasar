/*
 * MSBTEditorCli by Coolsonickirby / Random
 * Credits all due to IcySon55 and exelix11 for the original MSBT source code, all I did was rework it to make it
 * export MSBTs as JSON and vice-versa because there was nothing I could find that does that.
 * If you want to add yourself to the credits, feel free to do so with a pull request.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MsbtEditor;
using System.IO;
using Newtonsoft.Json;

namespace MSBTEditorCli
{

    class Program
    {

        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Help();
                System.Environment.Exit(0);
            }

            string file_ext = Path.GetExtension(args[0]);

            switch (file_ext)
            {
                case ".msbt":
                    ExtractMSBT(args[0], args[1]);
                    break;
                case ".json":
                    RepackMSBT(args[0], args[1]);
                    break;
                default:
                    Console.WriteLine("Invalid File!");
                    break;
            }
        }

        static void ExtractMSBT(string path, string output)
        {
            System.IO.FileInfo file = new System.IO.FileInfo(output);
            file.Directory.Create(); // If the directory already exists, this method does nothing.

            MSBT _msbt = new MSBT(path);

            MSBTList lstStrings = new MSBTList();

            MainJSON main = new MainJSON();
            main.header = _msbt.Header;
            main.SectionOrder = _msbt.SectionOrder;
            main.TXT2.NumberOfStrings = _msbt.TXT2.NumberOfStrings;
            main.TXT2.Identifier = _msbt.TXT2.Identifier;
            main.TXT2.Padding1 = _msbt.TXT2.Padding1;
            main.LBL1.NumberOfGroups = _msbt.LBL1.NumberOfGroups;
            main.LBL1.Groups = _msbt.LBL1.Groups;
            main.LBL1.Identifier = _msbt.LBL1.Identifier;
            main.LBL1.Padding1 = _msbt.LBL1.Padding1;
            main.ATO1 = _msbt.ATO1;
            main.ATR1 = _msbt.ATR1;
            main.NLI1 = _msbt.NLI1;
            main.TSY1 = _msbt.TSY1;

            List<JsonMSBT> json = new List<JsonMSBT>();

            for (int i = 0; i < _msbt.TXT2.NumberOfStrings; i++)
            {
                if (_msbt.HasLabels)
                {
                    lstStrings.Sorted = true;
                    lstStrings.Items.Add(_msbt.LBL1.Labels[i]);
                }
                else
                {
                    lstStrings.Sorted = false;
                    lstStrings.Items.Add(_msbt.TXT2.Strings[i]);
                }
            }

            //if (lstStrings.Sorted)
            //{
            //    for (int i = 0; i < lstStrings.Items.Count; i++)
            //    {
            //        var x = lstStrings.Items[i];
            //        var j = i;
            //        while (j > 0 && lstStrings.Items[j - 1].LabelName.CompareTo(x.LabelName) > 0)
            //        {
            //            lstStrings.Items[j] = lstStrings.Items[j - 1];
            //            j = j - 1;
            //        }
            //        lstStrings.Items[j] = x;
            //    }
            //}

            foreach (MsbtEditor.Label label in lstStrings.Items)
            {
                string TMPlabel = label.Name;
                string TMPvalue = _msbt.FileEncoding.GetString(label.Value).Replace("\n", "\r\n").TrimEnd('\0').Replace("\0", @"\0") + "\0";

                JsonMSBT tmpJson = new JsonMSBT();

                tmpJson.label = TMPlabel;
                tmpJson.value = TMPvalue.Replace("\u0000", "");


                json.Add(tmpJson);
            }

            main.strings = json;

            Newtonsoft.Json.JsonSerializer serializer = new Newtonsoft.Json.JsonSerializer();
            serializer.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
            serializer.TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Auto;
            serializer.Formatting = Newtonsoft.Json.Formatting.Indented;

            using (StreamWriter sw = new StreamWriter(output))
            using (Newtonsoft.Json.JsonWriter writer = new Newtonsoft.Json.JsonTextWriter(sw))
            {
                serializer.Serialize(writer, main, typeof(MainJSON));
            }
        }

        static void RepackMSBT(string path, string output)
        {
            System.IO.FileInfo file = new System.IO.FileInfo(output);
            file.Directory.Create(); // If the directory already exists, this method does nothing.
            MSBT _msbt = new MSBT();

            string jsonText = System.IO.File.ReadAllText(path);

            MainJSON json = JsonConvert.DeserializeObject<MainJSON>(jsonText, new Newtonsoft.Json.JsonSerializerSettings
            {
                TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Auto,
                NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore,
            });


            _msbt.Header = json.header;
            _msbt.SectionOrder = json.SectionOrder;
            _msbt.TXT2.NumberOfStrings = json.TXT2.NumberOfStrings;
            _msbt.TXT2.Identifier = json.TXT2.Identifier;
            _msbt.TXT2.Padding1 = json.TXT2.Padding1;
            _msbt.LBL1.NumberOfGroups = json.LBL1.NumberOfGroups;
            _msbt.LBL1.Padding1 = json.LBL1.Padding1;
            _msbt.LBL1.Identifier = json.LBL1.Identifier;
            _msbt.ATO1 = json.ATO1;
            _msbt.ATR1 = json.ATR1;
            _msbt.NLI1 = json.NLI1;
            _msbt.TSY1 = json.TSY1;

            _msbt.LBL1.Groups.Clear();

            for (int i = 0; i < json.LBL1.NumberOfGroups; i++)
            {
                Group tmp = new Group();
                tmp.NumberOfLabels = json.LBL1.Groups[i].NumberOfLabels;
                _msbt.LBL1.Groups.Add(tmp);
            }

            int total = 0;
            
            foreach (Group grp in json.LBL1.Groups)
            {
                for (int i = 0; i < grp.NumberOfLabels; i++)
                {
                    _msbt.AddLabelFromJson(json.strings[total].label, json.strings[total].value, (uint)json.LBL1.Groups.IndexOf(grp));
                    total += 1;
                }
            }

            // Tie in LBL1 labels
            foreach (Label lbl in _msbt.LBL1.Labels)
                lbl.String = _msbt.TXT2.Strings[(int)lbl.Index];

            _msbt.File = new FileInfo(output);

            _msbt.Save();

        }

        static void Help()
        {
            Console.WriteLine("Usage: MSBTEditorCli.exe <INPUT> <OUTPUT>");
            Console.WriteLine("Input support: *.msbt, *.json");
            Console.WriteLine("Output support: *.msbt, *.json");
            Console.WriteLine("Credits:");
            Console.WriteLine("MSBTEditorCli - Coolsonickirby/Random");
            Console.WriteLine("MSBT Original Source code - IcySon55");
            Console.WriteLine("MSBT Original Source code - exelix11");
        }
    }
}