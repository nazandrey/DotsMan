﻿using System;
using Unity.Entities;
using Unity.Mathematics;

namespace DotsMan
{
    [GenerateAuthoringComponent]
    public struct PathfindingComponent : IComponentData
    {
        public float3 lastDirection;
    }
}


