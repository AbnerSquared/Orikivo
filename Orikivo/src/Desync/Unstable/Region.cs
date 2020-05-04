using Orikivo.Drawing;
using System;

namespace Orikivo.Desync.Unstable
{
    // Represents the core of everything
    // related to a world.

    // the engine stores
    // - World: The main source of land
    // - Items: All items
    // - Characters: All characters
    // - Creatures: All creatures
    // - Dialog: The DialogMap
    // - Merits: All possible achievements
    /*
    The Engine is responsible for:
    - Market catalog generation
    - Path finding for characters
    - Dialog generation for characters
    - Handle real-time updates
    - Handle travel time calculations
    - Handle criteria checks
    - Handle loot generation for fields
    - Handle creature generation for fields
    - Handle events and story arc calculations
    - Handle objective completion checks
    - Handle relationship updates
    - Handle possible gifts and merit criterion checks
    - Handle how a specific location should be, in the case
        of an event that changes a location
    - Handle discount calculations
    - Handle special events
    */
    public class Engine
    {

    }

    // TODO: figure out a way to implement variants of the same location
    // whenever specific progress flags are set. this way, the world can feel like it's changing
    // maybe have an ID pointing to the variant of it?

    // TODO: Implement a main engine that handles generic world data.

    // TODO: Implement a container on HuskBrain that stores compressed world data
    // this is meant for memorials, a home, etc. so all of that stuff remains persistent.
    // store generated catalogs, previous deaths, their home data

    // represents a specific form of travel

    public enum TransportType
    {
        Ground = 1,
        Water = 2,
        Submerged = 4,
        Air = 8
    }

    // defines a specific position in a world.
    public class Locator
    {
        // the inner most location ID from which they are in.
        // if none is set, the x- and y-coordinates are
        // specified for the world.
        public string Id;

        // relative x- and y-coordinate values.
        public float X;
        public float Y;
    }

    // the end result of a travel
    public class Destination : Locator
    {
        // when the user will arrive at this location
        public DateTime Arrival;

        // when the user started going to this location
        public DateTime StartedAt;
    }

    // represents a massive body of land.
    public class World
    {
        public string Id;

        // the name of this world
        public string Name;

        // an image reference that points to the map for this world
        public Sprite Map;

        // the size of this world
        public RegionF Perimeter;

        // a collection of fields
        public Field[] Fields;

        // if you are outside of a field, it's just empty.


        // a collection of routes for this world.
        public Route[] Routes;

        // a collection of sectors
        public Sector[] Sectors;
    }

    // represents an inhabitable plot of land
    public class Sector : Location
    {
        // quick name references for specific sub-locations in a sector
        public Region[] Regions;

        // general groups of areas.
        public Area[] Areas;
        
        // a collection of buildings
        public Construct[] Constructs;

        // a collection of structures
        public Structure[] Structures;

        // connections between various paths.
        public Route[] Route;
    }

    // represents a cluster of buildings and constructs that don't apply travel time.
    // this is useful if you want the user to simply move around several places.
    public class Area : Location
    {
        // a collection of entry points for this area.
        public Vector2[] Entrances;

        // a collection of buildings; no perimeter has to be specified
        public Construct[] Constructs;

        // a collection of structures; no perimeter has to be specified
        public Structure[] Structures;
    }

    // represents a generic plot of land
    public class Region
    {
        // the id for this region
        public string Id { get; set; }

        // the name of this region
        public string Name { get; set; }

        // the area in the parent location from which this is set
        public RegionF Perimeter { get; set; }

        
        // Gets a floating-point integer that represents the global x-coordinate of the <see cref="Region"/> in a <see cref="World"/>.
       
        public float Longitude { get; }

       
        // Gets a floating-point integer that represents the global y-coordinate of the <see cref="Region"/> in a <see cref="World"/>.
       
        public float Latitude { get; }
    }

    // defines the appearance of a region
    public enum BiomeType
    {
        Grasslands = 1,
        Ocean = 2,
        Snowy = 3,
        Deprived = 4
    }

    // represents a sub-plot of land in a field.
    public class Biome : Region
    {
        // determines what the user needs in order to access this biome
        public TransportType Transport;

        // determines how the biome is displayed.
        public BiomeType Type;

        // determines how a user can travel in this biome
        // if left empty, it will default to the field atmosphere
        public Atmosphere Atmosphere;

        // determines what creatures can appear
        // leave empty for Field.SpawnTable
        public SpawnTable Table;
    }


    // alters how a user can travel in a region.
    public class Atmosphere
    {
        // how far the user can see
        public float ViewDistance;

        // how often the creatures can spawn
        public float SpawnRate;

        // how fast the player can go
        public float TravelSpeed;

        // how poisonous the air is
        public float ExposureStrength;

        // how often relics appear
        public float RelicRate;

        // how often minerals appear
        public float MineralRate;
    }

    // determines the kind of creatures that can appear in a biome
    public class SpawnTable : ITable
    {
        // list of spawnable entries
        public SpawnEntry[] Entries;

        ITableEntry[] ITable.Entries => Entries;
    }

    // maybe make spawns correlate to husk strength??
    public class SpawnEntry : ITableEntry
    {
        // the exact creature that can spawn
        public string CreatureId;

        // the groups of creatures that can appear
        public CreatureTag[] Groups;

        // determines spawn frequency for this entry.
        public float Weight { get; set; }
    }

    // each time a user performs a successful action, the chance of a creature appearing increases each time
    // and resets once the creature has been met.
    // defines a specific action that user might be doing when exploring
    public enum ExploreAction
    {
        // intending to look for minerals
        Mine = 1,

        // intending to look for creatures, spawn rates are increased
        Hunt = 2,

        // intending to look for relics
        Search = 4
    }

    // represents wilderness in a world.
    public class Field : Location
    {
        // a collection of sub-regions in this Field
        public Biome[] Biomes;

        // a collection of structures
        public Structure[] Structures;

        // a collection of buildings
        public Construct[] Constructs;

        // the base atmosphere for this field
        public Atmosphere Atmosphere;

        // determines the kind of creatures that can appear
        public SpawnTable SpawnTable;

        // determines the kind of items that can appear
        public LootTable LootTable;

    }

    // represents a generic generation table
    public interface ITable
    {
        ITableEntry[] Entries { get; }
    }

    // represents a generic table entry
    public interface ITableEntry
    {
        float Weight { get; set; }
    }

    public class LootTable : ITable
    {
        public LootEntry[] Entries;
        ITableEntry[] ITable.Entries => Entries;
    }

    public class LootEntry : ITableEntry
    {
        // the action required in order to receive this loot
        public ExploreAction Action;

        // the exact item id
        public string ItemId;

        // the specific groups
        public ItemTag Groups;

        // the rarity for this loot entry to appear.
        public float Weight { get; set; }
    }

    // a location can that can be entered
    public enum LocationType
    {
        World = 1,
        Field = 2,
        Sector = 4,
        Area = 8,
        Construct = 16
    }

    // TODO: Make location abstract, as the Type for the location needs to be set.
    // represents a generic enterable plot of land.
    public class Location : Region
    {
        // an image reference that showcases its appearance
        public Sprite Visual;

        // the flag that refers to the true type of this region.
        // each main location type will set their own flag here.
        public LocationType Type { get; protected set; }

        // gets this location's parent, if it has any
        // public Location Parent => GetParent();

        // gets this location's children, if any.
        // for every derived location that supports children,
        // you would override this method and toss in all children.
        // World.Children contains ALL generic locations.
        // private Location GetParent() => World.Children.
        public virtual Location[] GetChildren() => null;
    }

    // represents a generic building in a region
    public class Construct : Location
    {
        // an image to display whenever an NPC is in this location
        public Sprite Interior;

        // the times at which this building is available, null means this is always available
        public Schedule Schedule;
    }

    // represents the upgrade tier of a home.
    // maybe point each tier to a flag
    public enum HomeTier
    {
        Small = 1,
        Medium = 2,
        Large = 3
    }

    public class Home : Construct
    {
        // determines how many decor slots this home has.
        public HomeTier Tier;

        // the currently placed decorations in this house
        public Decor[] Decor;

        // the amount of items currently stored in this box
        public Storage Storage;
    }

    // represents a generic form of storage with a specified capacity
    public class Storage
    {
        // the max amount of items this storage can hold
        public int Capacity;
        
        // the current collection of items stored
        public Item[] Items;
    }

    // defines how a decor is represented
    public enum DecorType
    {
        // allows crafting
        Workbench = 1,

        // allows NPC communication from anywhere
        Computer = 2,

        // allows item fusion
        Fusion = 3
    }

    // represents an item that can appear in a home
    public class Decor
    {
        // determines how many slots this decor takes up for a home.
        public int Size;

        // determines the type of decor this is
        public DecorType Type;
    }

    public class Market : Construct
    {
        // if the market supports selling items to users
        // this however, can be set to true if there is no catalog.
        public bool CanBuyFrom;
        
        // if the market supports purchasing items from users
        public bool CanSellFrom;

        // represents the kind of items that can be bought here
        public CatalogTable Catalog;
    }
    public class CatalogSpecialEntry
    {
        // the groups that this special is applied to.
        public ItemTag Groups;

        // the max % bonus that can be applied.
        public float Max;
    }

    // represents the kinds of items that are bound to a market.
    public class CatalogTable : ITable
    {
        // determines how many of the items generated can be
        // at a discount;
        public int MaxDiscountsAllowed;

        // the maximum possible discount % to take off
        public float Discount;

        // determines how many of the entries specified
        // can be at a discount
        public int MaxSpecialsAllowed;

        // the % chance that a special will generate
        public float SpecialChance;

        // the amount of items this catalog stores
        public int Size;

        // the variant that this catalog is meant for.
        public ItemDimension Type;

        // info about each random item entry, if no entry is chosen, it can be left blank
        // this means it can be possible to roll an empty catalog, which means a market could be
        // out of stock on specific items
        public CatalogEntry[] Entries;

        // this is a collection of possible items that this market
        // will be paying MORE for.
        public CatalogSpecialEntry[] Specials;
        ITableEntry[] ITable.Entries => Entries;
    }

    // represents a specific item entry for a catalog.
    public class CatalogEntry : ITableEntry
    {
        // the exact item id
        public string ItemId;

        // the group of items that can be selected
        public ItemTag Groups;

        // the amount of times this entry can provide an item
        public int MaxAllowed;

        // determines if this entry can have a discount applied
        public bool CanDiscount;

        // the rarity of this entry being picked
        public float Weight { get; set; }
    }

    // represents a multi-level construct
    public class Highrise : Construct
    {
        // the variation of floors on this building; if no id is set,
        // the construct is referenced by index.
        public Construct[] Floors;
    }

    // represents an interactive external construction
    public class Structure : Region
    {
        // determines how this structure is handled
        // if left custom, it will not have any automatic features
        public StructureType Type;
    }

    public enum StructureType
    {
        // marks the structure as a location from which a user can pick up their belongings
        // a death marker
        Memorial = 1,

        // marks the structures as a simple visual piece
        Decor = 2,
        // this marks the structure as a custom derivation
        Custom = 4
    }

    // defines how a barrier blocks a region of land
    [System.Flags]
    public enum BarrierMode
    {
        // blocks view distance, which will prevent you from marking positions on a map
        Sight = 1,

        // blocks travel, which will prevent you from walking in that area.
        Travel = 2,

        All = Sight | Travel
    }

    // represents an impassable plot of land
    public class Barrier : Region
    {
        // the way this barrier blocks stuff
        public BarrierMode Mode;

        // determines the kind of transportation that this blocks
        // if none are set, it defaults to ground and water blocking
        public TransportType? Transport;

        // the criteria that needs to be met in order to knock down this barrier
        // if none is set, the barrier is permanent
        public UnlockCriteria Criteria;
    }

    // represents a set of criteria that needs to be met in order to access or perform a task
    public class UnlockCriteria
    {
        // the flag that will be stored when this criteria is met
        public string UnlockId;

        // the collection of flags that the user needs to have to unlock this
        public string[] RequiredFlags;
    }

    // represents a path that joins two locations together.
    public class Route : Path
    {
        // the form of transportation this route needs
        public TransportType Transport;

        // the travel speed on this route
        public float TravelSpeed;
    }

    // represents a generic connection of coordinates
    public class Path
    {
        // the first value is where this path starts
        // all of the joints for this path
        // the last value is where it ends up.
        public PathNode[] Nodes;
    }

    // represents a joint for a path
    public class PathNode
    {
        // the location id for this node. can be left empty to refer to the x and y position instead.
        public string Id;

        // the relative x position for this node
        public float X;

        // the relative y-position for this node
        public float Y;
    }
}
