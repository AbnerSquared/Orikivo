using System;
using System.Collections.Generic;
using Orikivo;
using Orikivo.Drawing;
using Orikivo.Framework;
using Orikivo.Text;

namespace Arcadia.Graphics
{
    public class CardBuilder
    {
        public static CardInfo BuildCardInfo(CardLayout layout, CardDetails details, CardProperties properties)
        {
            var components = new List<CardComponent>();

            var fill = new FillInfo
            {
                Palette = properties.PaletteOverride ?? GraphicsService.GetPalette(properties.Palette),
                Primary = Gamma.Max,
                Usage = FillMode.Solid
            };

            // TODO: Implement a way to handle component generation based on:
            // - The card layout given. This is a default guideline, but certain components can be hidden or have their own unique fill information
            // - The way the information is specified is through the use of IDs that point to certain data. The string is converted to component info for the card to render.
            // - The selection of components specified will be handled as a collection of strings, stored as Dictionary<ComponentType, string>. If a string is unspecified, the layout's default structure will be used.
            // - The card layout ONLY specifies how to position all of the components accordingly. It does not handle information related to colors and font types.
            // - The issue I'm having is related to how to ensure that the components I am generating match up with the card details given.
            // - Components that specify a specific group can be skipped entirely if the card properties explicitly specify to ignore the given component type
            // - Where issues occur is when, for example, I need to have a counter specified (stored as icon/text, in the group ComponentType.Money)
            //    - How would I determine how to correctly specify the needed information?
            //    - I could specify base requirements for a card layout, such as:
            //      - There can only be ONE existing component info for a ComponentType.
            //      - The only exceptions are components that are grouped with each other, such as levels and money. In this case, There can only be TWO component informations that are in the same group.
            //      - For a ComponentType.Level OR ComponentType.Money, they are considered a Counter, which can only specify a single Text and Icon component. Either one is optional, but at a minimum, 1 must be specified.
            // - The way the components will be determined will instead go in the format of reading by ComponentType instead.
            // - To start, all components will be ordered by their PRIORITY.
            // - On each component, their groupings will be compared. A method called GetComponents(ref IEnumerable<ComponentInfo> components, CardDetails, CardProperties) will be used,
            //    which will attempt to build the specified components.
            // FIRST, Order the components by their priority.
            // NEXT, Group the compontents by their ComponentType focus.
            // THEN, Check to see if the group of components are correctly specified (the correct amount is given).
            // IF no components in a group are specified, they can be completely looked over.
            // The following card sections are:
            // - Name
            // - Avatar
            // - Level
            // - Money
            // - Exp

            // The following component types are:
            // - Image
            // - Icon
            // - Text
            // - Solid

            // The following component groups are:
            // - Single
            // - Counter

            // - Using the ComponentType, the builder will filter for all specified components under that group.
            foreach (ComponentInfo info in layout.Components)
            {
                components.Add(BuildTarget(info, details, properties));
            }

            return new CardInfo
            {
                Width = layout.Width,
                Height = layout.Height,
                Padding = layout.Padding,
                Margin = layout.Margin,
                CursorOriginX = layout.CursorOriginX,
                CursorOriginY = layout.CursorOriginY,
                CanTrim = layout.CanTrim,
                Components = components,
                BorderFill = fill,
                BasePalette = properties.PaletteOverride ?? GraphicsService.GetPalette(properties.Palette),
                BorderEdge = BorderEdge.Outside,
                BorderThickness = 2,
                BorderAllow = properties.Border,
                Scale = properties.Scale
            };
        }

        private static CardComponent BuildTarget(ComponentInfo info, CardDetails details, CardProperties properties)
        {
            if (!info.PrimaryTarget && info.Type != ComponentType.Solid)
            {
                throw new Exception("Cannot initialize non-primary component targets if the underlying type is not a solid");
            }

            return info.Type switch
            {
                ComponentType.Solid => BuildSolidTarget(info, details, properties), // throw new NotImplementedException("Solids have not been implemented yet"),
                ComponentType.Text => BuildTextTarget(info, details, properties),
                ComponentType.Icon => BuildIconTarget(info, details, properties),
                ComponentType.Image => BuildImageTarget(info, details, properties),
                _ => throw new Exception("Unknown or invalid component information was specified")
            };
        }

        private static CardComponent BuildSolidTarget(ComponentInfo info, CardDetails details, CardProperties properties)
        {
            FillInfo fillInfo = GetDefaultFill(properties);

            if (info.Group.HasFlag(CardGroup.Exp))
            {
                // Make this dynamic later on
                fillInfo.Usage = FillMode.Bar;
                fillInfo.Primary = Gamma.Max;
                fillInfo.Secondary = Gamma.Standard;
                fillInfo.Direction = Direction.Right;

                int level = ExpConvert.AsLevel(details.Exp, details.Ascent);
                ulong currentExp = ExpConvert.AsExp(level, details.Ascent);
                ulong nextExp = ExpConvert.AsExp(level + 1, details.Ascent);

                fillInfo.FillPercent = RangeF.Convert(currentExp, nextExp, 0, 1, details.Exp);

                Logger.Debug($"{currentExp} to {nextExp} ({details.Exp}) [{fillInfo.FillPercent ?? 0}]");
            }

            return new SolidComponent(info, fillInfo);
        }

        private static CardComponent BuildTextTarget(ComponentInfo info, CardDetails details, CardProperties properties)
        {
            TextInfo textInfo = info.Group switch
            {
                CardGroup.Username => new TextInfo
                {
                    Content = details.Name.ToString(properties.Casing),
                    Font = GraphicsService.GetFont(properties.Font)
                },
                CardGroup.Level => new TextInfo
                {
                    Content = ExpConvert.AsLevel(details.Exp, details.Ascent).ToString(),
                    Font = GraphicsService.GetFont(FontType.Delta)
                },
                CardGroup.Money => new TextInfo
                {
                    Content = Format.Condense(details.Balance, out _),
                    Font = GraphicsService.GetFont(FontType.Delta)
                },
                CardGroup.Activity => new TextInfo
                {
                    Content = details.Program ?? details.Status.ToString(),
                    Font = GraphicsService.GetFont(FontType.Minic)
                },
                _ => throw new Exception("An invalid group was specified for the current component")
            };

            return new TextComponent(info, GetDefaultFill(properties))
            {
                Text = textInfo
            };
        }

        private static CardComponent BuildIconTarget(ComponentInfo info, CardDetails details, CardProperties properties)
        {
            SheetInfo sheetInfo = info.Group switch
            {
                CardGroup.Level => new SheetInfo
                {
                    Path = @"../assets/icons/levels.png",
                    CropHeight = 6,
                    CropWidth = 6,
                    Index = 0
                },
                CardGroup.Money => new SheetInfo
                {
                    Path = @"../assets/icons/coins.png",
                    CropHeight = 8,
                    CropWidth = 8,
                    Index = 0
                },
                _ => throw new Exception("An invalid group was specified for the current component")
            };

            return new IconComponent(info, GetDefaultFill(properties))
            {
                Sheet = sheetInfo
            };
        }

        private static CardComponent BuildImageTarget(ComponentInfo info, CardDetails details, CardProperties properties)
        {
            string url = info.Group switch
            {
                CardGroup.Avatar => details.AvatarUrl,
                _ => throw new Exception("An invalid group was specified for the current component")
            };

            return new ImageComponent(info, GetDefaultFill(properties))
            {
                Url = url
            };
        }

        private static FontFace GetDefaultFont(CardProperties properties)
        {
            return GraphicsService.GetFont(properties.Font);
        }

        private static FillInfo GetDefaultFill(CardProperties properties)
        {
            return new FillInfo
            {
                Palette = properties.PaletteOverride ?? GraphicsService.GetPalette(properties.Palette),
                Primary = Gamma.Max,
                Usage = FillMode.Solid
            };
        }
    }
}