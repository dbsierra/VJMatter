Shader "Unlit/InitialPositions_shd"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
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

    sampler2D _Random;
    sampler2D _Index;
    int _TextureSize;
    int _Mode; //0=box 1=sphere 2=debug

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

    fixed4 frag(v2f i) : SV_Target
    {
        float4 randValues = tex2D(_Random, i.uv);

        //Get the 0-1 particle index based on the input UV coordinates. If we were to pass this
        //through a texture, we would need a floating point texture to get enough percision. 8-bit is not enough.
        //Instead we can calculate directly in the shader based off of the UVs.
        float size = (float)_TextureSize;
        float halfPix = .5 / size;
        float y = i.uv.y;// -halfPix;
        float x = i.uv.x;// -halfPix;

        float ppIndex = saturate( y + x / size - halfPix );

        float4 finalColor = float4(0, 0, 0, 0);

        if (_Mode == 0)
            finalColor = float4(randValues.xyz - 0.5, ppIndex);
        else if (_Mode == 1)
            finalColor = float4(normalize(randValues.xyz * 2 - 1)*(cos(ppIndex*30.)*.5 + .5), ppIndex);
        else if (_Mode == 2)
            finalColor = float4(ppIndex*2. - 1, 0, 0, ppIndex);
        else if (_Mode == 3)
            finalColor = float4(i.uv.x - .5, i.uv.y - .5, 0, ppIndex);
        else if (_Mode == 4)
            finalColor = float4(Lissajou(ppIndex, float3(2, .41, 4.4), .5), ppIndex);
        
        finalColor.rgb *= 2;

        return finalColor;
    }
        ENDCG
    }
    }
}
