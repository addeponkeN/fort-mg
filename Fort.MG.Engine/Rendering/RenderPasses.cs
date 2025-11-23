using Microsoft.Xna.Framework.Graphics;

namespace Fort.MG.Rendering;

public static class RenderPasses
{
    public static DepthStencilState DepthStencilLess = new DepthStencilState
    {
        DepthBufferEnable = true,
        DepthBufferFunction = CompareFunction.LessEqual,
    };


    public static AlphaTestEffect AlphaTestEffectDefault = new AlphaTestEffect(Graphics.GraphicsDevice)
    {
        VertexColorEnabled = true,
        ReferenceAlpha = 1,
    };


    public static readonly RenderPass Default = new()
    {
        Name = "Default",
        SortMode = SpriteSortMode.FrontToBack,
        BlendState = BlendState.AlphaBlend,
        SamplerState = SamplerState.PointClamp,
        DepthStencilState = null,
        RasterizerState = null,
        Effect = AlphaTestEffectDefault
    };

    public static readonly RenderPass DefaultSharedDepth = new()
    {
        Name = "Default",
        SortMode = SpriteSortMode.FrontToBack,
        BlendState = BlendState.AlphaBlend,
        SamplerState = SamplerState.PointClamp,

        DepthStencilState = DepthStencilLess,

        RasterizerState = null,
        Effect = AlphaTestEffectDefault,
    };


    public static RenderPass DefaultTileMapRenderer = new RenderPass
    {
        BlendState = BlendState.AlphaBlend,
        SamplerState = SamplerState.PointClamp,
        Priority = -10,
        Name = "TileMapRenderer",
    };

    public static readonly RenderPass Additive = new()
    {
        Name = "Additive",
        SortMode = SpriteSortMode.Deferred,
        BlendState = BlendState.Additive,
        SamplerState = SamplerState.LinearClamp
    };

    public static readonly RenderPass UI = new()
    {
        Name = "UI",
        SortMode = SpriteSortMode.Deferred,
        BlendState = BlendState.AlphaBlend,
        SamplerState = SamplerState.PointClamp
    };

    public static readonly RenderPass OpaqueWithDepth = new()
    {
        Name = "OpaqueWithDepth",
        SortMode = SpriteSortMode.Deferred,
        BlendState = BlendState.Opaque,
        DepthStencilState = DepthStencilState.Default
    };

    // #todo: add more common passes(?)

}
