namespace Orikivo
{
    /*
     This command parser needs to read the following:

        - Prefix (this can be a bot mention OR a simple string that represents the command
        - Alias (the command that is being referenced)
       
        If the command does not have any options, this part of the parser can be ignored
        - Options ( --option_name value OR -option_alias value)
        - Arguments (value OR [value1, value2, value3] OR "infinite string" OR [value1 value2 value3]
         
         
         
         
         
         */
    public class CommandParser
    {
    }



    // can peek to 
    public class StringReader
    {
        // the value
        private string _string;
        
        // the offset
        private int _cursor;


        // return the char at the current cursor
        public char Peek()
        {
            return _string[_cursor];
        }

        public char Peek(int offset)
        {
            return _string[_cursor + offset];
        }

        public char Read()
        {
            return _string[_cursor++];
        }
    }
}
