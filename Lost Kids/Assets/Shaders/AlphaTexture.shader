Shader "Custom/AlphaTexture"
{
Properties
{
	_MainTex("Texture", 2D) = "white"{}
	_AlphaTex("Alpha Texture", 2D) = "black"{}
	_Cutoff("Alpha Cutoff", Range(0, 1)) = 0.5
}
	SubShader
	{
		Tags {"RenderType" = "Opaque" "Queue" = "AlphaTest"}
		CGPROGRAM
		#pragma surface surf BlinnPhong alphatest:_Cutoff
		#pragma exclude_renderers flash
		
		sampler2D _MainTex;
		sampler2D _AlphaTex;
		
		struct Input
		{
			float2 uv_MainTex;
			float2 uv_AlphaTex;
		};
	
		void surf(Input IN, inout SurfaceOutput o)
		{

			fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
			fixed4 aTex = tex2D(_AlphaTex, IN.uv_AlphaTex);
			
			o.Albedo = tex.rgb;

			o.Alpha = aTex.a;
		}
		ENDCG
	}
	Fallback "Diffuse"
}