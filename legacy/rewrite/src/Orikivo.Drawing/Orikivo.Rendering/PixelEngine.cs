using System;
using System.Drawing;
using System.Linq;
using System.Collections.Generic;
using Discord;
using Discord.WebSocket;
using Color = System.Drawing.Color;
using Orikivo.Static;
using System.IO;
using Orikivo.Systems.Services;
using Image = System.Drawing.Image;

namespace Orikivo
{

    // make the card layout first,
    // and referring to that,
    // place it

    public static partial class PixelEngine
    {
        /* OLD */
        public static Bitmap Pixelate(SocketUser user, PixelRenderingOptions options)
            => Pixelate(AvatarManager.GetAvatarBitmap(user), options);

        public static Bitmap Pixelate(Bitmap bmp, Color[] palette)
        {
            Size s = new Size(8 * 4, 8 * 4);
            bmp = bmp.ToPalette(palette);
            return bmp.HasSize(s) ? bmp : bmp.Resize(s);
        }

        public static Bitmap Pixelate(Bitmap bmp, PixelRenderingOptions options)
        {
            Size s = new Size(options.PPU * 4, options.PPU * 4);
            bmp = bmp.ToPalette(options.Palette);
            return bmp.HasSize(s) ? bmp : bmp.Resize(s);
        }

        // This focuses on building the base profile card.
        public static Bitmap DrawCard(Bitmap avatar, string name, string status,
            string activity, int level, ulong balance, ulong exp, ulong nextexp,
            PixelRenderingOptions options)
        {
            //Determine card size, by getting the render boxes
            //of each element.
            Image template = Image.FromFile($".//resources//profile//template.png");
            Bitmap card = new Bitmap(200, 150, template.PixelFormat);
            using (Graphics g = Graphics.FromImage(card))
            {
                // base background render.... border design???
                //Rectangle templateCrop = new Rectangle(0, 0, template.Width, template.Height);
                //g.DrawImage(template, templateCrop);

                // avatar render
                Point avatarPoint = new Point(2, 2);
                g.DrawImage(avatar, avatarPoint);

                // username render
                Bitmap nameImage = RenderString(name, options);
                Point namePoint = new Point(36, 2);
                g.DrawImage(nameImage, namePoint);

                Bitmap statusImage = RenderString(status, options, 4);
                Point statusPoint = new Point(36, 12);
                g.DrawImage(statusImage, statusPoint);

                //Bitmap activityImage = RenderString(activity, options, 4);
                //Point activityPoint = new Point(2, 22);
                //g.DrawImage(activityImage, activityPoint);

                Bitmap balanceImage = RenderString(balance.ToPlaceValue(), options, 3);
                Point balancePoint = new Point(2, 36);
                g.DrawImage(balanceImage, balancePoint);

                //Bitmap expImage = RenderString(exp.ToPlaceValue(), options, 3);
                //Point expPoint = new Point(2, 42);
                //g.DrawImage(expImage, expPoint);


            }

            return card.Color(options.Palette);
        }

        // this builds the profile base.
        // this focuses on placing the backdrop
        // the border
        // and all of the base decals
        /*public static Bitmap DrawCardBase(CardRenderingOptions options)
        {

        }*/

        public static CompactUser Compact(SocketUser user)
            => new CompactUser(user);
        
        public static Bitmap SetCard(SocketUser user, PixelRenderingOptions options)
            => SetCard(Compact(user), options);

        public static string GetCardAsPath(SocketUser user, PixelRenderingOptions options)
            => GetCardAsPath(Compact(user), options);

        // Draws on the Graphics at a specified point, a status bar with StatusRenderingOptions
        public static void SetStatus(Graphics g, Point p, StatusRenderingOptions status)
        {
            Bitmap statusMap = new Bitmap(Locator.StatusSheet);
            using (Bitmap b = statusMap)
            {
                // the bar is 2px in height.
                int statusBarHeight = 2;
                Rectangle crop = new Rectangle(0, status.Y, statusMap.Width, statusBarHeight);
                Bitmap bar = statusMap.Clone(crop, statusMap.PixelFormat);
                Rectangle spot = new Rectangle(p, bar.Size);
                g.SetClip(spot); // the spot to draw at the rectangle.
                g.DrawImage(bar, spot); // draws.
                statusMap.Dispose();
            }

        }

        public static void SetActivity()
        {

        }

        public static string GetCardAsPath(CompactUser user, PixelRenderingOptions options)
        {
            string s = "e";
            s.Debug("This is where the card is sent back as a path.");
            Bitmap card = SetCard(user, options);
            string path = $"{Directory.CreateDirectory($".//data//accounts//{user.Id}//resources//").FullName}card.png";
            card.SaveBitmap(path, ImageFormat.Png);
            return path;
        }

        public static Bitmap SetCard(CompactUser u, PixelRenderingOptions options)
        {
            string s = "e";
            s.Debug("This is where the card is being built.");
            // Set the default attributes.
            Bitmap template = new Bitmap(Locator.ProfileTemplate);
            Bitmap currencyMap = new Bitmap(Locator.CurrencySheet);
            Bitmap iconMap = new Bitmap(Locator.IconSheet);

            Bitmap tmp = new Bitmap(template.Width, template.Height, template.PixelFormat);

            // Build the display card.
            using (Graphics g = Graphics.FromImage(tmp))
            {
                // Gets the letter rendering options for the username.
                LetterRenderingOptions config = new LetterRenderingOptions(u.Username);
                // Gets the status rendering options.
                StatusRenderingOptions status = new StatusRenderingOptions(u.Status);

                // Reduce the need for constant image opening.
                // This will help reduce constant rebuilding.

                // Size: 150px by 200px.
                // Border: 2px empty
                // 2px pixels
                // 2px empty
                // everything after can be placed.
                
                // Actual Size: 138px by 188px
                    // actual starting x: 6px
                    // actual starting y: 6px;
                    // actual width: 144px
                    // actual height 194px

                int padding = 2; // 2px
                int lineWidth = 2; // 2px
                int borderWidth = (2*padding)+lineWidth; // 6px
                int avatarWidth = 32; // 32px

                // This is the vertical position of where the level and currency need to be.
                // This gets the username size, and shifts it down based on size.
                // This also gets shifted down by the current activity or state.


                Point defaultPoint = new Point(borderWidth, borderWidth);

                // The space after the starting border and avatar, with a 2px padding.
                int afterAvatarX = defaultPoint.X + avatarWidth + padding;

                Point usernamePoint = new Point(afterAvatarX, defaultPoint.Y);
                // This is the base board plus the font size plus padding.
                int usernameHeight = borderWidth + config.Spacing + padding; // 6 + spacing + 2

                

                int activityTextSize = 4; // 4px;
                Point activityPoint = new Point(afterAvatarX, usernameHeight);

                // The position at which the user name is written.
                // Font size used for activity is 4px.
                // To make a new line, it would be 4px + padding.
                int afterFirstActivity = activityPoint.Y + activityTextSize + padding;
                
                // This is where the second activity line would be placed if one exists
                bool hasSecondActivity = false;

                // this is where you want to see if there's an activity.

                // this sets the text to render.
                string firstActivity = ""; // the first line for the activity to render.

                int firstActivityTextLimit = 32; // Only 32 letters may be rendered per line.
                int secondActivityTextLimit = 32; // Only 32 letters may be rendered per line.

                string secondActivity = ""; // the second line for the activity to render.
                if (u.Activity.Exists())
                { // activity does exist
                    firstActivity = u.Activity.Type.ToTypeString(); // Playing, Listening, etc.
                    // deduct the length of the playing type from the limit of the first line.
                    firstActivityTextLimit -= firstActivity.Length;
                    // get the activity name, and check if the name is longer than the limit
                    // if so, set hasSecondActivity to true
                    // and set the second activity text to 32;
                    
                }
                else
                    firstActivity = status.State;









                int secondActivityX = afterAvatarX;
                Point secondActivityPoint = new Point(afterAvatarX, afterFirstActivity);
                
                // The y position of the level balance point after a second activity, if any.
                // 2px padding as default, with 1px added to help make it look nicer.
                int afterSecondActivity = secondActivityPoint.Y + padding + 1;

                // The + 1px is for another layer of padding.
                Point levelBalancePoint =
                    hasSecondActivity ?
                    new Point(afterAvatarX, afterSecondActivity) :
                    new Point(secondActivityPoint.X, secondActivityPoint.Y + 1);


                // This places the identifying state as a visual bar.
                // x: 6px
                // y: 6px + 32px + 1px : 39px // padding should only be one px.
                Point statusBarPoint = new Point(defaultPoint.X, defaultPoint.Y + avatarWidth + 1);


                // Places the template built before this method.
                /*using (Bitmap b = template)
                {
                    g.DrawImage(b, b.ToRectangle());
                    template.Dispose();
                }*/
                var borderBrush = new SolidBrush(options.Palette[0]);


                // Size: 150px by 200px.
                // Border: 2px empty
                // 2px pixels
                // 2px empty
                // everything after can be placed.
                
                // Actual Size: 138px by 188px
                    // actual starting x: 6px
                    // actual starting y: 6px;
                    // actual width: 144px
                    // actual height 194px

                    // 47 pixels tall - 2px padding on both ends = 43px tall
                
                // LEFT BORDER
                Rectangle lBorder = new Rectangle(2, 2, 2, 43);

                g.SetClip(lBorder);
                g.FillRectangle(borderBrush, lBorder);

                // RIGHT BORDER
                Rectangle rBorder = new Rectangle(196, 2, 2, 43);

                g.SetClip(rBorder);
                g.FillRectangle(borderBrush, rBorder);

                // TOP BORDER
                Rectangle tBorder = new Rectangle(2, 2, 196, 2);

                g.SetClip(tBorder);
                g.FillRectangle(borderBrush, tBorder);

                // BOTTOM BORDER
                Rectangle bBorder = new Rectangle(2, 43, 196, 2);

                g.SetClip(bBorder);
                g.FillRectangle(borderBrush, bBorder);

                g.ResetClip();

                borderBrush.Dispose();

                // Avatar: 32px by 32px
                // Draw the Avatar onto the graphics.
                using (Bitmap b = Pixelate(u.Avatar, options))
                {
                    g.DrawImage(b, defaultPoint);
                    u.Avatar.Dispose();
                }

                // Place user status bar.
                SetStatus(g, statusBarPoint, status);

                // Place username.
                TextConfiguration.PixelateText(u.Username, usernamePoint, config.Size, tmp, options);

                // Place activity, or state if not doing anything.

                // Place current level.
                    // Place icon
                /*using (Bitmap b = iconMap)
                {
                    // crops the sprite map icon to get the level icon.
                    Rectangle levelCrop = new Rectangle(0, 0, 4, iconMap.Height);
                    Rectangle coinCrop = new Rectangle(8, 0, iconMap.Height, iconMap.Height);

                    Bitmap icon = iconMap.Clone(levelCrop, iconMap.PixelFormat);
                    Bitmap coin = iconMap.Clone(coinCrop, iconMap.PixelFormat);

                    levelBalancePoint.Y -= activityTextSize + padding;
                    Rectangle iconSpot = new Rectangle(levelBalancePoint, icon.Size);
                    Point balancePoint = new Point(levelBalancePoint.X + 8, levelBalancePoint.Y);
                    Rectangle coinSpot = new Rectangle(balancePoint, coin.Size);

                    g.SetClip(iconSpot);
                    g.DrawImage(icon, iconSpot);
                    g.SetClip(coinSpot);
                    g.DrawImage(coin, coinSpot);

                    iconMap.Dispose();

                    // get the position of the level
                    // get the position of the coin
                    // build.
                }*/
                

                // Place current balance.
                    // Place coin icon
                    // then place the money amount
                    // and if needed, the icon of the money amount.

                // Place experience bar, with the percentage to the next level.
                    // first, get the point of where the experience bar is painted.
                    // get the width of the experience bar based on the width of the level
                    // divide the pixels into segments
                    // divide the experience by the segments
                    // if experience is higher than a segment, paint.
                    // otherwise, leave empty.

                // Close off the entire image, and scale.

                // Send.
            }

            tmp = tmp.Resize(options.Scale);

            return tmp;
        }

        public static BalanceCropOptions Compact(ulong b)
            => new BalanceCropOptions(ValuePoint(b), Simplify(b));
        
        public static double Simplify(ulong b)
            => (b > 999) ? b / Math.Pow(10, b.ToString().Length) : b;

        public static CurrencyPoint ValuePoint(ulong b)
        {
            if (b > 999999999) return CurrencyPoint.Billion;
            else if (b > 999999) return CurrencyPoint.Million;
            else if (b > 999) return CurrencyPoint.Thousand;
            else return CurrencyPoint.Hundred;
        }
    }

    public enum CurrencyPoint
    {
        Hundred = -1,
        Thousand = 0,
        Million = 10,
        Billion = 20
    }

    public class BalanceCropOptions
    {
        public BalanceCropOptions(CurrencyPoint point, double value)
        {
            SetValue(point);
            Value = value;
        }

        public int X { get; set; }
        public int Width { get; set; }
        public double Value { get; }
        
        public void SetValue(CurrencyPoint point)
        {
            switch(point)
            {
                // x is for x position.
                // y is for width.
                case CurrencyPoint.Hundred:
                    X = -1;
                    Width = -1;
                    break;
                case CurrencyPoint.Thousand:
                    X = 0;
                    Width = 7;
                    break;
                case CurrencyPoint.Million:
                    X = 10;
                    Width = 10;
                    break;
                case CurrencyPoint.Billion:
                    X = 20;
                    Width = 6;
                    break;
                default:
                    X = -1;
                    Width = -1;
                    break;
            }
        }
    }

    // Used to locate and place the status bar, while holding the status string if there is no
    // activity.
    public class StatusRenderingOptions
    {
        public StatusRenderingOptions(UserStatus status)
        {
            Set(status);
        }

        public string State { get; set; }
        public int Y { get; set; }

        public void Set(UserStatus status)
        {
            string s = "online";
            int y = 0;
            if (status.EqualsAny(UserStatus.Idle, UserStatus.AFK))
            {
                s = "idling";
                y = 2;
            }
            else if (status.Equals(UserStatus.DoNotDisturb))
            {
                s = "busy";
                y = 4;
            }
            else if (status.EqualsAny(UserStatus.Offline, UserStatus.Invisible))
            {
                s = "offline";
                y = 6;
            }

            State = s;
            Y = y;
        }
    }

    // Used to properly determine letter sizing
    public class LetterRenderingOptions
    {
        public LetterRenderingOptions(string s)
        {
            Set(s.Length);
        }
        public LetterRenderingOptions(int length)
        {
            Set(length);
        }

        public string Size { get; set; }
        public int Spacing { get; set; }

        public void Set(int i)
        {
            if (i > 31) // larger than SMALL
            {
                Size = "xs";
                Spacing = 3;
            }
            else if (i > 22) // larger than MEDIUM
            {
                Size = "s";
                Spacing = 4;
            }
            else if (i > 17) // larger than LARGE
            {
                Size = "m";
                Spacing = 6;
            }
            else // LARGE BY DEFAULT
            {
                Size = "l";
                Spacing = 8;
            }
        }
    }
}
