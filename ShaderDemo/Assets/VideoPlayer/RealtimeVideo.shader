Shader "Unlit/RealtimeVideo"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Font0 ("font_0", 2D) = "white" {}
		_Font1 ("font_1", 2D) = "white" {}
		_Font2 ("font_2", 2D) = "white" {}
		_Font3 ("font_3", 2D) = "white" {}
		_Font4 ("font_4", 2D) = "white" {}
		_Font5 ("font_5", 2D) = "white" {}
		_Font6 ("font_6", 2D) = "white" {}
		_Font7 ("font_7", 2D) = "white" {}
		_Colorful("colorful", int) = 0
	}
	SubShader
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct a2v
			{
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				float2 uv1 : TEXCOORD0;
				float2 uv2 : TEXCOORD1;
			};

			struct v2f
			{
				float4 uv : TEXCOORD0;
				int type : TEXCOORD1;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _Font0;
			float4 _Font0_ST;
			sampler2D _Font1;
			float4 _Font1_ST;
			sampler2D _Font2;
			float4 _Font2_ST;
			sampler2D _Font3;
			float4 _Font3_ST;
			sampler2D _Font4;
			float4 _Font4_ST;
			sampler2D _Font5;
			float4 _Font5_ST;
			sampler2D _Font6;
			float4 _Font6_ST;
			sampler2D _Font7;
			float4 _Font7_ST;
			int _Colorful;
			
			v2f vert (a2v v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv.xy = TRANSFORM_TEX(v.uv1, _MainTex);
				o.uv.zw = TRANSFORM_TEX(v.uv2, _Font1);
				if(v.color.r > 0.9){
					o.type = 1;
				}else{
					o.type = 0;
				}
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				if(i.type == 1){
					return tex2D(_MainTex, i.uv.zw);
				}

				fixed4 col = tex2D(_MainTex, i.uv.xy);

				fixed4 fontCol = fixed4(0, 0, 0, 0);
				float lum = 1 - Luminance(col);
				if (_Colorful == 1){
					lum = 1 - lum;
				}
				if(lum < 0.125){
					fontCol = tex2D(_Font0, i.uv.zw);
				}else if(lum < 0.25){
					fontCol = tex2D(_Font1, i.uv.zw);
				}else if(lum < 0.375){
					fontCol = tex2D(_Font2, i.uv.zw);
				}else if(lum < 0.5){
					fontCol = tex2D(_Font3, i.uv.zw);
				}else if(lum < 0.625){
					fontCol = tex2D(_Font4, i.uv.zw);
				}else if(lum < 0.75){
					fontCol = tex2D(_Font5, i.uv.zw);
				}else if(lum < 0.875){
					fontCol = tex2D(_Font6, i.uv.zw);
				}else{
					fontCol = tex2D(_Font7, i.uv.zw);
				}

				if (_Colorful == 1){

					fontCol = col - (fontCol / 0.545).r * col + fixed4(0.545, 0.545, 0.545, 0);
				}
				return fontCol;
			}
			ENDCG
		}
	}
}
