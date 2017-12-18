Shader "Unlit/RMS_shd"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

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

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _TexSize;
            float _IncomingRMS;
            int _AverageSize;
            int _CurrentCount;

            v2f vert (appdata v)  
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                // Sample the incoming texture
                fixed4 col = tex2D(_MainTex, i.uv);
                
                // Texture increment 
                float inc = 1 / _TexSize;

                // Previous texture row
                float newUVY = i.uv.y - inc;

                // Current texture row gets previous values from the previous row, unless it's the first row in the texture,
                // then it gets the incoming RMS value

                // Not the first row
                if (newUVY >= 0)
                {
                    col = tex2D(_MainTex, float2(i.uv.x, newUVY));
                }
                // First row
                else
                {
                    col = float4(_IncomingRMS,0,0,1);

                    // Green channel gets the running cumulative average
                    float n = min(_CurrentCount, _AverageSize);
                    float prevCumAvg = tex2D(_MainTex, i.uv).g;
                    col.g = (_IncomingRMS + n * prevCumAvg) / (n + 1);
                    
                    // Blue channel gets the novelty RMS
                    col.b = col.r - col.g;
                }

                return col;
            }
            ENDCG
        }
    }
}
