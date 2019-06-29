#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif



float _ScreenWidth;
float _ScreenHeight;

float4 _OutlineColor;

Texture2D SpriteTexture;
sampler2D SpriteTextureSampler = sampler_state
{
	Texture = <SpriteTexture>;
	MinFilter = Point;
	MagFilter = Point;
};

Texture2D _Background;
sampler2D BackgroundSampler = sampler_state
{
	Texture = <_Background>;
	MinFilter = Point;
	MagFilter = Point;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};

float4 MainPS(VertexShaderOutput input) : COLOR
{
	float2 uv = input.TextureCoordinates;
	float4 texel = tex2D(SpriteTextureSampler, uv);

	float4 bgTex = tex2D(BackgroundSampler, uv);

	float4 rTexel = tex2D(SpriteTextureSampler, uv + float2(1.0 / _ScreenWidth, 0.0) * 2);
	float4 lTexel = tex2D(SpriteTextureSampler, uv - float2(1.0 / _ScreenWidth, 0.0) * 2);

	float4 uTexel = tex2D(SpriteTextureSampler, uv + float2(0.0, 1.0 / _ScreenHeight) * 2);
	float4 dTexel = tex2D(SpriteTextureSampler, uv - float2(0.0, 1.0 / _ScreenHeight) * 2);

	float aa = ceil(length(rTexel.rgb)) + ceil(length(lTexel.rgb)) + ceil(length(uTexel.rgb)) + ceil(length(dTexel.rgb));

	aa = (1 - texel.a) * min(aa, 1);

	return lerp(lerp(texel, _OutlineColor, aa), bgTex, 1-(aa + texel.a));
}

technique SpriteDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};