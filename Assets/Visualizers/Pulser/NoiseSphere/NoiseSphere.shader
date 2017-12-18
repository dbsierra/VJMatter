Shader "Custom/NoiseSphere" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
                [HDR]

        _Emission ("Emission", Color) = (0,0,0,1) 

        [HDR]
        _Rim("Rim", Color) = (0,0,0,1) 
        _RimPower("Rim Power", float) = 2.0

		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0

        _Amp("Amplitude", Range(0,.02)) = .005
        _Freq("Frequency", Range(40, 120)) = 90
        _Speed("Speed", Range(.1, 2)) = .6
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard vertex:vert fullforwardshadows

        #include "../../../Syn/Noise/noiseSimplex.cginc"

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
            float3 worldRefl;
            float3 viewDir;
		};

		half _Glossiness;
		half _Metallic;
		float4 _Color;
        half _Amp;
        half _Speed;
        half _Freq;
        float4 _Emission;
        float4 _Rim;
        half _RimPower;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_CBUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_CBUFFER_END

         void vert (inout appdata_full v) 
         {
            float3 seed = v.vertex*_Freq;
            seed.z += _Time.y*_Speed;

            v.vertex.xyz += v.normal * snoise(seed)*_Amp;
        }

		void surf (Input IN, inout SurfaceOutputStandard o) {
            

			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;

            o.Emission = _Emission;

            float3 fresColor = float3(0,0,0);
            half rim = pow( 1 - saturate(dot (normalize(IN.viewDir), o.Normal)), _RimPower);

            fresColor = rim * _Rim;

            o.Emission += fresColor;

		}
		ENDCG
	}
	FallBack "Diffuse"
}
