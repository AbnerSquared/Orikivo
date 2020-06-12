using System.Collections.Generic;

namespace Arcadia.Old
{
    public class GameWindowProperties
    {
        public static GameWindowProperties Lobby
        {
            get
            {
                GameWindowProperties properties = new GameWindowProperties();
                properties.Name = "lobby"; // window.lobby

                GameTabProperties main = new GameTabProperties();
                main.Name = "main"; // tab.console

                ElementGroupConfig consoleConfig = ElementGroupConfig.Empty;
                consoleConfig.CanUseInvalidChars = true;
                consoleConfig.PageElementLimit = 8;
                consoleConfig.ContentFormatter = "```autohotkey\n{0}```";
                consoleConfig.ElementFormatter = ":: {0}";
                consoleConfig.ElementSeparator = "\n";
                consoleConfig.CanFormat = true;

                ElementGroupConfig triggerConfig = ElementGroupConfig.Empty;
                triggerConfig.ElementFormatter = "`{0}`";
                triggerConfig.ElementSeparator = " • ";

                ElementGroupConfig userConfig = ElementGroupConfig.Empty;
                userConfig.ElementFormatter = "{0}";
                userConfig.ElementSeparator = " • ";

                ElementConfig infoConfig = ElementConfig.Empty;
                infoConfig.ContentFormatter = "{0}";

                Element info = new Element(null, "info", infoConfig);
                info.CanUseInvalidChars = true;
                info.CanFormat = true;
                info.AllowNewLine = true;
                info.Priority = 5;

                ElementGroup console = new ElementGroup("console", consoleConfig);
                console.Priority = 4;
                console.FillEmpties = true;


                ElementGroup triggers = new ElementGroup("triggers", triggerConfig);
                triggers.Priority = 3;
                triggers.FillEmpties = false;

                Element userInfo = new Element(null, "user_info");
                userInfo.Priority = 2;
                userInfo.CanUseInvalidChars = true;
                userInfo.AllowNewLine = true;
                userInfo.CanFormat = true;

                ElementGroup users = new ElementGroup("users", userConfig);
                users.Priority = 1;
                users.FillEmpties = false;

                main.Elements.Add(info);
                main.Groups.Add(console);
                main.Groups.Add(triggers);
                main.Groups.Add(users);
                main.Elements.Add(userInfo);

                GameTab tab = new GameTab(main);

                properties.Tabs.Add(tab);
                properties.CurrentTabId = tab.Id;
                properties.Output = GameOutput.Lobby;

                return properties;
            }
        }

        public static GameWindowProperties Game
        {
            get
            {
                var window = new GameWindowProperties();
                window.Name = "game";
                var tab = new GameTabProperties();
                tab.Name = "main";
                var groupInfo = ElementGroupConfig.Empty;
                groupInfo.PageElementLimit = 8;
                groupInfo.ElementFormatter = "{0}";
                groupInfo.ElementSeparator = "\n";
                groupInfo.ContentFormatter = "{0}";
                var group = new ElementGroup("chat", groupInfo);
                tab.Groups.Add(group);
                window.Tabs.Add(new GameTab(tab));
                window.Output = GameOutput.Game;
                return window;
            }
        }

        public string Name { get; set; }
        public string CurrentTabId { get; set; }
        public GameOutput Output { get; set; }
        public List<GameTab> Tabs { get; set; } = new List<GameTab>();
    }
}
