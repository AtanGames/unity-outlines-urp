Shader "AtanOutlineLite/OutlineShader"
{
    HLSLINCLUDE
    
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

        TEXTURE2D_X(_CameraNormalsTexture);
        TEXTURE2D_X(_CameraOpaqueTexture);

        float _NormalThreshold;
        float4 _OutlineColor;
        float _OutlineSize;
        
        float FilterRobertsNormal(float2 uv)
        {
            float2 invScreen = 1.0 / _ScreenParams.xy;
            float2 offset = _OutlineSize * invScreen;

            float ox = offset.x;
            float oy = offset.y;
            
            float2 uvR  = uv + float2( ox,  0);
            float2 uvD  = uv + float2( 0 , -oy);
            float2 uvRD = uv + float2( ox, -oy);
            
            float4 n    = SAMPLE_TEXTURE2D_X(_CameraNormalsTexture, sampler_PointClamp, uv);
            float4 nR   = SAMPLE_TEXTURE2D_X(_CameraNormalsTexture, sampler_PointClamp, uvR);
            float4 nD   = SAMPLE_TEXTURE2D_X(_CameraNormalsTexture, sampler_PointClamp, uvD);
            float4 nRD  = SAMPLE_TEXTURE2D_X(_CameraNormalsTexture, sampler_PointClamp, uvRD);

            float3 Gx = n.rgb   - nRD.rgb;
            float3 Gy = nR.rgb  - nD.rgb;

            float robSq = dot(Gx, Gx) + dot(Gy, Gy);
            return robSq * 0.5;
        }  
        
        float4 OutlinePass (Varyings input) : SV_Target
        {
            float2 uv = input.texcoord;
            
            float4 currentColor = SAMPLE_TEXTURE2D_X(_CameraOpaqueTexture, sampler_PointClamp, uv);
            
            float outlineNormal = FilterRobertsNormal(uv);
            float normalThreshold = _NormalThreshold * _NormalThreshold;
            outlineNormal = step(normalThreshold, outlineNormal);

            return lerp(currentColor, _OutlineColor, outlineNormal * _OutlineColor.a);
        }
        
        ENDHLSL
    
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline"}
        LOD 100
        ZWrite Off Cull Off
        
        Pass
        {
            Name "OutlinePass"
            
            HLSLPROGRAM

            #pragma multi_compile_instancing
            
            #pragma vertex Vert
            #pragma fragment OutlinePass
            
            ENDHLSL
        }
    }
}
