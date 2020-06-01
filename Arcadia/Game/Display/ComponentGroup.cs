namespace Arcadia
{
    // a list of strings
    public class ComponentGroup : IComponent
    {
        public string Id { get; }

        public bool Active { get; set; }

        public int Position { get; set; }

        public ComponentFormatter Formatter { get; }

        // set the initial capacity, cannot be changed once set
        public int Capacity { get; }

        // the array is initialized with the specified capacity.
        public string[] Values;

        // if there are no empty slots before the first specified index
        // it will shift everything in the array to the right
        // and set the smallest index to this value
        public void Prepend(string value)
        {

        }

        // if there are no empty slots after the last specified index
        // it will shift everything in the array to the left
        // and set the largest index to this value
        public void Append(string value)
        {

        }

        // this will set a specific index to the specified value
        public void Set(int index, string value)
        {

        }

        // this will set a specific value in its index to null
        public void Remove(int index)
        {

        }

        // this will remove all values from the array
        public void Clear()
        {

        }


        // this renders the component group
        public string Draw()
        {
            return "";
        }
    }
}
