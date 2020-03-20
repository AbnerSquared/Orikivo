using System;
using Discord;
using Discord.WebSocket;

namespace Orikivo.Arcade
{
    public class Session
    {
        private readonly BaseSocketClient _client;
        public Data Data; // defines all data needed for a session.
        public Ruleset Ruleset; // defines method presets on criteria
        public Player[] Players; // controls who can execute methods
        public Receiver[] Receivers; // manages output
        public Display[] Displays; // manages visuals
        public Controller[] Controllers; // manages input
        public Key[] Keys; // controls what kind of keys are accessible
        
        // this acts as if the session was cancelled.
        public void Close() { }

        // this pauses all input, displays, and messages.
        // if no interaction is handled for a max of 10 minutes, the session closes.
        public void Pause() { }

        // these methods handle the discord counterpart, from which they can then pass this on
        private void OnMessageReceived(SocketMessage message) { }
        private void OnMessageUpdate(SocketMessage message) { }
        private void OnMessageDeleted(SocketMessage message) { }

        // this handles main game states, such as when it starts, ends, or is cancelled.
        private void OnStart() { }
        private void OnEnd() { }
        private void OnCancelled() { }

        // this handles how inputs and rules update a game.
        private void OnInputReceived(Input input) { }
        private void OnRuleHandled(Rule rule) { }

        // these handle what to do if a player leaves, and determine if the game is still playable.
        private void OnPlayerJoined(Player player) { }
        private void OnPlayerLeft(Player player) { }

        // methods.
        public void RemovePlayer(Player player) { }
    }

    public class Channel
    {
        public ulong Id; // who am i?
        private ulong _displayId; // where is my display bound to?
    }

    public class Game
    {
        public Details Details; // what is the information for this game?
        public Func<Lobby, Session> Generator; // how do i set up a new session?
        public Variable[] Variables; // what are the variables that i need to store?
        public Rule[] Rules; // what are the rules for this game?
        public Ending[] Endings; // how does a game end?
        public Action<Session> Close; // what do i do when a session is force closed?
        public Option[] Options; // what are some aspects of a game that i can modify?
        public Variable[] PlayerVariables; // what are the variables to give to each player?
        public Component[] Components; // what are the components that can be set to a display?
    }

    public class Option
    {
        public string Id; // who am i ?
        public Type Type; // what is the kind of object i'm storing?
    }

    public class Ending
    {
        public Func<Data, bool> Criteria; // what criteria needs to be met in order to end this game?
        public Action<Session> Action; // what do i do when this game ends?
    }

    public class Details
    {
        public string Id; // what is the id for this game?
        public string Name; // what is the name for this game?
        public string Summary; // what is this game about?
        public int RequiredPlayers; // how many players do i need?
        public int PlayerLimit; // how many players can i have?
    }

    public class Config
    {
        private Option[] _options; // what are the options i have accessible?

        public (string id, object value)[] Values; // what are the values of some specific options?

        public void SetValue(string id, object value) { }
    }

    public class Options
    {
        public Game Game; // what is the game that is about to be played?
        public string Name; // what is the server name?
        public string Password; // what password is required in order to join?
        public Config Config; // what is modified for the game?

        private void OnGameSet() { } // modify the config variables
        private void OnGameCleared() { }
    }

    public class Network
    {
        private readonly BaseSocketClient _client;

        public event Action<User> UserJoined;
        public event Action<User> UserLeft;
        public event Action<Server> ServerCreated;
        public event Action<Server> ServerDeleted;

        public Server[] Servers;

        public void CreateServer(IUser user, IMessageChannel channel) { }
        public void DeleteServer(Server server) { }
    }

    public class Server
    {
        private readonly BaseSocketClient _client;

        public event Action<Server> SessionStarted; // who do i tell when a session starts?
        public event Action<Server> SessionEnded; // who do i tell when a session ends?

        public string Id; // id for the server
        public Session Session; // currently active
        public Lobby Lobby; // currently inactive
        public Options Options; // configuration for the current server

        public void Close() { }

        private void OnUserJoined(User user) { }
        private void OnUserLeft(User user) { }
    }

    public class Lobby
    {
        public User[] Users; // what users are currently inactive?
        public Channel[] Channels; // what channels are currently inactive?
    }

    public class User
    {
        public ulong Id; // who am i?
    }

    public class Message
    {
        public User User; // who sent this message?
        public string Value; // what did they send?
    }

    public class Receiver
    {
        public event Action<Message> MessageReceived; // who do i tell about a successful message?
        public event Action<Message> MessageUpdated; // who do i tell about an updated message?
        public event Action<Message> MessageDeleted; // who do i tell about a deleted message?

        public ulong Id; // who am i?
        private ulong _displayId; // where is my display bound to?
        public Display Display; // what can i currently see?
        public Controller Controller; // what keys am i able to invoke?
        public Player[] Players; // who can i read messages from?

        private void OnMessageReceived(Message message) { }
        private void OnMessageUpdated(Message message) { }
        private void OnMessageDeleted(Message message) { }

        public void SetDisplay(Display display) { }
        public void ClearDisplay() { }
        public void BindController(Controller controller) { }
        public void UnbindController() { }
        public void OnDisplayUpdated(Display display) { }
    }

    public class Ruleset
    {
        public event Action<Rule> RuleHandled; // who do i tell about a successful rule?
        public Rule[] Rules; // what are my rules?
    }

    public class Rule
    {
        public Func<Variable, bool> Criteria; // what variable do i need to check?
        public Action<Session> Action; // what do i do when my criteria is met?
    }

    public class Variable
    {
        public string Id; // who am i?
        public long Value; // what do i have stored?
    }

    public class Key
    {
        public string Id; // who am i?
        public Action<Player, Server> Action; // what do i do when this key is invoked?
    }

    public class Display
    {
        public event Action<Display> DisplayUpdated; // who do i tell about this update?
        public Component[] Components; // what do i currently have stored?
        public Receiver[] Receivers; // who can currently see this?

        public void AddReceiver(Receiver receiver) { }
        public void RemoveReceiver(Receiver receiver) { }

        private void Update() { } // refresh all connected receviers.

        public void ToggleComponent(string id) { }
        public void SetComponent(string id, string value) { }
        public void HideComponent(string id) { }
        public void ShowComponent(string id) { }
        public void ClearComponent(string id) { }
        public void AppendToComponent(string id, string value) { }
        public void PrependToComponent(string id, string value) { }
        public void InsertAtComponent(string id, int index, string value) { }
    }

    public class Component
    {
        public string Id; // who am i?
        public int Position; // where am i positioned on the display?
        public string Value; // what do i currently have stored?
        public bool Active; // am i currently visible?

        public void Toggle() { }
        public void Show() { }
        public void Hide() { }
        public void Set(string value) { }
        public void Clear() { }
    }

    public class GroupedComponent
    {
        public string Id; // who am i?
        public int Position; // where am i positioned on the display?
        public string[] Values; // what do i currently have stored?
        public int Capacity; // how much can i store?
        public bool Active; // am i currently visible?

        public void Toggle() { }
        public void Show() { }
        public void Hide() { }

        public void Set(string value) { }
        public void Prepend(string value) { }
        public void Append(string value) { }
        public void Insert(int index, string value) { }
        public void RemoveAt(int index) { }

        public void Clear() { }
    }
    
    public class Controller
    {
        public string Id; // who am i?
        public event Action<Input> InputReceived; // who do I tell about a successful input?
        public Key[] Keys; // what keys can be invoked?
        public Receiver[] Receivers; // who can I receive messages from?

        public void OnMessageReceived(Message message) { }
        public void UnbindKey(Key key) { }
        public void ClearKeys() { }
        public void BindKey(Key key) { }
    }

    public class Input
    {
        public Player Player; // who executed this input?
        public Key Key; // what key did they invoke?
    }

    public class Player
    {
        public ulong Id; // who am i?
        public Variable[] Variables; // data unique to a specific player
        public Receiver[] Receivers; // all visible receivers
    }

    public class Data
    {
        public Variable[] Variables; // data unique to a session
    }
}
