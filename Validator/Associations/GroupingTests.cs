using DataModels.Resource;
using DataModels.User;
using System;
using System.Collections.ObjectModel;
using Workshop.Associations;
using Xunit;

namespace Validator.Grouping
{
    public class GroupingTests
    {
        [Fact]
        public void Groups_QuasarModTypeGroupsAreProperlyGenerated()
        {
            ObservableCollection<QuasarModType> QuasarModTypes = new()
            {
                new()
                {
                    GameElementFamilyID = 0,
                    GroupName = "First Group",
                    Name = "First Type",
                    ID = 0,
                    IgnoreGameElementFamily = false,
                    ExternalFolderPath = "",
                    IsExternal = false,
                    NoGameElement = false,
                    SlotCount = 8,
                    QuasarModTypeFileDefinitions = new(),
                    TypePriority = 10
                },
                new()
                {
                    GameElementFamilyID = 0,
                    GroupName = "First Group",
                    Name = "Second Type",
                    ID = 1,
                    IgnoreGameElementFamily = false,
                    ExternalFolderPath = "",
                    IsExternal = false,
                    NoGameElement = false,
                    SlotCount = 8,
                    QuasarModTypeFileDefinitions = new(),
                    TypePriority = 20
                },
                new()
                {
                    GameElementFamilyID = 0,
                    GroupName = "First Group",
                    Name = "Third Type",
                    ID = 2,
                    IgnoreGameElementFamily = false,
                    ExternalFolderPath = "",
                    IsExternal = false,
                    NoGameElement = false,
                    SlotCount = 8,
                    QuasarModTypeFileDefinitions = new(),
                    TypePriority = 30
                },
                new()
                {
                    GameElementFamilyID = 1,
                    GroupName = "Third Group",
                    Name = "Fourth Type",
                    ID = 3,
                    IgnoreGameElementFamily = false,
                    ExternalFolderPath = "",
                    IsExternal = false,
                    NoGameElement = false,
                    SlotCount = 8,
                    QuasarModTypeFileDefinitions = new(),
                    TypePriority = 200
                }
            };

            QuasarModTypeGroup Group = Grouper.GetQuasarModTypeGroups(0, QuasarModTypes);
            QuasarModTypeGroup Group2 = Grouper.GetQuasarModTypeGroups(1, QuasarModTypes);

            //The first one should contain these three elements
            Assert.True(Group.QuasarModTypeCollection.Count == 3);
            Assert.Contains(Group.QuasarModTypeCollection, _quasar_mod_type => _quasar_mod_type.Name == "First Type");
            Assert.Contains(Group.QuasarModTypeCollection, _quasar_mod_type => _quasar_mod_type.Name == "Second Type");
            Assert.Contains(Group.QuasarModTypeCollection, _quasar_mod_type => _quasar_mod_type.Name == "Third Type");

            //The second one should only contain this one element
            Assert.True(Group2.QuasarModTypeCollection.Count == 1);
            Assert.Contains(Group2.QuasarModTypeCollection, _quasar_mod_type => _quasar_mod_type.Name == "Fourth Type");


        }

        [Fact]
        public void Slots_QuasarModType_AssignmentContentsAreProperlyGenerated()
        {
            LibraryItem LibraryItem = new()
            {
                Editing = false,
                Guid = Guid.NewGuid(),
                Name = "Test Library Item 1"
            };

            ObservableCollection<ContentItem> TestContentItems = new()
            {
                //Right Library Item, any type
                new()
                {
                    GameElementID = 0,
                    QuasarModTypeID = 0,
                    LibraryItemGuid = LibraryItem.Guid,
                    Guid = Guid.NewGuid(),
                    Name = "First Content",
                    SlotNumber = 0,
                    ScanFiles = new()
                },
                //Right Library Item, Another Game Element, but the same Type
                new()
                {
                    GameElementID = 1,
                    QuasarModTypeID = 0,
                    LibraryItemGuid = LibraryItem.Guid,
                    Guid = Guid.NewGuid(),
                    Name = "Second Content",
                    SlotNumber = 0,
                    ScanFiles = new()
                },
                //Right Library Item, another Game Element and Type
                new()
                {
                    GameElementID = 2,
                    QuasarModTypeID = 1,
                    LibraryItemGuid = LibraryItem.Guid,
                    Guid = Guid.NewGuid(),
                    Name = "Third Content",
                    SlotNumber = 1,
                    ScanFiles = new()
                },
                //Wrong Library Item ID
                new()
                {
                    GameElementID = 2,
                    QuasarModTypeID = 1,
                    LibraryItemGuid = Guid.NewGuid(),
                    Guid = Guid.NewGuid(),
                    Name = "Fourth Content",
                    SlotNumber = 5,
                    ScanFiles = new()
                }
            };

            ObservableCollection<AssignmentContent> AssignmentContents = Grouper.GetAssignmentContents(TestContentItems, false);

            //There should only be the first three contents
            Assert.True(AssignmentContents.Count == 3);

            //There should not be any grouped contents
            Assert.DoesNotContain(AssignmentContents, ac => ac.Single == false);
        }

        [Fact]
        public void Slots_QuasarModTypeGroup_AssignmentContentsAreProperlyGenerated()
        {
            LibraryItem LibraryItem = new()
            {
                Editing = false,
                Guid = Guid.NewGuid(),
                Name = "Test Library Item 1"
            };

            ObservableCollection<ContentItem> TestContentItems = new()
            {
                //Right Library Item, First Group & Root
                new()
                {
                    GameElementID = 0,
                    QuasarModTypeID = 0,
                    LibraryItemGuid = LibraryItem.Guid,
                    Guid = Guid.NewGuid(),
                    Name = "First Content",
                    GroupName = "Test Group 1",
                    SlotNumber = 0,
                    OriginalSlotNumber = 0,
                    ScanFiles = new()
                    {
                        new ScanFile()
                        {
                            RootPath = "firstRoot"
                        }
                    }
                },
                //Right Library Item, First Group & Root but different QMT ID
                new()
                {
                    GameElementID = 0,
                    QuasarModTypeID = 1,
                    LibraryItemGuid = LibraryItem.Guid,
                    Guid = Guid.NewGuid(),
                    Name = "First Content",
                    GroupName = "Test Group 1",
                    SlotNumber = 0,
                    OriginalSlotNumber = 0,
                    ScanFiles = new()
                    {
                        new ScanFile()
                        {
                            RootPath = "firstRoot"
                        }
                    }
                },
                //Right Library Item, First Group, but different Root
                new()
                {
                    GameElementID = 1,
                    QuasarModTypeID = 0,
                    LibraryItemGuid = LibraryItem.Guid,
                    Guid = Guid.NewGuid(),
                    Name = "Second Content",
                    GroupName = "Test Group 1",
                    SlotNumber = 0,
                    OriginalSlotNumber = 0,
                    ScanFiles = new()
                    {
                        new ScanFile()
                        {
                            RootPath = "SecondRoot"
                        }
                    }
                },
                //Right Library Item, Second Group, same root
                new()
                {
                    GameElementID = 2,
                    QuasarModTypeID = 1,
                    LibraryItemGuid = LibraryItem.Guid,
                    Guid = Guid.NewGuid(),
                    Name = "Third Content",
                    GroupName = "Test Group 2",
                    SlotNumber = 1,
                    OriginalSlotNumber = 1,
                    ScanFiles = new()
                    {
                        new ScanFile()
                        {
                            RootPath = "firstRoot"
                        }
                    }
                },//Right Library Item, Second Group, Second root
                new()
                {
                    GameElementID = 2,
                    QuasarModTypeID = 1,
                    LibraryItemGuid = LibraryItem.Guid,
                    Guid = Guid.NewGuid(),
                    Name = "Third Content",
                    GroupName = "Test Group 2",
                    SlotNumber = 1,
                    OriginalSlotNumber = 1,
                    ScanFiles = new()
                    {
                        new ScanFile()
                        {
                            RootPath = "SecondRoot"
                        }
                    }
                },
                //Wrong Library Item ID
                new()
                {
                    GameElementID = 2,
                    QuasarModTypeID = 1,
                    LibraryItemGuid = Guid.NewGuid(),
                    Guid = Guid.NewGuid(),
                    Name = "Fourth Content",
                    GroupName = "Test Group 2",
                    SlotNumber = 5,
                    OriginalSlotNumber = 1,
                    ScanFiles = new()
                    {
                        new ScanFile()
                        {
                            RootPath = "firstRoot"
                        }
                    }
                },//Right Library Item, First Group & Root but different slot
                new()
                {
                    GameElementID = 0,
                    QuasarModTypeID = 0,
                    LibraryItemGuid = LibraryItem.Guid,
                    Guid = Guid.NewGuid(),
                    Name = "First Content",
                    GroupName = "Test Group 1",
                    SlotNumber = 1,
                    OriginalSlotNumber = 1,
                    ScanFiles = new()
                    {
                        new ScanFile()
                        {
                            RootPath = "firstRoot"
                        }
                    }
                },
                //Right Library Item, First Group & Root but different QMT ID & slot
                new()
                {
                    GameElementID = 0,
                    QuasarModTypeID = 1,
                    LibraryItemGuid = LibraryItem.Guid,
                    Guid = Guid.NewGuid(),
                    Name = "First Content",
                    GroupName = "Test Group 1",
                    SlotNumber = 1,
                    OriginalSlotNumber = 1,
                    ScanFiles = new()
                    {
                        new ScanFile()
                        {
                            RootPath = "firstRoot"
                        }
                    }
                },
            };

            ObservableCollection<AssignmentContent> AssignmentContents = Grouper.GetAssignmentContents(TestContentItems, true);

            Assert.True(AssignmentContents.Count == 5);
            Assert.True(AssignmentContents[0].AssignmentContentItems.Count == 2);
            Assert.True(AssignmentContents[1].AssignmentContentItems.Count == 1);
            Assert.True(AssignmentContents[2].AssignmentContentItems.Count == 1);
            Assert.True(AssignmentContents[3].AssignmentContentItems.Count == 1);
            Assert.True(AssignmentContents[4].AssignmentContentItems.Count == 2);
        }
    }
}
