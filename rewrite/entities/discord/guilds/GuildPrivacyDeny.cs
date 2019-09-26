using System;

namespace Orikivo
{
    [Flags]
    public enum GuildPrivacyDeny
    {
        None = 0, // deny none of the privacy options.
        Emoji = 1, // deny emojis from being included into the public emoji dictionary
        Channels = 2, // deny channels from being included into the public guild search.
        Roles = 4, // deny roles from being included into the public search.
        Users = 8, // deny users from being included into the public search.
        Internal = Channels | Roles | Users, // deny all internal information from being included into the public search.
        All = 128 // deny all forms of information from being visible on orikivo.
    }

    // find out what people would want to hide.
    [Flags]
    public enum UserPrivacyDeny
    {
        None = 0
    }
}