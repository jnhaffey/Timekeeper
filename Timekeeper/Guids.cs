// Guids.cs
// MUST match guids.h
using System;

namespace Timekeeper
{
    static class GuidList
    {
        public const string guidTimekeeperPkgString = "272bbd8a-10ff-4395-8cb7-e21ff1ddc1a2";
        public const string guidTimekeeperCmdSetString = "a66dc4f7-1400-46b6-a7cd-1fdcc17af132";

        public static readonly Guid guidTimekeeperCmdSet = new Guid(guidTimekeeperCmdSetString);
    };
}