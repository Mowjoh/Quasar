using DataModels.Resource;
using DataModels.User;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Workshop.Associations
{
    public static class Slotter
    {
        /// <summary>
        /// Slots Multiple Content Items in the Workspace at the selected Slot
        /// </summary>
        /// <param name="_content_items">List of ContentItems to slot</param>
        /// <param name="_slot_number">Number of the slot to put them in</param>
        /// <param name="_workspace">Workspace to slot them into</param>
        /// <returns>Edited Workspace</returns>
        public static Workspace SlotMultipleContents(ObservableCollection<ContentItem> _content_items, int _slot_number, Workspace _workspace)
        {
            foreach (ContentItem ContentItem in _content_items)
            {
                _workspace = SlotContent(ContentItem, _slot_number, _workspace);
            }

            return _workspace;
        }

        /// <summary>
        /// Slots a single Content Item in the workspace at the selected Slot
        /// </summary>
        /// <param name="_content_item"ContentItem to slot</param>
        /// <param name="_slot_number">Number of the slot to put them in</param>
        /// <param name="_workspace">Workspace to slot them into</param>
        /// <returns>Edited Workspace</returns>
        public static Workspace SlotContent(ContentItem _content_item, int _slot_number, Workspace _workspace)
        {
            //Trying to find an association in the same place
            Association AssociationInWorkspace = _workspace.Associations.SingleOrDefault(_ass => _ass.GameElementID == _content_item.GameElementID && _ass.QuasarModTypeID == _content_item.QuasarModTypeID && _ass.SlotNumber == _slot_number);

            //Removing it if existing
            if (AssociationInWorkspace != null)
                _workspace.Associations.Remove(AssociationInWorkspace);

            //Adding the association
            _workspace.Associations.Add(new Association() { ContentItemGuid = _content_item.Guid, GameElementID = _content_item.GameElementID, QuasarModTypeID = _content_item.QuasarModTypeID, SlotNumber = _slot_number });

            return _workspace;
        }

        /// <summary>
        /// Removes Associations for a specific QuasarModType/Group of QuasarModTypes and Slot number.
        /// </summary>
        /// <param name="_quasar_mod_type">QuasarModType to filter with</param>
        /// <param name="_slot_number">Slot number concerned by the removal</param>
        /// <param name="_game_element_id">Game Element ID concerned by the removal</param>
        /// <param name="_workspace">Workspace to remove associations from</param>
        /// <returns></returns>
        public static Workspace EmptySlot(QuasarModType _quasar_mod_type, int _slot_number, int _game_element_id, Workspace _workspace)
        {
            List<Association> AssociationsToRemove = new();

            Association RelatedAssociation = _workspace.Associations.SingleOrDefault(_ass => _ass.QuasarModTypeID == _quasar_mod_type.ID && _ass.SlotNumber == _slot_number && _ass.GameElementID == _game_element_id);


            return _workspace;
        }
    }
}
