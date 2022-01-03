using DataModels.User;
using System;
using System.Collections.ObjectModel;
using Workshop.Associations;
using Xunit;

namespace Validator.Associations
{
    public class SlottingTests
    {
        [Fact]
        public void Slotting_ContentItemIsSlottedOnEmptySlot()
        {
            Workspace TestWorkspace = new()
            {
                Name = "Test Workspace",
                Associations = new(),
                Guid = Guid.NewGuid()
                
            };

            ContentItem TestContentItem = new()
            {
                Guid = Guid.NewGuid(),
                GameElementID = 0,
                LibraryItemGuid = Guid.NewGuid(),
                Name = "Test ContentItem",
                SlotNumber = 0,
                QuasarModTypeID = 0,
                ScanFiles = new()
            };

            //Slotting to the ContentItem's Detected Slot
            TestWorkspace = Slotter.SlotContent(TestContentItem, TestContentItem.SlotNumber, TestWorkspace);

            //Slotting to a specific Slot
            TestWorkspace = Slotter.SlotContent(TestContentItem, 2, TestWorkspace);

            Assert.True(TestWorkspace.Associations.Count == 2);
            Assert.Contains(TestWorkspace.Associations, ass => ass.SlotNumber == TestContentItem.SlotNumber && ass.GameElementID == TestContentItem.QuasarModTypeID && ass.QuasarModTypeID == TestContentItem.QuasarModTypeID && ass.ContentItemGuid == TestContentItem.Guid);
            Assert.Contains(TestWorkspace.Associations, ass => ass.SlotNumber == 2 && ass.GameElementID == TestContentItem.QuasarModTypeID && ass.QuasarModTypeID == TestContentItem.QuasarModTypeID && ass.ContentItemGuid == TestContentItem.Guid);
            
        }

        [Fact]
        public void Slotting_ContentItemIsSlottedOnUsedSlot()
        {
            ContentItem TestContentItem = new()
            {
                Guid = Guid.NewGuid(),
                GameElementID = 0,
                LibraryItemGuid = Guid.NewGuid(),
                Name = "Test ContentItem",
                SlotNumber = 0,
                QuasarModTypeID = 0,
                ScanFiles = new()
            };

            ContentItem TestUsedContentItem = new()
            {
                Guid = Guid.NewGuid(),
                GameElementID = 0,
                LibraryItemGuid = Guid.NewGuid(),
                Name = "Test Used ContentItem",
                SlotNumber = 0,
                QuasarModTypeID = 0,
                ScanFiles = new()
            };

            Workspace TestWorkspace = new()
            {
                Name = "Test Workspace",
                Associations = new()
                {
                    new()
                    {
                        ContentItemGuid = TestUsedContentItem.Guid,
                        GameElementID = TestUsedContentItem.GameElementID,
                        SlotNumber = TestUsedContentItem.SlotNumber,
                        QuasarModTypeID = TestUsedContentItem.QuasarModTypeID
                    },
                    new()
                    {
                        ContentItemGuid = TestUsedContentItem.Guid,
                        GameElementID = TestUsedContentItem.GameElementID,
                        SlotNumber = 3,
                        QuasarModTypeID = TestUsedContentItem.QuasarModTypeID
                    }
                },
                Guid = Guid.NewGuid()
            };

            //Slotting The Test ContentItem to the ContentItem's Detected Slot
            TestWorkspace = Slotter.SlotContent(TestContentItem, TestContentItem.SlotNumber, TestWorkspace);

            //Slotting The Test ContentItem to the same specific Slot
            TestWorkspace = Slotter.SlotContent(TestContentItem, 3, TestWorkspace);


            //The Workspace should only include the two new associations
            Assert.True(TestWorkspace.Associations.Count == 2);
            Assert.Contains(TestWorkspace.Associations, ass => ass.SlotNumber == TestContentItem.SlotNumber && ass.GameElementID == TestContentItem.QuasarModTypeID && ass.QuasarModTypeID == TestContentItem.QuasarModTypeID && ass.ContentItemGuid == TestContentItem.Guid);
            Assert.Contains(TestWorkspace.Associations, ass => ass.SlotNumber == 3 && ass.GameElementID == TestContentItem.QuasarModTypeID && ass.QuasarModTypeID == TestContentItem.QuasarModTypeID && ass.ContentItemGuid == TestContentItem.Guid);
            
            Assert.DoesNotContain(TestWorkspace.Associations, ass => ass.ContentItemGuid == TestUsedContentItem.Guid);

        }

        [Fact]
        public void Slotting_ContentItemsAreSlottedOnEmptySlots()
        {
            Workspace TestWorkspace = new()
            {
                Name = "Test Workspace",
                Associations = new(),
                Guid = Guid.NewGuid()

            };

            ContentItem TestContentItem = new()
            {
                Guid = Guid.NewGuid(),
                GameElementID = 0,
                LibraryItemGuid = Guid.NewGuid(),
                Name = "Test ContentItem",
                SlotNumber = 0,
                QuasarModTypeID = 0,
                ScanFiles = new()
            };

            ContentItem TestContentItem2 = new()
            {
                Guid = Guid.NewGuid(),
                GameElementID = 0,
                LibraryItemGuid = Guid.NewGuid(),
                Name = "Test ContentItem 2",
                SlotNumber = 0,
                QuasarModTypeID = 1,
                ScanFiles = new()
            };

            ContentItem TestContentItem3 = new()
            {
                Guid = Guid.NewGuid(),
                GameElementID = 0,
                LibraryItemGuid = Guid.NewGuid(),
                Name = "Test ContentItem 2",
                SlotNumber = 0,
                QuasarModTypeID = 2,
                ScanFiles = new()
            };

            ObservableCollection<ContentItem> TestCollection = new()
            {
                TestContentItem,
                TestContentItem2,
                TestContentItem3
            };

            TestWorkspace = Slotter.SlotMultipleContents(TestCollection, 0, TestWorkspace);

            //The Workspace should only contain the three added associations
            Assert.True(TestWorkspace.Associations.Count == 3);
            Assert.Contains(TestWorkspace.Associations, ass => ass.ContentItemGuid == TestContentItem.Guid && ass.GameElementID == TestContentItem.GameElementID && ass.QuasarModTypeID == TestContentItem.QuasarModTypeID && ass.SlotNumber == 0);
            Assert.Contains(TestWorkspace.Associations, ass => ass.ContentItemGuid == TestContentItem2.Guid && ass.GameElementID == TestContentItem2.GameElementID && ass.QuasarModTypeID == TestContentItem2.QuasarModTypeID && ass.SlotNumber == 0);
            Assert.Contains(TestWorkspace.Associations, ass => ass.ContentItemGuid == TestContentItem3.Guid && ass.GameElementID == TestContentItem3.GameElementID && ass.QuasarModTypeID == TestContentItem3.QuasarModTypeID && ass.SlotNumber == 0);
        }

        [Fact]
        public void Slotting_ContentItemsAreSlottedOnPartiallyUsedSlots()
        {
            ContentItem TestContentItem = new()
            {
                Guid = Guid.NewGuid(),
                GameElementID = 0,
                LibraryItemGuid = Guid.NewGuid(),
                Name = "Test ContentItem",
                SlotNumber = 0,
                QuasarModTypeID = 0,
                ScanFiles = new()
            };

            ContentItem TestUsedContentItem = new()
            {
                Guid = Guid.NewGuid(),
                GameElementID = 0,
                LibraryItemGuid = Guid.NewGuid(),
                Name = "Test Used ContentItem",
                SlotNumber = 0,
                QuasarModTypeID = 0,
                ScanFiles = new()
            };

            ContentItem TestContentItem2 = new()
            {
                Guid = Guid.NewGuid(),
                GameElementID = 0,
                LibraryItemGuid = Guid.NewGuid(),
                Name = "Test ContentItem 2",
                SlotNumber = 0,
                QuasarModTypeID = 1,
                ScanFiles = new()
            };

            ContentItem TestUsedContentItem2 = new()
            {
                Guid = Guid.NewGuid(),
                GameElementID = 0,
                LibraryItemGuid = Guid.NewGuid(),
                Name = "Test Used ContentItem 2",
                SlotNumber = 0,
                QuasarModTypeID = 1,
                ScanFiles = new()
            };

            ContentItem TestContentItem3 = new()
            {
                Guid = Guid.NewGuid(),
                GameElementID = 0,
                LibraryItemGuid = Guid.NewGuid(),
                Name = "Test ContentItem 2",
                SlotNumber = 0,
                QuasarModTypeID = 2,
                ScanFiles = new()
            };

            Workspace TestWorkspace = new()
            {
                Name = "Test Workspace",
                Associations = new()
                {
                    new()
                    {
                        ContentItemGuid = TestUsedContentItem.Guid, 
                        GameElementID = TestUsedContentItem.GameElementID,
                        SlotNumber = 0,
                        QuasarModTypeID = TestUsedContentItem.QuasarModTypeID
                    },
                    new()
                    {
                        ContentItemGuid = TestUsedContentItem2.Guid,
                        GameElementID = TestUsedContentItem2.GameElementID,
                        SlotNumber = 0,
                        QuasarModTypeID = TestUsedContentItem2.QuasarModTypeID
                    }
                },
                Guid = Guid.NewGuid()

            };

            ObservableCollection<ContentItem> TestCollection = new()
            {
                TestContentItem,
                TestContentItem2,
                TestContentItem3
            };

            TestWorkspace = Slotter.SlotMultipleContents(TestCollection, 0, TestWorkspace);


            //The workspace should only contain the unused associations
            Assert.True(TestWorkspace.Associations.Count == 3);
            Assert.Contains(TestWorkspace.Associations, ass => ass.ContentItemGuid == TestContentItem.Guid && ass.GameElementID == TestContentItem.GameElementID && ass.QuasarModTypeID == TestContentItem.QuasarModTypeID && ass.SlotNumber == 0);
            Assert.Contains(TestWorkspace.Associations, ass => ass.ContentItemGuid == TestContentItem2.Guid && ass.GameElementID == TestContentItem2.GameElementID && ass.QuasarModTypeID == TestContentItem2.QuasarModTypeID && ass.SlotNumber == 0);
            Assert.Contains(TestWorkspace.Associations, ass => ass.ContentItemGuid == TestContentItem3.Guid && ass.GameElementID == TestContentItem3.GameElementID && ass.QuasarModTypeID == TestContentItem3.QuasarModTypeID && ass.SlotNumber == 0);

            Assert.DoesNotContain(TestWorkspace.Associations, ass => ass.ContentItemGuid == TestUsedContentItem.Guid);
            Assert.DoesNotContain(TestWorkspace.Associations, ass => ass.ContentItemGuid == TestUsedContentItem2.Guid);

        }

        [Fact]
        public void Slotting_ContentItemsAreSlottedOnUsedSlots()
        {
            ContentItem TestContentItem = new()
            {
                Guid = Guid.NewGuid(),
                GameElementID = 0,
                LibraryItemGuid = Guid.NewGuid(),
                Name = "Test ContentItem",
                SlotNumber = 0,
                QuasarModTypeID = 0,
                ScanFiles = new()
            };

            ContentItem TestUsedContentItem = new()
            {
                Guid = Guid.NewGuid(),
                GameElementID = 0,
                LibraryItemGuid = Guid.NewGuid(),
                Name = "Test Used ContentItem",
                SlotNumber = 0,
                QuasarModTypeID = 0,
                ScanFiles = new()
            };

            ContentItem TestContentItem2 = new()
            {
                Guid = Guid.NewGuid(),
                GameElementID = 0,
                LibraryItemGuid = Guid.NewGuid(),
                Name = "Test ContentItem 2",
                SlotNumber = 0,
                QuasarModTypeID = 1,
                ScanFiles = new()
            };

            ContentItem TestUsedContentItem2 = new()
            {
                Guid = Guid.NewGuid(),
                GameElementID = 0,
                LibraryItemGuid = Guid.NewGuid(),
                Name = "Test Used ContentItem 2",
                SlotNumber = 0,
                QuasarModTypeID = 1,
                ScanFiles = new()
            };

            ContentItem TestContentItem3 = new()
            {
                Guid = Guid.NewGuid(),
                GameElementID = 0,
                LibraryItemGuid = Guid.NewGuid(),
                Name = "Test ContentItem 2",
                SlotNumber = 0,
                QuasarModTypeID = 2,
                ScanFiles = new()
            };

            ContentItem TestUsedContentItem3 = new()
            {
                Guid = Guid.NewGuid(),
                GameElementID = 0,
                LibraryItemGuid = Guid.NewGuid(),
                Name = "Test Used ContentItem 3",
                SlotNumber = 0,
                QuasarModTypeID = 2,
                ScanFiles = new()
            };

            Workspace TestWorkspace = new()
            {
                Name = "Test Workspace",
                Associations = new()
                {
                    new()
                    {
                        ContentItemGuid = TestUsedContentItem.Guid,
                        GameElementID = TestUsedContentItem.GameElementID,
                        SlotNumber = 0,
                        QuasarModTypeID = TestUsedContentItem.QuasarModTypeID
                    },
                    new()
                    {
                        ContentItemGuid = TestUsedContentItem2.Guid,
                        GameElementID = TestUsedContentItem2.GameElementID,
                        SlotNumber = 0,
                        QuasarModTypeID = TestUsedContentItem2.QuasarModTypeID
                    },
                    new()
                    {
                        ContentItemGuid = TestUsedContentItem3.Guid,
                        GameElementID = TestUsedContentItem3.GameElementID,
                        SlotNumber = 0,
                        QuasarModTypeID = TestUsedContentItem3.QuasarModTypeID
                    }
                },
                Guid = Guid.NewGuid()

            };

            ObservableCollection<ContentItem> TestCollection = new()
            {
                TestContentItem,
                TestContentItem2,
                TestContentItem3
            };

            TestWorkspace = Slotter.SlotMultipleContents(TestCollection, 0, TestWorkspace);


            //The workspace should only contain the unused associations
            Assert.True(TestWorkspace.Associations.Count == 3);
            Assert.Contains(TestWorkspace.Associations, ass => ass.ContentItemGuid == TestContentItem.Guid && ass.GameElementID == TestContentItem.GameElementID && ass.QuasarModTypeID == TestContentItem.QuasarModTypeID && ass.SlotNumber == 0);
            Assert.Contains(TestWorkspace.Associations, ass => ass.ContentItemGuid == TestContentItem2.Guid && ass.GameElementID == TestContentItem2.GameElementID && ass.QuasarModTypeID == TestContentItem2.QuasarModTypeID && ass.SlotNumber == 0);
            Assert.Contains(TestWorkspace.Associations, ass => ass.ContentItemGuid == TestContentItem3.Guid && ass.GameElementID == TestContentItem3.GameElementID && ass.QuasarModTypeID == TestContentItem3.QuasarModTypeID && ass.SlotNumber == 0);

            Assert.DoesNotContain(TestWorkspace.Associations, ass => ass.ContentItemGuid == TestUsedContentItem.Guid);
            Assert.DoesNotContain(TestWorkspace.Associations, ass => ass.ContentItemGuid == TestUsedContentItem2.Guid);
            Assert.DoesNotContain(TestWorkspace.Associations, ass => ass.ContentItemGuid == TestUsedContentItem3.Guid);
        }

    }
}
