﻿using DataModels.Resource;
using DataModels.User;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;

namespace Workshop.Assignments
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
            QuasarModTypeGroup Group = new()
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
        /// <param name="_content_items">List of all ContentItems to parse from</param>
        /// <param name="_grouped">If you want ContentItems grouped by ModType.Group</param>
        /// <returns>The generated list of Assignment Contents</returns>
        public static ObservableCollection<AssignmentContent> GetAssignmentContents(ObservableCollection<ContentItem> _content_items, bool _grouped)
        {
            if (!_grouped)
            {
                return new(GetSeparatedAssignmentContents(_content_items));
            }
            else
            {
                return new(GetGroupedAssignmentContents(_content_items));
            }
        }

        public static List<AssignmentContent> GetSeparatedAssignmentContents(ObservableCollection<ContentItem> _content_items)
        {
            List<AssignmentContent> Contents = new();

            //Adding each ContentItem Separately
            foreach (ContentItem MatchingContentItem in _content_items)
            {
                Contents.Add(new()
                {
                    AssignmentName = MatchingContentItem.Name,
                    AssignmentContentItems = new() { MatchingContentItem },
                    SlotNumber = MatchingContentItem.OriginalSlotNumber
                });
            }

            return Contents;
        }

        public static List<AssignmentContent> GetGroupedAssignmentContents(ObservableCollection<ContentItem> _content_items)
        {
            List<AssignmentContent> Contents = new();

            foreach (ContentItem MatchingContentItem in _content_items)
            {
                bool ContentAdded = false;

                //Looping through existing contents for matches
                foreach (AssignmentContent AssignmentContent in Contents)
                {
                    if (AssignmentContent.AssignmentContentItems[0].GroupName == MatchingContentItem.GroupName && 
                        AssignmentContent.AssignmentContentItems[0].ScanFiles[0].RootPath == MatchingContentItem.ScanFiles[0].RootPath && 
                        AssignmentContent.AssignmentContentItems[0].OriginalSlotNumber == MatchingContentItem.OriginalSlotNumber &&
                        AssignmentContent.AssignmentContentItems[0].LibraryItemGuid == MatchingContentItem.LibraryItemGuid)
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
                        SlotNumber = MatchingContentItem.OriginalSlotNumber
                    });
                }
            }

            return Contents;
        }
    }
}
