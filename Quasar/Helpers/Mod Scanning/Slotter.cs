using Quasar.Data.V2;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quasar.Helpers.Mod_Scanning
{
    public static class Slotter
    {
        public static ObservableCollection<ContentItem> AddNewContentItems(ObservableCollection<ContentItem> ScannedItems, ObservableCollection<ContentItem> ContentItems)
        {

            return ContentItems;
        }

        /// <summary>
        /// Slots Content Items automatically in a Workspace
        /// </summary>
        /// <param name="ContentItems"></param>
        /// <param name="Workspace"></param>
        /// <param name="QuasarModTypes"></param>
        /// <returns></returns>
        public static Workspace AutomaticSlot(List<ContentItem> ContentItems, Workspace Workspace, ObservableCollection<QuasarModType> QuasarModTypes)
        {
            foreach (ContentItem ci in ContentItems)
            {
                if (ci.GameElementID != -1)
                {
                    QuasarModType imt = QuasarModTypes.Single(i => i.ID == ci.QuasarModTypeID);
                    if (imt.IsExternal)
                    {
                        int Slot = 0;
                        bool foundSlot = false;

                        while (!foundSlot)
                        {
                            if (!Workspace.Associations.Any(a => a.QuasarModTypeID == imt.ID && a.SlotNumber == Slot))
                            {
                                Workspace.Associations.Add(new Association() { ContentItemGuid = ci.Guid, GameElementID = ci.GameElementID, QuasarModTypeID = ci.QuasarModTypeID, SlotNumber = Slot });
                                foundSlot = true;
                            }
                            Slot++;
                        }
                    }
                    else
                    {
                        Association associations = Workspace.Associations.SingleOrDefault(ass => ass.GameElementID == ci.GameElementID && ass.QuasarModTypeID == ci.QuasarModTypeID && ass.SlotNumber == ci.SlotNumber);
                        if (associations != null)
                        {
                            Workspace.Associations.Remove(associations);
                        }
                        Workspace.Associations.Add(new Association() { ContentItemGuid = ci.Guid, GameElementID = ci.GameElementID, QuasarModTypeID = ci.QuasarModTypeID, SlotNumber = ci.SlotNumber });
                    }
                }
            }
            
            return Workspace;
        }
    }
}
