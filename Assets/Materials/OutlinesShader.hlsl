#ifndef OUTLINESSHADER_INCLUDE
#define OUTLINESSHADER_INCLUDE

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

struct Attributes {
    float4 positionOS : POSITION;
    float3 normalOS : NORMAL;
#ifdef USE_PRECALCULATED_OUTLINE_NORMALS
    float3 smoothNormalOS   : TEXCOORD1; // Calculated "smooth" normals to extrude along in object space
#endif
};

struct VertexOutput {
    float4 positionCS : SV_POSITION;
};

float _Thickness;
float4 _Color;
float _DepthOffset;

VertexOutput Vertex(Attributes input) {
    VertexOutput output = (VertexOutput)0;

    float3 normalOS;
#ifdef USE_PRECALCULATED_OUTLINE_NORMALS
    normalOS = input.smoothNormalOS;
#else
    normalOS = input.normalOS;
#endif


    float3 posOS = input.positionOS.xyz + normalOS * _Thickness;
    output.positionCS = GetVertexPositionInputs(posOS).positionCS;

    float depthOffset = _DepthOffset;
    // If depth is reversed on this platform, reverse the offset
#ifdef UNITY_REVERSED_Z
    depthOffset = -depthOffset;
#endif
    output.positionCS.z += depthOffset;

    return output;
}

float4 Fragment(VertexOutput input) : SV_Target {
    return _Color;
}

#endif