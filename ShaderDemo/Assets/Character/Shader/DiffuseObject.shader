// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Character/DiffuseObject"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_DiffuseColor("Diffuse", color) = (1, 1, 1, 1)
	}
	SubShader
	{
		Pass
		{
			Tags { "RenderType"="Opaque" "LightMode"="ForwardBase" }

			CGPROGRAM

			#pragma multi_compile_fwdbase

			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "AutoLight.cginc"

			struct a2v
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float3 worldNormal : TEXCOORD1;
				SHADOW_COORDS(2)
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _DiffuseColor;
			
			v2f vert (a2v v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.worldNormal = UnityObjectToWorldNormal(v.normal);
				TRANSFER_SHADOW(o);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed3 col = tex2D(_MainTex, i.uv).rgb;
				fixed shadow = SHADOW_ATTENUATION(i);

				fixed3 halfLambertWhite = dot(i.worldNormal, float3(0, 0, 1)) * 0.3 + 1;
				fixed3 diffuseWhite = halfLambertWhite * float4(1, 1, 1, 1);

				fixed3 halfLambertColor = dot(i.worldNormal, float3(0, 0, 1)) * 0.5 + 0.5;
				fixed3 diffuseColor = halfLambertColor * _DiffuseColor;

				col = col * lerp(diffuseWhite, diffuseColor, 0.3);
				
				col = col * shadow;

				return fixed4(col, 1.0);
			}
			ENDCG
		}
	}
	Fallback "VertexLit"
}
