Texture2D Texture;
SamplerState Sampler;

struct PS_INPUT
{
    float2 Position : TEXCOORD0;
	float2 TexCoord : TEXCOORD1;
};

float4 main(PS_INPUT input) : SV_Target0
{
    float2 texcoord = input.TexCoord;
    return Texture.Sample(Sampler, normalize(texcoord));
    
}