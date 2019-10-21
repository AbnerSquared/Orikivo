using System.Collections.Generic;

namespace Orikivo
{
    public class GameWindowProperties
    {
        public static GameWindowProperties Lobby
        {
            get
            {
                GameWindowProperties properties = new GameWindowProperties();
                properties.Name = "generic_lobby"; // window.generic_lobby

                GameTabProperties console = new GameTabProperties();
                console.Name = "main"; // window.generic_lobby:tab.main

                ElementGroupConfig chatConfig = ElementGroupConfig.Empty;
                chatConfig.CanUseInvalidChars = true;
                chatConfig.PageElementLimit = 8;
                chatConfig.ContentFormatter = "```autohotkey\n{0}```";
                chatConfig.ElementFormatter = ":: {0}";
                chatConfig.ElementSeparator = "\n";
                chatConfig.CanFormat = true;

                ElementGroup chat = new ElementGroup("chat", chatConfig); // window.generic_lobby:tab.main:elements.chat
                chat.Priority = 1;
                chat.FillEmpties = true;

                console.Groups.Add(chat);

                GameTab tab = new GameTab(console);

                properties.Tabs.Add(tab);
                properties.CurrentTabId = tab.Id;
                properties.Output = GameOutput.Lobby;

                return properties;
            }
        }

        public static GameWindowProperties Werewolf
        {
            get
            {
                GameWindowProperties properties = new GameWindowProperties();



                return properties;
            }
        }

        public string Name { get; set; }
        public string CurrentTabId { get; set; }
        public GameOutput Output { get; set; }
        public List<GameTab> Tabs { get; set; } = new List<GameTab>();
    }
}
