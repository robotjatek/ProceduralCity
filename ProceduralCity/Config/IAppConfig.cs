using System.Collections.Generic;

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

        float AreaBorderSize { get; set; }

        int WorldSize { get; set; }

        int MinVerticalBlockLength { get; set; }

        int MinHorizontalBlockLength { get; set; }

        IEnumerable<string> BuildingTextures { get; set; }

        int BillboardTextureNumber { get; set; }

        int BillboardTextureWidth { get; set; }

        int BillboardTextureHeight { get; set; }

        int BillboardLineHeight { get; set; }
    }
}
