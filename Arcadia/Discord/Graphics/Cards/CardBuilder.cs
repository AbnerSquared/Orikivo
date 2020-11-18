using System;
using System.Collections.Generic;
using Orikivo;
using Orikivo.Drawing;
using Orikivo.Text;

namespace Arcadia.Graphics
{
    public class CardBuilder
    {
        public static CardInfo BuildCardInfo(CardLayout layout, CardDetails details, CardProperties properties)
        {
            var components = new List<CardComponent>();
            BorderInfo borderInfo = null;

            if (layout.Border != null)
            {
                FillInfo borderFill = GetDefaultFill(properties);

                if (layout.Border.Fill != null)
                {
                    borderFill.SetBaseInfo(layout.Border.Fill);
                }

                borderInfo = layout.Border.Allowed == 0 ? null : new BorderInfo
                {
                    Allowed = layout.Border.Allowed,
                    Thickness = layout.Border.Thickness,
                    Edge = layout.Border.Edge,
                    Fill = borderFill
                };
            }

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
                Trim = GetTrimState(layout.TrimMode, properties.Trim),
                Components = components,
                Palette = properties.Palette,
                Border = borderInfo,
                Scale = properties.Scale
            };
        }

        private static bool GetTrimState(TrimMode mode, bool trim)
        {
            if (mode == TrimMode.None)
                return false;

            if (mode == TrimMode.Force)
                return true;

            return trim;
        }

        private static CardComponent BuildTarget(ComponentInfo info, CardDetails details, CardProperties properties)
        {
            if (!info.PrimaryTarget && info.Type != ComponentType.Solid)
            {
                throw new Exception("Cannot initialize non-primary component targets if the underlying type is not a solid");
            }

            CardComponent component = info.Type switch
            {
                ComponentType.Solid => BuildSolidTarget(info, details, properties), // throw new NotImplementedException("Solids have not been implemented yet"),
                ComponentType.Text => BuildTextTarget(info, details, properties),
                ComponentType.Icon => BuildIconTarget(info, details, properties),
                ComponentType.Image => BuildImageTarget(info, details, properties),
                _ => throw new Exception("Unknown or invalid component information was specified")
            };

            if (info.Outline != null)
            {
                FillInfo outlineInfo = GetDefaultFill(properties);
                outlineInfo.SetBaseInfo(info.Outline);
                component.Outline = outlineInfo;
            }

            return component;
        }

        private static CardComponent BuildSolidTarget(ComponentInfo info, CardDetails details, CardProperties properties)
        {
            FillInfo fillInfo = GetFillInfo(info, details, properties);
            return new SolidComponent(info, fillInfo);
        }

        private static FillInfo GetFillInfo(ComponentInfo info, CardDetails details, CardProperties properties)
        {
            FillInfo fillInfo = GetDefaultFill(properties);

            if (info.Fill != null)
                fillInfo.SetBaseInfo(info.Fill);

            if (info.Group.HasFlag(CardGroup.Exp) && fillInfo.Mode == FillMode.Bar)
            {
                int level = ExpConvert.AsLevel(details.Exp, details.Ascent);
                ulong currentExp = ExpConvert.AsExp(level, details.Ascent);
                ulong nextExp = ExpConvert.AsExp(level + 1, details.Ascent);

                fillInfo.FillPercent = RangeF.Convert(currentExp, nextExp, 0, 1, details.Exp);
            }

            return fillInfo;
        }

        private static CardComponent BuildTextTarget(ComponentInfo info, CardDetails details, CardProperties properties)
        {
            FillInfo fillInfo = GetFillInfo(info, details, properties);
            TextInfo textInfo = null;

            SetTextInfo(ref textInfo, info.Group, details, properties);

            if (textInfo == null)
                throw new Exception("Expected text information to be specified, but returned null");

            return new TextComponent(info, fillInfo)
            {
                Text = textInfo
            };
        }

        private static void SetTextInfo(ref TextInfo target, CardGroup group, CardDetails details, CardProperties properties)
        {
            if (group.HasFlag(CardGroup.Name))
            {
                target = new TextInfo
                {
                    Content = details.Name.ToString(properties.Casing),
                    Font = GraphicsService.GetFont(properties.Font)
                };
            }
            else if (group.HasFlag(CardGroup.Level))
            {
                target = new TextInfo
                {
                    Content = ExpConvert.AsLevel(details.Exp, details.Ascent).ToString(),
                    Font = GraphicsService.GetFont(FontType.Delta)
                };
            }
            else if (group.HasFlag(CardGroup.Money))
            {
                target = new TextInfo
                {
                    Content = Format.Condense(details.Balance, out _),
                    Font = GraphicsService.GetFont(FontType.Delta)
                };
            }
            else if (group.HasFlag(CardGroup.Activity))
            {
                target = new TextInfo
                {
                    Content = details.Program ?? details.Status.ToString(),
                    Font = GraphicsService.GetFont(FontType.Minic)
                };
            }
            else
            {
                throw new Exception("An invalid group was specified for a text component");
            }
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

        private static FillInfo GetDefaultFill(CardProperties properties)
        {
            return new FillInfo
            {
                Palette = properties.Palette,
                Primary = Gamma.Max,
                Mode = FillMode.Solid,
                Direction = Direction.Right
            };
        }
    }
}
