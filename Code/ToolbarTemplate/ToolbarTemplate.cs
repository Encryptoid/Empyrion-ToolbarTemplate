using System;
using System.Threading.Tasks;
using Eleon;
using Eleon.Modding;
using EmpyrionModdingFramework;
using InventoryManagement;
using ModLocator;

namespace ToolbarTemplate
{
    public class ToolbarTemplate : EmpyrionModdingFrameworkBase
    {
        private TemplateManager _templateManager;

        protected override void Initialize()
        {
            ModName = "ToolbarTemplate";

            var databaseFolder = new FolderLocator(Log).GetDatabaseFolder(ModName);

            var inventoryManager = new InventoryManager(new CsvManager(databaseFolder));
            _templateManager = new TemplateManager(inventoryManager, Log);

            CommandManager.CommandPrexix = "!";
            CommandManager.CommandList.Add(new ChatCommand("tt save", SaveTemplate));
            CommandManager.CommandList.Add(new ChatCommand("tt", LoadTemplate));
        }

        private async Task SaveTemplate(MessageData data)
        {
            Log($"SaveTemplate called with entityId: {data.SenderEntityId}");
            PlayerInfo player = (PlayerInfo)await RequestManager.SendGameRequest(CmdId.Request_Player_Info, new Id { id = data.SenderEntityId });
            Log("Retrieved player info.");

            _templateManager.SaveInventoryTemplate(player.steamId, player);
            Log("Saved template");

            await MessagePlayer(data.SenderEntityId, $"Saved template.", 5, MessagerPriority.Yellow);
        }

        private async Task LoadTemplate(MessageData data)
        {
            Log($"LoadTemplate called with entityId: {data.SenderEntityId}");
            PlayerInfo player = (PlayerInfo)await RequestManager.SendGameRequest(CmdId.Request_Player_Info, new Id() { id = data.SenderEntityId });
            Log("Retrieved player info.");

            Inventory template;
            try 
            {
                if (!_templateManager.LoadTemplateInventory(player, out template))
                {
                    Log($"No Template found, messaging player.");
                    await MessagePlayer(data.SenderEntityId, "Could not locate your template. Please create one first.", 5, MessagerPriority.Red);
                    return;
                }
            }
            catch (Exception e)
            {
                Log($"Error loading template: {e}");
                return;
            }

            await RequestManager.SendGameRequest(CmdId.Request_Player_SetInventory, template);
            await MessagePlayer(data.SenderEntityId, $"Loaded template.", 5, MessagerPriority.Yellow);
            Log("Loaded template");
        }
    }
}
