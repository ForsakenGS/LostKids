Shader "Custom/CelShadingAlpha" 
{
    Properties 
    {
		_MainTex ("Detail", 2D) = "white" {} 
		_AlphaTex("Alpha Texture", 2D) = "black"{}
		_Cutoff("Alpha Cutoff", Range(0, 1)) = 0.5
    }
   
    Subshader 
    {
    	Tags { "RenderType"="Opaque" }
		LOD 250
    	ZWrite On
	   	Cull Off
		Lighting Off
		Fog { Mode Off }
		
        Pass 
        {
            Name "CSA"
            CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma fragmentoption ARB_precision_hint_fastest
                #include "UnityCG.cginc"
                #pragma glsl_no_auto_normalization

                sampler2D _MainTex;
				half4 _MainTex_ST;

				sampler2D _AlphaTex;
				half4 _AlphaTex_ST;
				float _Cutoff;
				
                struct appdata_base0 
				{
					float4 vertex : POSITION;
					float3 normal : NORMAL;
					float4 texcoord : TEXCOORD0;
				};
				
                 struct v2f 
                 {
                    float4 pos : SV_POSITION;
                    half2 uv : TEXCOORD0;
                    half2 uvn : TEXCOORD1;
					half2 auv : TEXCOORD2;
                 };
               
                v2f vert (appdata_base0 v)
                {
                    v2f o;
                    o.pos = mul ( UNITY_MATRIX_MVP, v.vertex );
                    float3 n = mul((float3x3)UNITY_MATRIX_IT_MV, normalize(v.normal));
					normalize(n);
                    n = n * float3(0.5,0.5,0.5) + float3(0.5,0.5,0.5);
                    o.uvn = n.xy;
                    o.uv = TRANSFORM_TEX ( v.texcoord, _MainTex );

					o.auv = TRANSFORM_TEX(v.texcoord, _AlphaTex);
                    return o;
                }

                
                fixed4 frag (v2f i) : COLOR
                {

					float4 textureColor = tex2D(_AlphaTex, i.auv);
					if (textureColor.b < _Cutoff && textureColor.r < _Cutoff && textureColor.g < _Cutoff){
						discard;
					}
					
					fixed4 detail = tex2D ( _MainTex, i.uv );
					return  detail;
		
                }
            ENDCG
        }

    }
    Fallback "Legacy Shaders/Diffuse"
}