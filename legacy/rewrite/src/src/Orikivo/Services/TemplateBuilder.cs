using System;
using System.Drawing;
using System.Linq;

namespace Orikivo.Systems.Services
{
    public class TemplateBuilder
    {
        public static Bitmap BuildNewLevelTemplate(Bitmap userAvatar)
        {

            const string baseLevelTemplate = ".//Templates//lvl_wide.png";

            var template = Image.FromFile(baseLevelTemplate);
            var avatar = userAvatar;
            var templateBuilder = new Bitmap(template.Width, template.Height, template.PixelFormat);
            using (var graphic = Graphics.FromImage(templateBuilder))
            {
                var templateSize = new Rectangle(0, 0, template.Width, template.Height);
                graphic.DrawImage(template, templateSize);
                template.Dispose();
                var avatarPosition = new Point(16, 16);
                graphic.DrawImage(avatar, avatarPosition);
                avatar.Dispose();
                return templateBuilder;
            }
        }

        public static Bitmap BuildNewProfileBaseTemplate
        (Bitmap userAvatar,
            string userName,
            string userStatus,
            string userActivity,
            int userLevel,
            ulong userCurrency,
            ulong userExp, ulong nextLevelExp, PixelRenderingOptions options)
        {
            const string baseProfileTemplate = ".//resources//profile//template.png";
            const string statusType = ".//resources//profile//status.png";
            const string iconSet = ".//resources//profile//icons.png";
            const string currencyAmountSet = ".//resources//profile//currency-symbols.png";

            userName.Debug();
            userStatus.Debug();
            userActivity.Debug();
            userLevel.Debug();
            userCurrency.Debug();
            userExp.Debug();

            var template = Image.FromFile(baseProfileTemplate);
            
            var avatar = userAvatar;
            var templateBuilder = new Bitmap(template.Width, template.Height, template.PixelFormat);
            using (var graphics = Graphics.FromImage(templateBuilder))
            {
                var templatePosition = new Rectangle(0, 0, template.Width, template.Height);
                graphics.DrawImage(template, templatePosition);
                template.Dispose();
                var avatarPosition = new Point(6, 6);
                graphics.DrawImage(avatar, avatarPosition);
                avatar.Dispose();


                var largeLimit = 17;
                var mediumLimit = 22;
                var smallLimit = 31;
                //var extraSmallLimit = 37;
                var setSize = "l";
                var sizeSpacing = 8;
                if (userName.Length > largeLimit)
                {
                    setSize = "m";
                    sizeSpacing = 6;
                    if (userName.Length > mediumLimit)
                    {
                        setSize = "s";
                        sizeSpacing = 4;
                        if (userName.Length > smallLimit)
                        {
                            setSize = "xs";
                            sizeSpacing = 3;
                        }
                    }
                }

                var statusPos = 0;
                var statuses = new Bitmap(statusType);
                var statusBarPosition = new Rectangle(0, statusPos, statuses.Width, 2);
                var statusBar = statuses.Clone(statusBarPosition, statuses.PixelFormat);
                var statusPlacement = new Point(6, 39);

                if (userStatus.ToLower() == "online")
                {
                    statusPos = 0;
                    statusBarPosition = new Rectangle(0, statusPos, statuses.Width, 2);
                    statusBar = statuses.Clone(statusBarPosition, statuses.PixelFormat);
                    var placeStatus = new Rectangle(statusPlacement, new Size(statusBar.Width, statusBar.Height));
                    graphics.SetClip(placeStatus);
                    graphics.DrawImage(statusBar, placeStatus);
                }
                else if (userStatus.ToLower() == "idle" || userStatus.ToLower() == "afk")
                {
                    statusPos = 2;
                    statusBarPosition = new Rectangle(0, statusPos, statuses.Width, 2);
                    statusBar = statuses.Clone(statusBarPosition, statuses.PixelFormat);
                    var placeStatus = new Rectangle(statusPlacement, new Size(statusBar.Width, statusBar.Height));
                    graphics.SetClip(placeStatus);
                    graphics.DrawImage(statusBar, placeStatus);
                }
                else if (userStatus.ToLower() == "donotdisturb")
                {
                    statusPos = 4;
                    statusBarPosition = new Rectangle(0, statusPos, statuses.Width, 2);
                    statusBar = statuses.Clone(statusBarPosition, statuses.PixelFormat);
                    var placeStatus = new Rectangle(statusPlacement, new Size(statusBar.Width, statusBar.Height));
                    graphics.SetClip(placeStatus);
                    graphics.DrawImage(statusBar, placeStatus);
                }
                else if (userStatus.ToLower() == "offline" || userStatus.ToLower() == "invisible")
                {
                    statusPos = 6;
                    statusBarPosition = new Rectangle(0, statusPos, statuses.Width, 2);
                    statusBar = statuses.Clone(statusBarPosition, statuses.PixelFormat);
                    var placeStatus = new Rectangle(statusPlacement, new Size(statusBar.Width, statusBar.Height));
                    graphics.SetClip(placeStatus);
                    graphics.DrawImage(statusBar, placeStatus);
                }

                var iconPos = 0;
                //var levelLength = 0;
                var icons = new Bitmap(iconSet);
                
                var iconLevelPosition = new Rectangle(iconPos, 0, 4, icons.Height);
                var levelString = "";
                var iconCurrencyPosition = new Rectangle(8, 0, icons.Height, icons.Height);
                var currencyString = "";

                var actDetect = userActivity.Split(' ');
                var baseSpacer = 6 + sizeSpacing + 2;
                //var baseActivityLength = 32;


                var firstActivityLength = 32 - actDetect[0].Length;
                var secondActivityLength = 32;
                if (userActivity.StartsWith("listening to"))
                {
                    var activityName = actDetect[0].Length + 1 + actDetect[1].Length;
                    firstActivityLength = 32 - activityName;
                }


                var currencyPoint = new Point(40, baseSpacer + 6);
                var levelPoint = new Point(40, baseSpacer + 6);
                var gameNameCharacters = userActivity.TrimEnd(' ').ToCharArray();
                gameNameCharacters.Length.Debug();
                firstActivityLength.Debug();

                if (gameNameCharacters.Length - firstActivityLength > 0)
                {
                    levelPoint.Y += 5;
                    currencyPoint.Y += 5;
                }

                var currencyIcon = icons.Clone(iconCurrencyPosition, icons.PixelFormat);
                var levelIcon = icons.Clone(iconLevelPosition, icons.PixelFormat);

                var levelIndex = userLevel.ToString().ToCharArray();
                var levelWidth = 0;
                var levelIntCount = 0;
                var expLevelWidth = 0;
                foreach (var indexWidth in levelIndex)
                {
                    if (indexWidth.Equals('1'))
                    {
                        levelWidth += 3 + 1;
                        expLevelWidth += 3;
                    }
                    else
                    {
                        levelWidth += 8 + 1;
                        expLevelWidth += 8;
                    }

                    levelIntCount += 1;
                }

                currencyPoint.X = 40 + (levelWidth) + 8;



                levelString = $"{userLevel}";

                


                //var currencyPos = 0;
                var currencyTypes = new Bitmap(currencyAmountSet);
                var currencyWidth = 10;
                var currencyTypePos = 0;
                var currencyTypePosition = new Rectangle(currencyTypePos, 0, currencyWidth, currencyTypes.Height);
                var currencyTypePoint = new Point(currencyPoint.X, baseSpacer + 8);
                if (gameNameCharacters.Length - firstActivityLength > 0)
                {
                    currencyTypePoint.Y += 5;
                }

                if ( userCurrency > 999)
                {
                    //var segmentLimit = 0;
                    var currencyDetectDisplayWidth = 0;
                    var splitIntegers = userCurrency.ToString().ToCharArray();

                    //thousand 4-6 : 0,000 > 000,000
                    //million 7-9 : 0,000,000 > 000,000,000




                    if (userCurrency > 999999999)
                    {
                        if (splitIntegers.Length == 10)
                        {
                            currencyDetectDisplayWidth = 1;
                        }
                        if (splitIntegers.Length == 11)
                        {
                            currencyDetectDisplayWidth = 2;
                        }
                        if (splitIntegers.Length == 12)
                        {
                            currencyDetectDisplayWidth = 3;
                        }
                        currencyWidth = 6;
                        currencyTypePos = 20;
                        currencyTypePosition = new Rectangle(currencyTypePos, 0, currencyWidth, currencyTypes.Height);
                    }
                    else if (userCurrency > 999999)
                    {
                        if (splitIntegers.Length == 7)
                        {
                            currencyDetectDisplayWidth = 1;
                        }
                        if (splitIntegers.Length == 8)
                        {
                            currencyDetectDisplayWidth = 2;
                        }
                        if (splitIntegers.Length == 9)
                        {
                            currencyDetectDisplayWidth = 3;
                        }


                        currencyWidth = 10;
                        currencyTypePos = 10;
                        currencyTypePosition = new Rectangle(currencyTypePos, 0, currencyWidth, currencyTypes.Height);
                    }
                    else if (userCurrency > 999)
                    {
                        if (splitIntegers.Length == 4)
                        {
                            currencyDetectDisplayWidth = 1;
                        }
                        if (splitIntegers.Length == 5)
                        {
                            currencyDetectDisplayWidth = 2;
                        }
                        if (splitIntegers.Length == 6)
                        {
                            currencyDetectDisplayWidth = 3;
                        }
                        currencyWidth = 7;
                        currencyTypePos = 0;
                        currencyTypePosition = new Rectangle(currencyTypePos, 0, currencyWidth, currencyTypes.Height);
                    }
                    

                    for (var currencyCount = 0; currencyCount < currencyDetectDisplayWidth; currencyCount++)
                    {
                        currencyString += $"{splitIntegers[currencyCount]}";
                    }

                    var decimalPos = currencyString.Length;

                    if (!splitIntegers[decimalPos].Equals('0'))
                    {
                        currencyString += $".{splitIntegers[decimalPos]}";
                    }
                }
                else
                {
                    currencyString += $"{userCurrency}";
                }

                currencyTypePoint.X += 8 + 1;
                foreach (var display in currencyString)
                {
                    if (display == '1')
                    {
                        currencyTypePoint.X += 3;
                    }
                    else if (display == '.')
                    {
                        currencyTypePoint.X += 2;
                    }
                    else
                    {
                        currencyTypePoint.X += 8;
                    }

                    currencyTypePoint.X += 1;
                }

                var currencyTypeIcon = currencyTypes.Clone(currencyTypePosition, currencyTypes.PixelFormat);

                var levelPlacement = new Rectangle(levelPoint, new Size(4,8));
                var currencyPlacement = new Rectangle(currencyPoint, new Size(8,8));
                var currencyTypePlacement = new Rectangle(currencyTypePoint, new Size(currencyWidth, currencyTypes.Height));


                graphics.SetClip(levelPlacement);
                graphics.DrawImage(levelIcon, levelPlacement);
                levelPoint.X += 5;


                graphics.SetClip(currencyPlacement);
                graphics.DrawImage(currencyIcon, currencyPlacement);
                currencyPoint.X += 9;

                if (userCurrency > 999 && userCurrency < 999999999999)
                {
                    graphics.SetClip(currencyTypePlacement);
                    graphics.DrawImage(currencyTypeIcon, currencyTypePlacement);
                }

                if (userCurrency > 999999999999)
                {
                    currencyString = "∞";
                }

                var expValue = userExp;
                var expMaxValue = nextLevelExp;
                var expBarWidth = expLevelWidth + levelIntCount - 1;
                var expPoint = new Point(40 + 5, baseSpacer + 6 + 8 + 1);
                if (gameNameCharacters.Length - firstActivityLength > 0)
                {
                    expPoint.Y += 5;
                }
                var expPlacement = new Rectangle(expPoint, new Size(expBarWidth, 2));

                
                var fillBrush = new SolidBrush(Color.FromArgb(255, 255, 255));
                var emptyBrush = new SolidBrush(Color.FromArgb(146, 146, 146));
                graphics.SetClip(expPlacement);
                graphics.FillRectangle(emptyBrush, expPlacement);

                ulong expSegment = (ulong)(expMaxValue / (ulong)expBarWidth);
                var fillExpPlacement = new Rectangle(expPoint, new Size(0, 2));

                if (expValue > expSegment)
                {
                    for (ulong segment = 0; segment < (ulong)expBarWidth; segment++)
                    {
                        if (expValue > segment * expSegment)
                        {
                            fillExpPlacement.Width += 1;
                        }
                    }
                    graphics.SetClip(fillExpPlacement);
                    graphics.FillRectangle(fillBrush, fillExpPlacement);
                }

                    graphics.ResetClip();

                    var gameNameSet = userActivity.Split(' ');
                var primaryGameName = "";
                var secondaryGameName = "";
                var primaryLength = 0;
                var secondaryLength = 0;

                var activityList = new[]
                {
                            "playing ",
                            "streaming ",
                            "listening to ",
                            "watching "
                        };

                if (gameNameCharacters.Length >= firstActivityLength)
                    {
                    


                    foreach (var name in gameNameSet)
                    {
                            if (primaryLength >= firstActivityLength)
                            {
                                if (secondaryLength + name.Length <= secondActivityLength)
                                {
                                    secondaryGameName += name + " ";
                                    secondaryLength += name.Length;
                            }
                                else
                                {
                                    secondaryGameName += "...";
                                    break;
                            }

                                
                            }
                            else
                            {
                                if (primaryLength + name.Length <= firstActivityLength)
                                {
                                    if (name.ToLower().Equals("the"))
                                    {
                                        var actCheck = primaryGameName.ToLower();
                                        if (!activityList.Contains(actCheck))
                                        {
                                           
                                            secondaryGameName += name + " ";
                                            primaryLength += firstActivityLength - primaryLength;
                                            secondaryLength += name.Length;
                                        }
                                        else
                                        {
                                            primaryGameName += name + " ";
                                            primaryLength += name.Length;
                                    }
                                    }
                                    else if (name.EndsWith(':'))
                                    {
                                        primaryGameName += name + " ";
                                        primaryLength += firstActivityLength - primaryLength;
                                    }
                                    else {

                                        primaryGameName += name + " ";
                                        primaryLength += name.Length;
                                    }
                                }
                                else
                                {
                                    if (secondaryLength + name.Length <= secondActivityLength)
                                    {
                                        secondaryGameName += name + " ";
                                        secondaryLength += name.Length;
                                }
                                    else
                                    {
                                        secondaryGameName += "...";
                                        break;
                                    }

                                    
                                }
                            }
                    }

                        
                    if (secondaryGameName.StartsWith(' '))
                    {
                        secondaryGameName = secondaryGameName.Remove(0, 1);
                    }
                    //TextConfiguration.PixelateText(primaryGameName, 40, baseSpacer, "s", templateBuilder);
                    //TextConfiguration.PixelateText(secondaryGameName, 40, baseSpacer + 5, "s", templateBuilder)
                }




                //Bitmap lstr = PixelEngine.RenderString(levelString, options);

                //TextConfiguration.PixelateText(levelString, levelPoint.X, levelPoint.Y, "l", templateBuilder);
                //TextConfiguration.PixelateText(currencyString, currencyPoint.X, currencyPoint.Y, "l", templateBuilder);

                var un = PixelEngine.RenderString(userName, options);
                graphics.DrawImage(un, new Point(40, 6));

                int levelCurrencyY = 6 + un.Height + 2;

                if (!string.IsNullOrWhiteSpace(secondaryGameName))
                {
                    Bitmap pgn = PixelEngine.RenderString(primaryGameName, options, 4);
                    graphics.DrawImage(pgn, new Point(40, 6 + un.Height + 2));
                    levelCurrencyY += pgn.Height + 2;

                    Bitmap sgn = PixelEngine.RenderString(secondaryGameName, options, 4);
                    graphics.DrawImage(sgn, new Point(40, 6 + un.Height + pgn.Height + 2 + 1));
                    levelCurrencyY += sgn.Height + 2;
                }
                else
                {
                    var ua = PixelEngine.RenderString(userActivity, options, 4);
                    graphics.DrawImage(ua, new Point(40, 6 + un.Height + 2));
                    levelCurrencyY += ua.Height + 2;
                }

                var lvl = PixelEngine.RenderString(levelString, options, 0);
                graphics.DrawImage(lvl, new Point(levelPoint.X, levelCurrencyY));
                var cur = PixelEngine.RenderString(currencyString, options, 0);
                graphics.DrawImage(cur, new Point(currencyPoint.X, levelCurrencyY));


                //TextConfiguration.PixelateText(userName, 40, 6, setSize, templateBuilder);
                //TextConfiguration.PixelateText(userActivity, 40, 16, "s", templateBuilder);
            }


            return templateBuilder.Color(options.Palette);
        }
    }
}