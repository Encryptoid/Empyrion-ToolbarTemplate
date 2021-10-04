using Eleon;
using Eleon.Modding;
using EmpyrionModdingFramework;
using InventoryManagement;
using ModLocator;
using System;
using System.IO;
using System.Threading.Tasks;

namespace InventoryTemplate
{
    public class ToolbarTemplate : EmpyrionModdingFrameworkBase
    {
        private TemplateManager _templateManager;

        protected override void Initialize()
        {
            Log("ToolbarTemplate is running from folder: " + Directory.GetCurrentDirectory());

            ModName = "ToolbarTemplate";

            var modLocator = new FolderLocator((string s) => Log(s));

            var inventoryManager = new InventoryManager(new CsvManager(modLocator.GetDatabaseFolder(ModName), (string s) => Log(s)));
            _templateManager = new TemplateManager(inventoryManager, (string s) => Log(s));

            var chatPrefix = "!";
            CommandManager.CommandPrexix = chatPrefix;

            CommandManager.CommandList.Add(new ChatCommand($"t save", (I) => SaveTemplate(I)));
            CommandManager.CommandList.Add(new ChatCommand($"t", (I) => LoadTemplate(I)));
        }

        private async Task SaveTemplate(MessageData data)
        {
            Log($"SaveTemplate called with entityId: {data.SenderEntityId}");
            PlayerInfo player = (PlayerInfo)await RequestManager.SendGameRequest(CmdId.Request_Player_Info, new Id() { id = data.SenderEntityId });
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
                template = _templateManager.LoadTemplateInventory(player); 
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
