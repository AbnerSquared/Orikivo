using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Orikivo.rewrite.src.Orikivo.Drawing.Poxel
{
    public class PoxelCardFormatOptions
    {
        public PoxelCardAvatar Avatar { get; } // defines the style, format, and position of an avatar.
        public PoxelCardUsername Username { get; } // defines the SFP of a username.
        public PoxelCardLevel Level { get; } // defines the SFP of a user level.
        //public PoxelCardBalance Balance { get; } // defines the SFP of a user balance.
        //public PoxelCardMerit Merit { get; } // defines the SFP of a user merit.
       //public PoxelCardBackdrop Backdrop { get; } // defines the SF of a backdrop.
    }

    public class PoxelCardLayout
    {
        public Point Avatar { get; }
        public Point Username { get; }
        public Point Level { get; }
        public Point Balance { get; }
        public Point Merit { get; }
        public Point Backdrop { get; }
    }

    public class PoxelCardAvatar
    {
        public PoxelCardAvatarFormat Format { get; } // if the avatar is default or minimized.
        public Point Position { get; } // the top-left x,y coordinate of the avatar's placement
        public PoxelCardAvatarType Type { get; } // a possible style type that may be equipped to the avatar, otherwise hide.
    }

    public enum PoxelCardAvatarType
    {
        Default = 0,
        Animated = 1, // shown as a gif, if applicable.
    }

    public class PoxelCardUsername
    {
        public PoxelFontFace Font { get; } // the font that is used when drawing the username. Otherwise, it defaults to Orikos Alpha.
        public Point Position { get; } // the top-left x,y coord of username.
        public PoxelCardUsernameType Type { get; } // a possible style type that may be equipped to the username,
    }

    public enum PoxelCardUsernameType
    {
        Default = 0,
        Gradient = 1, // a gradient applied to the username.
        Glow = 2, // a glow applied to the username.
        Outline = 3, // outline applied to the username.
    }

    public class PoxelCardLevel
    {
        public PoxelCardLevelTextType Type { get; } // a possible style type used.
    }

    public enum PoxelCardLevelTextType
    {
        Empty, // hidden
        Solid, // default
        Dim, // dim solid
        Exp, // exp solid
        DimExp, // dim exp solid
        EmptyExp, // exp empty backdrop
        EmptyDimExp, // dim exp empty backdrop
        Outline, // outline with empty
        DimOutline, // dim outline with empty
        DimOutlineSolid, // dim outline with bright
        DimOutlineExp, // dim outline with bright exp text/
        ExpOutline, // exp outline with empty
        ExpOutlineDim, // exp outline with dim text
        ExpOutlineDimExp, // exp outline with hallow dim text

    }

    public enum PoxelCardLevelBoxType
    {
        Empty = 0, // Hidden
    }

    public enum PoxelCardLevelBarType
    {
        Empty = 0, // hidden
    }
}
