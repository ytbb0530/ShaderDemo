// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/SoilShader"
{
	Properties
	{
		_MainColor("Color", color) = (1,1,1,1)
		_HoleTex ("HoleTex", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct a2v
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 pos : SV_POSITION;
				float2 vertex : TEXCOORD1;
			};

			fixed4 _MainColor;
			sampler2D _HoleTex;
			float4 _HoleTex_ST;

			v2f vert (a2v v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _HoleTex);
				o.vertex = v.vertex.xy;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 hole = tex2D(_HoleTex, i.uv);

				if(hole.r == 1)
				{
					discard;
				}

				return _MainColor;
			}
			ENDCG
		}
	}
}
