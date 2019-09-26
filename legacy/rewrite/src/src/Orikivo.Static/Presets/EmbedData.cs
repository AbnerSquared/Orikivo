using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Discord;
using Orikivo.Modules;

namespace Orikivo.Systems.Presets
{
    public class EmbedData
    {
        public static Dictionary<string, string[]> GetInsultCollection(string dynamicInsultType)
        {
            var insultDictionary = new Dictionary<string, string[]>();
            string[] insultListT0 =
            {
                    $"Sorry, I tried to be insulting, but I feel too held back to be mean to you. You're too good of a {dynamicInsultType}.",
                    "I bet you take showers on a daily basis.",
                    "You provide the light of hope in the sea of darkness.",
                    "Are you an RGB systematic? Because you change the colors of this world.",
                    "I bet you make babies smile so hard, their future children will have smiles.",
                    "I'm sure you make a difference in this world.",
                    "You're too good for a 10 scale positivity meter. Why, you may ask? Because your number reaches infinity and beyond.",
                    "Just wanted to let you know that you are the true shooting star in this universe."
                };
            insultDictionary.Add("T0", insultListT0);
            string[] insultListT1 =
            {
                    "RANDOM1LEVEL1",
                    "RANDOM2LEVEL1",
                    "RANDOM3LEVEL1",
                    "RANDOM4LEVEL1",
                    "RANDOM5LEVEL1",
                    "RANDOM6LEVEL1",
                    "RANDOM7LEVEL1",
                    "RANDOM8LEVEL1"
                };
            insultDictionary.Add("T1", insultListT1);
            string[] insultListT2 =
            {
                    "RANDOM1LEVEL2",
                    "RANDOM2LEVEL2",
                    "RANDOM3LEVEL2",
                    "RANDOM4LEVEL2",
                    "RANDOM5LEVEL2",
                    "RANDOM6LEVEL2",
                    "RANDOM7LEVEL2",
                    "RANDOM8LEVEL2"
                };
            insultDictionary.Add("T2", insultListT2);
            string[] insultListT3 =
            {
                    "RANDOM1LEVEL3",
                    "RANDOM2LEVEL3",
                    "RANDOM3LEVEL3",
                    "RANDOM4LEVEL3",
                    "RANDOM5LEVEL3",
                    "RANDOM6LEVEL3",
                    "RANDOM7LEVEL3",
                    "RANDOM8LEVEL3"
                };
            insultDictionary.Add("T3", insultListT3);
            string[] insultListT4 =
            {
                    "RANDOM1LEVEL4",
                    "RANDOM2LEVEL4",
                    "RANDOM3LEVEL4",
                    "RANDOM4LEVEL4",
                    "RANDOM5LEVEL4",
                    "RANDOM6LEVEL4",
                    "RANDOM7LEVEL4",
                    "RANDOM8LEVEL4"
                };
            insultDictionary.Add("T4", insultListT4);
            string[] insultListT5 =
            {
                    "RANDOM1LEVEL5",
                    "RANDOM2LEVEL5",
                    "RANDOM3LEVEL5",
                    "RANDOM4LEVEL5",
                    "RANDOM5LEVEL5",
                    "RANDOM6LEVEL5",
                    "RANDOM7LEVEL5",
                    "RANDOM8LEVEL5"
                };
            insultDictionary.Add("T5", insultListT5);
            string[] insultListT6 =
            {
                    "RANDOM1LEVEL6",
                    "RANDOM2LEVEL6",
                    "RANDOM3LEVEL6",
                    "RANDOM4LEVEL6",
                    "RANDOM5LEVEL6",
                    "RANDOM6LEVEL6",
                    "RANDOM7LEVEL6",
                    "RANDOM8LEVEL6"
                };
            insultDictionary.Add("T6", insultListT6);
            string[] insultListT7 =
            {
                    "RANDOM1LEVEL7",
                    "RANDOM2LEVEL7",
                    "RANDOM3LEVEL7",
                    "RANDOM4LEVEL7",
                    "RANDOM5LEVEL7",
                    "RANDOM6LEVEL7",
                    "RANDOM7LEVEL7",
                    "RANDOM8LEVEL7"
                };
            insultDictionary.Add("T7", insultListT7);
            string[] insultListIncorrectLevel =
            {
                    "Lucky for you, you don't get insulted today. Know why? Because the creator of this bot was too stupid to realize he put an incorrect level integer.",
                    "RANDOM2INCORRECTLEVEL",
                    "RANDOM3INCORRECTLEVEL",
                    "RANDOM4INCORRECTLEVEL",
                    "RANDOM5INCORRECTLEVEL",
                    "RANDOM6INCORRECTLEVEL",
                    "RANDOM7INCORRECTLEVEL",
                    "RANDOM8INCORRECTLEVEL"
                };
            insultDictionary.Add("LVL_ERR", insultListIncorrectLevel);
            string[] insultListNullUser =
            {
                    "Oh, how the tables have turned. This insult that was planned for the user you were supposed to call, but because you failed to even mention the user to insult, you now are going to take this downfall.",
                    "RANDOM2NULLUSER",
                    "RANDOM3NULLUSER",
                    "RANDOM4NULLUSER",
                    "RANDOM5NULLUSER",
                    "RANDOM6NULLUSER",
                    "RANDOM7NULLUSER",
                    "RANDOM8NULLUSER"
                };
            insultDictionary.Add("NULL", insultListNullUser);
            string[] insultListSelf =
            {
                    "Trust me. With your looks and mindset, you don't need me to insult you.",
                    "RANDOM2SELF",
                    "RANDOM3SELF",
                    "RANDOM4SELF",
                    "RANDOM5SELF",
                    "RANDOM6SELF",
                    "RANDOM7SELF",
                    "RANDOM8SELF"
                };
            insultDictionary.Add("SELF", insultListSelf);
            string[] insultListBot =
            {
                    "Look. I don't know what kind of pleasure you derive from self-deprication, but I can't help you there.",
                    "RANDOM2BOT",
                    "RANDOM3BOT",
                    "RANDOM4BOT",
                    "RANDOM5BOT",
                    "RANDOM6BOT",
                    "RANDOM7BOT",
                    "RANDOM8BOT"
                };
            insultDictionary.Add("BOT", insultListBot);
            string[] insultListCreator =
            {
                    "My creator is dumb enough as it is to accept insults from the likes of you.",
                    "RANDOM2CREATOR",
                    "RANDOM3CREATOR",
                    "RANDOM4CREATOR",
                    "RANDOM5CREATOR",
                    "RANDOM6CREATOR",
                    "RANDOM7CREATOR",
                    "RANDOM8CREATOR"
                };
            insultDictionary.Add("OWN", insultListCreator);
            string[] insultListError =
            {
                    "Good job. I don't even know how, but you managed to mess something up so much that I can't even calculate an insult.",
                    "RANDOM2ERROR",
                    "RANDOM3ERROR",
                    "RANDOM4ERROR",
                    "RANDOM5ERROR",
                    "RANDOM6ERROR",
                    "RANDOM7ERROR",
                    "RANDOM8ERROR"
                };
            insultDictionary.Add("ERR", insultListError);
            string[] insultListOverflowedInt =
            {
                    "I wonder if this failed attempt of a number you put in matches the amount of failures in your life.",
                    "RANDOM2OVERFLOWEDINT",
                    "RANDOM3OVERFLOWEDINT",
                    "RANDOM4OVERFLOWEDINT",
                    "RANDOM5OVERFLOWEDINT",
                    "RANDOM6OVERFLOWEDINT",
                    "RANDOM7OVERFLOWEDINT",
                    "RANDOM8OVERFLOWEDINT"
                };
            insultDictionary.Add("OVERFLOW", insultListOverflowedInt);
            string[] insultListUnderflowedInt =
            {
                    "This number you called is lower than your IQ.",
                    "RANDOM2UNDERFLOWEDINT",
                    "RANDOM3UNDERFLOWEDINT",
                    "RANDOM4UNDERFLOWEDINT",
                    "RANDOM5UNDERFLOWEDINT",
                    "RANDOM6UNDERFLOWEDINT",
                    "RANDOM7UNDERFLOWEDINT",
                    "RANDOM8UNDERFLOWEDINT"
                };
            insultDictionary.Add("UNDERFLOW", insultListUnderflowedInt);
            string[] insultListFailedInt =
            {
                    "Honestly, I don't even think you can use this insult module if you can't even understand what an integer is.",
                    "RANDOM2FAILEDINT",
                    "RANDOM3FAILEDINT",
                    "RANDOM4FAILEDINT",
                    "RANDOM5FAILEDINT",
                    "RANDOM6FAILEDINT",
                    "RANDOM7FAILEDINT",
                    "RANDOM8FAILEDINT"

                };
            insultDictionary.Add("NOT_INT", insultListFailedInt);
            return insultDictionary;
        }

        // with this case, make ids for each entity.
        public static Embed CatchException(int code, Exception ex)
        {
            //ExceptionInfo exception = new ExceptionInfo(ex);
            //string path = $"{Directory.CreateDirectory(".//data//exceptions//").FullName}{ex.HResult}C{code}.json";
            //Manager.Save(exception, path);

            EmbedBuilder e = new EmbedBuilder();
            e.WithColor(GetColor("error"));
            e.WithTitle($"Oops! An error occured. ({ex.HResult})");

            EmbedFooterBuilder f = new EmbedFooterBuilder();
            f.WithText($"{ex.StackTrace}");

            e.WithFooter(f);
            e.WithDescription((ex.Message.Length > 2039 ? ex.Message.Substring(0, 2039) + "..." : ex.Message).DiscordBlock());
            return e.Build();
        }

        public static string GetReaction(string reactionType)
        {

            string reactor = reactionType.ToLower();
            string returnReaction = "";

            const string nullReaction = "The following reaction was not found.";

            const string owoLink = "https://orig00.deviantart.net/bc18/f/2018/140/2/1/orikivo_iii____animated_emoticons_owo_by_abnersquared-dbtju89.gif";

            switch (reactor)
            {
                case ("owo"):
                    returnReaction = owoLink;
                    break;
                default:
                    returnReaction = nullReaction;
                    break;
            }
            Console.WriteLine($"EmbedData.Reactions >> {returnReaction}");
            return returnReaction;
        }

        public static EmbedBuilder DefaultEmbed { get { return new EmbedBuilder().WithColor(GetColor("origreen")); } }

        public static Embed GenerateEmbedList(List<string> strings, int page = 1, EmbedBuilder e = null)
        {
            int PER_PAGE = 20;
            int MAX_PAGES = (int)Math.Ceiling((double)strings.Count / PER_PAGE);

            page = page.InRange(1, MAX_PAGES);
            int skip = (page - 1) * PER_PAGE;

            e = e ?? DefaultEmbed;
            const int MAX_DESC = EmbedBuilder.MaxDescriptionLength;

            int len = 0;
            if (!string.IsNullOrWhiteSpace(e.Description))
                len = e.Description.Length;

            string desc = e.Description ?? "";

            StringBuilder sb = new StringBuilder();
            int i = 0;
            foreach (string s in strings.Skip(skip))
            {
                if (i >= PER_PAGE)
                    break;
                
                if (len + sb.Length + '\n' + s.Length > MAX_DESC)
                {
                    e.WithDescription($"{desc}{sb.ToString()}");
                    return e.Build();
                }

                sb.AppendLine(s);
                i += 1;
            }

            e.WithDescription($"{desc}{sb.ToString()}");
            EmbedFooterBuilder f = e.Footer ?? new EmbedFooterBuilder();
            if (e.Footer.Exists())
            {
                if (MAX_PAGES > 1)
                {
                    f.WithText($"{e.Footer.Text} | Page {page} of {MAX_PAGES}");
                }
                else
                {
                    f.WithText(e.Footer.Text);
                }

                e.WithFooter(f);
            }
            else
            {
                if (MAX_PAGES > 1)
                {
                    f.WithText($"Page {page} of {MAX_PAGES}");
                    e.WithFooter(f);
                }
            }
            
            return e.Build();
        }

        public static Embed Throw(OrikivoCommandContext ctx, string reason, string example = null, bool prefix = false)
        {
            EmbedBuilder e = new EmbedBuilder();
            e.WithColor(GetColor("error"));
            e.WithTitle(reason);
            if (example.Exists())
            {
                if (prefix)
                {
                    example = ctx.Server.Config.GetPrefix(ctx) + example;
                }

                e.WithDescription(example);
            }

            return e.Build();
        }

        public enum ColorFormat
        {
            Default = 0,
            Yield = 1,
            Error = 2,
            New = 4
        }

        //ColorFormat.Default
        public static Color GetColor(string color)
        {
            color = color.ToLower();
            string confirmer = "";
            string confirmant = $"The color palette for {confirmer} has been set.";
            Color returnColor = new Color();

            Color nullColor = Color.Default;
            //const string confirmNull = "The color specified was not found.";

            Color orikivoGreen = new Color(129, 243, 193);
            const string confirmOri = "origreen";

            Color owoColor = new Color(74, 224, 226);
            const string confirmOwo = "owo";

            Color correctColor = new Color(112, 229, 130);
            const string confirmCorrect = "correct";

            Color yieldColor = new Color(255, 238, 129);
            const string confirmYield = "yield";

            Color errorColor = new Color(213, 16, 93);
            const string confirmErr = "error";

            Color steamError = new Color(186, 5, 97);
            const string confirmSteamErr = "steamerror";

            Color steam = new Color(0, 174, 239);
            const string confirmSteam = "steam";
        
            switch (color)
            {
                case (confirmOri):
                    returnColor = orikivoGreen;
                    break;
                case (confirmOwo):
                    returnColor = owoColor;
                    break;
                case (confirmErr):
                    returnColor = errorColor;
                    break;
                case (confirmCorrect):
                    returnColor = correctColor;
                    break;
                case (confirmYield):
                    returnColor = yieldColor;
                    break;
                case (confirmSteam):
                    returnColor = steam;
                    break;
                case (confirmSteamErr):
                    returnColor = steamError;
                    break;
                default:
                    returnColor = nullColor;
                    break;
            }
            return returnColor;
        }

        public enum ColorPaletteCollection
        {
            Default
        }

        public enum ColorPalette
        {
            Default = 0,
            Yield = 1,
            Error = 2,
            SteamDefault = 3,
            SteamError = 4
        }

        public enum EmbedCollection
        {
            Default,
            Yield,
            Error,
        }

        public static Color PickColor(ColorPalette type)
        {
            switch (type)
            {
                case ColorPalette.Default:
                    return new Color(129, 243, 193);
                case ColorPalette.Yield:
                    return new Color(255, 238, 129);
                case ColorPalette.Error:
                    return new Color(213, 16, 93);
                case ColorPalette.SteamDefault:
                    return new Color(0, 174, 239);
                case ColorPalette.SteamError:
                    return new Color(186, 5, 97);
                default:
                    return new Color(129, 243, 193);
            }
        }

        public static EmbedBuilder SetPostEmbed(EmbedCollection embedType = EmbedCollection.Default, String title = null, String description = null, EmbedAuthorBuilder author = null, EmbedFooterBuilder footer = null, List<EmbedFieldBuilder> fields = null)
        {
            EmbedBuilder e = GetPostEmbed(embedType);
            if (Exists(title)) e.WithTitle(title);
            if (Exists(description)) e.WithDescription(description);
            return e;
        }

        public static Embed SetSPEmbed(String title = null, String description = null, EmbedAuthorBuilder author = null, EmbedFooterBuilder footer = null, List<EmbedFieldBuilder> fields = null, EmbedCollection embedType = EmbedCollection.Default)
        {
            EmbedBuilder e = GetPostEmbed(embedType);
            if (Exists(title)) e.WithTitle(title);
            if (Exists(description)) e.WithDescription(description);
            if (ExistsType(author, new EmbedAuthorBuilder())) e.WithAuthor(author);
            if (ExistsType(footer, new EmbedFooterBuilder())) e.WithFooter(footer);
            if (ExistsType(footer, new EmbedFooterBuilder())) foreach (var field in fields) { e.AddField(field); }
            return e.Build();
        }

        private static EmbedBuilder GetPostEmbed(EmbedCollection embedType)
        {
            var embed = new EmbedBuilder();
            var colors = GetColorPalette(ColorPaletteCollection.Default);
            switch (embedType)
            {
                case (EmbedCollection.Default):
                    embed.WithColor(colors['g']);
                    break;
                case (EmbedCollection.Yield):
                    embed.WithColor(colors['y']);
                    break;
                case (EmbedCollection.Error):
                    embed.WithColor(colors['r']);
                    break;
            }
            return embed;
        }


        private static Dictionary<char, Color> GetColorSet(ColorPaletteCollection palette)
        {
            var setDictionary = new Dictionary<char, Color>();
            switch (palette)
            {
                case (ColorPaletteCollection.Default):
                    setDictionary.Add('b', new Color(74, 224, 226));
                    setDictionary.Add('g', new Color(129, 243, 193));
                    setDictionary.Add('y', new Color(255, 238, 129));
                    setDictionary.Add('r', new Color(213, 16, 93));
                    break;
            }
            return setDictionary;
        }
        public static Dictionary<char, Color> GetColorPalette(ColorPaletteCollection palette)
        {
            return GetColorSet(palette);
        }

        public static Color GetColorInPalette(ColorPaletteCollection palette, char key)
        {
            try
            {
                return GetColorSet(palette)[key];
            }
            catch (KeyNotFoundException)
            {
                return Color.LightGrey;
            }
        }

        public static List<Color> GetColorTones(Color color)
        {
            var maxByte = 255;
            var toneLimit = 9;
            var toneStorage = new List<Color>();
            var r = color.R;
            var rShift = 12;
            var g = color.G;
            var gShift = 13;
            var b = color.B;
            var bShift = 7;
            for (int i = 0; i < toneLimit; i++)
            {
                var red = r - (i * rShift) > maxByte ? r - (i * rShift) : 0;
                var green = g - (i * gShift) > maxByte ? g - (i * gShift) : 0;
                var blue = b - (i * bShift) > maxByte ? b - (i * bShift) : 0;
                toneStorage.Add(new Color(red, green, blue));
            }

            return toneStorage;
        }

        public static Color DefaultColor()
        {
            return new Color(129, 243, 193);
        }

        public static EmbedAuthorBuilder SetAuthor(String icon = null, String name = null, String url = null)
        {
            var iconExists = !String.IsNullOrWhiteSpace(icon);
            var nameExists = !String.IsNullOrWhiteSpace(name);
            var urlExists = !String.IsNullOrWhiteSpace(url);
            EmbedAuthorBuilder author = new EmbedAuthorBuilder();
            if (iconExists) { try { author.WithIconUrl(icon); } catch (Exception) { } };
            if (nameExists) author.WithName(name);
            if (urlExists) { try { author.WithUrl(url); } catch (Exception) { } };
            return author;
        }

        public static EmbedFooterBuilder SetFooter(String icon = null, String text = null)
        {
            EmbedFooterBuilder footer = new EmbedFooterBuilder();
            if (Exists(icon)) { try { footer.WithIconUrl(icon); } catch (Exception) { } };
            if (Exists(text)) footer.WithText(text);
            return footer;
        }

        public static EmbedFieldBuilder SetField(String name, String value, Boolean inline = false)
        {
            EmbedFieldBuilder field = new EmbedFieldBuilder();
            field.WithName(name);
            field.WithValue(value);
            field.WithIsInline(inline);
            return field;
        }

        public static EmbedBuilder SetEmbed(String title = null, String description = null, String image = null, String thumbnail = null, Boolean timestamp = false, EmbedAuthorBuilder author = null, EmbedFooterBuilder footer = null, List<EmbedFieldBuilder> fields = null)
        {
            EmbedBuilder embed = new EmbedBuilder();
            if (Exists(title)) embed.WithTitle(title);
            if (Exists(description)) embed.WithDescription(description);
            if (Exists(image)) { try { embed.WithImageUrl(image); } catch (Exception) { } }
            if (Exists(thumbnail)) { try { embed.WithThumbnailUrl(thumbnail); } catch (Exception) { } }
            if (timestamp) embed.WithCurrentTimestamp();
            embed.WithColor(DefaultColor());

            if (ExistsType(author, new EmbedAuthorBuilder())) embed.WithAuthor(author);
            if (ExistsType(footer, new EmbedFooterBuilder())) embed.WithFooter(footer);
            if (ExistsType(fields, new List<EmbedFooterBuilder>().GetType()))
            {
                foreach (var field in fields)
                {
                    embed.AddField(field);
                }
            }
            return embed;
        }
        public static EmbedBuilder SetEmbed(String title = null, String description = null, String image = null, String thumbnail = null, Boolean timestamp = false, ColorPaletteCollection color = ColorPaletteCollection.Default, char colorKey = 'g', EmbedAuthorBuilder author = null, EmbedFooterBuilder footer = null, List<EmbedFieldBuilder> fields = null)
        {
            EmbedBuilder embed = new EmbedBuilder();
            if (Exists(title)) embed.WithTitle(title);
            if (Exists(description)) embed.WithDescription(description);
            if (Exists(image)) { try { embed.WithImageUrl(image); } catch (Exception) { } }
            if (Exists(thumbnail)) { try { embed.WithThumbnailUrl(thumbnail); } catch (Exception) { } }
            if (timestamp) embed.WithCurrentTimestamp();
            embed.WithColor(GetColorPalette(color)[colorKey]);

            if (ExistsType(author, new EmbedAuthorBuilder())) embed.WithAuthor(author);
            if (ExistsType(footer, new EmbedFooterBuilder())) embed.WithFooter(footer);
            if (ExistsType(fields, new List<EmbedFooterBuilder>().GetType()))
            {
                foreach (var field in fields)
                {
                    embed.AddField(field);
                }
            }
            return embed;
        }
        public static EmbedBuilder SetEmbed(String title = null, String description = null, String image = null, String thumbnail = null, Boolean timestamp = false, string color = "129,243,193", EmbedAuthorBuilder author = null, EmbedFooterBuilder footer = null, List<EmbedFieldBuilder> fields = null)
        {
            EmbedBuilder embed = new EmbedBuilder();
            if (Exists(title)) embed.WithTitle(title);
            if (Exists(description)) embed.WithDescription(description);
            if (Exists(image)) { try { embed.WithImageUrl(image); } catch (Exception) { } }
            if (Exists(thumbnail)) { try { embed.WithThumbnailUrl(thumbnail); } catch (Exception) { } }
            if (timestamp) embed.WithCurrentTimestamp();
            string[] staticColor = color.Split(',');
            bool r = int.TryParse(staticColor[0] ?? "0", out int R);
            bool g = int.TryParse(staticColor[1] ?? "0", out int G);
            bool b = int.TryParse(staticColor[2] ?? "0", out int B);
            embed.WithColor(r ? R : 0, g ? G : 0, b ? B : 0);
            if (Exists(author)) { if (ExistsType(author, new EmbedAuthorBuilder())) embed.WithAuthor(author); }
            if (Exists(footer)) { if (ExistsType(footer, new EmbedFooterBuilder())) embed.WithFooter(footer); }
            if (Exists(fields))
            {
                if (ExistsType(fields, new List<EmbedFooterBuilder>().GetType()))
                {
                    foreach (var field in fields)
                    {
                        embed.AddField(field);
                    }
                }
            }
            return embed;
        }

        public static EmbedBuilder SetEmbed(Color color, String title = null, String description = null, String image = null, String thumbnail = null, Boolean timestamp = false, EmbedAuthorBuilder author = null, EmbedFooterBuilder footer = null, List<EmbedFieldBuilder> fields = null)
        {
            EmbedBuilder embed = new EmbedBuilder();
            if (Exists(title)) embed.WithTitle(title);
            if (Exists(description)) embed.WithDescription(description);
            if (Exists(image)) { try { embed.WithImageUrl(image); } catch (Exception) { } }
            if (Exists(thumbnail)) { try { embed.WithThumbnailUrl(thumbnail); } catch (Exception) { } }
            if (timestamp) embed.WithCurrentTimestamp();
            embed.WithColor(color);
            if (Exists(author)) { if (ExistsType(author, new EmbedAuthorBuilder())) embed.WithAuthor(author); }
            if (Exists(footer)) { if (ExistsType(footer, new EmbedFooterBuilder())) embed.WithFooter(footer); }
            if (Exists(fields))
            {
                if (ExistsType(fields, new List<EmbedFooterBuilder>().GetType()))
                {
                    foreach (var field in fields)
                    {
                        embed.AddField(field);
                    }
                }
            }
            return embed;
        }


        public static EmbedBuilder SetEmbed((String title, String description, String image, String thumbnail, Boolean timestamp, String color) core, (String icon, String name, String url) author, (String icon, String text) footer, List<(String name, String value, Boolean inline)> fields)
        {
            var titleExists = Exists(core.title);
            var descriptionExists = Exists(core.description);
            var imageExists = Exists(core.image);
            var thumbnailExists = Exists(core.thumbnail);
            var aIconExists = Exists(author.icon);
            var aNameExists = Exists(author.name);
            var aUrlExists = Exists(author.url);
            var footerExists = ExistsType(footer, new EmbedFooterBuilder());
            var fieldsExist = ExistsField(fields, new List<EmbedFieldBuilder>());

            var embed = new EmbedBuilder();
            if (titleExists) embed.WithTitle(core.title);
            if (descriptionExists) embed.WithDescription(core.description);
            if (imageExists) { try { embed.WithImageUrl(core.image); } catch (Exception) { } }
            if (thumbnailExists) { try { embed.WithThumbnailUrl(core.thumbnail); } catch (Exception) { } }
            if (core.timestamp) embed.WithCurrentTimestamp();
            var staticColor = core.color.Split(',');
            var r = int.TryParse(staticColor[0], out int R);
            var g = int.TryParse(staticColor[1], out int G);
            var b = int.TryParse(staticColor[2], out int B);
            embed.WithColor(r ? R : 0, g ? G : 0, b ? B : 0);
            embed.WithAuthor(author.icon, author.name, author.url);
            if (footerExists) embed.WithFooter(footer.text, footer.icon);
            if (fieldsExist)
            {
                foreach (var (name, value, inline) in fields)
                {
                    embed.AddField(name, value, inline);
                }
            }
            return embed;
        }

        public static bool ExistsField(List<(String Name, String Value, Boolean Inline)> typeA, List<EmbedFieldBuilder> typeB)
        {
            var TypeA = new List<EmbedFieldBuilder>();
            try
            {
                TypeA.Add(new EmbedFieldBuilder { Name = typeA[0].Name, Value = typeA[0].Value, IsInline = typeA[0].Inline });

            }
            catch (Exception)
            {
                return false;
            }
            return ((TypeA.GetType() == (typeB.GetType())));
        }

        public static bool ExistsField(Object typeA, List<EmbedFieldBuilder> typeB)
        {
            return ((typeA.GetType() == (typeB.GetType())));
        }

        public static bool ExistsType(Object typeA, Object typeB)
        {
            return (typeA.GetType() == (typeB.GetType()));
        }

        public static bool Exists(String reference)
        {
            return !String.IsNullOrWhiteSpace(reference);
        }

        public static bool Exists(object reference)
        {
            if (reference == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}