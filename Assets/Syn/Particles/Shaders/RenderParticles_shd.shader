Shader "Unlit/RenderParticles_shd"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Gradient("Texture", 2D) = "white" {}
        _Gradient2("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags{ "Queue" = "Transparent" "RenderType" = "Opaque" }
        ZWrite Off

        // Blend SrcAlpha OneMinusSrcAlpha
        Blend One One
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex VS_Main
            #pragma fragment FS_Main
            #pragma geometry GS_Main

            #include "UnityCG.cginc"
            #include "../../Noise/noiseSimplex.cginc"

            struct VS_INPUT
            {
                float4 pos : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct GS_INPUT
            {
                float4 pos : POSITION;
            };

            struct FS_INPUT
            {
                float4 pos : SV_POSITION;
                float2 quadCoords : TEXCOORD0;
                float2 currImageCoords : TEXCOORD1;
                nointerpolation float clrKey : TEXCOORD2;
                nointerpolation float camDist : TEXCOORD3;
                float4 ppVars1 : TEXCOORD4;
                float4 ppVars2 : TEXCOORD5;
            };

            // Info
            float3 _LocalSpaceCameraPos;
            float _GridW;
            float _GridH;

            // Parameters
            float _Brightness;
            float _NoiseScale;
            float _NoiseSpeed;
            float _UseNoiseForLU;
            float _UseRMSForLU;
            float _SizeMax;
            float _SizeMin;
            float _SizePow;
            float _UVColor;
            float _GradientLerp;

            // Textures
            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _Velocity;
            sampler2D _Position;
            sampler2D _NoveltyHistory;
            sampler2D _Gradient;
            sampler2D _Gradient2;

            float3 PositionValue(float2 uvIn)
            {
                return tex2Dlod(_Position, float4(uvIn, 0, 0)).xyz;
            }
            float NormalizedIndex(float2 uvIn)
            {
                return tex2Dlod(_Position, float4(uvIn, 0, 0)).w;
            }
            float3 VelocityValue(float2 uvIn)
            {
                return tex2Dlod(_Velocity, float4(uvIn, 0, 0)).xyz;
            }

            GS_INPUT VS_Main (VS_INPUT v)
            {
                GS_INPUT o = (GS_INPUT)0;
                o.pos = v.pos;

                return o;
            }
    
            float map(float s, float a1, float a2, float b1, float b2)
            {
                return b1 + (s - a1)*(b2 - b1) / (a2 - a1);
            }

            // Geometry Shader -----------------------------------------------------
            [maxvertexcount(4)]
            void GS_Main(point GS_INPUT p[1], inout TriangleStream<FS_INPUT> triStream)
            {
                float inverseWidth = 0, inverseHeight = 0;
                float2 currentImageCoords = float2(0, 0);
                uint particleIndex = (uint)p[0].pos.z;

                uint uintGridW = (uint)_GridW;
                uint uintGridH = (uint)_GridH;
                uint gridX = particleIndex % uintGridW;
                uint gridY = particleIndex / uintGridW;
                // Now compute the normalized texture coordinates
                inverseWidth = 1.0f / _GridW;
                inverseHeight = 1.0f / _GridH;

                float3 centerPos = PositionValue(p[0].pos.xy);
                float3 worldPos = mul(unity_ObjectToWorld, centerPos);

                // For calculations to get particles to face the camera
                float3 look = _LocalSpaceCameraPos - centerPos;
                float3 right = normalize(cross(look, float3(0, 1, 0)));
                float3 up = normalize(cross(right, look));

               // float rms = tex2Dlod( _NoveltyHistory, float4(.5, saturate(length(centerPos)), 0, 0) ).r;

                currentImageCoords = float2(gridX * inverseWidth, (uintGridH - gridY - 1.0f) * inverseHeight);

                float r = cos(particleIndex*130.)*.5 + .5;

                float sizeOffset = map(pow(r, _SizePow), 0, 1, .003, _SizeMax);

                float4x4 vp = UNITY_MATRIX_MVP;
                FS_INPUT pIn;
                pIn.camDist = length(look);
                pIn.clrKey = length(VelocityValue(p[0].pos.xy));
                pIn.ppVars1 = float4(NormalizedIndex(p[0].pos.xy), length(centerPos), 0, 0);
                pIn.ppVars2 = float4(worldPos, NormalizedIndex(p[0].pos.xy));

                pIn.pos = mul(vp, float4(centerPos + sizeOffset * right - sizeOffset * up, 1.0));
                pIn.quadCoords = float2(1.0f, 0.0f);
                pIn.currImageCoords = float2(currentImageCoords.x + inverseWidth, currentImageCoords.y);
                triStream.Append(pIn);

                pIn.pos = mul(vp, float4(centerPos - sizeOffset * right - sizeOffset * up, 1.0));
                pIn.quadCoords = float2(0.0f, 0.0f);
                pIn.currImageCoords = float2(currentImageCoords.x, currentImageCoords.y);
                triStream.Append(pIn);

                pIn.pos = mul(vp, float4(centerPos + sizeOffset * right + sizeOffset * up, 1.0));
                pIn.quadCoords = float2(1.0f, 1.0f);
                pIn.currImageCoords = float2(currentImageCoords.x + inverseWidth, currentImageCoords.y + inverseHeight);
                triStream.Append(pIn);

                pIn.pos = mul(vp, float4(centerPos - sizeOffset * right + sizeOffset * up, 1.0));
                pIn.quadCoords = float2(0.0f, 1.0f);
                pIn.currImageCoords = float2(currentImageCoords.x, currentImageCoords.y + inverseHeight);
                triStream.Append(pIn);
            }

            fixed4 FS_Main (FS_INPUT i) : SV_Target
            {
                float4 sprite = tex2D(_MainTex, i.quadCoords);

                // Particle info
                float distance = i.ppVars1.y;
                float3 wp = i.ppVars2.xyz;
                float3 id = i.ppVars2.x;

                // Noise
                float3 noiseSeed = wp;
                noiseSeed.z += _Time.y;
                float n = snoise(noiseSeed);
                n = abs(frac(n)*2.0 - 1.0); //triangle wave ensures smooth transition when wrapping between the start and end colors of gradient

                // Lookup info
                float rmsLookup = tex2D(_NoveltyHistory, float2(.5, saturate(distance*.75) ) ).r;
                float velLookup = saturate(i.clrKey*1.5);
                float nonRMSLookup = lerp(velLookup, n, _UseNoiseForLU);
                float lookupOffset = lerp(nonRMSLookup, rmsLookup*.4, _UseRMSForLU);

                float triWave = abs(frac(distance + lookupOffset - _Time.y*.2)*2.0 - 1.0);

                float2 textureLookup = float2(.5, triWave);

                float3 gradient1 = tex2D(_Gradient, textureLookup).rgb;  
                float3 gradient2 = tex2D(_Gradient2, textureLookup).rgb;
                float3 gradient = lerp(gradient1, gradient2, _GradientLerp);
                    
                sprite.rgb = gradient * lerp(float3(1,1,1), float3(i.quadCoords, 1.0f), _UVColor) * _Brightness;

                sprite.rgb = lerp(sprite.rgb, float3(0, 0, 0), distance*.5f);

              //  sprite.rgb = float3(rmsLookup, rmsLookup, rmsLookup);
              //  sprite.rgb = tex2D(_Velocity, i.currImageCoords).rgb;

                sprite = sprite * sprite.a;

                //sprite.rgb *= (.8 - i.clrKey + .2) * .28 + .1;
                
               // sprite.rgb *= saturate(i.clrKey);

                return sprite;
            }
            ENDCG
        }
    }
}
