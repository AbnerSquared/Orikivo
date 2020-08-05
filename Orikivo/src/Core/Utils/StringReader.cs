using System.Text;

namespace Orikivo
{
    // https://github.com/Mojang/brigadier/blob/master/src/main/java/com/mojang/brigadier/StringReader.java
    public class StringReader
    {
        private const char SYNTAX_ESCAPE = '\\';
        private const char SYNTAX_DOUBLE_QUOTE = '"';
        private const char SYNTAX_SINGLE_QUOTE = '\'';

        public static bool IsQuotedStringStart(char c)
        {
            return c == SYNTAX_DOUBLE_QUOTE || c == SYNTAX_SINGLE_QUOTE;
        }

        public static bool IsUnquotedStringValid(char c)
        {
            return c >= '0' && c <= '9'
                || c >= 'A' && c <= 'Z'
                || c >= 'a' && c <= 'z'
                || c == '_' || c == '-'
                || c == '.' || c == '+';
        }

        public static bool IsNumberValid(char c)
        {
            return c >= '0' && c <= '9' || c == '.' || c == '-';
        }

        public StringReader(string input)
        {
            _string = input;
            _cursor = 0;
        }

        // the value
        private string _string;

        // the offset
        private int _cursor;

        public int GetCursor()
        {
            return _cursor;
        }

        public void SetCursor(int cursor)
        {
            _cursor = cursor;
        }


        // Get the char without moving
        public char Peek()
        {
            return _string[_cursor];
        }

        // Get the char at the cursor + offset without moving
        public char Peek(int offset)
        {
            return _string[_cursor + offset];
        }

        // Get the char at the cursor and move 1
        public char Read()
        {
            return _string[_cursor++];
        }

        // Get the set of chars at the cursor + length.
        public string Read(int len)
        {
            return _string[_cursor..(_cursor + len)];
        }

        public string GetRead()
        {
            return _string[.._cursor];
        }

        public string GetRemaining()
        {
            return _string[_cursor..];
        }

        public bool CanRead(int len)
        {
            return _cursor + len <= _string.Length;
        }

        public bool StartsWith(char c)
        {
            return GetRemaining().StartsWith(c);
        }

        public bool StartsWith(string text)
        {
            return GetRemaining().StartsWith(text);
        }

        public bool CanRead()
        {
            return CanRead(1);
        }

        public void Skip()
        {
            _cursor++;
        }

        public void Skip(int len)
        {
            _cursor += len;
        }

        public void SkipWhiteSpace()
        {
            while (CanRead() && char.IsWhiteSpace(Peek()))
                Skip();
        }

        public void SkipUntil(char terminator)
        {
            while (CanRead() && Peek() != terminator)
                Skip();

            throw new System.Exception("Expected terminator");
        }

        public string ReadNumber(out int start)
        {
            start = _cursor;

            while (CanRead() && IsNumberValid(Peek()))
                Skip();

            string number = _string[start.._cursor];

            if (string.IsNullOrWhiteSpace(number))
                throw new System.Exception("Expected numerical value");

            return number;
        }

        public int ReadInt()
        {
            string number = ReadNumber(out int start);

            if (int.TryParse(number, out int result))
                return result;

            _cursor = start;
            throw new System.Exception("Invalid Int32 value");
        }

        public long ReadLong()
        {
            string number = ReadNumber(out int start);

            if (long.TryParse(number, out long result))
                return result;

            _cursor = start;
            throw new System.Exception("Invalid Int64 value");
        }

        public float ReadFloat()
        {
            string number = ReadNumber(out int start);

            if (float.TryParse(number, out float result))
                return result;

            _cursor = start;
            throw new System.Exception("Invalid Single value");
        }

        public double ReadDouble()
        {
            string number = ReadNumber(out int start);

            if (double.TryParse(number, out double result))
                return result;

            _cursor = start;
            throw new System.Exception("Invalid Double value");
        }

        public string ReadUntil(char terminator)
        {
            var result = new StringBuilder();
            bool escaped = false;

            while (CanRead())
            {
                char c = Read();

                if (escaped)
                {
                    if (c == terminator || c == SYNTAX_ESCAPE)
                    {
                        result.Append(c);
                        escaped = false;
                    }
                    else
                    {
                        _cursor--;
                        throw new System.Exception("Invalid escape");
                    }
                }
                else if (c == SYNTAX_ESCAPE)
                {
                    escaped = true;
                }
                else if (c == terminator)
                {
                    return result.ToString();
                }
                else
                {
                    result.Append(c);
                }
            }

            throw new System.Exception("Expected terminator");
        }

        public string ReadString()
        {
            if (!CanRead())
            {
                return "";
            }

            char next = Peek();

            if (IsQuotedStringStart(next))
            {
                Skip();
                return ReadUntil(next);
            }

            return ReadUnquotedString();
        }

        public string ReadUnquotedString()
        {
            int start = _cursor;

            while (CanRead() && IsUnquotedStringValid(Peek()))
                Skip();

            return _string[start.._cursor];
        }

        public bool ReadBool()
        {
            int start = _cursor;
            string value = ReadString();

            if (string.IsNullOrWhiteSpace(value))
                throw new System.Exception("Expected boolean value");

            if (value.Equals("true"))
                return true;
            else if (value.Equals("false"))
                return false;
            else
            {
                _cursor = start;
                throw new System.Exception("Invalid boolean value");
            }
        }

        public bool Contains(char c)
        {
            while (CanRead())
            {
                if (Read() == c)
                    return true;
            }

            return false;
        }


        public void Expect(char c)
        {
            if (!CanRead() || Peek() != c)
                throw new System.Exception($"Expected char '{c}'");

            Skip();
        }
    }
}
