using System;

namespace Arcadia.Graphics
{
    /// <summary>
    /// Defines what a component inherits from the previous component that was handled.
    /// </summary>
    [Flags]
    public enum SizeInherit
    {
        /// <summary>
        /// Nothing is inherited.
        /// </summary>
        None = 0,

        /// <summary>
        /// Inherits the previous width.
        /// </summary>
        Width = 1,

        /// <summary>
        /// Inherits the previous padding width.
        /// </summary>
        PaddingWidth = 2,

        /// <summary>
        /// Inherits the previous height.
        /// </summary>
        Height = 4,

        /// <summary>
        /// Inherits the previous padding height.
        /// </summary>
        PaddingHeight = 8,

        /// <summary>
        /// Inherits the previous combined width.
        /// </summary>
        X = Width | PaddingWidth,

        /// <summary>
        /// Inherits the previous combined height.
        /// </summary>
        Y = Height | PaddingHeight,

        /// <summary>
        /// Inherits everything from the previous component.
        /// </summary>
        XY = Width | PaddingWidth | Height | PaddingHeight
    }
}