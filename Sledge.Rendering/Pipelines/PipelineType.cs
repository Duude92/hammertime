namespace Sledge.Rendering.Pipelines
{
    public enum PipelineType
    {
        Skybox,
        Wireframe,

        TexturedOpaque,
        BillboardOpaque,
        
        TexturedAlpha,
        TexturedAdditive,
        BillboardAlpha,

        WireframeModel,
        TexturedModel,

        Overlay,
    }
}