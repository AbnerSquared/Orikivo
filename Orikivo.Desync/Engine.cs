using Orikivo.Drawing;
using Orikivo.Drawing.Graphics2D;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Orikivo.Desync
{
    // TODO: Once the methods are figured out, make this class non-static, from which is loaded from a cache.
    /// <summary>
    /// Represents the central process manager for a DesyncClient.
    /// </summary>
    public static partial class Engine
    {
        public static Husk Initialize(HuskBrain brain)
        {
            var location = World.Find("area0");

            if (location == null)
                throw new ArgumentException("The specified initial location ID does not point to a location.");

            var husk = new Husk(location.GetLocator());
            brain.SetFlag(DesyncFlags.Initialized);

            return husk;
        }

        // Retreival
        public static Character GetCharacter(string id)
            => Characters[id];

        // return a random dialogue pool for any pool that is marked as random.
        public static DialogTree GetGenericTree()
        {
            var trees = Dialogs.Values.Where(x => x.IsGeneric);

            return Randomizer.Choose(trees);
        }

        // get a specified dialogue pool.
        public static DialogTree GetTree(string id)
        {
            return Dialogs[id];
        }

        public static Claimable GetClaimable(string id)
            => Claimables[id];

        public static Merit GetMerit(string id)
            => Merits[id];

        public static IEnumerable<Merit> GetMerits(MeritGroup group)
            => Merits.Values.Where(x => x.Group == group);

        public static Booster GetBooster(string id)
            => Boosters[id];

        public static Item GetItem(string id)
            => Items[id];

        public static IEnumerable<Item> GetItemsByDimension(ItemDimension dimension)
            => Items.Values.Where(x => x.Dimension == dimension);

        // Mapping
        public static byte[] CompressMap(Grid<bool> data)
        {
            // amount of bytes that need to be stored
            int count = (int)Math.Ceiling((double)data.Count / 8);

            int i = 0;
            int b = 0;

            byte[] bytes = new byte[count];
            bool[] bits = new bool[8];
            foreach (bool bit in data)
            {
                bits[i] = bit;
                i++;

                if (i >= 8)
                {
                    bytes[b] = bits.ToByte();
                    bits = new bool[8];
                    b++;
                    i = 0;
                }
            }

            if (i != 0)
            {
                for (int u = i; u < 8; u++)
                    bits[u] = false;

                bytes[b] = bits.ToByte();
            }

            return bytes;
        }

        public static Grid<bool> DecompressMap(int width, int height, byte[] data)
        {
            var progress = new Grid<bool>(width, height, false);
            int i = 0;
            foreach (byte fragment in data)
            {
                foreach (bool bit in fragment.GetBits())
                {
                    progress[i] = bit;
                    i++;

                    if (i >= progress.Count)
                        break;
                }
            }

            return progress;
        }

        public static Grid<bool> GetMapData(string id, byte[] data)
        {
            if (World.Id == id)
            {
                if (World.Map != null)
                    return DecompressMap(World.Map.Width, World.Map.Height, data);
            }

            if (World.Sectors.Any(x => x.Id == id))
            {
                Sector sector = World.GetSector(id);

                if (sector.Map != null)
                    return DecompressMap(sector.Map.Width, sector.Map.Height, data);
            }

            throw new ArgumentException("The specified ID does not exists for any Sector or World.");
        }

        public static Map GetMap(string id, HuskBrain brain)
        {
            if (!(World.Id == id) && !World.Sectors.Any(x => x.Id == id))
                throw new ArgumentException("The specified ID does not exists for any Sector or World.");

            if (!brain.Maps.ContainsKey(id))
                brain.Maps[id] = new byte[] { };

            Grid<bool> progress = GetMapData(id, brain.Maps[id]);

            if (World.Id == id)
                return new Map(World.Map, progress);

            if (World.Sectors.Any(x => x.Id == id))
            {
                Sector sector = World.GetSector(id);
                return new Map(sector.Map, progress);
            }

            throw new ArgumentException("The specified ID does not exists for any Sector or World.");
        }

        public static bool CanShopAt(Husk husk, out Market market)
        {
            if (IsInLocation(husk, out market))
            {
                if (market.IsActive())
                    return true;
                else
                {
                    // If the market is closed, attempt to take them out of the market if they are in it
                    TryLeaveLocation(husk);
                }
            }

            return false;
        }

        public static bool CanChatWith(Husk husk, string characterId, out Character character)
        {
            character = null;
            var characters = GetVisibleCharacters(husk);

            if (characters.Any(x => x.Id == characterId))
            {
                character = characters.First(x => x.Id == characterId);
                return true;
            }

            return false;
        }

        public static bool TryLeaveLocation(Husk husk)
        {
            Location location = husk.Location.GetLocation();

            switch (location.Type)
            {
                case LocationType.Area:
                    Area area = location as Area;
                    Vector2 entrance = area.Entrances?.FirstOrDefault() ?? area.Shape.Position;
                    husk.Location.X = entrance.X;
                    husk.Location.Y = entrance.Y;

                    // this should point back to the sector.
                    husk.Location.Id = location.GetParent().Id;
                    break;

                case LocationType.Construct:
                    var parent = location.GetParent();

                    if (parent == null)
                        throw new ArgumentException("The specified location does not have a parent to exit.");

                    if (parent?.Type == LocationType.Area)
                    {
                        husk.Location.X = parent.Shape.X;
                        husk.Location.Y = parent.Shape.Y;
                        husk.Location.Id = parent.Id;
                    }
                    else
                    {
                        // just move their position to the outskirts of the inner location.
                        // TODO: Implement entrance references for locations.
                        husk.Location.X = location.Shape.X;
                        husk.Location.Y = location.Shape.Y;
                        husk.Location.Id = parent.Id;
                    }
                    break;

                default: // if (!(LocationType.Area | LocationType.Construct).HasFlag(location.Type))
                    return false;
            }

            return true;
        }
          
        public static TravelResult TryGoTo(Husk husk, float x, float y, out Destination attempted)
        {
            attempted = null;

            if (!(LocationType.Sector | LocationType.Field | LocationType.World).HasFlag(husk.Location.GetInnerType()))
                throw new ArgumentException("The specified Husk is not within a coordinate-based location.");

            if (husk.Destination != null)
            {
                if (!husk.Destination.Complete)
                    throw new ArgumentException("Husk is currently in transit.");
                else
                    UpdateLocation(husk, husk.Destination);
            }

            Route route = CreateRoute(husk.Location.X, husk.Location.Y, x, y);
            var now = DateTime.UtcNow;

            // TODO: Implement region naming
            // sector.GetInnerRegion(float x, float y);
            // iterates through all specified regions, and returns the one that contains those coordinates
            var info = new Destination(husk.Location.WorldId, husk.Location.Id, x, y, now, now.Add(route.Time));
            attempted = info;

            // If the travel time is short enough, just instantly go to the location.
            if (route.GetTime().TotalSeconds <= 1f)
            {
                UpdateLocation(husk, info);
                return TravelResult.Instant;
            }

            husk.Destination = info;

            return TravelResult.Start;
        }

        public static TravelResult TryGoTo(Husk husk, HuskBrain brain, string id, out Region attempted)
        {
            attempted = null;

            Location location = husk.Location.GetLocation();

            switch (location.Type)
            {
                case LocationType.Area:
                    foreach (Construct construct in (location as Area).Constructs)
                    {
                        if (construct.Id == id)
                            continue;

                        attempted = construct;

                        if (construct is Market)
                        {
                            if (!(construct as Market).IsActive())
                                return TravelResult.Closed;
                        }

                        husk.Location.Id = construct.Id;
                        return TravelResult.Instant;
                    }

                    return TravelResult.Invalid;

                case LocationType.Sector:
                    return TryGoToInSector(husk, brain, id, out attempted);

                default:
                    throw new NotImplementedException("Travelling in this location has not been implemented yet");
            }
        }

        // see if this can be simplified
        public static TravelResult TryGoToInSector(Husk husk, HuskBrain brain, string id, out Region attempted)
        {
            attempted = null;

            if (husk.Location.GetInnerType() != LocationType.Sector)
                throw new ArgumentException("The specified Husk is not within a sector.");

            Sector sector = husk.Location.GetLocation() as Sector;

            if (husk.Destination != null)
            {
                if (!husk.Destination.Complete)
                    throw new ArgumentException("Husk is currently in transit.");
                else
                    UpdateLocation(husk, husk.Destination);
            }

            foreach (Area area in sector.Areas)
            {
                if (area.Id == id)
                {
                    attempted = area;

                    Route route = CreateRoute(husk.Location.X, husk.Location.Y, area.Shape);
                    var now = DateTime.UtcNow;

                    Destination info = new Destination(area.GetLocator(), now, now.Add(route.Time));

                    // if the travel time is short enough, just instantly go to the location.
                    if (route.GetTime().TotalSeconds <= 1f)
                    {
                        UpdateLocation(husk, info);
                        return TravelResult.Instant;
                    }

                    husk.Destination = info;
                    return TravelResult.Start;
                }
            }

            foreach (Structure structure in sector.Structures.Where(x => brain.HasDiscoveredRegion(x.Id)))
            {
                if (structure.Id == id)
                {
                    attempted = structure;

                    Route route = CreateRoute(husk.Location.X, husk.Location.Y, structure.Shape);
                    var now = DateTime.UtcNow;

                    Destination info = new Destination(husk.Location.WorldId, husk.Location.Id, structure.Shape.Midpoint.X, structure.Shape.Midpoint.Y, now, now.Add(route.Time));

                    // if the travel time is short enough, just instantly go to the location.
                    if (route.GetTime().TotalSeconds <= 1f)
                    {
                        UpdateLocation(husk, info);
                        return TravelResult.Instant;
                    }

                    husk.Destination = info;
                    return TravelResult.Start;
                }
            }

            return TravelResult.Invalid;
        }

        public static void Recover(Husk husk, HuskBrain brain, Memorial memorial)
        {
            if (!brain.Memorials.Contains(memorial))
                return;

            // TODO: create a system that allows you to selectively recover items
            husk.Backpack = memorial.Backpack;
            // MemorialHandler.StartAsync(); // You search through your past items. Most are missing, but you can take what you need.
            // (List out all of the available items, modules, and upgrades here)

            brain.Memorials.Remove(memorial);
        }

        public static bool CanAct(ref Husk husk, HuskBrain brain)
        {
            if (husk == null)
                return TryResync(ref husk, brain);

            return true;
        }

        internal static bool TryResync(ref Husk husk, HuskBrain brain)
        {
            if (husk != null)
                return false;

            if (!brain.ResyncAt.HasValue)
                return false;

            if (DateTime.UtcNow - brain.ResyncAt.Value <= TimeSpan.Zero)
                return false;

            husk = new Husk(World.Find("ctr0").GetLocator());
            brain.ResyncAt = null;
            return true;
        }

        public static bool CanMove(Husk husk, out string notification)
        {
            notification = null;

            if (husk.Destination == null)
                return true;

            if (!husk.Destination.Complete)
                return false;

            notification = $"You have arrived at **{husk.Destination.GetInnerName()}**.";

            UpdateLocation(husk, husk.Destination);
            husk.Destination = null;

            return true;
        }

        public static void UpdateLocation(Husk husk, Locator info)
        {
            husk.Location = husk.Location.Id == info.Id ? info : info.GetLocation().GetLocator();
        }

        // TODO: This method is meant for BindToRegion, as it will get all visible regions that a husk can currently see.
        public static IEnumerable<Region> GetVisibleRegions(Husk husk, HuskBrain brain)
        {
            throw new NotImplementedException();
        }

        private static IEnumerable<Character> GetVisibleCharacters(Husk husk)
        {
            Location location = husk.Location.GetLocation();

            switch (location.Type)
            {
                case LocationType.Area:
                    return Characters.Values.Where(x => x.DefaultLocation.Id == location.Id);

                case LocationType.Construct:
                    var construct = location as Construct;
                    var characters = Characters.Values.Where(x => x.DefaultLocation.Id == location.Id).ToList();

                    if (construct.Tag.HasFlag(ConstructType.Market))
                        characters.AddRange((location as Market).Npcs);

                    if (construct.Tag.HasFlag(ConstructType.Highrise))
                        characters.AddRange((location as Highrise).Npcs);

                    return Characters.Values.Where(x => x.DefaultLocation.Id == location.Id);

                case LocationType.Sector:
                case LocationType.Field:
                case LocationType.World:
                    return Characters.Values.Where(x =>
                        x.DefaultLocation.Id == location.Id
                        && CanSeePoint(husk, x.DefaultLocation.Vector));

                default:
                    throw new ArgumentException("The specified Husk is currently at a location that does not support NPCs.");
            }
        }

        public static bool IsInLocation<TLocation>(Husk husk, out TLocation location)
            where TLocation : Location
        {
            location = default;
            var current = husk.Location.GetLocation();

            if (current is TLocation)
            {
                location = current as TLocation;
                return true;
            }

            return false;
        }

        public static bool IsInLocation(Husk husk, LocationType type, out Location location)
        {
            location = null;
            var current = husk.Location.GetLocation();

            if (type.HasFlag(current.Type))
            {
                location = current;
                return true;
            }

            return false;
        }

        internal static EntityHitbox GetHitbox(Husk husk)
            => CreateHitbox(husk.Location.X,
                husk.Location.Y,
                husk.Status.Sight,
                husk.Status.Reach,
                husk.Location.GetInnerType());

        public static bool TryIdentifyStructure(Husk husk, HuskBrain brain, Structure structure)
        {
            if (structure != null)
            {
                if (brain.HasDiscoveredRegion(structure.Id))
                    return false;

                brain.IdentifyRegion(structure.Id);
                return true;
            }

            return false;
        }

        public static bool IsNearStructure(Husk husk, Structure structure)
        {
            EntityHitbox hitbox = GetHitbox(husk);

            if (structure != null)
                return IsNearStructure(hitbox, structure);

            return false;
        }

        public static bool CanSeePoint(Husk husk, Vector2 point)
        {
            EntityHitbox hitbox = GetHitbox(husk);

            return CanSeePoint(hitbox, point);
        }

        public static bool CanSeePoint(Husk husk, float x, float y)
        {
            EntityHitbox hitbox = GetHitbox(husk);

            return CanSeePoint(hitbox, x, y);
        }

        public static bool IsNearClosestStructure(Husk husk, out Structure closest)
        {
            EntityHitbox hitbox = GetHitbox(husk);
            closest = GetClosestStructure(husk);

            if (closest != null)
                return CanSeeStructure(hitbox, closest);

            return false;
        }

        internal static bool IsNearStructure(EntityHitbox hitbox, Structure structure)
            => IsNearRegion(hitbox, structure.Shape);

        internal static bool CanSeeStructure(EntityHitbox hitbox, Structure structure)
            => CanSeeRegion(hitbox, structure.Shape);

        internal static IEnumerable<Structure> GetVisibleStructures(EntityHitbox hitbox, IEnumerable<Structure> structures)
        {
            if (!(structures?.Count() > 0))
                yield break;

            foreach (Structure structure in structures)
            {
                if (CanSeeStructure(hitbox, structure))
                    yield return structure;
            }
        }

        internal static Structure GetClosestStructure(EntityHitbox hitbox, IEnumerable<Structure> structures)
        {
            Structure closest = null;
            float nearest = float.MaxValue;

            foreach (Structure structure in structures)
            {
                Line line = GetLineToRegion(hitbox.X, hitbox.Y, structure.Shape);
                float dist = line.GetLength();

                if (dist < nearest)
                {
                    nearest = dist;
                    closest = structure;
                }
            }

            return closest;
        }

        internal static IEnumerable<Structure> GetVisibleStructures(Husk husk, Sector sector)
            => GetVisibleStructures(GetHitbox(husk), sector.Structures);

        internal static Structure GetClosestStructure(Husk husk)
        {
            EntityHitbox hitbox = GetHitbox(husk);

            // FIELD
            // SECTOR
            // WORLD

            // AREA (all structures are instant)

            if (!IsInLocation(husk, out Sector sector))
                throw new ArgumentException("The specified Husk is not currently within a valid location.");

            // this change now only gets the closest VISIBLE structure
            // GetVisibleStructures(hitbox, sector.Structures)
            return GetClosestStructure(hitbox, sector.Structures);
        }

        private static Route CreateRoute(float x, float y, RegionF region)
            => new Route
            {
                Enforce = true,
                From = new Vector2(x, y),
                To = region.Midpoint
            };

        private static Route CreateRoute(float x, float y, float u, float v)
            => new Route
            {
                Enforce = true,
                From = new Vector2(x, y),
                To = new Vector2(u, v)
            };

        // TODO: Finish creating method.
        public static List<Route> GetRoutes(Husk husk)
        {
            // ensure that the husk travelling is in a location that supports routes.
            // routes are a path reference from one location to the other.
            // route progression is determined by if the path intersects with a barrier
            // otherwise, Routes can be placed, and could be enforced.
            // TODO: Account for possible barriers in the surrounding area.

            // use user's current position.
            var routes = new List<Route>();
            // if the user is in an AREA, do not account for travel time when listing CONSTRUCTS.
            // if the user is in a SECTOR, account for AREAS


            return routes;
            // foreach route, you would want to get the travel time based on position.
        }
    }
}
