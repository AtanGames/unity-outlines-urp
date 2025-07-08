using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace AtanOutlineLite
{
    public class OutlineFeatureLite : ScriptableRendererFeature
    {
        public OutlineSettings settings;

        private OutlinePass outlinePass;
        
        public override void Create()
        {
            settings ??= new OutlineSettings();
            
            if (settings.outlineMaterial == null)
                return;

            CreatePass();
        }
        
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (outlinePass == null)
            {
                if (settings.outlineMaterial != null)
                    CreatePass();
                else
                    return;
            }
            
            outlinePass!.ConfigureInput(ScriptableRenderPassInput.Normal | ScriptableRenderPassInput.Color);
            
            if ((renderingData.cameraData.cameraType & CameraType.Game) != 0)
                renderer.EnqueuePass(outlinePass);
        }
        
        private void CreatePass()
        {
            outlinePass = new OutlinePass(settings)
            {
                renderPassEvent = RenderPassEvent.BeforeRenderingTransparents
            };
        }
    }
}
