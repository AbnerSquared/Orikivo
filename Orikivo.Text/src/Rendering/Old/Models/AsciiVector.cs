using System;
using System.Threading.Tasks;

namespace Orikivo.Text
{
    /// <summary>
    /// The vector used for an <see cref="AsciiObject"/> that contains its initial velocity and acceleration values.
    /// </summary>
    public class AsciiVector
    {
        public static AsciiVector Zero = new AsciiVector(0f, 0f, 0f, 0f);

        /// <summary>
        /// Creates a new <see cref="AsciiVector"/> with the specified velocity and acceleration values in both the X and Y direction.
        /// </summary>
        public AsciiVector(float vX, float vY, float aX, float aY)
        {
            VX = vX;
            VY = vY;
            AX = aX;
            AY = aY;
        }

        /// <summary>
        /// Creates a new <see cref="AsciiVector"/> with the specified velocity and angle.
        /// </summary>
        public static AsciiVector Velocity(float velocity, float angle)
            => new AsciiVector(Utils.GetAngledVectorX(velocity, angle), Utils.GetAngledVectorY(velocity, angle), 0f, 0f);

        /// <summary>
        /// Creates a new <see cref="AsciiVector"/> with the specified acceleration and angle.
        /// </summary>
        public static AsciiVector Acceleration(float acceleration, float angle)
            => new AsciiVector(0f, 0f, Utils.GetAngledVectorX(acceleration, angle), Utils.GetAngledVectorY(acceleration, angle));

        /// <summary>
        /// Creates a new <see cref="AsciiVector"/> with the specified velocity, acceleration, and their corresponding angles.
        /// </summary>
        public static AsciiVector FromAngle(float velocity, float acceleration, float velocityAngle, float accelerationAngle)
            => new AsciiVector(Utils.GetAngledVectorX(velocity, velocityAngle), Utils.GetAngledVectorY(velocity, velocityAngle),
                Utils.GetAngledVectorX(acceleration, accelerationAngle), Utils.GetAngledVectorY(acceleration, accelerationAngle));
        
        /// <summary>
        /// The initial velocity for the X vector.
        /// </summary>
        public float VX { get; }

        public VectorDouble X { get; }
        
        /// <summary>
        /// The initial acceleration for the X vector.
        /// </summary>
        public float AX { get; }

        /// <summary>
        /// The initial velocity for the Y vector.
        /// </summary>
        public float VY { get; }

        public VectorDouble Y { get; }
        
        /// <summary>
        /// The initial acceleration for the Y vector.
        /// </summary>
        public float AY { get; }


        /// <summary>
        /// Sets a constant value for the vector of X.
        /// </summary>
        /// <param name="x"></param>
        public void SetVectorX(float x) // set a constant value
            => SetVectorX(vX => x);

        /// <summary>
        /// Set a <see cref="Func{float, float}"/> that returns the value for X with its input value being time.
        /// </summary>
        public void SetVectorX(Func<float, float> functionX)
        {

        }

        /// <summary>
        /// Sets a constant value for the vector of Y.
        /// </summary>
        /// <param name="y"></param>
        public void SetVectorY(float y)
            => SetVectorY(vY => y);

        public float GetVectorX(float time)
        {
            return 0f; // gets the vector value for x with respect to time.
        }

        public float GetVectorY(float time)
        {
            return 0f; // gets the vector value for x with respect to time.
        }

        /// <summary>
        /// Set a <see cref="Func{float, float}"/> that returns the value for Y with its input value being time.
        /// </summary>
        public void SetVectorY(Func<float, float> functionY)
        {

        }
    }

    public class VectorDouble
    {
        /// <summary>
        /// The function that is used to determine the value of the <see cref="VectorDouble"/>.
        /// </summary>
        public Func<float, float> Function { get; set; }
    }
}
