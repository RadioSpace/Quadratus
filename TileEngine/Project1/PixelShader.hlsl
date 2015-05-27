
Texture2D tex : register(t0);
SamplerState psampler;

struct PS_IN
{
	float4 pos : SV_POSITION;
	float4 col : COLOR;
	float2 tex : TEXCOORD;
};

float4 main(PS_IN input) : SV_TARGET
{



	return tex.Sample(psampler,input.tex) * input.col;
}