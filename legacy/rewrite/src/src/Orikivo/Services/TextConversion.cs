using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
namespace Orikivo.Systems.Services
{    
    public class TextConfiguration
    {
        ///<summary>Pixelate the given text onto the specified Bitmap using an automated font size.</summary>
        public static Bitmap PixelateText(string messageRef, int x, int y, Bitmap destination)
        {
            using (var graphic = Graphics.FromImage(destination))
            {
                //var img = new ImageConfiguration();
                //graphic.PixelOffsetMode = PixelOffsetMode.None;
                var cleanMessage = LowerMessage(messageRef);
                var message = cleanMessage.ToLower().ToCharArray();
                var count = message.Length;
                var resultSize = "";
                var rawSize = 0;
                var autoSpacer = 0;
                var fontData = File.ReadAllText("FontData.json");
                var fontDictionary = JsonConvert.DeserializeObject<Dictionary<string, FontDictionaryType>>(fontData);
                var letterSelectionDictionary = new Dictionary<string, Rectangle>();
                var letterSheetDictionary = new Dictionary<string, Bitmap[]>();

                foreach (var font in fontDictionary)
                {
                    var fontSize = font.Key;
                    var fontStorage = font.Value;
                    var fontType = fontStorage.WidthLimit;
                    var widthSize = fontType[0];
                    var urlStorage = fontStorage.Sheets;
                    var sheetList = new List<Bitmap>();

                    foreach (var url in urlStorage)
                    {
                        sheetList.Add(new Bitmap(url));
                        Console.WriteLine($"Letter sheet acquired. ({url})");
                    }
                    letterSheetDictionary.Add(fontSize, sheetList.ToArray());
                    var widthPoint = new Point(0, 0);
                    var cropBounds = new Size(widthSize, widthSize);
                    letterSelectionDictionary.Add(fontSize, new Rectangle(widthPoint, cropBounds));
                }

                foreach (var limit in fontDictionary)
                {
                    var fontSize = limit.Key;
                    var limitStorage = limit.Value;
                    var fontType = limitStorage.WidthLimit;
                    var widthSize = fontType[0];
                    var limitMax = fontType[1];

                    if (count <= limitMax)
                    {
                        rawSize = widthSize;
                        resultSize = fontSize;
                        break;
                    }
                    Console.WriteLine("Limit hit! Decreasing size... (Unicode values are no longer valid at 21 characters.)");
                }
                Console.WriteLine($"Set font size to {resultSize.ToUpper()}. ({rawSize}px)");

                var dataList = File.ReadAllText("AlphabetData.json");
                var dataValues = JsonConvert.DeserializeObject<Dictionary<string, string[][]>>(dataList);
                
                foreach (var letter in message)
                {
                    var resultLetter = "";
                    var resultSheet = letterSheetDictionary[resultSize][0];
                    var resultCrop = letterSelectionDictionary[resultSize];
                    var type = letter.ToString();
                    if (resultSize == "l")
                    {
                        foreach (var dataValue in dataValues)
                        {
                            var dataSet = int.Parse(dataValue.Key);
                            var arrays = dataValue.Value;
                            foreach (var array in arrays)
                            {
                                foreach (var value in array)
                                {
                                    if (!value.Contains(type)) continue;
                                    resultLetter = value;
                                    resultSheet = letterSheetDictionary[resultSize][dataSet - 1];
                                    resultCrop.X = array.ToList().IndexOf(value) * rawSize;
                                    resultCrop.Y = arrays.ToList().IndexOf(array) * rawSize;
                                    Console.WriteLine(
                                        $"Set '{value}' bounds @ ({resultCrop.X}, {resultCrop.Y}) >> ({resultCrop.Width}px * {resultCrop.Height}px).");
                                }
                            }
                        }
                    }
                    else
                    {
                        var dataValue = dataValues["1"];
                        var dataSet = int.Parse(dataValues.Keys.ToList()[0]);
                        var arrays = dataValue;
                        foreach (var array in arrays)
                        {
                            foreach (var value in array)
                            {
                                if (!value.Contains(type)) continue;
                                resultLetter = value;
                                resultSheet = letterSheetDictionary[resultSize][dataSet - 1];
                                resultCrop.X = array.ToList().IndexOf(value) * rawSize;
                                resultCrop.Y = arrays.ToList().IndexOf(array) * rawSize;
                                Console.WriteLine(
                                    $"Set '{value}' bounds @ ({resultCrop.X}, {resultCrop.Y}) >> ({resultCrop.Width}px * {resultCrop.Height}px).");
                            }
                        }
                    }


                    var resultPixelLetter = resultSheet.Clone(resultCrop, resultSheet.PixelFormat);
                    resultCrop.Width = GetWidth(resultPixelLetter, rawSize);
                    var returnPoint = x + autoSpacer;
                    if (letter == ' ')
                    {
                        autoSpacer += rawSize + 1;
                    }
                    else
                    {
                        resultPixelLetter = resultSheet.Clone(resultCrop, resultSheet.PixelFormat);
                        autoSpacer += resultCrop.Width + 1;
                    }

                    var letterBounds = new Rectangle(returnPoint, y, resultCrop.Width, resultCrop.Height);
                    Console.WriteLine($"Placed '{resultLetter}' @ ({returnPoint}, {y}) >> ({resultCrop.Width}px * {resultCrop.Height}px).");

                    graphic.SetClip(letterBounds, CombineMode.Replace);
                    graphic.DrawImage(resultPixelLetter, letterBounds);
                    resultPixelLetter.Dispose();
                }

                foreach (var sheets in letterSheetDictionary.Values)
                {
                    foreach (var sheet in sheets)
                    {
                        sheet.Dispose();
                    }
                }

                return destination;
            }
        }

        public static Bitmap PixelateText(string s, Point p, string size, Bitmap image, PixelRenderingOptions options = null)
            => PixelateText(s, p.X, p.Y, size, image, options);

        ///<summary>Pixelate the given text onto the specified Bitmap using an manually set font size.</summary>
        public static Bitmap PixelateText(string s, int x, int y, string size, Bitmap destination, PixelRenderingOptions options = null)
        {
            using (var graphic = Graphics.FromImage(destination))
            {
                //var img = new ImageConfiguration();
                //graphic.PixelOffsetMode = PixelOffsetMode.None;
                var cleanMessage = LowerMessage(s);
                var message = cleanMessage.ToLower().ToCharArray();
                var resultSize = "";
                var rawSize = 0;
                var autoSpacer = 0;
                var fontData = File.ReadAllText("FontData.json");
                var fontDictionary = JsonConvert.DeserializeObject<Dictionary<string, FontDictionaryType>>(fontData);
                var letterSelectionDictionary = new Dictionary<string, Rectangle>();
                var letterSheetDictionary = new Dictionary<string, Bitmap[]>();

                foreach (var font in fontDictionary)
                {
                    var fontSize = font.Key;
                    var fontStorage = font.Value;
                    var fontType = fontStorage.WidthLimit;
                    var widthSize = fontType[0];
                    var urlStorage = fontStorage.Sheets;
                    var sheetList = new List<Bitmap>();

                    foreach (var url in urlStorage)
                    {
                        sheetList.Add(new Bitmap(url));
                        Console.WriteLine($"Letter sheet acquired. ({url})");
                    }
                    letterSheetDictionary.Add(fontSize, sheetList.ToArray());
                    var widthPoint = new Point(0, 0);
                    var cropBounds = new Size(widthSize, widthSize);
                    letterSelectionDictionary.Add(fontSize, new Rectangle(widthPoint, cropBounds));
                }

                foreach (var limit in fontDictionary)
                {
                    var fontSize = limit.Key;
                    var limitStorage = limit.Value;
                    var fontType = limitStorage.WidthLimit;
                    var widthSize = fontType[0];

                    if (size == fontSize)
                    {
                        rawSize = widthSize;
                        resultSize = fontSize;
                        break;
                    }
                    Console.WriteLine("Decreasing size... (Unicode values are no longer valid at 21 characters.)");
                }
                Console.WriteLine($"Set font size to {resultSize.ToUpper()}. ({rawSize}px)");

                var dataList = File.ReadAllText("AlphabetData.json");
                var dataValues = JsonConvert.DeserializeObject<Dictionary<string, string[][]>>(dataList);

                foreach (var letter in message)
                {
                    if (letter.Equals(' '))
                    {
                        autoSpacer += rawSize + 1;
                    }

                    var resultLetter = "";
                    var resultSheet = letterSheetDictionary[resultSize][0];
                    var resultCrop = letterSelectionDictionary[resultSize];
                    var type = letter.ToString();
                    if (resultSize == "l")
                    {
                        foreach (var dataValue in dataValues)
                        {
                            var dataSet = int.Parse(dataValue.Key);
                            var arrays = dataValue.Value;
                            foreach (var array in arrays)
                            {
                                foreach (var value in array)
                                {
                                    if (!value.Contains(type)) continue;
                                    resultLetter = value;
                                    resultSheet = letterSheetDictionary[resultSize][dataSet - 1];
                                    resultCrop.X = array.ToList().IndexOf(value) * rawSize;
                                    resultCrop.Y = arrays.ToList().IndexOf(array) * rawSize;
                                    //Console.WriteLine($"Set '{value}' bounds @ ({resultCrop.X}, {resultCrop.Y}) >> ({resultCrop.Width}px * {resultCrop.Height}px).");
                                }
                            }
                        }
                    }
                    else
                    {
                        var dataValue = dataValues["1"];
                        var dataSet = int.Parse(dataValues.Keys.ToList()[0]);
                        var arrays = dataValue;
                        foreach (var array in arrays)
                        {
                            foreach (var value in array)
                            {
                                if (!value.Contains(type)) continue;
                                resultLetter = value;
                                resultSheet = letterSheetDictionary[resultSize][dataSet - 1];
                                resultCrop.X = array.ToList().IndexOf(value) * rawSize;
                                resultCrop.Y = arrays.ToList().IndexOf(array) * rawSize;
                                //Console.WriteLine($"Set '{value}' bounds @ ({resultCrop.X}, {resultCrop.Y}) >> ({resultCrop.Width}px * {resultCrop.Height}px).");
                            }
                        }
                    }

                    if (resultLetter.Equals("")) continue;
                    var resultPixelLetter = resultSheet.Clone(resultCrop, resultSheet.PixelFormat);
                    resultCrop.Width = GetWidth(resultPixelLetter, rawSize);
                    var returnPoint = x + autoSpacer;
                    if (letter != ' ')
                    {
                        resultPixelLetter = resultSheet.Clone(resultCrop, resultSheet.PixelFormat);
                        autoSpacer += resultCrop.Width + 1;
                    }

                    var letterBounds = new Rectangle(returnPoint, y, resultCrop.Width, resultCrop.Height);
                    //Console.WriteLine($"Placed '{resultLetter}' @ ({returnPoint}, {y}) >> ({resultCrop.Width}px * {resultCrop.Height}px).");

                    graphic.SetClip(letterBounds, CombineMode.Replace);
                    graphic.DrawImage(resultPixelLetter, letterBounds);
                    resultPixelLetter.Dispose();
                }

                foreach (var sheets in letterSheetDictionary.Values)
                {
                    foreach (var sheet in sheets)
                    {
                        sheet.Dispose();
                    }
                }

                return destination;
            }
        }

        public static int GetWidth(Bitmap image, int size)
        {
            var imageGrid = image;
            var binary = new List<int>();

            for (var y = 0; y < imageGrid.Height; y++)
            {
                for (var x = 0; x < imageGrid.Width; x++)
                {
                    var pixel = imageGrid.GetPixel(x, y);
                    binary.Add(pixel.A == 0 ? 0 : 1);
                }
            }

            var binaryGrid = new int[size][];
            var gridDisplay = "";
            var binaryGridRotated = new int[size][];
            var rotatedGridDisplay = "";
            var valPos = size;
            var cAmt = 0;

            for (var p = 0; p < size; p++)
            {
                var gridRow = new List<int>();
                for (var val = size * cAmt; val < valPos; val++)
                {
                    gridRow.Add(binary.ToArray()[val]);
                }
                binaryGrid[p] = gridRow.ToArray();
                var rowDisplay = string.Join("", binaryGrid[p]);
                gridDisplay += $"{rowDisplay}\n";
                valPos += size;
                cAmt += 1;
            }
            //Console.WriteLine($"Binary grid set.\n{gridDisplay}");

            for (var y = 0; y < binaryGrid.Length; y++)
            {
                var gridCol = new List<int>();
                for (var x = 0; x < binaryGrid[y].Length; x++)
                {
                    gridCol.Add(binaryGrid[x].Reverse().ToArray()[y]);
                }
                binaryGridRotated[y] = gridCol.ToArray();
                var rotatedRowDisplay = string.Join("", binaryGridRotated[y]);
                rotatedGridDisplay += $"{rotatedRowDisplay}\n";
            }
            //Console.WriteLine($"Rotated binary grid 90 degrees.\n{rotatedGridDisplay}");
            return binaryGridRotated.Select(column => column.Aggregate("", (current, value) => current + $"{value}")).TakeWhile(columnString => !columnString.Contains('1')).Aggregate(size, (current1, columnString) => current1 - 1);
        }

        public static string LowerMessage(string message)
        {
            var loweredMessage = message.ToLower();
            var lowMsgChars = message.ToLower().ToCharArray();
            var codeList = new char[2][];
            codeList[0] = new[] {'Ç', 'Ü', 'É', 'Â', 'Ä', 'À', 'Å', 'Ê', 'Ë', 'È', 'Ï', 'Î', 'Ì', 'Æ', 'Ô', 'Ö', 'Ò',
                                 'Û', 'Ù', 'Ÿ', 'Á', 'Í', 'Ó', 'Ú', 'Ñ', 'Š', 'Œ', 'Ž', 'Ã', 'Ð', 'Õ', 'Ø', 'Ý', 'Þ'};
            codeList[1] = new[] {'ç', 'ü','é', 'â', 'ä', 'à', 'å', 'ê', 'ë', 'è', 'ï', 'î', 'ì', 'æ', 'ô', 'ö', 'ò',
                                 'û', 'ù', 'ÿ', 'á', 'í', 'ó', 'ú', 'ñ', 'š', 'œ', 'ž', 'ã', 'ð', 'õ', 'ø', 'ý', 'þ'};

            foreach (var character in lowMsgChars)
            {
                if (!codeList[0].Contains(character)) continue;
                var location = codeList[0].ToList().IndexOf(character);
                loweredMessage = message.Replace(codeList[0][location], codeList[1][location]);
            }
            message = loweredMessage;

            return message;
        }
    }

    public class FontDictionaryType
    {
        [JsonProperty("widthlimit")] public List<int> WidthLimit { get; set; }
        [JsonProperty("sheetlist")] public List<string> Sheets { get; set; }
    }

    public class TextRepair
    {
        public string CleanHtml(string htmlString)
        {
            string tagPattern = "<.*?>";
            var regex = new Regex("(\\<script(.+?)\\</script\\>) | (\\<style(.+?)\\</style\\>)", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            htmlString = htmlString.Replace("<br>", "\n");
            htmlString = htmlString.Replace("<br />", "\n");
            htmlString = htmlString.Replace("<strong>", "**");
            htmlString = htmlString.Replace("</strong>", "**");
            htmlString = htmlString.Replace("\t", string.Empty);
            htmlString = Regex.Replace(htmlString, tagPattern, string.Empty);
            htmlString = Regex.Replace(htmlString, @"^\s+$[\r\n]*", "", RegexOptions.Multiline);
            htmlString = htmlString.Replace("&nbsp;", string.Empty);
            htmlString = htmlString.Replace("&amp;", "&");
            htmlString = htmlString.Replace("&quot;", "\"");
            htmlString = htmlString.Replace("&gt;", ">");

            return htmlString;
        }
    }
}