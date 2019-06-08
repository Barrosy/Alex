﻿using System;
using Newtonsoft.Json;

namespace Alex
{
    public sealed class Settings
    {
		[JsonIgnore]
	    internal bool IsDirty { get; set; }

        public string Username { get; set; }
        public int RenderDistance { get; set; }
        public double MouseSensitivy { get; set; }
		public string[] ResourcePacks { get; set; }

	    public string Anvil { get; set; }
		public bool UseBuiltinGenerator { get; set; }

		public bool ClientSideLighting { get; set; }
		public bool UseAlexChunks { get; set; }

        public PerformanceSettings Performance { get; set; }
        public Settings(string username)
        {
            Username = username;
	        ResourcePacks = new string[]
	        {
				
	        };
	        RenderDistance = 6;
            MouseSensitivy = 1.0;

	        Anvil = string.Empty;
	        UseBuiltinGenerator = false;
	        IsDirty = false;

	        ClientSideLighting = true;
	        UseAlexChunks = false;

            Performance = new PerformanceSettings();
        }

        public sealed class PerformanceSettings
        {
            public bool AgressiveMemoryManagement { get; set; }
            public int ChunkThreads { get; set; }

            public PerformanceSettings()
            {
                AgressiveMemoryManagement = false;
                ChunkThreads = Environment.ProcessorCount / 2;
            }
        }
    }
}
