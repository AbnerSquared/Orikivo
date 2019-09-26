using System;
using System.Collections.Generic;
namespace Orikivo.Static
{
    public static class UnicodeIndex
    {
        //ftp://ftp.unicode.org/Public/UNIDATA/Blocks.txt
        // Possible to transfer data into a class for unicode?

        public static string LargeEmojiSpacer = "        ";
        public static string SmallEmojiSpacer = "      ";
        public static char Space = ' ';
        public static char HairSpace = ' ';
        public static char Invisible = '­';

        public static string ItalicMarker = "*";
        public static string BoldMarker = "**";
        public static string BoldItalicOpeningMarker = "_**";
        public static string BoldItalicClosingMarker = "**_";
        public static string LineMarker = "`";
        public static string CodeMarker = "```";

        public static Dictionary<char, char> Directional = new Dictionary<char, char>
        {
            { '<', '>' },
            { '[', ']' },
            { '{', '}' },
            { '(', ')' },
            { '/', '\\' }
        };

        public static List<int> OddNumbers = new List<int>
        {
            1,
            3,
            5,
            7,
            9
        };

        public static List<int> EvenNumbers = new List<int>
        {
            0,
            2,
            4,
            6,
            8
        };

        public static List<char> Operators = new List<char>
        {
            '+',
            '-',
            '=',
            '^',
            '/',
            '*',
        };

        public static char OpeningEquation = '(';
        public static char ClosingEquation = ')';

        public static List<char> Letters = new List<char>
        {
            'A', 'a',
            'B', 'b',
            'C', 'c',
            'D', 'd',
            'E', 'e',
            'F', 'f',
            'G', 'g',
            'H', 'h',
            'I', 'i',
            'J', 'j',
            'K', 'k',
            'L', 'l',
            'M', 'm',
            'N', 'n',
            'O', 'o',
            'P', 'p',
            'Q', 'q',
            'R', 'r',
            'S', 's',
            'T', 't',
            'U', 'u',
            'V', 'v',
            'W', 'w',
            'X', 'x',
            'Y', 'y',
            'Z', 'z',
        };

        public static List<char> Numbers = new List<char>
        {
            '0',
            '1',
            '2',
            '3',
            '4',
            '5',
            '6',
            '7',
            '8',
            '9',
        };

        public static List<char> Symbols = new List<char>
        {
            '~', '`',
            '!',
            '@',
            '#',
            '$',
            '%',
            '^',
            '&',
            '*',
            '(',
            ')',
            '_', '-',
            '+', '=',
            '{', '[',
            '}', ']',
            '|', '\\',
            ':', ';',
            '"', '\'',
            '<', ',',
            '>', '.',
            '?', '/',
            ' '
        };

        public static Dictionary<char, string> Subscripts = new Dictionary<char, string>
        {
            {'a', "ₐ"},
            {'e', "ₑ"},
            {'i', "ᵢ"},
            {'o', "ₒ"},
            {'r', "ᵣ"},
            {'u', "ᵤ"},
            {'v', "ᵥ"},
            {'x', "ₓ"},
            {'0', "₀"},
            {'1', "₁"},
            {'2', "₂"},
            {'3', "₃"},
            {'4', "₄"},
            {'5', "₅"},
            {'6', "₆"},
            {'7', "₇"},
            {'8', "₈"},
            {'9', "₉"},
            {'+', "₊"},
            {'-', "₋"},
            {'=', "₌"},
            {'(', "₍"},
            {')', "₎"},
        };

        public static Dictionary<char, string> Superscripts = new Dictionary<char, string>
        {
            {'A', "ᴬ"}, {'a', "ᵃ"},
            {'B', "ᴮ"}, {'b', "ᵇ"},
            {'D', "ᴰ"}, {'d', "ᵈ"},
            {'E', "ᴱ"}, {'e', "ᵉ"},
                        {'f', "ᶠ"},
            {'G', "ᴳ"}, {'g', "ᵍ"},
            {'H', "ᴴ"}, {'h', "ʰ"},
            {'I', "ᴵ"}, {'i', "ⁱ"},
            {'J', "ᴶ"}, {'j', "ʲ"},
            {'K', "ᴷ"}, {'k', "ᵏ"},
            {'L', "ᴸ"}, {'l', "ˡ"},
            {'M', "ᴹ"}, {'m', "ᵐ"},
            {'N', "ᴺ"}, {'n', "ⁿ"},
            {'O', "ᴼ"}, {'o', "ᵒ"},
            {'P', "ᴾ"}, {'p', "ᵖ"},
            {'R', "ᴿ"}, {'r', "ʳ"},
                        {'s', "ˢ"},
            {'T', "ᵀ"}, {'t', "ᵗ"},
            {'U', "ᵁ"}, {'u', "ᵘ"},
                        {'v', "ᵛ"},
            {'W', "ᵂ"}, {'w', "ʷ"},
                        {'x', "ˣ"},
                        {'y', "ʸ"},
                        {'z', "ᶻ"},
            {'0', "⁰"},
            {'1', "¹"},
            {'2', "²"},
            {'3', "³"},
            {'4', "⁴"},
            {'5', "⁵"},
            {'6', "⁶"},
            {'7', "⁷"},
            {'8', "⁸"},
            {'9', "⁹"},
            {'+', "⁺"},
            {'-', "⁻"},
            {'=', "⁼"},
            {'(', "⁽"},
            {')', "⁾"},
        };

        public static Dictionary<char, string> Bold = new Dictionary<char, string>
        {
            {'A', "𝐀"}, {'a', "𝐚"},
            {'B', "𝐁"}, {'b', "𝐛"},
            {'C', "𝐂"}, {'c', "𝐜"},
            {'D', "𝐃"}, {'d', "𝐝"},
            {'E', "𝐄"}, {'e', "𝐞"},
            {'F', "𝐅"}, {'f', "𝐟"},
            {'G', "𝐆"}, {'g', "𝐠"},
            {'H', "𝐇"}, {'h', "𝐡"},
            {'I', "𝐈"}, {'i', "𝐢"},
            {'J', "𝐉"}, {'j', "𝐣"},
            {'K', "𝐊"}, {'k', "𝐤"},
            {'L', "𝐋"}, {'l', "𝐥"},
            {'M', "𝐌"}, {'m', "𝐦"},
            {'N', "𝐍"}, {'n', "𝐧"},
            {'O', "𝐎"}, {'o', "𝐨"},
            {'P', "𝐏"}, {'p', "𝐩"},
            {'Q', "𝐐"}, {'q', "𝐪"},
            {'R', "𝐑"}, {'r', "𝐫"},
            {'S', "𝐒"}, {'s', "𝐬"},
            {'T', "𝐓"}, {'t', "𝐭"},
            {'U', "𝐔"}, {'u', "𝐮"},
            {'V', "𝐕"}, {'v', "𝐯"},
            {'W', "𝐖"}, {'w', "𝐰"},
            {'X', "𝐗"}, {'x', "𝐱"},
            {'Y', "𝐘"}, {'y', "𝐲"},
            {'Z', "𝐙"}, {'z', "𝐳"}
        };

        public static Dictionary<char, string> Italics = new Dictionary<char, string>
        {
            {'A', "𝐴"}, {'a', "𝑎"},
            {'B', "𝐵"}, {'b', "𝑏"},
            {'C', "𝐶"}, {'c', "𝑐"},
            {'D', "𝐷"}, {'d', "𝑑"},
            {'E', "𝐸"}, {'e', "𝑒"},
            {'F', "𝐹"}, {'f', "𝑓"},
            {'G', "𝐺"}, {'g', "𝑔"},
            {'H', "𝐻"}, {'h', "ℎ"},
            {'I', "𝐼"}, {'i', "𝑖"},
            {'J', "𝐽"}, {'j', "𝑗"},
            {'K', "𝐾"}, {'k', "𝑘"},
            {'L', "𝐿"}, {'l', "𝑙"},
            {'M', "𝑀"}, {'m', "𝑚"},
            {'N', "𝑁"}, {'n', "𝑛"},
            {'O', "𝑂"}, {'o', "𝑜"},
            {'P', "𝑃"}, {'p', "𝑝"},
            {'Q', "𝑄"}, {'q', "𝑞"},
            {'R', "𝑅"}, {'r', "𝑟"},
            {'S', "𝑆"}, {'s', "𝑠"},
            {'T', "𝑇"}, {'t', "𝑡"},
            {'U', "𝑈"}, {'u', "𝑢"},
            {'V', "𝑉"}, {'v', "𝑣"},
            {'W', "𝑊"}, {'w', "𝑤"},
            {'X', "𝑋"}, {'x', "𝑥"},
            {'Y', "𝑌"}, {'y', "𝑦"},
            {'Z', "𝑍"}, {'z', "𝑧"}
        };
    }
}