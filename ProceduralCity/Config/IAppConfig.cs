﻿using System.Collections.Generic;

namespace ProceduralCity.Config
{
    public interface IAppConfig
    {
        int ResolutionWidth { get; set; }

        int ResolutionHeight { get; set; }

        string WindowTitle { get; set; }

        int FrameRate { get; set; }

        int MinBuildingHeight { get; set; }

        int MaxBuildingHeight { get; set; }

        int AreaMinLength { get; set; }

        int AreaMaxLength { get; set; }

        int WorldSize { get; set; }

        IEnumerable<string> BuildingTextures { get; set; }
    }
}
