using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.IO;
using System.Collections;
using Quasar.XMLResources;

namespace Quasar.XMLResources
{
    

    public static class WorkspaceXML 
    {
        public static List<Workspace> GetWorkspaces()
        {
            List<Workspace> Workspaces = new List<Workspace>();
            string AssociationLibraryPath = Properties.Settings.Default.DefaultDir + @"\Library\Workspaces.xml";
            XmlSerializer serializer = new XmlSerializer(typeof(Workspaces));

            if (File.Exists(AssociationLibraryPath))
            {
                using (FileStream fileStream = new FileStream(AssociationLibraryPath, FileMode.Open))
                {

                    Workspaces result = (Workspaces)serializer.Deserialize(fileStream);
                    foreach (Workspace workspace in result)
                    {
                        Workspaces.Add(workspace);
                    }
                }
            }
            return Workspaces;
        }

        public static void WriteWorkspaces(List<Workspace> Workspaces)
        {
            string AssociationLibraryPath = Properties.Settings.Default["DefaultDir"].ToString() + "\\Library\\Workspaces.xml";

            Workspaces al = new Workspaces(Workspaces);

            XmlSerializer LibrarySerializer = new XmlSerializer(typeof(Workspaces));
            using (StreamWriter Writer = new StreamWriter(AssociationLibraryPath))
            {
                LibrarySerializer.Serialize(Writer, al);
            }
        }


    }

}
