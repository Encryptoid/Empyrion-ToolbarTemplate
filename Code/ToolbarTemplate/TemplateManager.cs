using Eleon.Modding;
using EmpyrionModdingFramework;
using InventoryManagement;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InventoryTemplate
{
    public class TemplateManager
    {
        private InventoryManager _invManager;
        private Action<string> _log;

        public TemplateManager(InventoryManager invManager, Action<string> logFunc)
        {
            _invManager = invManager;
            _log = logFunc;
        }

        internal void SaveInventoryTemplate(string steamId, PlayerInfo playerInfo)
        { 
            _invManager.SaveBar(steamId, playerInfo.toolbar);
        }

        internal Inventory LoadTemplateInventory(PlayerInfo playerInfo)
        {
            //Combine allItems from bag and bar
            var allItems = playerInfo.bag.ToInvetoryItems();
            allItems.AddRange(playerInfo.toolbar.ToInvetoryItems());

            //Load template
            var barTemplate = _invManager.LoadBar(playerInfo.steamId).ToInvetoryItems();
            var newToolbar = new List<ItemStack>();

            //Match items from template to allItems
            foreach (var templateItem in barTemplate)
            {
                var match = allItems.FirstOrDefault(curItem => curItem.Id == templateItem.Id);
                if (match == null) continue;

                match.SlotId = templateItem.SlotId;
                //Add to toolbar, clear from original place
                newToolbar.Add(match.ToItemStack());
                match.ClearItem();
            }
            var clearedItems = allItems.RemoveAll(m => m.Count == 0); //Remove cleared items
            _log("Removed " + clearedItems);

            //Converts inventory overflow to toolbar
            while (allItems.Count() > 40)
            {
                _log("Item overflow occurred");
                var item = allItems.First();
                newToolbar.Add(item.ToItemStack());
                allItems.RemoveAt(0);
            }

            //Create new inv
            var newBag = allItems.CleanSlotIds(true).Select(item => item.ToItemStack()).ToArray();
            var newInv = new Inventory() { playerId = playerInfo.entityId, toolbelt = newToolbar.ToArray().CleanSlotIds(false), bag = newBag };

            return newInv;
        }
    }
}
