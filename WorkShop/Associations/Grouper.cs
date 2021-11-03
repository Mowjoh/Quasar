using DataModels.Resource;
using DataModels.User;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshop.Associations
{
    public static class Grouper
    {
        /// <summary>
        /// Provides the list of QuasarModTypeGroups associated to this Game Element Family
        /// </summary>
        /// <param name="_GameElementFamilyID">ID of the associated Game Element Family</param>
        /// <param name="AvailableQuasarModTypes">List of all Quasar Mod Types</param>
        /// <returns></returns>
        public static ObservableCollection<QuasarModTypeGroup> GetQuasarModTypeGroups(int _GameElementFamilyID, ObservableCollection<QuasarModType> AvailableQuasarModTypes)
        {
            //Instanciating Group collection
            ObservableCollection<QuasarModTypeGroup> QuasarModTypeGroups = new();


            //Adding matching types to the groups
            foreach(QuasarModType Type in AvailableQuasarModTypes.Where(qmt => qmt.GameElementFamilyID == _GameElementFamilyID).ToArray())
            {
                if(QuasarModTypeGroups.Any(group => group.Name == Type.GroupName))
                {
                    QuasarModTypeGroups.Single(group => group.Name == Type.GroupName).QuasarModTypeCollection.Add(Type);
                }
                else
                {
                    QuasarModTypeGroups.Add(new()
                    {
                        Name = Type.GroupName,
                        QuasarModTypeCollection = new()
                        {
                            Type
                        }
                    });
                }
            }

            return QuasarModTypeGroups;
        }

        /// <summary>
        /// Provides SlotContents for a specific QuasarModType and GameElement
        /// </summary>
        /// <param name="_QuasarModType">Quasar Mod Type used to filter ContentItems</param>
        /// <param name="_ContentItems">List of all ContentItems to parse from</param>
        /// <param name="_AssociatedElement">Associated Game Element to filter ContentItems</param>
        /// <returns>The generated list of Slot Contents</returns>
        public static ObservableCollection<SlotContent> GetSlotContents(QuasarModType _QuasarModType, ObservableCollection<ContentItem> _ContentItems, GameElement _AssociatedElement)
        {
            ObservableCollection<SlotContent> Contents = new();

            foreach (ContentItem MatchingContentItem in _ContentItems.Where(ci => ci.GameElementID == _AssociatedElement.ID && ci.QuasarModTypeID == _QuasarModType.ID).ToList())
            {
                Contents.Add(new()
                {
                    SlotName = MatchingContentItem.Name,
                    SlotContentItems = new()
                    {
                        MatchingContentItem
                    }
                });
            }

            return Contents;
        }

        /// <summary>
        /// Provides SlotContents for a specific QuasarModTypeGroup and GameElement
        /// </summary>
        /// <param name="_TypesGroup">Quasar Mod Type Group used to filter ContentItems</param>
        /// <param name="_ContentItems">List of all ContentItems to parse from</param>
        /// <param name="_AssociatedElement">Associated Game Element to filter ContentItems</param>
        /// <returns>The generated list of Slot Contents</returns>
        public static ObservableCollection<SlotContent> GetSlotContents(QuasarModTypeGroup _TypesGroup, ObservableCollection<ContentItem> _ContentItems, GameElement _AssociatedElement)
        {
            ObservableCollection<SlotContent> Contents = new();

            //Listing all Content Items that match the QuasarModTypeGroup and Associated Game Element
            List<ContentItem> MatchingContentItems = _ContentItems.Where(ci => ci.GameElementID == _AssociatedElement.ID && _TypesGroup.QuasarModTypeCollection.Any(t => t.ID == ci.QuasarModTypeID)).ToList();
            
            //Processing SlotContents for each slot

            foreach (ContentItem MatchingContentItem in MatchingContentItems)
            {
                //If there is a SlotContent with the same Slot Number
                if(Contents.Any(s => s.SlotNumber == MatchingContentItem.SlotNumber))
                {
                    //If the Origin Path is the same
                    
                }
                Contents.Add(new()
                {
                    SlotName = MatchingContentItem.Name,
                    SlotContentItems = new()
                    {
                        MatchingContentItem
                    }
                });
            }

            return Contents;
        }
    }
}
