using Newtonsoft.Json;
using Orikivo.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Orikivo
{
    // Essentially complete at this point. New option additions can be added on.

    /// <summary>
    /// Represents the inner configuration of an Account.
    /// </summary>
    public class AccountOptions
    {
        /// <summary>
        /// Constructs a new AccountOptions using its default values.
        /// </summary>
        public AccountOptions()
        {
            AutoFix = false;
            Overflow = false;
            IgnoreTooltips = false;
            Nickname = null;
            Prefix = null;
            ClosingPrefix = null;
            LocaleBlacklist = new List<string>();
            SiteBlacklist = new List<string>();
            AllowLinks = true;
            Exceptions = true;
            SafeGuard = true;
            OutputFormat = OutputFormat.Pixel;
            EmptyFormat = NullObjectHandling.Include;
            Visibility = Visibility.Public;
            SledgePower = null;
            IconFormat = IconFormat.Portable;
            Locale = null;
            PreDecode = false;
            GlobalDecoder = false;
            IgnoreDirectional = false;
            UsernameFormat = NameFormat.Default;
            MatchHandling = MatchHandling.Match;
            Risk = 0;
            ColorFormat = ColorFormat.RGB;
            WordGuard = false;
            WordGuardHandling = WordGuardControl.Inactive;
            SearchHandling = MatchValueHandling.Equals;
            SymbolNameDisplay = true;
        }

        [JsonIgnore]
        public static AccountOptions Default = new AccountOptions();

        [JsonIgnore]
        public static List<PropertyInfo> DefaultProperties { get { return typeof(AccountOptions).GetProperties().ToList(); } }

        #region Properties

        [JsonIgnore]
        private List<PropertyInfo> _properties;

        [JsonIgnore]
        private List<PropertyInfo> Properties
        {
            get
            {
                TryLoadProperties();
                return _properties;
            }
            set
            {
                _properties = value;
            }
        }

        [JsonIgnore]
        public List<PropertyInfo> ValidProperties { get { return Properties.Where(x => x.TryGetAccountOption(out AccountOption option)).ToList(); } }

        private void TryLoadProperties()
        {
            if (!_properties.Exists())
                _properties = GetType().GetProperties().ToList();
        }

        public bool TryGetOptionProperty(AccountOption ao, out PropertyInfo property)
        {
            property = null;

            foreach (PropertyInfo p in Properties)
            {
                AccountOptionAttribute attribute = p.GetCustomAttribute<AccountOptionAttribute>();
                if (attribute.Exists())
                {
                    if (ao.Equals(attribute.Option))
                    {
                        property = p;
                        "".Debug($"Found Property {property.Name}");
                        return true;
                    }
                }
            }

            return false;
        }

        public bool TryGetType(AccountOption ao, out Type type)
        {
            bool success = TryGetOptionProperty(ao, out PropertyInfo property);
            type = property.Exists() ? property.PropertyType : null;
            return success;
        }

        public bool TryGetValue(AccountOption ao, out object value)
        {
            bool success = TryGetOptionProperty(ao, out PropertyInfo property);
            value = property.Exists() ? property.GetValue(this) : null;
            return success;
        }

        public bool TrySetValue(AccountOption ao, object value = null)
        {
            if (TryGetOptionProperty(ao, out PropertyInfo property))
            {
                object obj = property.GetValue(this);
                Type objT = property.PropertyType;
                value.Read().Debug();
                if (value == null)
                {
                    if (property.PropertyType == typeof(bool))
                    {
                        return TryToggleValue(property);
                    }

                    objT.Debug();
                    if (!objT.IsNullable())
                    {
                        "A value can only be left empty, in the case of a bool.".Debug(); // Delete.
                        return false;
                    }
                }
                if (property.PropertyType.IsEnum)
                {
                    Array enums = property.PropertyType.GetEnumValues();
                    for (int i = 0; i < enums.Length; i++)
                    {
                        object data = enums.GetValue(i);
                        string name = data.GetType().GetEnumName(data);
                        int raw = (int)data.GetType().GetField(name).GetRawConstantValue();

                        raw.Debug("The raw Enum value."); // Delete.
                        name.Debug("The Enum name."); // Delete.
                        value.Debug("The value specified."); // Delete.

                        if ($"{value}" == $"{raw}")
                        {
                            try
                            {
                                obj.Read().Debug("The previous value."); // Delete.
                                property.SetValue(this, data);
                                property.GetValue(this).Debug("The new value."); // Delete.
                            }
                            catch (ArgumentException)
                            {
                                "Inbound enum type is not in par with the specified type.".Debug(); // Delete.
                                return false;
                            }
                            return true;
                        }
                        if (name.Matches($"{value}", MatchHandling.Match, MatchValueHandling.Equals))
                        {
                            try
                            {
                                obj.Read().Debug("The previous value."); // Delete.
                                property.SetValue(this, data);
                                property.GetValue(this).Read().Debug("The new value."); // Delete.
                            }
                            catch (ArgumentException)
                            {
                                "Inbound enum type is not in par with the specified type.".Debug(); // Delete.
                                return false;
                            }
                            return true;
                        }
                    }
                    return false;
                }
                if (obj.IsList())
                {
                    if (value.IsList())
                    {
                        return TryAddValues(property, value);

                    }
                    return TryAddValue(property, value);
                }

                try
                {
                    obj.Read().Debug("The previous value."); // Delete.
                    property.SetValue(this, value);
                    property.GetValue(this).Read().Debug("The new value."); // Delete.
                }
                catch (ArgumentException)
                {
                    return false;
                }
            }
            return true;
        }

        private bool TryAddValue(PropertyInfo property, object value)
        {
            object obj = property.GetValue(this);
            Type innerT = obj.GetInnerType();
            innerT.Debug();
            value.GetType().Debug();
            IEnumerable<object> v = (obj as IEnumerable<object>).Cast<object>();
            obj.Debug();
            v.Read().Debug();
            value.Debug();

            if (value.GetType() == innerT)
            {
                if (v.Contains(value))
                {
                    return TryRemoveValue(property, value);
                }
                try
                {
                    List<object> r = v.ToList();
                    r.Add(value);
                    r.Read().Debug();
                    property.SetValue(this, r.ConvertAll(x => x.ToString()));
                    property.GetValue(this).Debug();
                }
                catch (ArgumentException)
                {
                    "Inbound parameter is not in par with the specified type.".Debug();
                    return false;
                }
            }
            return true;
        }

        private bool TryAddValues(PropertyInfo property, object value)
        {
            object obj = property.GetValue(this);
            Type innerT = obj.GetInnerType();
            Type innerK = value.GetInnerType();
            innerT.Debug();
            value.GetType().Debug();
            IEnumerable<object> k = (value as IEnumerable<object>).Cast<object>();
            IEnumerable<object> v = (obj as IEnumerable<object>).Cast<object>();
            obj.Debug();
            k.Read().Debug();
            v.Read().Debug();
            value.Debug();

            List<string> keys = k.ToList().ConvertAll(x => x.ToString());
            List<object> r = v.ToList();
            if (innerK == innerT)
            {

                foreach (string o in keys)
                {
                    o.Debug();
                    if (r.Contains(o))
                    {
                        r.Remove(o);
                        r.Read().Debug();
                    }
                    else
                    {
                        r.Add(o);
                        r.Read().Debug();
                    }
                }
            }
            try
            {
                property.SetValue(this, r.ConvertAll(x => x.ToString()));
                property.GetValue(this).Debug();
            }
            catch (ArgumentException)
            {
                "Inbound parameter is not in par with the specified type.".Debug();
                return false;
            }
            return true;
        }

        private bool TryToggleValue(PropertyInfo property)
        {
            try
            {
                object value = property.GetValue(this);
                value.Debug();
                property.SetValue(this, !(bool)value);
                property.GetValue(this).Debug();
            }
            catch (ArgumentException)
            {
                "i have no idea how i got here.".Debug();
                return false;
            }
            return true;
        }

        private bool TryRemoveValue(PropertyInfo property, object value)
        {
            object obj = property.GetValue(this);
            IEnumerable<object> v = (obj as IEnumerable<object>).Cast<object>();
            try
            {
                obj.Debug();
                v.Read().Debug();
                value.Debug();
                List<object> r = v.ToList();
                if (r.Remove(value))
                {
                    r.Read().Debug();
                    property.SetValue(this, r.ConvertAll(x => x.ToString()));
                    property.GetValue(this).Debug();
                }
                else
                {
                    return false;
                }
            }
            catch (ArgumentException)
            {
                "Inbound parameter is not in par with the specified type.".Debug();
                return false;
            }
            return true;
        }

        #endregion

        #region Fields

        [Icon("🚧")]
        [DisplayName("AutoFix")]
        [JsonProperty("autofix")]
        [AccountOption(AccountOption.AutoFix)]
        [Definition("Defines if parsing errors on commands should automatically be corrected.")]
        public bool AutoFix { get; set; }

        [Icon("💰")]
        [DisplayName("Overflow")]
        [JsonProperty("overflow")]
        [AccountOption(AccountOption.Overflow)]
        [Definition("Determines if overspent values should be read as the user's balance.")]
        public bool Overflow { get; set; }

        [Icon("🏥")]
        [DisplayName("Tooltips")]
        [JsonProperty("tooltips")]
        [AccountOption(AccountOption.Tooltips)]
        [Definition("Controls if Orikivo should offer tips for commands when needed.")]
        public bool IgnoreTooltips { get; set; }

        [Icon("💬")]
        [DisplayName("Nickname")]
        [JsonProperty("nickname")]
        [AccountOption(AccountOption.Nickname)]
        [Definition("An optional parameter that defines the nickname used on services.")]
        public string Nickname { get; set; }

        [Icon("📞")]
        [DisplayName("Prefix")]
        [JsonProperty("prefix")]
        [AccountOption(AccountOption.Prefix)]
        [Definition("An optional property that defines a prefix for an account.")]
        public string Prefix { get; set; }

        [Icon("☎")]
        [DisplayName("Closing Prefix")]
        [JsonProperty("closedprefix")]
        [AccountOption(AccountOption.ClosingPrefix)]
        [Definition("An optional property that defines the closing prefix for an account.")]
        public string ClosingPrefix { get; set; }

        [Icon("🙊")]
        [DisplayName("Locale Blacklist")]
        [JsonProperty("bannedwords")]
        [AccountOption(AccountOption.LocaleBlacklist)]
        [Definition("A collection of banned statements that are automatically ignored.")]
        public List<string> LocaleBlacklist { get; set; }

        [Icon("💻")]
        [DisplayName("Site Blacklist")]
        [JsonProperty("bannedsites")]
        [AccountOption(AccountOption.SiteBlacklist)]
        [Definition("A collection of banned websites that are automatically ignored.")]
        public List<string> SiteBlacklist { get; set; }

        [Icon("🔌")]
        [DisplayName("Linking")]
        [JsonProperty("allowlinks")]
        [AccountOption(AccountOption.AllowLinks)]
        [Definition("Determines if links are able to be sent directly to you.")]
        public bool AllowLinks { get; set; }

        [Icon("🔥")]
        [DisplayName("Exceptions")]
        [JsonProperty("throw")]
        [AccountOption(AccountOption.Exceptions)]
        [Definition("A toggle for runtime errors being displayed. In the case of a fatal error, it will be shown regardless.")]
        public bool Exceptions { get; set; }

        [Icon("🔰")]
        [DisplayName("SafeGuard")]
        [JsonProperty("safeguard")]
        [AccountOption(AccountOption.SafeGuard)]
        [Definition("Determines if unsafe content is displayed.")]
        public bool SafeGuard { get; set; }

        [Icon("📤")]
        [DisplayName("Output Format")]
        [JsonProperty("rendermode")]
        [AccountOption(AccountOption.OutputFormat)]
        [Definition("Defines how Orikivo displays executed results.")]
        public OutputFormat OutputFormat { get; set; }

        [Icon("🈳")]
        [DisplayName("Empty Display Format")]
        [JsonProperty("onempty")]
        [AccountOption(AccountOption.NullFormat)]
        [Definition("Defines how empty objects are shown in results.")]
        public NullObjectHandling EmptyFormat { get; set; }

        [Icon("👀")]
        [DisplayName("Visibility")]
        [JsonProperty("visibility")]
        [AccountOption(AccountOption.Visibility)]
        [Definition("Specifies the publicity of your account.")]
        public Visibility Visibility { get; set; }

        [Icon("🔨")]
        [DisplayName("Sledge Power")]
        [JsonProperty("sledge")]
        [AccountOption(AccountOption.SledgePower)]
        [Definition("An optional property that defines the sledge power of Orikivo.")]
        public SledgePower? SledgePower { get; set; }

        public bool PortableMode { get; set; }

        [Icon("📺")]
        [DisplayName("Icon Rendering Format")]
        [JsonProperty("iconstyle")]
        [AccountOption(AccountOption.IconFormat)]
        [Definition("An optional property that defines how emoji icons are displayed.")]
        public IconFormat IconFormat { get { return PortableMode ? IconFormat.Portable : IconFormat; } set { IconFormat = value; } }

        [Icon("📢")]
        [DisplayName("Locale")]
        [JsonProperty("locale")]
        [AccountOption(AccountOption.Locale)]
        [Definition("An optional property that defines the language of Orikivo.")]
        public Locale? Locale { get; set; }

        [Icon("📲")]
        [DisplayName("Pre-Decode")]
        [JsonProperty("predecode")]
        [AccountOption(AccountOption.PreDecode)]
        [Definition("A toggle defining if all encoded strings should be decoded beforehand.")]
        public bool PreDecode { get; set; }

        [Icon("🌄")]
        [DisplayName("Global Decoder")]
        [JsonProperty("globaldecode")]
        [AccountOption(AccountOption.GlobalDecoder)]
        [Definition("A property defining if all strings should be decoded, regardless of context.")]
        public bool GlobalDecoder { get; set; }

        [Icon("↪")]
        [DisplayName("Ignore Directional")]
        [JsonProperty("ignoredirectional")]
        [AccountOption(AccountOption.IgnoreDirectional)]
        [Definition("A toggle defining if directional characters are automatically flipped upon reversing.")]
        public bool IgnoreDirectional { get; set; }

        [Icon("📛")]
        [DisplayName("Username Format")]
        [JsonProperty("usernameformat")]
        [AccountOption(AccountOption.UsernameFormat)]
        [Definition("A property defining how the name of a user is visually shown.")]
        public NameFormat UsernameFormat { get; set; }

        [Icon("🔍")]
        [DisplayName("Match Handling")]
        [JsonProperty("matchhandling")]
        [AccountOption(AccountOption.MatchHandling)]
        [Definition("Controls how comparing variables should result in a match.")]
        public MatchHandling MatchHandling { get; set; }

        [Icon("🔖")]
        [DisplayName("Search Handling")]
        [JsonProperty("searchmode")]
        [AccountOption(AccountOption.SearchHandling)]
        [Definition("A property defining how compared objects will be considered a match.")]
        public MatchValueHandling SearchHandling { get; set; }

        // Move to CasinoOptions;
        [Icon("🚥")]
        [DisplayName("Risk")]
        [JsonProperty("risk")]
        [AccountOption(AccountOption.Risk)]
        [Definition("A property defining the risk percent for gambling commands.")]
        public int Risk { get; set; }

        public bool WordGuard { get; set; }

        [Icon("🚓")]
        [DisplayName("WordGuard")]
        [JsonProperty("wordguard")]
        [AccountOption(AccountOption.WordGuard)]
        [Definition("A property defining if messages will be checked for matches using the Locale Blacklist.")]
        public WordGuardControl WordGuardHandling { get; set; }

        [Icon("🎨")]
        [DisplayName("Color Display Format")]
        [JsonProperty("colortype")]
        [AccountOption(AccountOption.ColorFormat)]
        [Definition("A property defining how color reference is displayed.")]
        public ColorFormat ColorFormat { get; set; }

        [Icon("🔤")]
        [DisplayName("Symbol Name Display")]
        [JsonProperty("showoptionname")]
        [AccountOption(AccountOption.SymbolNameDisplay)]
        [Definition("A toggle that defines if an symbol should also include its name.")]
        public bool SymbolNameDisplay { get; set; }

        #endregion

        #region Methods

        public void SetNameFormat(NameFormat format)
            => UsernameFormat = format;

        public void ResetNameFormat()
            => UsernameFormat = Default.UsernameFormat;

        public void SetMatchType(MatchHandling handle)
            => MatchHandling = handle;

        public void ResetMatchType()
            => MatchHandling = Default.MatchHandling;

        public void ResetRisk()
            => Risk = Default.Risk;

        public void SetRisk(int risk)
            => Risk = risk.InRange(2, 98);

        public void ToggleWordGuard()
            => WordGuard = !WordGuard;

        public void SetWordGuard(WordGuardControl ctrl)
            => WordGuardHandling = ctrl;

        public void ResetWordGuard()
            => WordGuardHandling = Default.WordGuardHandling;

        public void SetColorMethod(ColorFormat model)
            => ColorFormat = model;

        public void ResetColorMethod()
            => ColorFormat = Default.ColorFormat;

        public void SetSearchMethod(MatchValueHandling match)
            => SearchHandling = match;

        public void ResetSearchMethod()
            => SearchHandling = Default.SearchHandling;

        public void ToggleSymbolNameDisplay()
            => SymbolNameDisplay = !SymbolNameDisplay;

        public void SetOnEmpty(NullObjectHandling n)
            => EmptyFormat = n;

        public void ResetOnEmpty()
            => EmptyFormat = Default.EmptyFormat;

        public void SetVisibility(Visibility v)
            => Visibility = v;

        public void ResetVisibility()
            => Visibility = Default.Visibility;

        public void SetOutputFormat(OutputFormat r)
            => OutputFormat = r;

        public void ResetOutputFormat()
            => OutputFormat = Default.OutputFormat;

        public bool TrySetNickname(string s)
        {
            if (!s.Length.IsInRange(Global.NicknameLimit))
                return false;

            if (Nickname.Exists())
            {
                if (Nickname == s)
                    return false;
            }

            SetNickname(s);
            return true;
        }

        private void SetNickname(string s)
            => Nickname = s;

        public void ClearNickname()
            => Nickname = null;

        public void TogglePreDecoding()
            => PreDecode = !PreDecode;

        public void ToggleGlobalDecoder()
            => GlobalDecoder = !GlobalDecoder;

        public void ToggleIgnoreDirectional()
            => IgnoreDirectional = !IgnoreDirectional;

        public void ToggleAutoCorrect()
            => AutoFix = !AutoFix;

        public void ToggleOverflow()
            => Overflow = !Overflow;

        public void ToggleSafeGuard()
            => SafeGuard = !SafeGuard;

        public void ToggleLinks()
            => AllowLinks = !AllowLinks;

        public void ToggleExceptions()
            => Exceptions = !Exceptions;

        public void ToggleTooltips()
            => IgnoreTooltips = !IgnoreTooltips;

        public bool TrySetPrefix(string s)
        {
            if (!s.Length.IsInRange(Global.PrefixLimit))
                return false;

            if (Prefix.Exists())
            {
                if (Prefix == s)
                    return false;
            }

            SetPrefix(s);
            return true;
        }

        public void ClearPrefix()
            => Prefix = null;

        private void SetPrefix(string s)
            => Prefix = s;

        public bool TrySetSledgePower(SledgePower p)
        {
            if (SledgePower.HasValue)
            {
                if (SledgePower == p)
                    return false;
            }

            SetSledgePower(p);
            return true;
        }

        private void SetSledgePower(SledgePower p)
            => SledgePower = p;

        public void ClearSledgePower()
        {
            if (SledgePower.HasValue)
                SledgePower = null;
        }

        public void SetIconFormat(IconFormat i)
            => IconFormat = i;

        public void ResetIconFormat()
            => IconFormat = Default.IconFormat;

        public bool TrySetLocale(Locale l)
        {
            if (Locale.HasValue)
            {
                if (Locale == l)
                    return false;
            }

            SetLocale(l);
            return true;
        }

        private void SetLocale(Locale l)
            => Locale = l;

        public void ClearLocale()
        {
            if (Locale.HasValue)
                Locale = null;
        }

        public string CensorAllBannedWords(string s)
        {
            if (!UsesWordGuard)
                return s;

            s = s.Replace("||", "||".EscapeString());
            if (ContainsBannedWord(s))
            {
                foreach (string b in GetMatchingBannedWords(s))
                {
                    string n = b.Censor(WordGuardHandling);
                    s = s.Replace(b, n);
                }
            }
            return s;
        }

        private IEnumerable<string> GetMatchingBannedWords(string s)
            => LocaleBlacklist.Where(x => s.Contains(x));

        public bool ContainsAnyBannedWord(IEnumerable<string> args)
        {
            if (!UsesWordGuard)
                return false;

            return args.Any(x => ContainsBannedWord(x));
        }

        public bool ContainsAnyBannedWord(params string[] args)
        {
            if (!UsesWordGuard)
                return false;

            return args.Any(x => ContainsBannedWord(x));
        }

        public bool UsesWordGuard { get { return !(WordGuard == Default.WordGuard); } }

        public bool ContainsBannedWord(string s)
        {
            if (!UsesWordGuard)
                return false;
            return s.ContainsAny(LocaleBlacklist);
        }

        public bool TrySetPrefixEnd(string s)
        {
            if (!s.Length.IsInRange(Global.PrefixLimit))
                return false;

            if (ClosingPrefix.Exists())
            {
                if (s == ClosingPrefix)
                    return false;
            }

            SetPrefixEnd(s);
            return true;
        }

        public void ClearPrefixEnd(string s)
            => ClosingPrefix = null;

        private void SetPrefixEnd(string s)
            => ClosingPrefix = s;

        public bool TryBanWord(string s)
        {
            s = s.ToLower();
            if (IsBannedWord(s))
                return false;
            
            BanWord(s);
            return true;
        }

        public bool TryRevokeWord(string s)
        {
            if (!IsBannedWord(s))
                return false;

            RevokeWord(s);
            return true;
        }

        public bool TryRevokeWords(string s)
        {
            if (!IsBannedWord(s))
                return false;

            RevokeMatchingWords(s);
            return true;
        }

        public bool IsBannedWord(string s)
            => LocaleBlacklist.Contains(s.ToLower());

        private void BanWord(string s)
            => LocaleBlacklist.Add(s.ToLower());

        private void RevokeWord(string s)
            => LocaleBlacklist.Remove(s.ToLower());

        private void RevokeMatchingWords(string s)
        {
            s = s.ToLower();
            foreach (string word in LocaleBlacklist)
            {
                if (word.Contains(s))
                    RevokeWord(word);
            }
        }

        public void ClearLocaleBlacklist()
            => LocaleBlacklist = new List<string>();

        public bool TryBanLink(string s)
        {
            if (IsBannedLink(s))
                return false;

            BanLink(s);
            return true;
        }

        public bool TryRevokeLink(string s)
        {
            if (!IsBannedLink(s))
                return false;

            RevokeLink(s);
            return true;
        }

        public bool TryRevokeLinks(string s)
        {
            if (!IsBannedLink(s))
                return false;

            RevokeMatchingLinks(s);
            return true;
        }
        public bool IsBannedLink(string s)
            => SiteBlacklist.Contains(s);

        private void BanLink(string s)
            => SiteBlacklist.Add(s);

        private void RevokeLink(string s)
            => SiteBlacklist.Remove(s);

        private void RevokeMatchingLinks(string s)
        {
            foreach (string link in SiteBlacklist)
            {
                if (link.StartsWith(s))
                    RevokeLink(link);
            }
        }
        private void ClearSiteBlacklist()
            => SiteBlacklist = new List<string>();

        #endregion
    }
}
