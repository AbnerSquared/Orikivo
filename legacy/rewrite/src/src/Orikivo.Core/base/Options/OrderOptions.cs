using System.Collections;

namespace Orikivo
{
    /// <summary>
    /// Represents a collection of options for ordering collections.
    /// </summary>
    public class OrderOptions
    {
        /// <summary>
        /// Constructs a new OrderOptions with optional specified values.
        /// </summary>
        public OrderOptions(OrderDirection direction, NullObjectHandling? inclusion = null, bool? compare = null)
        {
            Direction = direction;
            Inclusion = inclusion ?? Default.Inclusion;
            Compare = compare ?? Default.Compare;
        }

        /// <summary>
        /// Constructs a new OrderOptions with default values.
        /// </summary>
        public OrderOptions()
        {
            Direction = OrderDirection.Descending;
            Inclusion = NullObjectHandling.Include;
            Compare = false;
        }

        public static OrderOptions Default = new OrderOptions();
        public OrderDirection Direction { get; set; }
        public NullObjectHandling Inclusion { get; set; }
        public bool Compare { get; set; }
        public IComparer Comparer { get; set; }

        public OrderOptions WithDirection(OrderDirection direction)
        { SetDirection(direction); return this; }
        public OrderOptions WithInclusion(NullObjectHandling inclusion)
        { SetInclusion(inclusion); return this; }
        public OrderOptions WithCompare(bool compare)
        { SetCompare(compare); return this; }
        public OrderOptions WithComparer(IComparer comparer)
        { SetComparer(comparer); return this; }
        public void SetDirection(OrderDirection direction)
            => Direction = direction;
        public void SetInclusion(NullObjectHandling inclusion)
            => Inclusion = inclusion;
        public void SetComparer(IComparer comparer)
            => Comparer = comparer;
        public void SetCompare(bool b)
            => Compare = b;
    }
}