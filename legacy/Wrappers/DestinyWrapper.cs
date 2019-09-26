using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;

namespace Orikivo.Wrappers
{
    /// <summary>
    /// Provides utility tools for conversion with Epoch timestamps.
    /// </summary>
    public class EpochHelper
    {
        public static DateTime EpochTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static DateTime ToEpochSecondsDateTime(long epoch)
        {
            return EpochTime.AddSeconds(epoch);
        }

        public static TimeSpan GetEpochDuration(long epoch)
        {
            return (DateTime.UtcNow - ToEpochSecondsDateTime(epoch));
        }

    }

    public class DestinyWrapper
    {
        private readonly string _token;
        private const string BaseUrl = "https://www.bungie.net/Platform/";
        //internal OriWebClient _webClient;

        public DestinyWrapper(string token)
        {
            //_webClient = new OriWebClient(BaseUrl);
        }

        public async Task<DestinyProfile> GetProfileAsync(long id, int type)
        {
            return null;
        }

        /*
         
        /Destiny2/ {membershipType} /Profile / {membershipId} /
         Responses:
            ErrorCode
            ThrottleSeconds
            ErrorStatus
            Message
                Type (string)
            MessageData
                Type (object)
                Dictionary Contents (string)
                Dictionary Key Type (string)

            DetailedErrorTrace
                Type (string)
            
         
         */
    }

    // DERIVABLE (To prevent duplicate properties)

    // Common sharing of DisplayProperties
    public class Displayable
    {
        [JsonProperty("displayProperties")]
        public DestinyDisplayPropertiesDefinition DisplayProperties { get; set; }
    }


    // PROPERTIES



    public class DestinyProfile
    {
        [JsonProperty("vendorReceipts")]
        public List<DestinyVendorReceipt> VendorReceipts { get; set; }

        [JsonProperty("profileInventory")]
        public DestinySingleInventoryComponent Inventory { get; set; }

        [JsonProperty("profileCurrencies")]
        public DestinySingleInventoryComponent Currencies { get; set; }

        // depends onf Component Profiles
        [JsonProperty("profile")]
        public DestinySingleProfileComponent Profile { get; set; }


    }

    public class DestinySingleProfileProgressionComponent
    {

    }

    public class DestinyProfileProgressionComponent
    {
        public Dictionary<uint, object> Checklists { get; set; }
    }

    // Complete?
    public class DestinyDisplayPropertiesDefinition
    {
        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("icon")]
        public string IconUrl { get; set; }

        [JsonProperty("hasIcon")]
        public bool HasIcon { get; set; }
    }

    public class DestinyChecklistDefinition
    {
        [JsonProperty("displayProperties")]
        public DestinyDisplayPropertiesDefinition DisplayProperties { get; set; }


        [JsonProperty("viewActionString")]
        public string ViewActionString { get; set; }

        [JsonProperty("scope")]
        public int Scope { get; set; }

        [JsonProperty("entries")]
        public List<DestinyChecklistEntryDefinition> Entries { get; set; }

        [JsonProperty("hash")]
        public uint Hash { get; set; }

        [JsonProperty("index")]
        public int Index { get; set; }

        [JsonProperty("redacted")]
        public bool Redacted { get; set; }

    }

    public class DestinyChecklistEntryDefinition
    {
        [JsonProperty("hash")]
        public uint Hash { get; set; }

        [JsonProperty("displayProperties")]
        public DestinyDisplayPropertiesDefinition DisplayProperties { get; set; }

        [JsonProperty("destinationHash")]
        public uint? DestinationHash { get; set; }

        [JsonProperty("locationHash")]
        public uint? LocationHash { get; set; }

        [JsonProperty("bubbleHash")]
        public uint? BubbleHash { get; set; }

        [JsonProperty("activityHash")]
        public uint? ActivityHash { get; set; }

        [JsonProperty("itemHash")]
        public uint? ItemHash { get; set; }

        [JsonProperty("vendorHash")]
        public uint? VendorHash { get; set; }




    }

    public class DestinySinglePlugSetsComponent
    {

    }

    public class DestinyPlugSetsComponent
    {

    }

    public class DestinySingleKiosksComponent
    {

    }

    public class DestinyKiosksComponent
    {

    }

    // complete?
    public class DestinySingleProfileComponent
    {
        [JsonProperty("data")]
        public DestinyProfileComponent Data { get; set; }

        [JsonProperty("privacy")]
        public int Privacy { get; set; }
    }

    //Complete?
    public class DestinyProfileComponent
    {
        [JsonProperty("userInfo")]
        public DestinyUserInfoCard Info { get; set; }

        [JsonProperty("dateLastPlayed")]
        public DateTime LastPlayed { get; set; }

        [JsonProperty("versionsOwned")]
        public int Versions { get; set; }

        [JsonProperty("characterIds")]
        public List<long> CharacterIds { get; set; }
    }

    //complete?
    public class DestinyUserInfoCard
    {
        [JsonProperty("supplementalDisplayName")]
        public string PlatformDisplayName { get; set; }

        public string IconUrl { get; set; }

        public bool HasIcon
        {
            get
            {
                return !string.IsNullOrWhiteSpace(IconUrl);
            }
        }

        [JsonProperty("membershipType")]
        public int MembershipType { get; set; }

        [JsonProperty("membershipId")]
        public long MembershipId { get; set; }

        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

    }

    //complete?
    public class DestinySingleVendorReceiptsComponent
    {
        [JsonProperty("data")]
        public DestinyVendorReceiptsComponent Data { get; set; }

        [JsonProperty("privacy")]
        public int Privacy { get; set; }
    }

    //complete?
    public class DestinyVendorReceiptsComponent
    {
        [JsonProperty("receipts")]
        public List<DestinyVendorReceipt> Receipts { get; set; }
    }

    // fix vendorflag
    public class DestinyVendorReceipt
    {
        [JsonProperty("currencyPaid")]
        public List<DestinyItemQuantity> Payment { get; set; }

        [JsonProperty("itemReceived")]
        public DestinyItemQuantity ItemReceived { get; set; }

        [JsonProperty("licenseUnlockHash")]
        public uint UnlockHash { get; set; }

        [JsonProperty("purchasedByCharacterId")]
        public long BuyerId { get; set; }

        [JsonProperty("refundPolicy")]
        private int _refundPolicy;

        public VendorItemRefundPolicy RefundPolicy
        {
            get
            {
                return (VendorItemRefundPolicy)_refundPolicy;
            }
        }

        [JsonProperty("sequenceNumber")]
        public int Sequence { get; set; }

        [JsonProperty("timeToExpiration")]
        private long _epochExpiration;

        public TimeSpan UntilExpiration
        {
            get
            {
                return EpochHelper.GetEpochDuration(_epochExpiration);
            }
        }

        [JsonProperty("expiresOn")]
        public DateTime ExpirationDate { get; set; }
    }

    //complete?
    public class DestinyItemQuantity
    {
        [JsonProperty("itemHash")]
        public uint Hash { get; set; }

        [JsonProperty("itemInstanceId")]
        public long? InstanceId { get; set; }

        public bool IsInstanced()
            => InstanceId.HasValue;

        [JsonProperty("quantity")]
        public int Quantity { get; set; }

    }

    //complete?
    public class DestinySingleInventoryComponent
    {
        [JsonProperty("data")]
        public DestinyInventoryComponent Data { get; set; }

        [JsonProperty("privacy")]
        public int Privacy { get; set; }
    }

    //complete?
    public class DestinyInventoryComponent
    {
        [JsonProperty("items")]
        public List<DestinyItemComponent> Items { get; set; }
    }

    //complete?
    public class DestinyItemComponent
    {
        [JsonProperty("itemHash")]
        public uint Hash { get; private set; }

        [JsonProperty("itemInstanceId")]
        public long? InstanceId { get; private set; }

        [JsonProperty("quantity")]
        private int _quantity;

        /*
         Note that Instanced items cannot stack. If an instanced item,
         this value will always be 1 (as the stack has exactly one item in it)
        */

        public bool IsInstanced { get { return InstanceId.HasValue; } }

        public int Quantity
        {
            get
            {
                if (InstanceId.HasValue)
                    return 1;
                return _quantity;
            }
        }

        [JsonProperty("bindStatus")]
        public int BindStatus { get; private set; }
        // if bound to a location

        [JsonProperty("location")]
        public int Location { get; private set; }

        [JsonProperty("transferStatus")]
        private int _transferStatus;

        public TransferStatus TransferStatus { get { return (TransferStatus)_transferStatus; } }

        [JsonProperty("lockable")]
        public bool IsLockable { get; set; }

        [JsonProperty("state")]
        private int _state;

        [JsonProperty("overrideStyleItemHash")]
        public uint? StyleHash { get; set; }

        public ItemState State { get { return (ItemState)_state; } }

        [JsonProperty("expirationDate")]
        public DateTime? ExpirationDate { get; set; }

        public bool HasExpired
        {
            get
            {
                if (ExpirationDate.HasValue)
                {
                    return DateTime.UtcNow > ExpirationDate;
                }
                return false;
            }
        }

    }

    public class DestinyInventoryItemDefinition
    {
        [JsonProperty("displayProperties")]
        public DestinyDisplayPropertiesDefinition DisplayProperties { get; set; }

        [JsonProperty("collectibleHash")]
        public uint? CollectibleHash { get; set; }

        [JsonProperty("secondaryIcon")]
        public string SecondaryIconUrl { get; set; }

        [JsonProperty("secondaryOverlay")]
        public string SecondaryIconOverlayUrl { get; set; }

        [JsonProperty("secondarySpecial")]
        public string SecondaryIconSpecialUrl { get; set; }

        [JsonProperty("backgroundColor")]
        private DestinyColor _bgColor;

        public Color BackgroundColor {
            get
            {
                return Color.FromArgb(_bgColor.Alpha, _bgColor.Red, _bgColor.Green, _bgColor.Blue);
            }
        }

        [JsonProperty("screenshot")]
        public string Screenshot { get; set; }

        public bool HasScreenshot()
        {
            return !string.IsNullOrWhiteSpace(Screenshot);
        }

        [JsonProperty("itemTypeDisplayName")]
        public string ItemTypeDisplayName { get; set; }

        [JsonProperty("uiItemDisplayStyle")]
        public string UiItemDisplayStyle { get; set; }

        [JsonProperty("itemTypeAndTierDisplayName")]
        public string TierTypeDisplayName { get; set; }

        [JsonProperty("displaySource")]
        public string DisplaySource { get; set; }

        public bool HasDisplaySource()
        {
            return !string.IsNullOrWhiteSpace(DisplaySource);
        }

        [JsonProperty("tooltipStyle")]
        public string TooltipStyle { get; set; }

        [JsonProperty("action")]
        public DestinyItemActionBlockDefinition Action { get; set; }

        [JsonProperty("inventory")]
        public DestinyItemInventoryBlockDefinition Inventory { get; set; }

        [JsonProperty("setData")]
        public DestinyItemSetBlockDefinition SetData { get; set; }

        [JsonProperty("stats")]
        public DestinyItemStatBlockDefinition Stats { get; set; }

        [JsonProperty("emblemObjectiveHash")]
        public uint? EmblemObjectiveHash { get; set; }

        [JsonProperty("equippingBlock")]
        public DestinyEquippingBlockDefinition EquippingBlock { get; set; }

        [JsonProperty("translationBlock")]
        public DestinyItemTranslationBlockDefinition TranslationBlock { get; set; }

        [JsonProperty("preview")]
        public DestinyItemPreviewBlockDefinition Preview { get; set; }

        [JsonProperty("quality")]
        public DestinyItemQualityBlockDefinition Quality { get; set; }


    }

    // I'm gonna cry myself to sleep ngl

    public class DestinyItemQualityBlockDefinition
    {

    }

    // complete?
    public class DestinyItemPreviewBlockDefinition
    {
        [JsonProperty("screenStyle")]
        public string ScreenStyle { get; set; }

        [JsonProperty("previewVendorHash")]
        public uint PreviewVendorHash { get; set; }

        [JsonProperty("previewActionString")]
        public string PreviewActionString { get; set; }

        [JsonProperty("derivedItemCategories")]
        public List<DestinyDerivedItemCategoryDefinition> DerivedItemCategories { get; set; }
    }

    // complete?
    public class DestinyDerivedItemCategoryDefinition
    {
        [JsonProperty("categoryDescription")]
        public string CategoryDescription { get; set; }

        [JsonProperty("items")]
        public List<DestinyDerivedItemDefinition> Items { get; set; }
    }

    // complete?
    public class DestinyDerivedItemDefinition
    {
        [JsonProperty("itemHash")]
        public uint? ItemHash { get; set; }

        [JsonProperty("itemName")]
        public string Name { get; set; }

        [JsonProperty("itemDetail")]
        public string Details { get; set; }

        [JsonProperty("itemDescription")]
        public string Description { get; set; }

        [JsonProperty("iconPath")]
        public string IconUrl { get; set; }

        [JsonProperty("vendorItemIndex")]
        public int VendorItemIndex { get; set; }

        public bool DerivesFromVendor()
        {
            return VendorItemIndex == -1;
        }
    }

    // complete?
    public class DestinyVendorDefinition
    {
        [JsonProperty("displayProperties")]
        public DestinyVendorDisplayPropertiesDefinition DisplayProperties { get; set; }

        [JsonProperty("buyString")]
        public string BuyText { get; set; }

        [JsonProperty("sellString")]
        public string SellText { get; set; }

        [JsonProperty("displayItemHash")]
        public uint DisplayItemHash { get; set; }

        [JsonProperty("inhibitBuying")]
        public bool CanBuyFrom { get; set; }

        [JsonProperty("inhibitSelling")]
        public bool CanSellFrom { get; set; }

        [JsonProperty("factionHash")]
        public uint FactionHash { get; set; }

        // in minutes
        [JsonProperty("resetIntervalMinutes")]
        public int ResetInterval { get; set; }

        // in minutes
        [JsonProperty("resetOffsetMinutes")]
        public int ResetOffset { get; set; }

        [JsonProperty("failureStrings")]
        public List<string> FailureStrings { get; set; }

        [JsonProperty("unlockRanges")]
        public List<DestinyDateRange> UnlockRanges { get; set; }

        // might be primarily useless...?
        [JsonProperty("vendorIdentifier")]
        public string VendorIdentifier { get; set; }

        [JsonProperty("vendorPortrait")]
        public string VendorPortraitUrl { get; set; }

        [JsonProperty("vendorBanner")]
        public string VendorBannerUrl { get; set; }

        [JsonProperty("enabled")]
        public bool Enabled { get; set; }

        [JsonProperty("visible")]
        public bool Visible { get; set; }

        [JsonProperty("vendorSubcategoryIdentifier")]
        public string VendorSubcategoryIdentifier { get; set; }

        [JsonProperty("consolidateCategories")]
        public bool ConsolidateCategories { get; set; }

        //unused as of now
        [JsonProperty("actions")]
        public List<DestinyVendorActionDefinition> Actions { get; set; }

        [JsonProperty("categories")]
        public List<DestinyVendorCategoryEntryDefinition> Categories { get; set; }

        [JsonProperty("originalCategories")]
        public List<DestinyVendorCategoryEntryDefinition> OriginalCategories { get; set; }

        [JsonProperty("displayCategories")]
        public List<DestinyDisplayCategoryDefinition> DisplayCategories { get; set; }

        [JsonProperty("interactions")]
        public List<DestinyVendorInteractionDefinition> Interactions { get; set; }

        [JsonProperty("inventoryFlyouts")]
        public List<DestinyVendorInventoryFlyoutDefinition> InventoryFlyouts { get; set; }

        [JsonProperty("itemList")]
        public List<DestinyVendorItemDefinition> Items { get; set; }

        [JsonProperty("services")]
        public List<DestinyVendorServiceDefinition> Services { get; set; }

        [JsonProperty("acceptedItems")]
        public List<DestinyVendorAcceptedItemDefinition> AcceptedItems { get; set; }

        [JsonProperty("returnWithVendorRequest")]
        public bool ReturnWithVendorRequest { get; set; }

        [JsonProperty("locations")]
        public List<DestinyVendorLocationDefinition> Locations { get; set; }

        [JsonProperty("groups")]
        public List<DestinyVendorGroupReference> Groups { get; set; }

        [JsonProperty("ignoreSaleItemHashes")]
        public List<uint> IgnoreSaleItemHashes { get; set; }

        [JsonProperty("hash")]
        public uint Hash { get; set; }

        [JsonProperty("index")]
        public int Index { get; set; }

        [JsonProperty("redacted")]
        public bool Redacted { get; set; }
    }

    // complete?
    public class DestinyVendorCategoryEntryDefinition
    {
        [JsonProperty("categoryIndex")]
        public int Index { get; set; }

        [JsonProperty("categoryId")]
        public string Id { get; set; }

        // dont recalc yourself...?
        [JsonProperty("sortValue")]
        public int SortValue { get; set; }

        [JsonProperty("categoryHash")]
        public uint CategoryHash { get; set; }

        [JsonProperty("quantityAvailable")]
        public int QuantityAvailable { get; set; }

        [JsonProperty("showUnavailableItems")]
        public bool ShowUnavailableItems { get; set; }

        [JsonProperty("hideIfNoCurrency")]
        public bool HideOnEmptyCurrency { get; set; }

        [JsonProperty("hideFromRegularPurchase")]
        public bool HideFromRegularPurchase { get; set; }

        [JsonProperty("buyStringOverride")]
        public string BuyTextOverride { get; set; }

        [JsonProperty("disabledDescription")]
        public string DisabledDescription { get; set; }

        [JsonProperty("displayTitle")]
        public string DisplayTitle { get; set; }

        [JsonProperty("overlay")]
        public DestinyVendorCategoryOverlayDefinition Overlay { get; set; }

        [JsonProperty("vendorItemIndexes")]
        public List<int> VendorItemIndexes { get; set; }

        [JsonProperty("isPreview")]
        public bool IsPreview { get; set; }

        [JsonProperty("isDisplayOnly")]
        public bool IsDisplayOnly { get; set; }

        [JsonProperty("resetIntervalMinutesOverride")]
        public int ResetIntervalOverride { get; set; }

        [JsonProperty("resetOffsetMinutesOverride")]
        public int ResetOffsetOverride { get; set; }
    }

    // complete?
    public class DestinyVendorCategoryOverlayDefinition
    {
        [JsonProperty("choiceDescription")]
        public string ChoiceDescription { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("icon")]
        public string IconUrl { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("currencyItemHash")]
        public uint? CurrencyItemHash { get; set; }
    }

    // complete?
    public class DestinyDisplayCategoryDefinition
    {
        [JsonProperty("identifier")]
        public string Identifier { get; set; }

        [JsonProperty("displayCategoryHash")]
        public uint DisplayCategoryHash { get; set; }

        [JsonProperty("displayProperties")]
        public DestinyDisplayPropertiesDefinition DisplayProperties { get; set; }

        [JsonProperty("displayInBanner")]
        public bool DisplayInBanner { get; set; }

        [JsonProperty("progressionHash")]
        public uint? ProgressionHash { get; set; }

        [JsonProperty("sortOrder")]
        public int SortOrder { get; set; }

        [JsonProperty("displayStyleHash")]
        public uint DisplayStyleHash { get; set; }

        [JsonProperty("displayStyleIndentifier")]
        public string DisplayStyleIdentifier { get; set; }
    }

    // complete?
    public class DestinyVendorInteractionDefinition
    {
        [JsonProperty("interactionIndex")]
        public int InteractionIndex { get; set; }

        [JsonProperty("replies")]
        public List<DestinyVendorInteractionReplyDefinition> Replies { get; set; }

        [JsonProperty("vendorCategoryIndex")]
        public int VendorCategoryIndex { get; set; }

        [JsonProperty("questlineItemHash")]
        public uint QuestlineItemHash { get; set; }

        [JsonProperty("sackInteractionList")]
        public List<DestinyVendorInteractionSackEntryDefinition> SackInteractions { get; set; }

        [JsonProperty("uiInteractionType")]
        public uint UiInteractionType { get; set; }

        [JsonProperty("interactionType")]
        private int _interactionType;

        public InteractionType InteractionType
        {
            get
            {
                return (InteractionType)_interactionType;
            }
        }

        [JsonProperty("rewardBlockLabel")]
        public string RewardBlockLabel { get; set; }

        [JsonProperty("rewardVendorCategoryIndex")]
        public int RewardVendorCategoryIndex { get; set; }

        [JsonProperty("flavorLineOne")]
        public string FlavorTextStart { get; set; }

        [JsonProperty("flavorLineTwo")]
        public string FlavorTextEnd { get; set; }

        [JsonProperty("headerDisplayProperties")]
        public DestinyDisplayPropertiesDefinition HeaderDisplayProperties { get; set; }

        [JsonProperty("instructions")]
        public string Instructions { get; set; }
    }

    // complete.
    public class DestinyVendorInteractionSackEntryDefinition
    {
        [JsonProperty("sackType")]
        public uint SackType { get; set; }
    }

    // complete.
    public class DestinyVendorInteractionReplyDefinition
    {
        [JsonProperty("itemRewardsSelection")]
        public int ItemRewardsSelection { get; set; }

        [JsonProperty("reply")]
        public string Reply { get; set; }

        [JsonProperty("replyType")]
        private int _replyType { get; set; }

        public VendorReplyType ReplyType
        {
            get
            {
                return (VendorReplyType)_replyType;
            }
        }

    }

    // complete.
    public class DestinyVendorInventoryFlyoutDefinition
    {
        [JsonProperty("lockedDescription")]
        public string LockedDescription { get; set; }

        [JsonProperty("displayProperties")]
        public DestinyDisplayPropertiesDefinition DisplayProperties { get; set; }

        [JsonProperty("buckets")]
        public List<DestinyVendorInventoryFlyoutBucketDefinition> Buckets { get; set; }

        [JsonProperty("flyoutId")]
        public uint FlyoutId { get; set; }

        [JsonProperty("suppressNewness")]
        public bool SupressOnNew { get; set; }

        [JsonProperty("equipmentSlotHash")]
        public uint EquipmentSlotHash { get; set; }
    }

    // complete.
    public class DestinyVendorInventoryFlyoutBucketDefinition
    {
        [JsonProperty("collapsible")]
        public bool CanCollapse { get; set; }

        [JsonProperty("inventoryBucketHash")]
        public uint InventoryBucketHash { get; set; }

        [JsonProperty("sortItemsBy")]
        public int SortBy { get; set; }
    }

    public class DestinyVendorItemDefinition
    {
        [JsonProperty("vendorItemIndex")]
        public int VendorItemIndex { get; set; }

        [JsonProperty("itemHash")]
        public uint ItemHash { get; set; }

        [JsonProperty("quantity")]
        public int Quantity { get; set; }

        [JsonProperty("failureIndexes")]
        public List<int> FailureIndexes { get; set; }

        [JsonProperty("currencies")]
        public List<DestinyVendorItemQuantity> Currencies { get; set; }

        [JsonProperty("refundPolicy")]
        public int RefundPolicy { get; set; }

        [JsonProperty("refundTimeLimit")]
        public int RefundDuration { get; set; }

        [JsonProperty("creationLevels")]
        public List<DestinyItemCreationEntryLevelDefinition> CreationLevels { get; set; }

        [JsonProperty("displayCategoryIndex")]
        public int DisplayCategoryIndex { get; set; }

        [JsonProperty("categoryIndex")]
        public int CategoryIndex { get; set; }

        [JsonProperty("originalCategoryIndex")]
        public int OriginalCategoryIndex { get; set; }

        [JsonProperty("minimumLevel")]
        public int MinLevel { get; set; }

        [JsonProperty("maximumLevel")]
        public int MaxLevel { get; set; }

        [JsonProperty("action")]
        public DestinyVendorSaleItemActionBlockDefinition Action { get; set; }

        [JsonProperty("displayCategory")]
        public string DisplayCategory { get; set; }

        [JsonProperty("inventoryBucketHash")]
        public uint InventoryBucketHash { get; set; }

        //incorporate DestinyGatingScope?
        [JsonProperty("visibilityScope")]
        public int VisibilityScope { get; set; }

        //incorporate DestinyGatingScope?
        [JsonProperty("purchasableScope")]
        public int PurchasableScope { get; set; }

        [JsonProperty("exclusivity")]
        public int Exclusivity { get; set; }

        [JsonProperty("isOffer")]
        public bool? IsOffer { get; set; }

        [JsonProperty("isCrm")]
        public bool? IsCrm { get; set; }

        [JsonProperty("sortValue")]
        public int SortValue { get; set; }

        [JsonProperty("expirationTooltip")]
        public string TooltipExpirationText { get; set; }

        [JsonProperty("redirectToSaleIndexes")]
        public List<int> SaleRedirectIndexes { get; set; }

        [JsonProperty("socketOverrides")]
        public List<DestinyVendorItemSocketOverride> SocketOverrides { get; set; }
    }

    // complete?
    public class DestinyVendorSaleItemActionBlockDefinition
    {
        [JsonProperty("executeSeconds")]
        public float ExecutionDuration { get; set; }

        [JsonProperty("isPositive")]
        public bool IsPositive { get; set; }
    }

    public class DestinyVendorItemSocketOverride
    {
        [JsonProperty("singleItemHash")]
        public uint? SingleItemHash { get; set; }

        // if greater than -1, randomizes...
        [JsonProperty("randomizedOptionsCount")]
        public int RandomOptionsCounter { get; set; }

        [JsonProperty("socketTypeHash")]
        public uint SocketTypeHash { get; set; }
    }

    // complete?
    public class DestinySocketTypeDefinition
    {
        [JsonProperty("displayProperties")]
        public DestinyDisplayPropertiesDefinition DisplayProperties { get; set; }

        [JsonProperty("insertAction")]
        public DestinyInsertPlugActionDefinition InsertAction { get; set; }

        [JsonProperty("plugWhitelist")]
        public List<DestinyPlugWhitelistEntryDefinition> PlugWhitelist { get; set; }

        [JsonProperty("socketCategoryHash")]
        public uint SocketCategoryHash { get; set; }

        [JsonProperty("visibility")]
        public int Visibility { get; set; }

        [JsonProperty("alwaysRandomizeSockets")]
        public bool SocketAlwaysRandom { get; set; }

        [JsonProperty("isPreviewEnabled")]
        public bool IsInPreview { get; set; }

        [JsonProperty("overridesUiAppearance")]
        public bool UiAppearanceOverride { get; set; }

        [JsonProperty("avoidDuplicatesOnInitialization")]
        public bool AvoidDupesOnStart { get; set; }

        [JsonProperty("currencyScalars")]
        public List<DestinySocketTypeScalarMaterialRequirementEntry> CurrencyScalars { get; set; }

        [JsonProperty("hash")]
        public uint Hash { get; set; }

        [JsonProperty("index")]
        public int Index { get; set; }

        [JsonProperty("redacted")]
        public bool Redacted { get; set; }

    }

    public class DestinySocketTypeScalarMaterialRequirementEntry
    {

    }

    public class DestinyPlugWhitelistEntryDefinition
    {

    }

    public class DestinyInsertPlugActionDefinition
    {

    }

    // complete.
    public class DestinyItemCreationEntryLevelDefinition
    {
        [JsonProperty("level")]
        public int Level { get; set; }
    }

    public class DestinyVendorItemQuantity
    {

    }

    // complete.
    public class DestinyVendorServiceDefinition
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }

    // complete.
    public class DestinyVendorAcceptedItemDefinition
    {
        [JsonProperty("acceptedInventoryBucketHash")]
        public uint AcceptedInventoryBucketHash { get; set; }

        [JsonProperty("destinationInventoryBucketHash")]
        public uint DestinationInventoryBucketHash { get; set; }
    }

    // complete.
    public class DestinyVendorLocationDefinition
    {
        [JsonProperty("destinationHash")]
        public uint DestinationHash { get; set; }

        [JsonProperty("backgroundImagePath")]
        public string BackgroundImageUrl { get; set; }
    }

    // complete.
    public class DestinyVendorGroupReference
    {
        [JsonProperty("vendorGroupHash")]
        public uint VendorGroupHash { get; set; }
    }

    // complete.
    public class DestinyVendorGroupDefinition
    {
        [JsonProperty("order")]
        public int Order { get; set; }

        [JsonProperty("categoryName")]
        public string CategoryName { get; set; }

        [JsonProperty("hash")]
        public uint Hash { get; set; }

        [JsonProperty("index")]
        public int Index { get; set; }

        [JsonProperty("redacted")]
        public bool Redacted { get; set; }
    }

    // complete.
    public class DestinyVendorActionDefinition
    {
        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("executeSeconds")]
        public int ExecuteSeconds { get; set; }

        [JsonProperty("icon")]
        public string IconUrl { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("verb")]
        public string Verb { get; set; }

        [JsonProperty("isPositive")]
        public bool IsPositive { get; set; }

        [JsonProperty("actionId")]
        public string ActionId { get; set; }

        [JsonProperty("actionHash")]
        public uint ActionHash { get; set; }

        [JsonProperty("autoPerformAction")]
        public bool Automated { get; set; }
    }

    // complete.
    public class DestinyDateRange
    {
        [JsonProperty("start")]
        public DateTime Start { get; set; }

        [JsonProperty("end")]
        public DateTime End { get; set; }
    }

    // complete?
    public class DestinyFactionDefinition
    {
        [JsonProperty("displayProperties")]
        public DestinyDisplayPropertiesDefinition DisplayProperties { get; set; }

        [JsonProperty("progressionHash")]
        public uint ProgressionHash { get; set; }

        [JsonProperty("tokenValues")]
        public Dictionary<uint, uint> TokenValues { get; set; }

        [JsonProperty("rewardItemHash")]
        public uint RewardItemHash { get; set; }

        [JsonProperty("rewardVendorHash")]
        public uint RewardVendorHash { get; set; }

        [JsonProperty("vendors")]
        public List<DestinyFactionVendorDefinition> Vendors { get; set; }

        [JsonProperty("hash")]
        public uint Hash { get; set; }

        [JsonProperty("index")]
        public int Index { get; set; }

        [JsonProperty("redacted")]
        public bool Redacted { get; set; }
    }

    // complete?
    public class DestinyFactionVendorDefinition
    {
        [JsonProperty("vendorHash")]
        public uint VendorHash { get; set; }

        [JsonProperty("destinationHash")]
        public uint DestinationHash { get; set; }

        [JsonProperty("backgroundImagePath")]
        public string BackgroundImagePath { get; set; }
    }

    // complete?
    public class DestinyDestinationDefinition
    {
        [JsonProperty("displayProperties")]
        public DestinyDisplayPropertiesDefinition DisplayProperties { get; set; }

        [JsonProperty("placeHash")]
        public uint PlaceHash { get; set; }

        [JsonProperty("defaultFreeroamActivityHash")]
        public uint DefaultFreeroamHash { get; set; }

        [JsonProperty("activityGraphEntries")]
        public List<DestinyActivityGraphListEntryDefinition> ActivityGraphEntries { get; set; }

        [JsonProperty("bubbleSettings")]
        public List<DestinyDestinationBubbleSettingDefinition> BubbleSettings { get; set; }

        [JsonProperty("bubbles")]
        public List<DestinyBubbleDefinition> Bubbles { get; set; }

        [JsonProperty("hash")]
        public uint Hash { get; set; }

        [JsonProperty("index")]
        public int Index { get; set; }

        [JsonProperty("redacted")]
        public bool Redacted { get; set; }
    }

    // complete?
    public class DestinyBubbleDefinition
    {
        [JsonProperty("hash")]
        public uint Hash { get; set; }

        [JsonProperty("displayProperties")]
        public DestinyDisplayPropertiesDefinition DisplayProperties { get; set; }
    }

    // complete? >>>> DEPRECATED
    public class DestinyDestinationBubbleSettingDefinition
    {
        [JsonProperty("displayProperties")]
        public DestinyDisplayPropertiesDefinition DisplayProperties { get; set; }
    }

    public class DestinyActivityGraphListEntryDefinition
    {

    }

    public class DestinyActivityDefinition
    {

    }

    public class DestinyPlaceDefinition
    {

    }

    // complete?
    public class DestinyProgressionDefinition
    {
        [JsonProperty("displayProperties")]
        public DestinyProgressionDisplayPropertiesDefinition DisplayProperties { get; set; }

        [JsonProperty("scope")]
        private int _scope;

        public DestinyProgressionScope Scope
        {
            get
            {
                return (DestinyProgressionScope)_scope;
            }
        }

        [JsonProperty("repeatLastStep")]
        public bool RepeatsLast { get; set; }

        [JsonProperty("source")]
        public string Source { get; set; }

        [JsonProperty("steps")]
        public List<DestinyProgressionStepDefinition> Steps { get; set; }

        [JsonProperty("visible")]
        public bool Visible { get; set; }

        [JsonProperty("factionHash")]
        public uint? FactionHash { get; set; }

        [JsonProperty("color")]
        private DestinyColor _color;

        public Color Color
        {
            get
            {
                return Color.FromArgb(_color.Alpha, _color.Red, _color.Green, _color.Blue);
            }
        }

        [JsonProperty("rankIcon")]
        public string RankIcon { get; set; }

        [JsonProperty("hash")]
        public uint Hash { get; set; }

        [JsonProperty("index")]
        public int Index { get; set; }

        [JsonProperty("redacted")]
        public bool Redacted { get; set; }
    }

    // complete?
    public class DestinyProgressionStepDefinition
    {
        [JsonProperty("stepName")]
        public string StepName { get; set; }

        [JsonProperty("displayEffectType")]
        private int _displayEffectType { get; set; }

        public DestinyProgressionStepDisplayEffect DisplayEffectType
        {
            get
            {
                return (DestinyProgressionStepDisplayEffect)_displayEffectType;
            }
        }

        [JsonProperty("progressTotal")]
        public int TotalProgression { get; set; }

        [JsonProperty("rewardItems")]
        public List<DestinyItemQuantity> RewardItems { get; set; }

        [JsonProperty("icon")]
        public string Icon { get; set; }
    }

    // complete?
    public class DestinyProgressionDisplayPropertiesDefinition
    {
        [JsonProperty("displayUnitsName")]
        public string DisplayUnitName { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("icon")]
        public string IconUrl { get; set; }

        [JsonProperty("hasIcon")]
        public bool HasIcon { get; set; }
    }

    // complete?
    public class DestinyVendorDisplayPropertiesDefinition
    {
        [JsonProperty("largeIcon")]
        public string MediumIconUrl { get; set; }

        [JsonProperty("subtitle")]
        public string Subtitle { get; set; }

        [JsonProperty("originalIcon")]
        public string DefaultIconUrl { get; set; }

        [JsonProperty("requirementsDisplay")]
        public List<DestinyVendorRequirementDisplayEntryDefinition> RequirementsDisplay { get; set; }

        [JsonProperty("smallTransparentIcon")]
        public string SmallTransparentIconUrl { get; set; }

        [JsonProperty("mapIcon")]
        public string MapIcon { get; set; }

        [JsonProperty("largeTransparentIcon")]
        public string LargeTransparentIconUrl { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("icon")]
        public string IconUrl { get; set; }

        [JsonProperty("hasIcon")]
        public bool HasIcon { get; set; }
    }

    // complete?
    public class DestinyVendorRequirementDisplayEntryDefinition
    {
        [JsonProperty("icon")]
        public string IconUrl { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("source")]
        public string Source { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }

    // complete?
    public class DestinyItemTranslationBlockDefinition
    {
        [JsonProperty("weaponPatternIdentifier")]
        public string WeaponPatternIdentifier { get; set; }

        [JsonProperty("WeaponPatternHash")]
        public uint WeaponPatternHash { get; set; }

        [JsonProperty("defaultDyes")]
        public List<DestinyDyeReference> DefaultDyes { get; set; }

        [JsonProperty("lockedDyes")]
        public List<DestinyDyeReference> LockedDyes { get; set; }

        [JsonProperty("customDyes")]
        public List<DestinyDyeReference> CustomDyes { get; set; }

        [JsonProperty("arrangements")]
        public List<DestinyGearArtArrangementReference> Arrangements { get; set; }

        [JsonProperty("hasGeometry")]
        public bool HasGeometry { get; set; }
    }

    // complete?
    public class DestinyGearArtArrangementReference
    {
        [JsonProperty("classHash")]
        public uint ClassHash { get; set; }

        [JsonProperty("artArrangementHash")]
        public uint ArtArrangementHash { get; set; }
    }

    // complete?
    public class DestinyDyeReference
    {
        [JsonProperty("channelHash")]
        public uint ChannelHash { get; set; }

        [JsonProperty("dyeHash")]
        public uint DyeHash { get; set; }
    }

    // maybe allow uints as a class constructor...?

    // complete?
    public class DestinyEquippingBlockDefinition
    {
        [JsonProperty("gearsetItemHash")]
        public uint GearSetItemHash { get; set; }

        [JsonProperty("uniqueLabel")]
        public string UniqueLabel { get; set; }

        [JsonProperty("uniqueLabelHash")]
        public uint UniqueLabelHash { get; set; }

        [JsonProperty("equipmentSlotTypeHash")]
        public uint EquipmentSlotTypeHash { get; set; }

        [JsonProperty("attributes")]
        public int Attributes { get; set; }

        [JsonProperty("ammoType")]
        private int _ammoType;

        public AmmunitionType AmmunitionType
        {
            get
            {
                return (AmmunitionType)_ammoType;
            }
        }

        [JsonProperty("displayStrings")]
        public List<string> DisplayStrings { get; set; }
    }

    // complete?
    public class DestinyEquipmentSlotDefinition
    {
        [JsonProperty("displayProperties")]
        public DestinyDisplayPropertiesDefinition DisplayProperties { get; set; }

        [JsonProperty("equipmentCategoryHash")]
        public uint EquipmentCategoryHash { get; set; }

        [JsonProperty("bucketTypeHash")]
        public uint BucketTypeHash { get; set; }

        [JsonProperty("applyCustomArtDyes")]
        public bool ApplyCustomArtDyes { get; set; }

        [JsonProperty("artDyeChannels")]
        public List<DestinyArtDyeReference> ArtDyeChannels { get; set; }

        [JsonProperty("hash")]
        public uint Hash { get; set; }

        [JsonProperty("index")]
        public int Index { get; set; }

        [JsonProperty("redacted")]
        public bool Redacted { get; set; }
    }

    // complete?
    public class DestinyArtDyeReference
    {
        [JsonProperty("artDyeChannelHash")]
        public int ChannelHash { get; set; }
    }

    public class DestinyStatDefinition
    {

    }

    // complete?
    public class DestinyItemStatBlockDefinition
    {
        [JsonProperty("disablePrimaryStatDisplay")]
        public bool DisablePrimaryStatDisplay { get; set; }

        [JsonProperty("statGroupHash")]
        public uint? StatGroupHash { get; set; }

        [JsonProperty("stats")]
        public Dictionary<uint, DestinyInventoryItemStatDefinition> Stats { get; set; }

        [JsonProperty("hasDisplayableStats")]
        public bool HasVisibleStats { get; set; }

        [JsonProperty("primaryBaseStatHash")]
        public uint PrimaryBaseStatHash { get; set; }
    }

    // complete?
    public class DestinyInventoryItemStatDefinition
    {
        [JsonProperty("statHash")]
        public uint StatHash { get; set; } // hash to DestinyStatDefinition

        [JsonProperty("value")]
        public int Value { get; set; }

        [JsonProperty("minimum")]
        public int Min { get; set; }

        // consider as deprecated
        [JsonProperty("maximum")]
        public int Max { get; }
    }

    // complete?
    public class DestinyStatGroupDefinition
    {
        [JsonProperty("maximumValue")]
        public int MaxValue { get; set; }

        // uncertain
        [JsonProperty("uiPosition")]
        public int UiPosition { get; set; }

        [JsonProperty("scaledStats")]
        public List<DestinyStatDisplayDefinition> ScaledStats { get; set; }

        [JsonProperty("overrides")]
        public Dictionary<uint, DestinyStatOverrideDefinition> Overrides { get; set; }

        [JsonProperty("hash")]
        public uint Hash { get; set; }

        [JsonProperty("index")]
        public int Index { get; set; }

        [JsonProperty("redacted")]
        public bool Redacted { get; set; }
    }

    // WHY IS THIS SO FREAKING LONG HOLY CRAP

    // complete?
    public class DestinyStatOverrideDefinition
    {
        [JsonProperty("statHash")]
        public uint StatHash { get; set; }

        [JsonProperty("displayProperties")]
        public DestinyDisplayPropertiesDefinition DisplayProperties { get; set; }
    }

    // complete?
    public class DestinyStatDisplayDefinition
    {
        [JsonProperty("statHash")]
        public uint StatHash { get; set; }

        // min is always 0
        [JsonProperty("maximumValue")]
        public int MaxValue { get; set; }

        [JsonProperty("displayAsNumeric")]
        public bool ShowAsNumeric { get; set; }

        [JsonProperty("displayInterpolation")]
        public List<DestinyInterpolationPoint> DisplayInterpolation { get; set; }
    }

    // complete?
    public class DestinyInterpolationPoint
    {
        [JsonProperty("value")]
        public int Value { get; set; }

        [JsonProperty("weight")]
        public int Weight { get; set; }
    }

    // complete?
    public class DestinyItemSetBlockDefinition
    {
        [JsonProperty("itemList")]
        public List<DestinyItemSetBlockEntryDefinition> ItemList { get; set; }

        [JsonProperty("requireOrderedSetItemAdd")]
        public bool RequireOrderedSetItemAddition { get; set; }

        [JsonProperty("setIsFeatured")]
        public bool IsFeaturedSet { get; set; }

        [JsonProperty("setType")]
        public string SetType { get; set; }
    }

    // complete?
    public class DestinyItemSetBlockEntryDefinition
    {
        [JsonProperty("trackingValue")]
        public int TrackingValue { get; set; }

        [JsonProperty("itemHash")]
        public uint ItemHash { get; set; }
    }

    // complete?
    public class DestinyItemInventoryBlockDefinition
    {
        [JsonProperty("stackUniqueLabel")]
        public string StackUniqueLabel { get; set; }

        [JsonProperty("maxStackSize")]
        public int StackLimit { get; set; }

        [JsonProperty("bucketTypeHash")]
        public uint BucketTypeHash { get; set; }

        [JsonProperty("recoveryBucketTypeHash")]
        public uint RecoveryBucketTypeHash { get; set; }

        [JsonProperty("tierTypeHash")]
        public uint TierTypeHash { get; set; }

        [JsonProperty("isInstanceItem")]
        public bool IsInstanced { get; set; }

        [JsonProperty("tierTypeName")]
        public string TierTypeName { get; set; }

        [JsonProperty("tierType")]
        public TierType TierType { get; set; }

        [JsonProperty("expirationTooltip")]
        public string TooltipWhenExpired { get; set; }

        [JsonProperty("expiredInActivityMessage")]
        public string ActivityExpirationMessage { get; set; }

        [JsonProperty("expiredInOrbitMessage")]
        public string OrbitExpirationMessage { get; set; }

        [JsonProperty("suppressExpirationWhenObjectivesComplete")]
        public bool SuppressOnCompletion { get; set; }
    }

    // complete?
    public class DestinyInventoryBucketDefinition
    {
        [JsonProperty("displayProperties")]
        public DestinyDisplayPropertiesDefinition DisplayProperties { get; set; }

        [JsonProperty("scope")]
        private int _scope;

        public BucketScope Scope
        {
            get
            {
                return (BucketScope)_scope;
            }
        }

        [JsonProperty("category")]
        private int _category;

        public BucketCategory Category
        {
            get
            {
                return (BucketCategory)_category;
            }
        }

        [JsonProperty("bucketOrder")]
        public int BucketOrder { get; set; }

        [JsonProperty("itemCount")]
        public int ItemSlotLimit { get; set; }

        [JsonProperty("location")]
        public int Location { get; set; }

        [JsonProperty("hasTransferDestination")]
        public bool HasTransferDestination { get; set; }

        [JsonProperty("enabled")]
        public bool Enabled { get; set; }

        [JsonProperty("fifo")]
        public bool Fifo { get; set; }

        [JsonProperty("hash")]
        public uint Hash { get; set; }

        [JsonProperty("index")]
        public int Index { get; set; }

        [JsonProperty("redacted")]
        public bool Redacted { get; set; }

    }

    // complete?
    public class DestinyItemActionBlockDefinition
    {
        [JsonProperty("verbName")]
        public string Name { get; set; }

        [JsonProperty("verbDescription")]
        public string Description { get; set; }

        [JsonProperty("isPositive")]
        public bool IsPositive { get; set; }

        [JsonProperty("overlayScreenName")]
        public string OverlayScreenName { get; set; }

        [JsonProperty("overlayIcon")]
        public string OverlayIcon { get; set; }

        [JsonProperty("requiredCooldownSeconds")]
        private int _cooldownSeconds;

        public TimeSpan CooldownDuration
        {
            get
            {
                return TimeSpan.FromSeconds(_cooldownSeconds);
            }
        }

        [JsonProperty("requiredItems")]
        public List<DestinyActionRequiredItemDefinition> RequiredItems { get; set; }

        [JsonProperty("progressionRewards")]
        public List<DestinyProgressionRewardDefinition> ProgressionRewards { get; set; }

        [JsonProperty("actionTypeLabel")]
        public string ActionTypeLabel { get; set; }

        //useless atm
        [JsonProperty("requiredLocation")]
        public string RequiredLocation { get; set; }

        [JsonProperty("requiredCooldownHash")]
        public uint CooldownHash { get; set; }

        [JsonProperty("deleteOnAction")]
        public bool DeleteOnCompletion { get; set; }

        [JsonProperty("consumeEntireStack")]
        public bool ConsumeEntireStack { get; set; }

        [JsonProperty("useOnAcquire")]
        public bool UseOnPickUp { get; set; }

    }

    // complete?
    public class DestinyProgressionRewardDefinition
    {
        [JsonProperty("progressionMappingHash")]
        public uint ProgressionMappingHash { get; set; }

        [JsonProperty("amount")]
        public int Amount { get; set; }

        [JsonProperty("applyThrottles")]
        public bool ApplyThrottles { get; set; }
    }

    // complete?
    public class DestinyProgressionMappingDefinition
    {
        [JsonProperty("displayProperties")]
        public DestinyDisplayPropertiesDefinition DisplayProperties { get; set; }

        [JsonProperty("displayUnits")]
        public string DisplayUnits { get; set; }

        [JsonProperty("hash")]
        public uint Hash { get; set; }

        [JsonProperty("index")]
        public int Index { get; set; }

        [JsonProperty("redacted")]
        public bool Redacted { get; set; }
    }

    // complete?
    public class DestinyActionRequiredItemDefinition
    {
        [JsonProperty("count")]
        public int Count { get; set; }

        [JsonProperty("itemHash")]
        public uint Hash { get; set; } // direct to InventoryItemDefinition

        [JsonProperty("deleteOnAction")]
        public bool DeleteOnCompletion { get; set; }
    }

    // complete?
    public class DestinyColor
    {
        [JsonProperty("red")]
        public byte Red { get; set; }

        [JsonProperty("green")]
        public byte Green { get; set; }

        [JsonProperty("blue")]
        public byte Blue { get; set; }

        [JsonProperty("alpha")]
        public byte Alpha { get; set; }
    }

    public enum DestinyGatingScope
    {
        None = 0,
        Global = 1,
        Clan = 2,
        Profile = 3,
        Character = 4,
        Item = 5,
        AssumedWorstCase = 6
    }

    public enum InteractionType
    {

    }

    // complete.
    public enum VendorReplyType
    {
        Accept = 0,
        Decline = 1,
        Complete = 2
    }

    public enum DestinyProgressionStepDisplayEffect
    {

    }

    public enum DestinyProgressionScope
    {

    }

    // complete?
    public enum AmmunitionType
    {
        None = 0,
        Primary = 1,
        Special = 2,
        Heavy = 3,
        Unknown = 4
    }

    public enum BucketCategory
    {

    }

    // complete?
    public enum BucketScope
    {
        Character = 0,
        Account = 1
    }

    // complete?
    public enum TierType
    {
        Unknown = 0,
        Currency = 1,
        Basic = 2,
        Common = 3,
        Rare = 4,
        Superior = 5,
        Exotic = 6
    }

    public enum VendorItemRefundPolicy
    {

    }

    // complete?
    public enum ItemState
    {
        None = 0,
        Locked = 1,
        Tracked = 2,
        Masterwork = 4
    }

    //complete?
    public enum TransferStatus
    {
        CanTransfer = 0,
        ItemIsEquipped = 1,
        NotTransferrable = 2,
        NoRoomInDestination = 4
    }

    //complete?
    public enum ItemBindStatus
    {
        NotBound = 0,
        BoundToCharacter = 1,
        BoundToAccount = 2,
        BoundToGuild = 3
    }
}
