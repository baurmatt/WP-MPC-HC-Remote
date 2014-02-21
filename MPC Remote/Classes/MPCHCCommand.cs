using System;

namespace MPC_Remote
{
    public sealed class MPCHCCommand
    {
        public static readonly string Play = "887";
        public static readonly string Pause = "888";
        public static readonly string Mute = "909";
        public static readonly string VolumeUp = "907";
        public static readonly string VolumeDown = "908";
        public static readonly string SkipForward = "920";
        public static readonly string SkipBackward = "921";
        public static readonly string Faster = "895";
        public static readonly string Slower = "894";
        public static readonly string FullScreen = "830";
        public static readonly string NewPosition = "-1&position=";
    }
}
