// Guids.cs
// MUST match guids.h
using System;

namespace Company.VSPackage1
{
    static class GuidList
    {
        public const string guidVSPackage1PkgString = "4b4d7db3-d8a6-440f-be0c-3ac21f9953aa";
        public const string guidVSPackage1CmdSetString = "dd850c6c-21bc-48e0-83a8-ffe9c13fe28e";

        public static readonly Guid guidVSPackage1CmdSet = new Guid(guidVSPackage1CmdSetString);
    };
}