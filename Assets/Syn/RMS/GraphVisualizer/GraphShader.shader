// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "ImageEffects/GraphViz"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
            
            sampler2D _MainTex;
            sampler2D _Data;
            sampler2D _VelocityTex;
            uniform int _ValuesLength;

            float _R;
            float _G;
            float _B;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);

                // Upper left part of viewport, visualize the RMS values as graph
                float2 size = float2(.2, .2);
                if (i.uv.x < size.x)
                {
                    if (i.uv.y < size.y)
                    {
                        float v = 1 - i.uv.y / size.y;
                        float x = (int)((i.uv.x / size.x) * 200);
                        x = min(x, _ValuesLength);

                        float uvx = ( (size.x-i.uv.x) / size.x);

                        float height = tex2D(_Data, float2(.5, uvx) ).r + .5;
                        float height2 = tex2D(_Data, float2(.5, uvx)).g + .5;
                        float height3 = tex2D(_Data, float2(.5, uvx)).b;

                        height3 = clamp( height3*1.8, -.5, .5);

                        height3 += .5;

                        float c = 0;
                        float c2 = 0;
                        float c3 = 0;
                        
                        if (v < height)
                        {
                            c = 1;
                        }
                        if (v < height2)
                        {
                            c2 = 1;
                        }
                        if (v < height3)
                        {
                            c3 = 1;
                        }

                        col = float4( c, c2, c3, 1 );
                        col.r *= _R;
                        col.g *= _G;
                        col.b *= _B;

                        // Baseline
                        float mid = size.y / 2.;
                        
                        if (i.uv.y > (mid-.001) && i.uv.y < (mid+.001))
                        {
                            col = float4(1, 0, .3, 1);
                        }

                    }

                }

                // In upper right part of viewport, visualize the RMS data in its texture form
                size = float2(.2, .4);
                if (i.uv.x > 1 - size.x)
                {
                    if (i.uv.y < size.y)
                    {
                        col = tex2D(_Data, i.uv / size);
                        col.r *= _R;
                        col.g *= _G;
                        col.b *= _B;
                    }
                }
                
                return col;
            }
            ENDCG
        }
    }
}
