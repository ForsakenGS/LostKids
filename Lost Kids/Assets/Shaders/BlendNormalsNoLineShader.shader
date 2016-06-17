Shader "Custom/BlendNormalsNoLineShader" {
    Properties {
        _LightColor ("LightColor", Color) = (1,1,1,1)
        _Diffuse ("Diffuse", 2D) = "black" {}
        _Glossiness ("Glossiness", Range(0, 11)) = 0
        _Specularity ("Specularity", Range(0, 4)) = 1
        _Normals ("Normals", 2D) = "bump" {}
        _NormalRedChannel ("Normal Red Channel", Range(1, 10)) = 1
        _NormalGreenChannel ("Normal Green Channel", Range(1, 10)) = 1
        _NormalBlueChannel ("Normal Blue Channel", Range(0.1, 10)) = 0.5
        _Noise ("Noise", 2D) = "white" {}
        _BlendStrength ("BlendStrength", Range(-0.5, 1)) = 0.1741122
        _TextureAlt ("TextureAlt", 2D) = "white" {}
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma exclude_renderers metal xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform sampler2D _Diffuse; uniform float4 _Diffuse_ST;
            uniform sampler2D _Normals; uniform float4 _Normals_ST;
            uniform float _Glossiness;
            uniform float _Specularity;
            uniform float4 _LightColor;
            uniform float _NormalRedChannel;
            uniform float _NormalGreenChannel;
            uniform float _NormalBlueChannel;
            uniform sampler2D _Noise; uniform float4 _Noise_ST;
            uniform float _BlendStrength;
            uniform sampler2D _TextureAlt; uniform float4 _TextureAlt_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float3 tangentDir : TEXCOORD3;
                float3 bitangentDir : TEXCOORD4;
                LIGHTING_COORDS(5,6)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( _Object2World, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(_Object2World, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex );
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 _Normals_var = UnpackNormal(tex2D(_Normals,TRANSFORM_TEX(i.uv0, _Normals)));
                float3 normalLocal = (((float3(1,0,0)*_NormalRedChannel)+(float3(0,1,0)*_NormalGreenChannel)+(float3(0,0,1)*_NormalBlueChannel))*_Normals_var.rgb);
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float4 _TextureAlt_var = tex2D(_TextureAlt,TRANSFORM_TEX(i.uv0, _TextureAlt));
                float4 _Diffuse_var = tex2D(_Diffuse,TRANSFORM_TEX(i.uv0, _Diffuse));
                float4 _Noise_var = tex2D(_Noise,TRANSFORM_TEX(i.uv0, _Noise));
                float3 finalColor = (lerp(_TextureAlt_var.rgb,_Diffuse_var.rgb,saturate((saturate((_BlendStrength+_Diffuse_var.rgb))+saturate((_BlendStrength+_Noise_var.rgb))-1.0)))*((_Diffuse_var.rgb*max(0,dot(lightDirection,normalDirection)))+(pow(max(0,dot(normalDirection,halfDirection)),exp(_Glossiness))*_Specularity))*attenuation*_LightColor0.rgb*_LightColor.rgb);
                return fixed4(finalColor,1);
            }
            ENDCG
        }
        Pass {
            Name "FORWARD_DELTA"
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
            #pragma multi_compile_fwdadd_fullshadows
            #pragma exclude_renderers metal xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform sampler2D _Diffuse; uniform float4 _Diffuse_ST;
            uniform sampler2D _Normals; uniform float4 _Normals_ST;
            uniform float _Glossiness;
            uniform float _Specularity;
            uniform float4 _LightColor;
            uniform float _NormalRedChannel;
            uniform float _NormalGreenChannel;
            uniform float _NormalBlueChannel;
            uniform sampler2D _Noise; uniform float4 _Noise_ST;
            uniform float _BlendStrength;
            uniform sampler2D _TextureAlt; uniform float4 _TextureAlt_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float3 tangentDir : TEXCOORD3;
                float3 bitangentDir : TEXCOORD4;
                LIGHTING_COORDS(5,6)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( _Object2World, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(_Object2World, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex );
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 _Normals_var = UnpackNormal(tex2D(_Normals,TRANSFORM_TEX(i.uv0, _Normals)));
                float3 normalLocal = (((float3(1,0,0)*_NormalRedChannel)+(float3(0,1,0)*_NormalGreenChannel)+(float3(0,0,1)*_NormalBlueChannel))*_Normals_var.rgb);
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float4 _TextureAlt_var = tex2D(_TextureAlt,TRANSFORM_TEX(i.uv0, _TextureAlt));
                float4 _Diffuse_var = tex2D(_Diffuse,TRANSFORM_TEX(i.uv0, _Diffuse));
                float4 _Noise_var = tex2D(_Noise,TRANSFORM_TEX(i.uv0, _Noise));
                float3 finalColor = (lerp(_TextureAlt_var.rgb,_Diffuse_var.rgb,saturate((saturate((_BlendStrength+_Diffuse_var.rgb))+saturate((_BlendStrength+_Noise_var.rgb))-1.0)))*((_Diffuse_var.rgb*max(0,dot(lightDirection,normalDirection)))+(pow(max(0,dot(normalDirection,halfDirection)),exp(_Glossiness))*_Specularity))*attenuation*_LightColor0.rgb*_LightColor.rgb);
                return fixed4(finalColor * 1,0);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
