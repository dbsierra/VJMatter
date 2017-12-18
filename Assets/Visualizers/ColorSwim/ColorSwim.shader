Shader "Unlit/ColorSwim"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
        _Red ("Red", range(0,1)) = 0
        _Green ("Green", range(0,1)) = 0
        _Blue ("Blue", range(0,1)) = 0
        _Mod ("Modulator", vector) = (0,0,0,0)
        _Freq ("Frequency", range(0,1)) = .25
        _Speed ("Speed", range(0,1)) = .1
        _NoiseFreq("Noise Frequency", range(0,1)) = .5

        _Saturation("Saturation", range(0,1)) = 1
        _Value("Value", range(0,1)) = 1

        _Mix("Mix", Range(0,1)) = 0

        _Logo("Logo", 2D) = "black" {}
        _LogoMix("LogoMix", Range(0,1)) = 0

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
			
			#include "UnityCG.cginc"
            #include "../../Syn/Noise/noiseSimplex.cginc"
            #include "../../Syn/Noise/shaderUtil.cginc"

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
            sampler2D _Logo;
			float4 _MainTex_ST;
            float _Red;
            float _Green;
            float _Blue;
			float _Freq;
            float _Speed;
            float _Saturation;
            float _Value;
            float _NoiseFreq;
            float4 _Mod;
            half _Mix;
            half _LogoMix;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
            float random (float2 st) {
                return frac(sin(dot(st.xy,
                                     float2(12.9898,78.233)))*
                    43758.5453123);
            }

			fixed4 frag (v2f i) : SV_Target
			{
                float2 uv = i.uv;

				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);

                float direction = uv.x; //1.0-(uv.x * uv.y);
                direction += (snoise(float3(8*_NoiseFreq*uv,_Time.y*.52))+1)/3;

                float3 mod = float3(1,1,1);
                mod.r = lerp(1, (cos(_Time.y*_Red)*.5+.5), saturate(_Mod.r));
                mod.g = lerp(1, (cos(_Time.y*_Green)*.5+.5), saturate(_Mod.g));
                mod.b = lerp(1, (cos(_Time.y*_Blue)*.5+.5), saturate(_Mod.b));

                float r = cos(_Time.y*_Speed*30.0 +  mod.r*6.28 + direction*_Freq*14.0)*.5+.5;
                float g = cos(_Time.y*_Speed*30.0 + mod.g*.33*6.28 + direction*_Freq*14.0)*.5+.5;
                float b = cos(_Time.y*_Speed*30.0 + mod.b*.66*6.28 + direction*_Freq*14.0)*.5+.5;
                float3 pcol = float3(r,g,b) * col.a;
                
                float3 colSwim = pcol;//col * float4(r, g, b, 1.0);  

                colSwim.rgb = rgb2hsv(colSwim.rgb);

                colSwim.g *= _Saturation;
                colSwim.b *= _Value;

                colSwim.rgb = hsv2rgb(colSwim.rgb);

                colSwim.rgb += random(float2(i.uv.x+_Time.y, i.uv.y))*.1;

                col.rgb = lerp(col.rgb, colSwim, _Mix);

                col.rgb += lerp(float3(0,0,0), tex2D(_Logo, i.uv).rgb, _LogoMix) * saturate(cos(_Time.y*120.)*.5+.75);

               // col.rgb = colSwim;

              //  col = float4(col.a, col.a, col.a, 1.0);

				return col;
			}
			ENDCG
		}
	}
}
