// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

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

// The main source texture from the previous stage.
sampler2D _MainTex;
float4 _MainTex_ST;


// Just does basic vertex transform.
v2f defaultVert(appdata v)
{
	v2f o;
	o.vertex = UnityObjectToClipPos(v.vertex);
	o.uv = TRANSFORM_TEX(v.uv, _MainTex);
	return o;
}

// Just copies the previous stage.
fixed4 defaultFrag(v2f i) : SV_Target
{
	fixed4 col = tex2D(_MainTex, i.uv);
return col;
}