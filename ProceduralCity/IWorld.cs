﻿using System;
using System.Collections.Generic;
using ProceduralCity.GameObjects;
using ProceduralCity.Renderer;

namespace ProceduralCity
{
    interface IWorld : IDisposable
    {
        IEnumerable<IRenderable> Renderables { get; }
        IEnumerable<TrafficLight> Traffic { get; }
    }
}