// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Hidden/CustomInternal-DeferredReflections" {
Properties {
    _SrcBlend ("", Float) = 1
    _DstBlend ("", Float) = 1
}
SubShader {

// Calculates reflection contribution from a single probe (rendered as cubes) or default reflection (rendered as full screen quad)
Pass {
    ZWrite Off
    ZTest LEqual
    Blend [_SrcBlend] [_DstBlend]
CGPROGRAM
// ---------------------------------------------- NoiseCrimeStudios : OBB Support
#define OBB_PROJECTION 1
// ----------------------------------------------

#pragma target 3.0
#pragma vertex vert_deferred
//#pragma vertex vert_deferredCustom
#pragma fragment frag

#include "BoxProjRot/UnityCG.cginc"
#include "BoxProjRot/UnityDeferredLibrary.cginc"
#include "BoxProjRot/UnityStandardUtils.cginc"
#include "BoxProjRot/UnityGBuffer.cginc"
#include "BoxProjRot/UnityStandardBRDF.cginc"
#include "BoxProjRot/UnityPBSLighting.cginc"

sampler2D _CameraGBufferTexture0;
sampler2D _CameraGBufferTexture1;
sampler2D _CameraGBufferTexture2;

// ---------------------------------------------- NoiseCrimeStudios : OBB Support
float4x4    _OBBProbeWorldToLocal;
// float4x4    _BoxProbeMatrix;
// ----------------------------------------------

half3 distanceFromAABB(half3 p, half3 aabbMin, half3 aabbMax)
{
    return max(max(p - aabbMax, aabbMin - p), half3(0.0, 0.0, 0.0));
}

unity_v2f_deferred vert_deferredCustom (float4 vertex : POSITION, float3 normal : NORMAL)
{
    unity_v2f_deferred o;

    // ---------------------------------------------- NoiseCrimeStudios : OBB Support
    // Testing trying to rotate the rendered cube - doesn't work
    // Might be looking in the wrong place - might be a stencil issue elsewhere!
    
    //  float4x4 m = _OBBProbeWorldToLocal; //  (float3x3)
    //  m[0][0] = 1; m[1][1] = 1; m[2][2] = 1; 
    //  vertex = mul(_BoxProbeMatrix, vertex);
    //  vertex.xyz = mul((float3x3)_BoxProbeMatrix, vertex.xyz);
    
    /*
    float vx =  cos(0.2) * vertex.z + sin(0.2) * vertex.x;
    float vz = -sin(0.2) * vertex.x + cos(0.2) * vertex.z;
    vertex.x = vx; 
    vertex.z = vz;
    */

    /*
    float4x4 rotated = mul(unity_ObjectToWorld, _BoxProbeMatrix );
    float4 newVertex =  mul( rotated, vertex );
    o.pos = mul(UNITY_MATRIX_VP, newVertex);
    o.ray = UnityObjectToViewPos(newVertex) * float3(-1,-1,1);
    o.uv = ComputeScreenPos(o.pos);
    */

    // vertex.xyz = vertex.xyz * 1.2;

    // ----------------------------------------------

    o.pos = UnityObjectToClipPos(vertex);
    o.uv = ComputeScreenPos(o.pos);
    o.ray = UnityObjectToViewPos(vertex) * float3(-1,-1,1);  
    
    // normal contains a ray pointing from the camera to one of near plane's
    // corners in camera space when we are drawing a full screen quad.
    // Otherwise, when rendering 3D shapes, use the ray calculated here.
    o.ray = lerp(o.ray, normal, _LightAsQuad);

    return o;
}


half4 frag (unity_v2f_deferred i) : SV_Target
{
    // Stripped from UnityDeferredCalculateLightParams, refactor into function ?
    i.ray = i.ray * (_ProjectionParams.z / i.ray.z);
    float2 uv = i.uv.xy / i.uv.w;

    // read depth and reconstruct world position
    float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, uv);
    depth = Linear01Depth (depth);
    float4 viewPos = float4(i.ray * depth,1);
    float3 worldPos = mul (unity_CameraToWorld, viewPos).xyz;

    half4 gbuffer0 = tex2D (_CameraGBufferTexture0, uv);
    half4 gbuffer1 = tex2D (_CameraGBufferTexture1, uv);
    half4 gbuffer2 = tex2D (_CameraGBufferTexture2, uv);
    UnityStandardData data = UnityStandardDataFromGbuffer(gbuffer0, gbuffer1, gbuffer2);

    float3 eyeVec = normalize(worldPos - _WorldSpaceCameraPos);
    half oneMinusReflectivity = 1 - SpecularStrength(data.specularColor);

    half3 worldNormalRefl = reflect(eyeVec, data.normalWorld);

    // Unused member don't need to be initialized
    UnityGIInput d;
    d.worldPos = worldPos;
    d.worldViewDir = -eyeVec;
    d.probeHDR[0] = unity_SpecCube0_HDR;
    d.boxMin[0].w = 1; // 1 in .w allow to disable blending in UnityGI_IndirectSpecular call since it doesn't work in Deferred

    float blendDistance = unity_SpecCube1_ProbePosition.w; // will be set to blend distance for this probe
    #ifdef UNITY_SPECCUBE_BOX_PROJECTION
    d.probePosition[0]  = unity_SpecCube0_ProbePosition;
    d.boxMin[0].xyz     = unity_SpecCube0_BoxMin - float4(blendDistance,blendDistance,blendDistance,0);
    d.boxMax[0].xyz     = unity_SpecCube0_BoxMax + float4(blendDistance,blendDistance,blendDistance,0);
    #endif

    Unity_GlossyEnvironmentData g = UnityGlossyEnvironmentSetup(data.smoothness, d.worldViewDir, data.normalWorld, data.specularColor);

    // ---------------------------------------------- NoiseCrimeStudios : OBB Support
    // Note: Maybe we should be doing this in UnityGlossyEnvironmentSetup to catch all code paths?
    // Might have to alter UNITY_GLOSSY_ENV_FROM_SURFACE too.
    // EXCEPT - we redefine glossIn.reflUVW in UnityGI_IndirectSpecular to use the box projected result!!!!
    #ifdef UNITY_SPECCUBE_BOX_PROJECTION 
        #if OBB_PROJECTION
		    // Intersection with OBB - Transform in local unit parallax cube space of probe (scaled and rotated)
		    g.probeLocalReflUVW		= mul((float3x3)_OBBProbeWorldToLocal, g.reflUVW);  // normalise?
		    g.probeLocalPosition	= mul(_OBBProbeWorldToLocal, float4(d.worldPos, 1)).xyz;
	    #endif
    #endif
    // ----------------------------------------------
   
    half3 env0 = UnityGI_IndirectSpecular(d, data.occlusion, g);

    UnityLight light;
    light.color = half3(0, 0, 0);
    light.dir = half3(0, 1, 0);

    UnityIndirect ind;
    ind.diffuse = 0;
    ind.specular = env0;

    half3 rgb = UNITY_BRDF_PBS (0, data.specularColor, oneMinusReflectivity, data.smoothness, data.normalWorld, -eyeVec, light, ind).rgb;

    // Calculate falloff value, so reflections on the edges of the probe would gradually blend to previous reflection.
    // Also this ensures that pixels not located in the reflection probe AABB won't
    // accidentally pick up reflections from this probe.
    half3 distance = distanceFromAABB(worldPos, unity_SpecCube0_BoxMin.xyz, unity_SpecCube0_BoxMax.xyz);
    half falloff = 1;// saturate(1.0 - length(distance)/blendDistance);

    return half4(rgb, falloff);
}

ENDCG
}

// Adds reflection buffer to the lighting buffer
Pass
{
    ZWrite Off
    ZTest Always
    Blend [_SrcBlend] [_DstBlend]

    CGPROGRAM
        #pragma target 3.0
        #pragma vertex vert
        #pragma fragment frag
        #pragma multi_compile ___ UNITY_HDR_ON

        #include "BoxProjRot/UnityCG.cginc"

        sampler2D _CameraReflectionsTexture;
 
        struct v2f {
            float2 uv : TEXCOORD0;
            float4 pos : SV_POSITION;
        };

        v2f vert (float4 vertex : POSITION)
        {
            v2f o;
            
            o.pos = UnityObjectToClipPos(vertex);
            o.uv = ComputeScreenPos (o.pos).xy;
            return o;
        }

        half4 frag (v2f i) : SV_Target
        {
            half4 c = tex2D (_CameraReflectionsTexture, i.uv);
            #ifdef UNITY_HDR_ON
            return float4(c.rgb, 0.0f);
            #else
            return float4(exp2(-c.rgb), 0.0f);
            #endif

        }
    ENDCG
}

}
Fallback Off
}
