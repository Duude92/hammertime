struct FragmentIn
{
    float4 fPosition : SV_Position;
    float4 fNormal : NORMAL0;
    float4 fColour : COLOR0;
    float2 fTexture : TEXCOORD0;
    float4 fTint : COLOR1;
    uint1 Flags : POSITION1;
};
static const uint Flags_Wireframed = 1 << 4;

float4 main(FragmentIn input) : SV_Target0
{
    if ((input.Flags & Flags_Wireframed) != 0)
    {
        return float4(0, 0, 1, 1);

    }
    return input.fColour;
}
