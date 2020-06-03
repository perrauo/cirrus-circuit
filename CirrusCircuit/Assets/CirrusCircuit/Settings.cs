using UnityEngine;
using System.Collections;
using System;
using Cirrus;

namespace Cirrus.Circuit
{
    public class Settings
    {
        public static PlayerPref IpAddress = new PlayerPref("IPAddress", "25.100.98.14:2024");

        public static PlayerPref NumberOfRounds = new PlayerPref("NumberOfRounds", 3);

        public static PlayerPref RoundTime = new PlayerPref("RoundTime", 30);

        public static PlayerPref IsUsingAlternateControlScheme = new PlayerPref("UseAlternativeControlScheme", false);
        // TODO is player 2,3,4 bound to direction

        public static PlayerPref AreGemsSpawned = new PlayerPref("AreGemsSpawned", true);

    }
}