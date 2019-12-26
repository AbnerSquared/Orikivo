﻿using System;

namespace Orikivo
{
    public class OriBaseUser
    {
        public string Username { get; }
        public ulong Id { get; }
        public string Discriminator { get; }
        public DateTime CreatedAt { get; }
        public VarData[] Vars { get; }
        // Config
        //public UserConfig Config { get; }
    }
}
