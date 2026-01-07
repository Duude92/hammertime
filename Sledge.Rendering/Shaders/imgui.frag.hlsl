struct PS_INPUT
{
    float4 pos : SV_POSITION;
    float4 col : COLOR0;
    float2 uv : TEXCOORD0;
};
//FIXME: Correct layout
Texture2D FontTexture : register(t0, space1);
sampler FontSampler : register(s1, space0);

float4 main(PS_INPUT input) : SV_Target
{
    float4 out_col = input.col * FontTexture.Sample(FontSampler, input.uv);
    return out_col;
}