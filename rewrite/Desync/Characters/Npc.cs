using System.Collections.Generic;

namespace Orikivo.Unstable
{
    public class Npc
    {
        string Id;
        string Name;
        NpcPersonality Personality;
        List<Relation> InitialRelations;

        // string => TriggerId, NpcDialogue => Responses
        Dictionary<string, NpcDialogue> Dialogue { get; } = new Dictionary<string, NpcDialogue>();
    }
}
