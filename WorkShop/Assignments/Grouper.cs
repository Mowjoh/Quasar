using DataModels.Resource;
using DataModels.User;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;

namespace Workshop.Associations
{
    public static class Grouper
    {
        /// <summary>
        /// Provides the list of QuasarModTypeGroups associated to this Game Element Family
        /// </summary>
        /// <param name="_game_element_family_id">ID of the associated Game Element Family</param>
        /// <param name="_available_quasar_mod_types">List of all Quasar Mod Types</param>
        /// <returns></returns>
        public static QuasarModTypeGroup GetQuasarModTypeGroups(int _game_element_family_id, ObservableCollection<QuasarModType> _available_quasar_mod_types)
        {
            //Instantiating Group collection
            QuasarModTypeGroup Group = new();
            Group = new()
            {
                QuasarModTypeCollection = new()
            };

            //Adding matching types to the groups
            foreach (QuasarModType Type in _available_quasar_mod_types.Where(_quasar_mod_type => _quasar_mod_type.GameElementFamilyID == _game_element_family_id).ToArray())
            {
                Group.Name = Type.GroupName;
                Group.QuasarModTypeCollection.Add(Type);
            }

            return Group;
        }

        /// <summary>
        /// Provides AssignmentContents for a specific QuasarModType and GameElement
        /// </summary>
        /// <param name="_quasar_mod_type">Quasar Mod Type used to filter ContentItems</param>
        /// <param name="_content_items">List of all ContentItems to parse from</param>
        /// <param name="_associated_element">Associated Game Element to filter ContentItems</param>
        /// <returns>The generated list of Assignment Contents</returns>
        public static ObservableCollection<AssignmentContent> GetAssignmentContents(LibraryItem _library_item, ObservableCollection<ContentItem> _content_items, bool _grouped)
        {
            ObservableCollection<AssignmentContent> Contents = new();

            if (!_grouped)
            {
                //Adding each ContentItem Separately
                foreach (ContentItem MatchingContentItem in _content_items.Where(_content_item => _content_item.LibraryItemGuid == _library_item.Guid))
                {
                    Contents.Add(new()
                    {
                        AssignmentName = MatchingContentItem.Name,
                        AssignmentContentItems = new() { MatchingContentItem },
                        SlotNumber = MatchingContentItem.SlotNumber
                    });
                }
            }
            else
            {
                foreach (ContentItem MatchingContentItem in _content_items.Where(_content_item => _content_item.LibraryItemGuid == _library_item.Guid))
                {
                    bool ContentAdded = false;

                    //Looping through existing contents for matchs
                    foreach (AssignmentContent AssignmentContent in Contents)
                    {
                        if (AssignmentContent.AssignmentContentItems[0].GroupName == MatchingContentItem.GroupName && AssignmentContent.AssignmentContentItems[0].ScanFiles[0].RootPath ==
                            MatchingContentItem.ScanFiles[0].RootPath && AssignmentContent.AssignmentContentItems[0].SlotNumber == MatchingContentItem.SlotNumber)
                        {
                            AssignmentContent.AssignmentContentItems.Add(MatchingContentItem);
                            ContentAdded = true;
                        }
                    }

                    //If no match was found
                    if (!ContentAdded)
                    {
                        //Creating a new item
                        Contents.Add(new()
                        {
                            AssignmentName = MatchingContentItem.Name,
                            AssignmentContentItems = new() { MatchingContentItem },
                            SlotNumber = MatchingContentItem.SlotNumber
                        });
                    }
                }
            }
            

            return Contents;
        }
    }
}
