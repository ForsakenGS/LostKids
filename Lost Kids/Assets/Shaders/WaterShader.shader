Shader "Custom/WaterShader" {
    Properties {
        _Color ("Color", Color) = (0.7,0.9,1,1)
        _Albedo ("Albedo", 2D) = "white" {}
        _Specular ("Specular", Range(0, 1)) = 0.9
        _Smoothness ("Smoothness", Range(0, 1)) = 0.7
        _Opacity ("Opacity", Range(0, 1)) = 0.2
        _SpeedU ("Speed U", Float ) = 0
        _SpeedV ("Speed V", Float ) = 0.5
        _Normal ("Normal", 2D) = "bump" {}
        _RefractionCount ("Refraction Count", Range(0, 1)) = 0.2
        _Value1 ("Value1", Float ) = 0
        _WaveSpeed ("WaveSpeed", Float ) = 1
        _Value2 ("Value2", Float ) = 5
        _Value3 ("Value3", Float ) = 5
        _VertexWaveOffset ("Vertex Wave Offset", Range(0, 1)) = 0.4
        _VertexDetailOffset ("Vertex Detail Offset", Range(0, 1)) = 0.03
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        GrabPass{ }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma exclude_renderers xbox360 ps3 
            #pragma target 3.0
            #pragma glsl
            uniform float4 _LightColor0;
            uniform sampler2D _GrabTexture;
            uniform float4 _TimeEditor;
            uniform float4 _Color;
            uniform sampler2D _Albedo; uniform float4 _Albedo_ST;
            uniform float _Specular;
            uniform float _Smoothness;
            uniform float _Opacity;
            uniform sampler2D _Normal; uniform float4 _Normal_ST;
            uniform float _SpeedU;
            uniform float _SpeedV;
            uniform float _RefractionCount;
            uniform float _Value1;
            uniform float _WaveSpeed;
            uniform float _Value2;
            uniform float _Value3;
            uniform float _VertexWaveOffset;
            uniform float _VertexDetailOffset;
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
                float4 screenPos : TEXCOORD5;
                UNITY_FOG_COORDS(6)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( _Object2World, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                float4 node_652 = _Time + _TimeEditor;
                float2 node_3745 = (o.uv0+(float2(_SpeedU,_SpeedV)*node_652.g));
                float4 _Albedo_var = tex2Dlod(_Albedo,float4(TRANSFORM_TEX(node_3745, _Albedo),0.0,0));
                float4 node_5650 = _Time + _TimeEditor;
                float node_5783 = frac((o.uv0+(float2(_Value1,_WaveSpeed)*node_5650.g)).g);
                float node_9681 = (pow((1.0 - node_5783),_Value2)+pow(node_5783,_Value2));
                v.vertex.xyz += (saturate(((_Albedo_var.rgb*_VertexDetailOffset)+((pow(node_9681,_Value3)*pow((1.0 - node_9681),_Value3))*_VertexWaveOffset)))*v.normal);
                o.posWorld = mul(_Object2World, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                o.screenPos = o.pos;
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                #if UNITY_UV_STARTS_AT_TOP
                    float grabSign = -_ProjectionParams.x;
                #else
                    float grabSign = _ProjectionParams.x;
                #endif
                i.normalDir = normalize(i.normalDir);
                i.screenPos = float4( i.screenPos.xy / i.screenPos.w, 0, 0 );
                i.screenPos.y *= _ProjectionParams.x;
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float4 node_652 = _Time + _TimeEditor;
                float2 node_3745 = (i.uv0+(float2(_SpeedU,_SpeedV)*node_652.g));
                float3 _Normal_var = UnpackNormal(tex2D(_Normal,TRANSFORM_TEX(node_3745, _Normal)));
                float3 normalLocal = _Normal_var.rgb;
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float2 sceneUVs = float2(1,grabSign)*i.screenPos.xy*0.5+0.5 + (float2(_Normal_var.r,_Normal_var.g)*_RefractionCount);
                float4 sceneColor = tex2D(_GrabTexture, sceneUVs);
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
////// Lighting:
                float attenuation = 1;
                float3 attenColor = attenuation * _LightColor0.xyz;
///////// Gloss:
                float gloss = _Smoothness;
                float specPow = exp2( gloss * 10.0+1.0);
////// Specular:
                float NdotL = max(0, dot( normalDirection, lightDirection ));
                float3 specularColor = float3(_Specular,_Specular,_Specular);
                float3 directSpecular = (floor(attenuation) * _LightColor0.xyz) * pow(max(0,dot(reflect(-lightDirection, normalDirection),viewDirection)),specPow)*specularColor;
                float3 specular = directSpecular;
/////// Diffuse:
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float3 directDiffuse = max( 0.0, NdotL) * attenColor;
                float3 indirectDiffuse = float3(0,0,0);
                indirectDiffuse += UNITY_LIGHTMODEL_AMBIENT.rgb; // Ambient Light
                float4 _Albedo_var = tex2D(_Albedo,TRANSFORM_TEX(node_3745, _Albedo));
                float3 diffuseColor = saturate(( (_Albedo_var.rgb*0.2+0.4) > 0.5 ? (1.0-(1.0-2.0*((_Albedo_var.rgb*0.2+0.4)-0.5))*(1.0-_Color.rgb)) : (2.0*(_Albedo_var.rgb*0.2+0.4)*_Color.rgb) ));
                float3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse + specular;
                fixed4 finalRGBA = fixed4(lerp(sceneColor.rgb, finalColor,_Opacity),1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "FORWARD_DELTA"
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdadd
            #pragma multi_compile_fog
            #pragma exclude_renderers xbox360 ps3 
            #pragma target 3.0
            #pragma glsl
            uniform float4 _LightColor0;
            uniform sampler2D _GrabTexture;
            uniform float4 _TimeEditor;
            uniform float4 _Color;
            uniform sampler2D _Albedo; uniform float4 _Albedo_ST;
            uniform float _Specular;
            uniform float _Smoothness;
            uniform float _Opacity;
            uniform sampler2D _Normal; uniform float4 _Normal_ST;
            uniform float _SpeedU;
            uniform float _SpeedV;
            uniform float _RefractionCount;
            uniform float _Value1;
            uniform float _WaveSpeed;
            uniform float _Value2;
            uniform float _Value3;
            uniform float _VertexWaveOffset;
            uniform float _VertexDetailOffset;
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
                float4 screenPos : TEXCOORD5;
                LIGHTING_COORDS(6,7)
                UNITY_FOG_COORDS(8)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( _Object2World, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                float4 node_652 = _Time + _TimeEditor;
                float2 node_3745 = (o.uv0+(float2(_SpeedU,_SpeedV)*node_652.g));
                float4 _Albedo_var = tex2Dlod(_Albedo,float4(TRANSFORM_TEX(node_3745, _Albedo),0.0,0));
                float4 node_5650 = _Time + _TimeEditor;
                float node_5783 = frac((o.uv0+(float2(_Value1,_WaveSpeed)*node_5650.g)).g);
                float node_9681 = (pow((1.0 - node_5783),_Value2)+pow(node_5783,_Value2));
                v.vertex.xyz += (saturate(((_Albedo_var.rgb*_VertexDetailOffset)+((pow(node_9681,_Value3)*pow((1.0 - node_9681),_Value3))*_VertexWaveOffset)))*v.normal);
                o.posWorld = mul(_Object2World, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                o.screenPos = o.pos;
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                #if UNITY_UV_STARTS_AT_TOP
                    float grabSign = -_ProjectionParams.x;
                #else
                    float grabSign = _ProjectionParams.x;
                #endif
                i.normalDir = normalize(i.normalDir);
                i.screenPos = float4( i.screenPos.xy / i.screenPos.w, 0, 0 );
                i.screenPos.y *= _ProjectionParams.x;
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float4 node_652 = _Time + _TimeEditor;
                float2 node_3745 = (i.uv0+(float2(_SpeedU,_SpeedV)*node_652.g));
                float3 _Normal_var = UnpackNormal(tex2D(_Normal,TRANSFORM_TEX(node_3745, _Normal)));
                float3 normalLocal = _Normal_var.rgb;
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float2 sceneUVs = float2(1,grabSign)*i.screenPos.xy*0.5+0.5 + (float2(_Normal_var.r,_Normal_var.g)*_RefractionCount);
                float4 sceneColor = tex2D(_GrabTexture, sceneUVs);
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 lightColor = _LightColor0.rgb;
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
///////// Gloss:
                float gloss = _Smoothness;
                float specPow = exp2( gloss * 10.0+1.0);
////// Specular:
                float NdotL = max(0, dot( normalDirection, lightDirection ));
                float3 specularColor = float3(_Specular,_Specular,_Specular);
                float3 directSpecular = attenColor * pow(max(0,dot(reflect(-lightDirection, normalDirection),viewDirection)),specPow)*specularColor;
                float3 specular = directSpecular;
/////// Diffuse:
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float3 directDiffuse = max( 0.0, NdotL) * attenColor;
                float4 _Albedo_var = tex2D(_Albedo,TRANSFORM_TEX(node_3745, _Albedo));
                float3 diffuseColor = saturate(( (_Albedo_var.rgb*0.2+0.4) > 0.5 ? (1.0-(1.0-2.0*((_Albedo_var.rgb*0.2+0.4)-0.5))*(1.0-_Color.rgb)) : (2.0*(_Albedo_var.rgb*0.2+0.4)*_Color.rgb) ));
                float3 diffuse = directDiffuse * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse + specular;
                fixed4 finalRGBA = fixed4(finalColor * _Opacity,0);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "ShadowCaster"
            Tags {
                "LightMode"="ShadowCaster"
            }
            Offset 1, 1
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_SHADOWCASTER
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma multi_compile_fog
            #pragma exclude_renderers xbox360 ps3 
            #pragma target 3.0
            #pragma glsl
            uniform float4 _TimeEditor;
            uniform sampler2D _Albedo; uniform float4 _Albedo_ST;
            uniform float _SpeedU;
            uniform float _SpeedV;
            uniform float _Value1;
            uniform float _WaveSpeed;
            uniform float _Value2;
            uniform float _Value3;
            uniform float _VertexWaveOffset;
            uniform float _VertexDetailOffset;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
                float2 uv0 : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                float4 node_652 = _Time + _TimeEditor;
                float2 node_3745 = (o.uv0+(float2(_SpeedU,_SpeedV)*node_652.g));
                float4 _Albedo_var = tex2Dlod(_Albedo,float4(TRANSFORM_TEX(node_3745, _Albedo),0.0,0));
                float4 node_5650 = _Time + _TimeEditor;
                float node_5783 = frac((o.uv0+(float2(_Value1,_WaveSpeed)*node_5650.g)).g);
                float node_9681 = (pow((1.0 - node_5783),_Value2)+pow(node_5783,_Value2));
                v.vertex.xyz += (saturate(((_Albedo_var.rgb*_VertexDetailOffset)+((pow(node_9681,_Value3)*pow((1.0 - node_9681),_Value3))*_VertexWaveOffset)))*v.normal);
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex );
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 normalDirection = i.normalDir;
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
