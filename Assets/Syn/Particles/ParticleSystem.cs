using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Syn.Particles
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class ParticleSystem : MonoBehaviour
    {
        public int particleCount = 1000;
        int textureDimension;

        MeshFilter mF;
        MeshRenderer mR;

        RenderTexture initPositions;
        RenderTexture posTexPrev;
        RenderTexture posTexCurr;
        public RenderTexture velTexCurr;
        RenderTexture velTexPrev;
        Texture2D randomTexture;
        Texture2D meshStartPosTexture;

        public Material InitPosMat { get { return initPosMat; } }
        Material initPosMat;
        public Material PositionMat { get { return positionMat; } }
        Material positionMat;
        public Material VelocityMat { get { return velocityMat; } }
        Material velocityMat;

        [SerializeField]
        Shader initialPositionsShader;
        [SerializeField]
        Shader positionShader;
        [SerializeField]
        Shader velocityShader;
        [SerializeField]
        Shader renderShader;

        public RMS rmsObj;

        bool ready;
        public bool DebugOn;
        public Material DebugMaterial;
        public Mesh CustomMesh;

        void Awake()
        {
            Initialize(particleCount);
            ready = true;
        }

        void Update()
        {
            if (ready)
            {
                if (DebugOn)
                {
                    mR.material.SetTexture("_MainTex", rmsObj.GetNoveltyHistoryTexture());
                }
                else
                {
                    mR.material.SetVector("_LocalSpaceCameraPos", transform.InverseTransformPoint(Camera.main.transform.position));
                    UpdateParticles();

                    // Set RMS history texture
                    velocityMat.SetTexture("_NoveltyHistory", rmsObj.GetNoveltyHistoryTexture());
                    positionMat.SetTexture("_NoveltyHistory", rmsObj.GetNoveltyHistoryTexture());
                    mR.material.SetTexture("_NoveltyHistory", rmsObj.GetNoveltyHistoryTexture());
                }
            }
        }

        private void UpdateParticles()
        {
            // Calculate velocity in the shader, write out results to velTexCurr
            Graphics.Blit(velTexPrev, velTexCurr, velocityMat);
            // Copy results to velTexPrev to use for the next iteration
            Graphics.Blit(velTexCurr, velTexPrev);

            // Calculate position in the shader, write out results to posTexCurr
            Graphics.Blit(posTexPrev, posTexCurr, positionMat);
            // Copy results to posTexPrev to use for the next iteration
            Graphics.Blit(posTexCurr, posTexPrev);
        }

        public void Initialize(int pcount)
        {
            mF = GetComponent<MeshFilter>();
            mR = GetComponent<MeshRenderer>();

            if (!DebugOn)
            {
                LoadMaterials();

                // Start with our particle count. We want a texture that has a pixel for each particle. Because textures are
                // square for us, we must have a particle count that has an integer square root.
                SetTextureDimensions(pcount);
                CreateParticleTextures(textureDimension);
                if (CustomMesh != null)
                    CreateParticlesFromMesh(CustomMesh);
                else
                    CreateParticleGridMesh(textureDimension);

                SetDefaultRenderMatProperties();
                SetDefaultPositionMatProperties();
                SetDefaultVelocityMatProperties();

                // Calculate initial positions
                initPosMat.SetInt("_TextureSize", textureDimension);
                initPosMat.SetTexture("_Random", this.randomTexture);
                initPosMat.SetInt("_Mode", 3);

                Graphics.Blit(posTexPrev, posTexCurr, initPosMat);
                Graphics.Blit(posTexCurr, posTexPrev);
                Graphics.Blit(posTexCurr, initPositions);

                // Our render material will take position and velocity data and render our particles from them
                mR.material.SetTexture("_Position", posTexCurr);
                mR.material.SetTexture("_Velocity", velTexCurr);

                // Manually point to velocity texture data for positions material. We will pass the positions texture through graphics.blit.
                positionMat.SetTexture("_Velocity", velTexCurr);
                positionMat.SetTexture("_Random", this.randomTexture);

                // Manually point to position texture data for velocity material. We will pass the velocity texture through graphics.blit.
                velocityMat.SetTexture("_Position", posTexCurr);
                velocityMat.SetTexture("_Random", this.randomTexture);
                velocityMat.SetTexture("_InitialPos", initPositions);

            }
            else
            {
                mR.material = DebugMaterial;
            }
        }

        private void SetDefaultRenderMatProperties()
        {
            mR.material.SetFloat("_GridW", textureDimension);
            mR.material.SetFloat("_GridH", textureDimension);
            mR.material.SetFloat("_Brightness", .5f);
            mR.material.SetFloat("_NoiseScale", 1.0f);
            mR.material.SetFloat("_NoiseSpeed", .5f);
            mR.material.SetFloat("_UseNoiseForLU", .2f);
            mR.material.SetFloat("_UseRMSForLU", .7f);
            mR.material.SetFloat("_SizeMax", .001f);
            mR.material.SetFloat("_SizeMin", .003f);
            mR.material.SetFloat("_SizePow", 1.0f);
            mR.material.SetFloat("_UVColor", 0.0f);
        }

        private void SetDefaultPositionMatProperties()
        {
            positionMat.SetFloat("_GridW", textureDimension);
            positionMat.SetFloat("_GridH", textureDimension);
            positionMat.SetFloat("_ParticlesTextureDimension", textureDimension);
            //    mR.material.SetFloat(" _NoiseScale", 1.0f);
            //    mR.material.SetFloat("_NoiseSpeed", .5f);
            //    mR.material.SetFloat("_UseNoiseForLU", .2f);
            //    mR.material.SetFloat("_UseRMSForLU", .7f);
            //    mR.material.SetFloat("_SizeMax", .1f);
            //    mR.material.SetFloat("_SizeMin", .3f);
            //    mR.material.SetFloat("_SizePow", 1.0f);
            //    mR.material.SetFloat("_UVColor", 0.0f);
            positionMat.SetInt("_Shape", 0);
            positionMat.SetFloat("_ShapePercentage", 0.0f);
        }

        private void SetDefaultVelocityMatProperties()
        {
            velocityMat.SetFloat("_DragCoeff", 10.0f);
            velocityMat.SetFloat("_PulserMult", 14.0f);
            velocityMat.SetFloat("_AntiPulserMult", 1.0f);
            velocityMat.SetFloat("_AccelMultiplier", 1.0f);
            velocityMat.SetFloat("_VelocityLimit", 1.5f);
            velocityMat.SetFloat("_VelocityMult", 1.0f);
            velocityMat.SetFloat("_TargetForceMult", .2f);
            velocityMat.SetFloat("_LissForceMult", .5f);
            velocityMat.SetFloat("_LissRadius", .54f);
            velocityMat.SetFloat("_LissNoiseAmp", 1.0f);
            velocityMat.SetFloat("_LissNoiseFreq", 0.0f);
            velocityMat.SetVector("_LissFrequencies", new Vector4(4.27f, 5.0f, 2.0f, 0.0f));
            velocityMat.SetVector("_PulserTarget", new Vector4(0.0f, 0.0f, 0.0f));
            velocityMat.SetVector("_Target", new Vector4(0.0f, 0.0f, 0.0f));
            velocityMat.SetFloat("_ShapePercentage", 0.0f);
            velocityMat.SetInt("_Shape", 0);
        }


        // Tweakable parameters 
        public void SetPulserTargetX(float v)
        {
            Vector3 curTarget = velocityMat.GetVector("_PulserTarget");
            curTarget.x = v;
            velocityMat.SetVector("_PulserTarget", curTarget);
        }
        public void SetPulserMag(float v)
        {
            velocityMat.SetFloat("_PulserMult", v);
        }
        public void SetAntiPulserMag(float v)
        {
            velocityMat.SetFloat("_AntiPulserMult", v);
        }
        public void SetTargetForcerMult(float v)
        {
            velocityMat.SetFloat("_TargetForceMult", v);
        }
        public void SetLissForceMult(float v)
        {
            velocityMat.SetFloat("_LissForceMult", v);
        }
        public void SetLissRadius(float v)
        {
            velocityMat.SetFloat("_LissRadius", v);
        }
        public void SetDragCoeff(float v)
        {
            velocityMat.SetFloat("_DragCoeff", v);
        }
        public void SetVelocityMult(float v)
        {
            velocityMat.SetFloat("_VelocityMult", v);
        }
        public void SetSizeMin(float v)
        {
            mR.material.SetFloat("_SizeMin", v);
        }
        public void SetSizeMax(float v)
        {
            mR.material.SetFloat("_SizeMax", v);
        }
        public void SetSizePow(float v)
        {
            mR.material.SetFloat("_SizePow", v);
        }
        public void SetBrightness(float v)
        {
            mR.material.SetFloat("_Brightness", v);
        }
        public void SetShape(int v)
        {
            positionMat.SetInt("_Shape", v);
        }
        public void SetTexture1(Texture2D tex)
        {
            mR.material.SetTexture("_Gradient", tex);
        }
        public void SetTexture2(Texture2D tex)
        {
            mR.material.SetTexture("_Gradient2", tex);
        }
        public void SetGradientLerp(float v)
        {
            mR.material.SetFloat("_GradientLerp", v);
        }
        public void SetShapePercentage(float v)
        {
            positionMat.SetFloat("_ShapePercentage", Mathf.Clamp(2.0f * (v - .5f), 0.0f, 1.0f));
            velocityMat.SetFloat("_ShapePercentage", Mathf.Clamp( 2.0f * v, 0.0f, 1.0f));
           // Debug.Log(Mathf.Clamp(2.0f * v, 0.0f, 1.0f));  
        }
        private void SetTextureDimensions(int pcount)
        {
            if (CustomMesh != null)
            {
                particleCount = CustomMesh.vertices.Length;
                textureDimension = (int)Mathf.Sqrt(particleCount);
                particleCount = textureDimension * textureDimension;
            }
            else
            {
                textureDimension = (int)Mathf.Sqrt(pcount);
                particleCount = textureDimension * textureDimension;
            }
        }
        private void CreateParticlesFromMesh(Mesh mesh)
        {
            float inverseTextureDimension = 1.0f / textureDimension;
            Mesh newMesh = new Mesh();
            Vector3[] positions = mesh.vertices;
            Vector2[] uvs = mesh.uv;
            Debug.Log(newMesh.GetIndices(0).Length);
            this.randomTexture = new Texture2D(textureDimension, textureDimension, TextureFormat.RGBAHalf, false);
            this.meshStartPosTexture = new Texture2D(textureDimension, textureDimension, TextureFormat.RGBAFloat, false);
            List<Vector3> newPositions = new List<Vector3>();
            List<Vector2> newUvs = new List<Vector2>();
            List<int> indices = new List<int>();
            int particleIndex = 0;
            // Create random texture for points
            for (int j = 0; j < textureDimension; ++j)
            {
                for (int i = 0; i < textureDimension; ++i, ++particleIndex)
                {
                    Vector3 p = positions[j * textureDimension + i];
                    Vector2 uv = uvs[j * textureDimension + i];
                    Debug.Log(uv.x);
                    newPositions.Add(new Vector3((i + 0.5f) * inverseTextureDimension, (j + 0.5f) * inverseTextureDimension, particleIndex));
                    indices.Add(indices.Count);
                    newUvs.Add(uv);
                    this.randomTexture.SetPixel(i, j, new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value, 0.0f));
                    this.meshStartPosTexture.SetPixel(i, j, new Color(p.x, p.y, p.z, 0.0f));
                }
            }
            this.randomTexture.Apply();
            this.meshStartPosTexture.Apply();
            newMesh.name = "CustomMesh";
            newMesh.SetVertices(newPositions);
            newMesh.SetIndices(indices.ToArray(), MeshTopology.Points, 0);
            newMesh.SetUVs(0, newUvs);
            newMesh.bounds = new Bounds(Vector3.zero, new Vector3(5, 5, 5));
            newMesh.UploadMeshData(true);
            Debug.Log(textureDimension);
            mF.mesh = newMesh;
        }

        private void LoadMaterials()
        {
            if (velocityMat == null)
                velocityMat = new Material(velocityShader);
            else
                velocityMat.shader = velocityShader;

            if (positionMat == null)
                positionMat = new Material(positionShader);
            else
                positionMat.shader = positionShader;

            if (initPosMat == null)
                initPosMat = new Material(initialPositionsShader);
            else
                initPosMat.shader = initialPositionsShader;

            if (mR.material == null)
            {
                mR.material = new Material(renderShader);
                Debug.Log(mR.material);

            }
            //   else
            //     mR.material.shader = renderShader;
        }

        private void CreateParticleTextures(int textureDimension)
        {
            // Create render textures for our position and velocity textures
            posTexPrev = CreateNewParticleTexture(textureDimension);
            posTexCurr = CreateNewParticleTexture(textureDimension);
            velTexCurr = CreateNewParticleTexture(textureDimension);
            velTexPrev = CreateNewParticleTexture(textureDimension);
            initPositions = CreateNewParticleTexture(textureDimension);
        }

        public static RenderTexture CreateNewParticleTexture(int size)
        {
            RenderTexture newTexture = new RenderTexture(size, size, 0, RenderTextureFormat.ARGBFloat);
            newTexture.filterMode = FilterMode.Point;
            newTexture.wrapMode = TextureWrapMode.Clamp;

            return newTexture;
        }

        // Create a grid of points to match our particle texture
        private void CreateParticleGridMesh(int textureDimension)
        {
            float inverseTextureDimension = 1.0f / textureDimension;

            List<Vector3> positions = new List<Vector3>();
            List<int> indices = new List<int>();
            int particleIndex = 0;

            this.randomTexture = new Texture2D(textureDimension, textureDimension, TextureFormat.RGBAHalf, false);

            for (int j = 0; j < textureDimension; ++j)
            {
                for (int i = 0; i < textureDimension; ++i, ++particleIndex)
                {
                    // The vertex's XY position in the mesh is used as a texture coordinate to address particle properties such as position and velocity.
                    // The Z component is the particle index.
                    positions.Add(new Vector3((i + 0.5f) * inverseTextureDimension, (j + 0.5f) * inverseTextureDimension, particleIndex));

                    indices.Add(indices.Count);

                    this.randomTexture.SetPixel(i, j, new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value, 0.0f));
                }
            }
            this.randomTexture.Apply();

            Mesh gridMesh = new Mesh();
            gridMesh.name = "Particles";
            gridMesh.SetVertices(positions);
            gridMesh.SetIndices(indices.ToArray(), MeshTopology.Points, 0);
            gridMesh.bounds = new Bounds(Vector3.zero, new Vector3(5, 5, 5));
            gridMesh.UploadMeshData(true);

            mF.mesh = gridMesh;
        }

    }

}