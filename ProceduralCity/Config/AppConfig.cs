﻿using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace ProceduralCity.Config
{
    class AppConfig : IAppConfig
    {
        public AppConfig()
        {
            var config = new ConfigurationBuilder()
               .AddJsonFile("appsettings.json")
               .Build();
            config.Bind(this);
        }

        public int ResolutionWidth { get; set; }

        public int ResolutionHeight { get; set; }

        public string WindowTitle { get; set; }

        public int FrameRate { get; set; }

        public int MinBuildingHeight { get; set; }

        public int MaxBuildingHeight { get; set; }

        public int AreaMinLength { get; set; }

        public int AreaMaxLength { get; set; }

        public IEnumerable<string> BuildingTextures { get; set; }

        public int WorldSize { get; set; }
    }
}