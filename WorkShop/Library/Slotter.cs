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
    public static class Slotter
    {
        /// <summary>
        /// Slots Multiple Content Items in the Workspace at the selected Slot
        /// </summary>
        /// <param name="_ContentItems">List of ContentItems to slot</param>
        /// <param name="_SlotNumber">Number of the slot to put them in</param>
        /// <param name="_Workspace">Workspace to slot them into</param>
        /// <returns>Edited Workspace</returns>
        public static Workspace SlotMultipleContents(ObservableCollection<ContentItem> _ContentItems, int _SlotNumber, Workspace _Workspace)
        {
            foreach (ContentItem _ContentItem in _ContentItems)
            {
                _Workspace = SlotContent(_ContentItem, _SlotNumber, _Workspace);
            }

            return _Workspace;
        }

        /// <summary>
        /// Slots a single Content Item in the workspace at the selected Slot
        /// </summary>
        /// <param name="_ContentItem"ContentItem to slot</param>
        /// <param name="_SlotNumber">Number of the slot to put them in</param>
        /// <param name="_Workspace">Workspace to slot them into</param>
        /// <returns>Edited Workspace</returns>
        public static Workspace SlotContent(ContentItem _ContentItem, int _SlotNumber, Workspace _Workspace)
        {
            //Trying to find an association in the same place
            Association AssociationInWorkspace = _Workspace.Associations.SingleOrDefault(ass => ass.GameElementID == _ContentItem.GameElementID && ass.QuasarModTypeID == _ContentItem.QuasarModTypeID && ass.SlotNumber == _SlotNumber);

            //Removing it if existing
            if (AssociationInWorkspace != null)
                _Workspace.Associations.Remove(AssociationInWorkspace);

            //Adding the association
            _Workspace.Associations.Add(new Association() { ContentItemGuid = _ContentItem.Guid, GameElementID = _ContentItem.GameElementID, QuasarModTypeID = _ContentItem.QuasarModTypeID, SlotNumber = _SlotNumber });

            return _Workspace;
        }

        /// <summary>
        /// Removes Associations for a specific QuasarModType/Group of QuasarModTypes and Slot number.
        /// </summary>
        /// <param name="_QuasarModType">QuasarModType to filter with</param>
        /// <param name="_SlotNumber">Slot number concerned by the removal</param>
        /// <param name="_GameElementID">Game Element ID concerned by the removal</param>
        /// <param name="_Workspace">Workspace to remove associations from</param>
        /// <returns></returns>
        public static Workspace EmptySlot(QuasarModType _QuasarModType, int _SlotNumber, int _GameElementID, Workspace _Workspace)
        {
            List<Association> AssociationsToRemove = new();

            Association RelatedAssociation = _Workspace.Associations.SingleOrDefault(ass => ass.QuasarModTypeID == _QuasarModType.ID && ass.SlotNumber == _SlotNumber && ass.GameElementID == _GameElementID);


            return _Workspace;
        }
    }
}
