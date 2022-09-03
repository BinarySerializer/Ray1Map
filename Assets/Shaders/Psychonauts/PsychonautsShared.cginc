#ifndef SHARED_PSYCHONAUTS
#define SHARED_PSYCHONAUTS
#include "UnityCG.cginc"

struct appdata
{
	float4 vertex : POSITION;
	float2 uv0 : TEXCOORD0;
	float2 uv1 : TEXCOORD1;
	float2 uv2 : TEXCOORD2;
	float2 uvLM : TEXCOORD3;
	float4 color : COLOR;
};

struct v2f
{
	float4 vertex : SV_POSITION;
	float4 color : COLOR;
	float2 uv0 : TEXCOORD0;
	float2 uv1 : TEXCOORD1;
	float2 uv2 : TEXCOORD2;
	float2 uvLM : TEXCOORD3;
};

sampler2D _Tex0, _Tex1, _Tex2, _TexLightMap, _TexGlossMap, _TexReflectionMap;
float4 _Tex0_ST, _Tex1_ST, _Tex2_ST, _TexLightMap_ST, _TexGlossMap_ST, _TexReflectionMap_ST;
float4 _TexturesInUse, _TexturesInUse2;
float _IsSelfIllumination;
float4 _MaterialColor;

v2f process_vert(appdata v, float isAdd) {
	v2f o;
	o.vertex = UnityObjectToClipPos(v.vertex);
	o.uv0 = TRANSFORM_TEX(v.uv0, _Tex0);
	o.uv1 = TRANSFORM_TEX(v.uv1, _Tex1);
	o.uv2 = TRANSFORM_TEX(v.uv2, _Tex2);
	o.uvLM = TRANSFORM_TEX(v.uvLM, _TexLightMap);
	o.color = v.color;
	return o;
}

float4 TextureOp(float4 color_in, sampler2D tex, float2 uv, float type, float index) {
	float4 texColor = tex2D(tex, uv.xy);

	if (type == 0) {
		if (index == 0) {
			return color_in + texColor;
		} else {
			float finalAlpha = clamp(color_in.w + texColor.w, 0, 1);
			return float4(
				lerp(float4(color_in.xyz, 1), float4(texColor.xyz, 1), texColor.w).xyz,
				finalAlpha);
		}
	} else if (type == 1) {
		// Lightmap
		if (_IsSelfIllumination == 1) {
			return lerp(color_in * float4(texColor.z, texColor.y, texColor.x, 1) * 2, color_in, color_in.w);
		} else {
			return color_in * float4(texColor.z, texColor.y, texColor.x, 1) * 2;
		}
	}
	return color_in;
}

float4 process_frag(v2f i, float clipAlpha, float isAdd, float alphaTest) : SV_TARGET {
	float4 c = float4(0.0, 0.0, 0.0, 0.0);

	// sample the texture
	if (_TexturesInUse.x != 0.0) c = TextureOp(c, _Tex0, i.uv0, 0, 0);
	if (_TexturesInUse.y != 0.0) c = TextureOp(c, _Tex1, i.uv1, 0, 1);
	if (_TexturesInUse.z != 0.0) c = TextureOp(c, _Tex2, i.uv2, 0, 2);
	if (_TexturesInUse.w != 0.0) c = TextureOp(c, _TexLightMap, i.uvLM, 1, 3);
	
	if (length(_TexturesInUse) <= 0.0) {
		c = i.color * _MaterialColor;
	} else {
		c = c * i.color * _MaterialColor;
	}
	if (_IsSelfIllumination == 1) {
		c = float4(c.xyz, 1);
	}
	/*if (clipAlpha >= 0) {
		clip(c.a - clipAlpha);
	} else {
		clip(-c.a - clipAlpha);
	}*/
	if (clipAlpha < 0) { // Clip discards values below 0.
		clip(1.0 - c.a); // If clipAlpha == -1, then any color with alpha < 1 will be rendered
		clip(c.a - alphaTest);
	} else if(clipAlpha > 0) {
		clip(c.a - (1-alphaTest)); // If clipAlpha == 1, then any color with alpha > 1 will be rendered
	}

	return c;

}
#endif // SHARED_PSYCHONAUTS