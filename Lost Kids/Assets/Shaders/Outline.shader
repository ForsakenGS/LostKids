Shader "Custom/Outline"
{
    Properties 
    {

		[MaterialToggle(_TEX_ON)] _DetailTex ("Enable Detail texture", Float) = 0 	//1
		_MainTex ("Detail", 2D) = "white" {}        								//2
		_ToonShade ("Shade", 2D) = "white" {}  										//3
		[MaterialToggle(_COLOR_ON)] _TintColor ("Enable Color Tint", Float) = 0 	//4
		_Color ("Base Color", Color) = (1,1,1,1)									//5	
		[MaterialToggle(_VCOLOR_ON)] _VertexColor ("Enable Vertex Color", Float) = 0//6        
		_Brightness ("Brightness 1 = neutral", Float) = 1.0							//7	
		_OutlineColor ("Outline Color", Color) = (0.0,0.0,0.0,1.0)					//10
		_Outline ("Outline width", Float) = 3.0										//11

    }
 
    SubShader
    {
		Name "OTL"
        Tags { "RenderType"="Opaque" }
		LOD 250 
        Lighting Off
        Fog { Mode Off }        
        
		UsePass "Custom/CelShading/CS"

        Pass
        {
            Cull Front
            ZWrite On
            CGPROGRAM
			#include "UnityCG.cginc"
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma glsl_no_auto_normalization
            #pragma vertex vert
 			#pragma fragment frag
			
            struct appdata_t 
            {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};

			struct v2f 
			{
				float4 pos : SV_POSITION;
			};

            fixed _Outline;

            
            v2f vert (appdata_t v) 
            {
                v2f o;
			    o.pos = v.vertex;
			    o.pos.xyz += normalize(v.normal.xyz) *_Outline*0.01;
				o.pos = mul(UNITY_MATRIX_MVP, o.pos);
				/*
				float4 zero = { 0.0, 0.0, 0.0, 1.0 };
				float4 center = mul(UNITY_MATRIX_MVP, zero);
			    o.pos = mul(UNITY_MATRIX_MVP, o.pos);
				float2 diff = (o.pos.xy - center.xy);
				float2 offset = normalize(diff)*0.01;
				o.pos.xy += offset;
				o.pos.z += 0.0001;
				*/
			    return o;
            }
            
            fixed4 _OutlineColor;
            
            fixed4 frag(v2f i) :COLOR 
			{
		    	return _OutlineColor;
			}
            
            ENDCG
        }
    }
Fallback "Legacy Shaders/Diffuse"
}