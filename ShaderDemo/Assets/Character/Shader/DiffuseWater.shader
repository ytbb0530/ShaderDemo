Shader "Character/DiffuseWater"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_SecondTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Pass
		{
			Tags { "LightMode"="ForwardBase"}

//			ZWrite Off
//			Blend SrcAlpha OneMinusSrcAlpha
			
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "AutoLight.cginc"

			struct a2v
			{
				float4 vertex : POSITION;
				float2 uvFirst : TEXCOORD0;
				float2 uvSecond : TEXCOORD1;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float2 uvFirst : TEXCOORD0;
				float2 uvSecond : TEXCOORD1;
				SHADOW_COORDS(2)
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _SecondTex;
			float4 _SecondTex_ST;
			
			v2f vert (a2v v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);

				o.uvFirst = TRANSFORM_TEX(v.uvFirst, _MainTex);
				o.uvFirst.x += _Time.x * 1.5 + sin(_Time.x) * 0.6;

				o.uvSecond = TRANSFORM_TEX(v.uvSecond, _SecondTex);
				o.uvSecond.y -= _Time.x + sin(_Time.x * 1.2) * 0.3;

				TRANSFER_SHADOW(o);

				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed3 firstCol = tex2D(_MainTex, i.uvFirst).rgb;
				fixed3 secondCol = tex2D(_SecondTex, i.uvSecond).rgb;

				fixed3 col = lerp(firstCol, secondCol, 0.5);

				fixed shadow = SHADOW_ATTENUATION(i);

				return fixed4(col * shadow , 0.5);
			}
			ENDCG
		}
	}
	Fallback "VertexLit"
}
