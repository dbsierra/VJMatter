Shader "Unlit/CalculatePositions_shd"
{

Properties
{
    _MainTex("Texture", 2D) = "white" {}
    _MaxAutomaticSpeed("Max Automatic Speed", Range(0,2)) = 0.2
    _TimeToMaxAutomaticSpeed("Time To Max Automatic Speed", float) = 2
}
 
SubShader
{
    Tags{ "RenderType" = "Opaque" }
    LOD 100

    Pass
    {
        ZTest Always
        ZWrite Off

        CGPROGRAM
            #pragma vertex defaultVert
            #pragma fragment frag
            #pragma target 5.0

            #include "UnityCG.cginc"
            #include "../../BlitIncludes.cginc"
            #include "../../Noise/noiseSimplex.cginc"

            // Info
            uint _ParticlesTextureDimension;
            uint _GridW;
            uint _GridH;

            // Parameters
            float _ReconstructTime;
            float _MaxAutomaticSpeed;
            float _TimeToMaxAutomaticSpeed;
            float _ShapePercentage;

            // Textures
            sampler2D _NoveltyHistory;
            sampler2D _Velocity;
            sampler2D _Random;

            int _Shape;

            float3 Lissajou(float i, float3 frequencies, float radius)
            {
                float3 nextPos = float3(0, 0, 0);

                nextPos.x = sin(i*6.28 * frequencies.x) * radius;
                nextPos.y = cos(i*6.28 * frequencies.y + _Time.y*.5) * radius;
                nextPos.z = sin(i*6.28 * frequencies.z + _Time.y) * radius;

                // Add noise to Lissajou pattern
                float3 Normal = nextPos;
                float3 seed = nextPos;
                seed.z += _Time.y;
                //  nextPos += Normal * snoise(5 * seed) * .2;

                return nextPos;
            }

            float3 Spiral(float i, float offset)
            {
                float3 nextPos = float3(0, 0, 0);
                nextPos.x = sin(i*6.28*2.) * i ;
                nextPos.y = cos(i*6.28*2. ) * i ;
                nextPos.z = i * (sin(i * 100. - offset + _Time.y)*.35 + sin(i * 100. + offset + _Time.y)*.35);
                return nextPos;
            }

            float3 PerlinForceField(float4 ppos) {
                /* A time-warying Simplex force field */
                return
                    normalize(float3(
                        snoise(float3(ppos.x, ppos.y, ppos.z + _Time.y)),
                        snoise(float3(ppos.x + 400. + _Time.y, ppos.y, ppos.z)),
                        snoise(float3(ppos.x + 500., ppos.y + _Time.y, ppos.z))
                        ));
            }
           
            float3 FormShape(float3 ppos, float3 shape, float percentage)
            {
                float3 newPos = percentage*(shape - ppos);
                return newPos;
                //return lerp(ppos, shape, percentage);
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float4 v = tex2D(_Velocity, i.uv);
                float4 p = tex2D(_MainTex, i.uv);
                float4 r = tex2D(_Random, i.uv);

                p.xyz += v.xyz * unity_DeltaTime.x;

                // Box
                // Sphere
                if (_Shape == 1)
                {
                    //p.xyz = Spiral(p.w, 1);
                   // p.xyz = normalize(r.xyz * 2 - 1)*(cos(p.w*30.)*.5 + .5);
                    p.xyz = .5*(r.xyz - .5);
                }
                // Triangle
                else if (_Shape == 2)
                {
                    //p.xyz = Spiral(p.w, 1);
                    float min = lerp(0, .5, i.uv.y);
                    float max = lerp(1, .5, i.uv.y);
                    p.xyz = float3(clamp(i.uv.x, min, max) - .5, i.uv.y - .5, 0);
                }          
                // grid
                else if (_Shape == 3)
                    p.xyz = 2*float3(i.uv.x - .5, i.uv.y - .5, 0);
                // Liss
                else if (_Shape == 4)
                {
                    p.xyz = Spiral(p.w, 1);
                    //p.xyz = Lissajou(p.w, float3(2, .41, 4.4), .4);
                }

                p.xyz += FormShape(p.xyz, 2*float3(i.uv.x - .5, i.uv.y - .5, 0), _ShapePercentage );
                    
               // float planarLength = length(p.xy);
                //float PulseForce = tex2D(_NoveltyHistory, float2(.5, saturate(planarLength) )).b;

            //    p.xyz = 2 * float3(i.uv.x - .5, i.uv.y - .5, 0);

                 return p;
            }
        ENDCG
    }
}

}
