using System;
using System.Collections.Generic;
using Orikivo.Drawing;

namespace Arcadia.Graphics
{
    public class CardBuilder
    {
        public static CardInfo BuildCardInfo(CardLayout layout, CardDetails details, CardProperties properties)
        {
            var components = new List<ICardComponent>();

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
            foreach (ComponentInfo component in layout.Components)
            {
                components.Add(component.Type switch
                {
                    ComponentType.Text when component.Group.HasFlag(CardComponent.Username) => new TextComponent(component, fill)
                    {
                        Content = details.Name,
                        Casing = properties.Casing,
                        Font = GraphicsService.GetFont(properties.Font)
                    },
                    ComponentType.Text when component.Group.HasFlag(CardComponent.Username) => new TextComponent(component, fill)
                    {
                        Content = details.Name,
                        Casing = properties.Casing,
                        Font = GraphicsService.GetFont(properties.Font)
                    },
                    ComponentType.Text => new TextComponent(component, fill),
                    ComponentType.Image => new ImageComponent(component, fill),
                    ComponentType.Icon => new IconComponent(component, fill),
                    _ => throw new Exception("Unknown component type was specified")
                });
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
                Components = components
            };
        }

        private static ICardComponent BuildComponent(ComponentInfo component, CardDetails details, CardProperties properties)
        {
            // Things to set:

            throw new NotImplementedException();
        }
    }
}