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
            foreach(QuasarModType q in AvailableQuasarModTypes.Where(qmt => qmt.GameElementFamilyID == _GameElementFamilyID).ToArray())
            {
                if(QuasarModTypeGroups.Any(group => group.Name == q.GroupName))
                {
                    QuasarModTypeGroups.Single(group => group.Name == q.GroupName).QuasarModTypeCollection.Add(q);
                }
                else
                {
                    QuasarModTypeGroups.Add(new()
                    {
                        Name = q.GroupName,
                        QuasarModTypeCollection = new()
                        {
                            q
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

            return Contents;
        }
    }
}
