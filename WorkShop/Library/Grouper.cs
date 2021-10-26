using DataModels.Resource;
using DataModels.User;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshop.Library
{
    public static class Grouper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_QuasarModType"></param>
        /// <param name="AvailableQuasarModTypes"></param>
        /// <returns></returns>
        public static QuasarModTypeGroup GetQuasarModTypeGroup(QuasarModType _QuasarModType, ObservableCollection<QuasarModType> AvailableQuasarModTypes)
        {
            //Instanciating Group
            QuasarModTypeGroup QuasarModTypeGroup = new()
            {
                Name = _QuasarModType.GroupName,
                QuasarModTypeCollection = new()
            };

            //Adding matching types to the group
            foreach(QuasarModType q in AvailableQuasarModTypes.Where(qmt => qmt.GroupName == _QuasarModType.GroupName && qmt.GameElementFamilyID == _QuasarModType.GameElementFamilyID).ToArray())
            {
                QuasarModTypeGroup.QuasarModTypeCollection.Add(q);
            }

            return QuasarModTypeGroup;
        }

        public static List<ObservableCollection<ContentItem>> GetContentItemGroups(GameElement _AssociatedElement, ObservableCollection<ContentItem> _ContentItems)
        {
            List<ObservableCollection<ContentItem>> GroupedContents = new();



            return GroupedContents;
        }
    }
}
