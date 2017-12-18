Shader "Unlit/DelayRGB"
{
    Properties
    {
        _MyArr ("Tex", 2DArray) = "" {}
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
            // make fog work
            #pragma multi_compile_fog
                        #pragma target 3.5

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            int _ReadIndexR;
            int _ReadIndexG;
            int _ReadIndexB;
            float _WriteIndexR;
            UNITY_DECLARE_TEX2DARRAY(_MyArr);
            float4 _MainTex_ST;
            sampler3D _RenderStream;
            sampler2D _DelayR;

            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                
                col.rgb *= .5;

                col.r += UNITY_SAMPLE_TEX2DARRAY(_MyArr, float3(i.uv,_ReadIndexR)).rgb;
                col.g += UNITY_SAMPLE_TEX2DARRAY(_MyArr, float3(i.uv,_ReadIndexG)).rgb;
                col.b += UNITY_SAMPLE_TEX2DARRAY(_MyArr, float3(i.uv,_ReadIndexB)).rgb;

                return col;
            }
            ENDCG
        }
    }
}
