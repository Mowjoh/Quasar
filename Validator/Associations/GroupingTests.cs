using DataModels.Resource;
using DataModels.User;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.Associations;
using Xunit;

namespace Validator.Associations
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
                    GameElementFamilyID = 0,
                    GroupName = "Second Group",
                    Name = "Fourth Type",
                    ID = 3,
                    IgnoreGameElementFamily = false,
                    ExternalFolderPath = "",
                    IsExternal = false,
                    NoGameElement = false,
                    SlotCount = 8,
                    QuasarModTypeFileDefinitions = new(),
                    TypePriority = 100
                },
                new()
                {
                    GameElementFamilyID = 1,
                    GroupName = "Third Group",
                    Name = "Fifth Type",
                    ID = 4,
                    IgnoreGameElementFamily = false,
                    ExternalFolderPath = "",
                    IsExternal = false,
                    NoGameElement = false,
                    SlotCount = 8,
                    QuasarModTypeFileDefinitions = new(),
                    TypePriority = 200
                }
            };

            ObservableCollection<QuasarModTypeGroup> GroupedTypes = Grouper.GetQuasarModTypeGroups(0, QuasarModTypes);

            //There should only be two groups there
            Assert.True(GroupedTypes.Count == 2);

            //The first one should contain these three elements
            Assert.True(GroupedTypes[0].QuasarModTypeCollection.Count == 3);
            Assert.Contains(GroupedTypes[0].QuasarModTypeCollection, qmt => qmt.Name == "First Type");
            Assert.Contains(GroupedTypes[0].QuasarModTypeCollection, qmt => qmt.Name == "Second Type");
            Assert.Contains(GroupedTypes[0].QuasarModTypeCollection, qmt => qmt.Name == "Third Type");

            //The second one should only contain this one element
            Assert.True(GroupedTypes[1].QuasarModTypeCollection.Count == 1);
            Assert.Contains(GroupedTypes[1].QuasarModTypeCollection, qmt => qmt.Name == "Fourth Type");


        }

        [Fact]
        public void Slots_SingleType_SlotContentsAreProperlyGenerated()
        {
            ObservableCollection<ContentItem> TestContentItems = new()
            {
                new()
                {
                    GameElementID = 0,
                    QuasarModTypeID = 0,
                    LibraryItemGuid = Guid.NewGuid(),
                    Guid = Guid.NewGuid(),
                    Name = "First Content",
                    SlotNumber = 0,
                    ScanFiles = new()
                },
                new()
                {
                    GameElementID = 0,
                    QuasarModTypeID = 0,
                    LibraryItemGuid = Guid.NewGuid(),
                    Guid = Guid.NewGuid(),
                    Name = "Second Content",
                    SlotNumber = 0,
                    ScanFiles = new()
                },
                new()
                {
                    GameElementID = 0,
                    QuasarModTypeID = 0,
                    LibraryItemGuid = Guid.NewGuid(),
                    Guid = Guid.NewGuid(),
                    Name = "Third Content",
                    SlotNumber = 1,
                    ScanFiles = new()
                },
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
            QuasarModType TestQuasarModType = new()
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
            };

            GameElement TestGameElement = new()
            {
                FilterValue = "",
                GameFolderName = "",
                ID = 0,
                isDLC = false,
                Name = "A new Challenger"
            };

            ObservableCollection<SlotContent> SlotContents = Grouper.GetSlotContents(TestQuasarModType, TestContentItems, TestGameElement);

            Assert.True(SlotContents.Count == 3);
        }

        [Fact]
        public void Slots_QuasarModTypeGroup_SlotContentsAreProperlyGenerated()
        {
            ObservableCollection<ContentItem> TestContentItems = new()
            {
                new()
                {
                    GameElementID = 0,
                    QuasarModTypeID = 0,
                    LibraryItemGuid = Guid.NewGuid(),
                    Guid = Guid.NewGuid(),
                    Name = "First Content",
                    SlotNumber = 0,
                    ScanFiles = new()
                },
                new()
                {
                    GameElementID = 0,
                    QuasarModTypeID = 1,
                    LibraryItemGuid = Guid.NewGuid(),
                    Guid = Guid.NewGuid(),
                    Name = "Second Content",
                    SlotNumber = 0,
                    ScanFiles = new()
                },
                new()
                {
                    GameElementID = 0,
                    QuasarModTypeID = 0,
                    LibraryItemGuid = Guid.NewGuid(),
                    Guid = Guid.NewGuid(),
                    Name = "Third Content",
                    SlotNumber = 1,
                    ScanFiles = new()
                },
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

            QuasarModTypeGroup TestQuasarModTypeGroup = new()
            {
                Name = "Group Name",
                 QuasarModTypeCollection = new()
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
                         TypePriority = 10
                     }
                 }
            };

            QuasarModType TestQuasarModType = new()
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
            };

            GameElement TestGameElement = new()
            {
                FilterValue = "",
                GameFolderName = "",
                ID = 0,
                isDLC = false,
                Name = "A new Challenger"
            };

            ObservableCollection<SlotContent> SlotContents = Grouper.GetSlotContents(TestQuasarModType, TestContentItems, TestGameElement);

            Assert.True(SlotContents.Count == 3);
        }
    }
}
