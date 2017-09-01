Shader "Unlit/ColorPaint"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Cull Back

		Pass
		{

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			struct appdata
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float4 color : TEXCOORD1;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.color = v.color;
				
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				if(o.color.r >= 0.5){
					o.uv.x = 1 - o.uv.x;
				}
				if(o.color.g >= 0.5){
					o.uv.y = 1 - o.uv.y;
				}

				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				float alpha = col.r;
				if(alpha < 0.05f){
					discard;
				}
				return fixed4(i.color.rgb, 1);
			}
			ENDCG
		}
	}
}
