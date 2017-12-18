using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Syn.Particles
{
    public class ParticlesUtil
    {
        //Initialize a new render texture with given size
        public static RenderTexture CreateNewParticleTexture(int size)
        {
            RenderTexture newTexture = new RenderTexture(size, size, 0, RenderTextureFormat.ARGBFloat);
            newTexture.filterMode = FilterMode.Point;
            newTexture.wrapMode = TextureWrapMode.Clamp;

            return newTexture;
        }

    }
}

