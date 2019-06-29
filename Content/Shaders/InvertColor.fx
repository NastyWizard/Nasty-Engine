sampler s0;

float4 PixelShaderFunction(float2 coords: TEXCOORD0) : COLOR0
{
	float4 texel = tex2D(s0,coords);
	texel.bgr = 1 - texel.rgb;

	clip(texel.a < 0.1 ? -1 : 1);

	return texel;
}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}