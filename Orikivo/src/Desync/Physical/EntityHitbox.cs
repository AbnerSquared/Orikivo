using Orikivo.Drawing;
using Orikivo.Drawing.Graphics2D;

namespace Orikivo.Desync
{
    /// <summary>
    /// Represents a hitbox for a generic entity.
    /// </summary>
    public class EntityHitbox
    {
        /// <summary>
        /// Initializes a new <see cref="EntityHitbox"/>.
        /// </summary>
        /// <param name="x">The x-position of this entity.</param>
        /// <param name="y">The y-position of this entity.</param>
        /// <param name="sight">The maximum view distance of this entity.</param>
        /// <param name="reach">The maximum reach of this entity.</param>
        public EntityHitbox(float x, float y, float sight, float reach)
        {
            _x = x;
            _y = y;
            Sight = new CircleF(x, y, sight);
            Reach = new CircleF(x, y, reach);
        }

        private float _x;
        private float _y;

        public float X { get => _x; set => MoveTo(value, _y); }

        public float Y { get => _y; set => MoveTo(_x, value); }

        /// <summary>
        /// Represents the maximum distance at which this entity can see.
        /// </summary>
        public CircleF Sight { get; }

        /// <summary>
        /// Represents the maximum distance at which this entity can reach.
        /// </summary>
        public CircleF Reach { get; }

        public void Offset(float x, float y)
        {
            X += x;
            Y += y;
            Sight.Offset(x, y);
            Reach.Offset(x, y);
        }

        public void MoveTo(float x, float y)
            => Offset(x - X, y - Y);


    }
}
