using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

namespace AtanOutlineLite
{
    public class OutlinePass : ScriptableRenderPass
    {
        private const string OUTLINE_PASS_NAME = "OutlinePass";
        
        private static readonly int normalThreshold = Shader.PropertyToID("_NormalThreshold");
        private static readonly int outlineColor = Shader.PropertyToID("_OutlineColor");
        private static readonly int outlineSize = Shader.PropertyToID("_OutlineSize");
        
        class OutlinePassData
        {
            internal Material blitMaterial;
        }
        
        private readonly OutlineSettings settings;
        
        public OutlinePass(OutlineSettings settings)
        {
            this.settings = settings;
        }

        private void UpdateOutlineSettings()
        {
            settings.outlineMaterial.SetFloat(normalThreshold, settings.normalThreshold);
            settings.outlineMaterial.SetColor(outlineColor, settings.outlineColor);
            settings.outlineMaterial.SetFloat(outlineSize, settings.outlineSize);
        }

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
            UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();
            UniversalRenderingData renderingData = frameData.Get<UniversalRenderingData>();
            
            if (resourceData.isActiveTargetBackBuffer)
                 return;
            
            var cameraTexHandle = resourceData.activeColorTexture;
            
            if (settings == null || settings.outlineMaterial == null)
                return;
            
            UpdateOutlineSettings();

            if (!cameraTexHandle.IsValid())
            {
                Debug.LogWarning("OutlinePass: Invalid source or destination texture.");
                return;
            }

            using var builder = renderGraph.AddRasterRenderPass<OutlinePassData>(OUTLINE_PASS_NAME, out var passData);
            passData.blitMaterial = settings.outlineMaterial;

            builder.SetRenderAttachment(cameraTexHandle, 0);
                
            builder.AllowPassCulling(false);

            builder.SetRenderFunc((OutlinePassData data, RasterGraphContext ctx) =>
            {
                Blitter.BlitTexture(ctx.cmd,
                    new Vector4(1, 1, 0, 0),
                    data.blitMaterial,
                    0
                );
            });
        }
    }
}