using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quasar.Data.V1;
using DataModels.Common;
using DataModels.User;
using DataModels.Resource;
using Quasar.Helpers.Json;

namespace Quasar.Data.Converter
{
    static class V1toV2Converter
    {
        #region Dev Converters
        public static void ProcessGameFile(ObservableCollection<V1.Game> V1Games, ObservableCollection<V1.GameData> V1GameDatas)
        {
            //New format instancing
            ObservableCollection<V2.Game> Games = new ObservableCollection<V2.Game>();
            V2.Game game = new V2.Game();

            //Foreach source game
            foreach(V1.Game v1game in V1Games)
            {
                if(v1game.ID == 1)
                {
                    game.ID = 1;
                    game.APIGameName = v1game.GameName;
                    game.Name = v1game.Name;
                    game.GameAPICategories = new ObservableCollection<V2.GameAPICategory>();
                    game.GameElementFamilies = new ObservableCollection<GameElementFamily>();

                    //Parsing game's GameAPICategories
                    foreach(V1.GameModType gmt in v1game.GameModTypes)
                    {
                        if(gmt.ID != -1)
                        {
                            V2.GameAPICategory GameAPICategory = new V2.GameAPICategory()
                            {
                                APICategoryName = gmt.APIName,
                                ID = gmt.ID,
                                LibraryFolderName = gmt.LibraryFolder,
                                Name = gmt.Name,
                                GameAPISubCategories = new ObservableCollection<GameAPISubCategory>()
                            };

                            //Parsing Categories' SubCategories
                            foreach(Category cat in gmt.Categories)
                            {
                                if(cat.ID != -1 && cat.ID != 0)
                                {
                                    V2.GameAPISubCategory gameAPISubCategory = new V2.GameAPISubCategory()
                                    {
                                        ID = cat.ID,
                                        APISubCategoryID = cat.APICategory,
                                        APISubCategoryName = cat.Name
                                    };
                                    GameAPICategory.GameAPISubCategories.Add(gameAPISubCategory);
                                }
                            }

                            game.GameAPICategories.Add(GameAPICategory);
                        }
                        
                    }

                    //Parsing game's GameData
                    foreach(V1.GameData gd in V1GameDatas)
                    {
                        if(gd.GameID == 1)
                        {
                            //Parsing Game Element Families
                            foreach(GameDataCategory gdc in gd.Categories)
                            {
                                GameElementFamily Family = new GameElementFamily()
                                {
                                    ID = gdc.ID,
                                    Name = gdc.Name,
                                    GameElements = new ObservableCollection<GameElement>()
                                };

                                //Parsing Game Elements
                                foreach(GameDataItem gdi in gdc.Items)
                                {
                                    GameElement Element = new GameElement()
                                    {
                                        ID = gdi.ID,
                                        Name = gdi.Name,
                                        GameFolderName = gdi.Attributes[0].Value,
                                        isDLC = gdi.Attributes[1].Value.Contains("True")
                                    };
                                    Family.GameElements.Add(Element);
                                }

                                game.GameElementFamilies.Add(Family);
                            }

                        }
                    }
                }
            }

            Games.Add(game);
            JSonHelper.SaveGamesFile(Games);
        }
        public static void ProcessQuasarModTypes(ObservableCollection<InternalModType> V1InternalModTypes)
        {
            ObservableCollection<QuasarModType> QuasarModTypes = new ObservableCollection<QuasarModType>();

            foreach(InternalModType imt in V1InternalModTypes)
            {
                QuasarModType qmt = new QuasarModType()
                {
                    ID = imt.ID,
                    Name = imt.Name,
                    GameElementFamilyID = imt.Association,
                    GroupName = imt.TypeGroup,
                    IgnoreGameElementFamily = imt.IgnoreableGameDataAssociation,
                    NoGameElement = imt.NoGameData,
                    IsExternal = imt.OutsideFolder,
                    ExternalFolderPath = imt.OutsideFolderPath,
                    SlotCount = imt.Slots,
                    QuasarModTypeFileDefinitions = new ObservableCollection<QuasarModTypeFileDefinition>()
                };

                foreach(InternalModTypeFile bf in imt.Files)
                {
                    QuasarModTypeFileDefinition fileDefinition = new QuasarModTypeFileDefinition()
                    {
                        ID = bf.ID,
                        SearchPath = bf.SourcePath,
                        SearchFileName = bf.SourceFile,
                        QuasarModTypeBuilderDefinitions = new ObservableCollection<QuasarModTypeBuilderDefinition>()
                    };
                    
                    foreach(BuilderFile f in bf.Files)
                    {
                        fileDefinition.QuasarModTypeBuilderDefinitions.Add(new QuasarModTypeBuilderDefinition()
                        {
                            ModLoaderID = f.BuilderID,
                            OutputFileName = f.File,
                            OutputPath = f.Path
                        });
                    }
                    qmt.QuasarModTypeFileDefinitions.Add(fileDefinition);
                }

                QuasarModTypes.Add(qmt);
            }

            JSonHelper.SaveQuasarModTypes(QuasarModTypes);
        }
        public static void ProcessModLoaders(ObservableCollection<V1.ModLoader> V1ModLoaders)
        {
            ObservableCollection<V2.ModLoader> V2ModLoaders = new ObservableCollection<V2.ModLoader>();
            foreach(V1.ModLoader ml in V1ModLoaders)
            {
                V2ModLoaders.Add(new V2.ModLoader()
                {
                    GameID = ml.GameID,
                    ID = ml.ModLoaderID,
                    Name = ml.Name,
                    WorkspacePath = ml.BasePath
                });
            }
            JSonHelper.SaveModLoaders(V2ModLoaders);
        }
        #endregion

        #region Update Converters
        public static void ProcessLibrary(ObservableCollection<LibraryMod> V11LibraryMods)
        {
            /*
            ObservableCollection<LibraryItem> V2Library = new ObservableCollection<LibraryItem>();

            foreach(LibraryMod lm in V11LibraryMods)
            {
                LibraryItem li = new LibraryItem()
                {
                    ID = lm.ID,
                    GameID = lm.GameID,
                    Name = lm.Name,
                    UpdateCount = lm.Updates,
                    GamebananaSubCategoryID = lm.APICategoryID,
                    GamebananaItemID = lm.ID,
                    Authors = new ObservableCollection<Author>()
                };
                
                foreach(string[] author in lm.Authors)
                {
                    Author au = new Author()
                    {
                        Name = author[0],
                        Role = author[1],
                        GamebananaAuthorID = int.Parse(author[2])
                    };
                    li.Authors.Add(au);
                }

                V2Library.Add(li);
            }

            JSonHelper.SaveLibrary(V2Library);*/
        }
        public static void ProcessWorkspace(ObservableCollection<V1.Workspace> V1Workspaces)
        {
            /*ObservableCollection<V2.Workspace> V2Workspaces = new ObservableCollection<V2.Workspace>();

            foreach(V1.Workspace ws in V1Workspaces)
            {
                V2.Workspace V2Workspace = new V2.Workspace()
                {
                    Name = ws.Name,
                    Guid = ws.ID,
                    Associations = new ObservableCollection<V2.Association>(),
                    BuildDate = ws.BuildDate
                };

                foreach(V1.Association ass in ws.Associations)
                {
                    V2.Association V2Association = new V2.Association()
                    {
                        ContentItemGuid = ass.ContentMappingID,
                        GameElementID = ass.GameDataItemID,
                        SlotNumber = ass.Slot,
                        QuasarModTypeID = ass.InternalModTypeID
                    };

                    V2Workspace.Associations.Add(V2Association);
                }

                V2Workspaces.Add(V2Workspace);
            }
            JSonHelper.SaveWorkspaces(V2Workspaces);*/
            
        }
        public static void ProcessContent(ObservableCollection<V1.ContentMapping> V1ContentMappings)
        {
            /*
            ObservableCollection<V2.ContentItem> V2ContentItems = new ObservableCollection<V2.ContentItem>();
            foreach (ContentMapping cm in V1ContentMappings)
            {
                ContentItem ci = new ContentItem()
                {
                    ID = cm.ID,
                    LibraryItemGuid = cm.ModID,
                    QuasarModTypeID = cm.InternalModType,
                    Name = cm.Name,
                    SlotNumber = cm.Slot,
                    GameElementID = cm.GameDataItemID
                };
                V2ContentItems.Add(ci);
            }

            JSonHelper.SaveContentItems(V2ContentItems);*/
        }
        #endregion
    }
}
