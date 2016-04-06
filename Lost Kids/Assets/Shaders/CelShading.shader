Shader "Custom/CelShading" 
{
    Properties 
    {

		_MainTex ("Detail", 2D) = "white" {}
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
            Name "CS"
            CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma fragmentoption ARB_precision_hint_fastest
                #include "UnityCG.cginc"
                #pragma glsl_no_auto_normalization

                
                sampler2D _MainTex;
				half4 _MainTex_ST;
				
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
  
                    return o;
                }


                
                fixed4 frag (v2f i) : COLOR
                {

					fixed4 detail = tex2D ( _MainTex, i.uv );
					return  detail;

                }
            ENDCG
        }

		
    }
    Fallback "Legacy Shaders/Diffuse"
}