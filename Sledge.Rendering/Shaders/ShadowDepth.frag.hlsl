struct PS_INPUT
{
    float4 Position : SV_Position;
};

float4 main(PS_INPUT input) : SV_Target0
{
    float depthValue = input.Position.z / input.Position.w;
    return float4(depthValue, depthValue, depthValue, 1.0f);
}