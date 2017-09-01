// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "River/River"
{
	Properties
	{
		_MirroTex ("Mirro Tex", 2D) = "white"{}
		_OffsetTex ("Offset Tex", 2D) = "bump" {}
		_RiverColor("Color", Color) = (1,1,1,1)
	}
	SubShader
	{
		Tags { "Queue"="Transparent" "RenderType"="Transparent" }

		GrabPass{ "_EnviromentTex" }

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "AutoLight.cginc"

			sampler2D _MirroTex;
			float4 _MirroTex_TexelSize;
			sampler2D _EnviromentTex;
			float4 _EnviromentTex_TexelSize;
			sampler2D _OffsetTex;
			float4 _OffsetTex_ST;
			fixed4 _RiverColor;

			struct a2v
			{
				float4 vertex : POSITION;
				float2 texcoord: TEXCOORD0;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float4 scrPos : TEXCOORD0;
				float2 uv : TEXCOORD1;
				fixed3 worldNormal : TEXCOORD2;
  				fixed3 worldViewDir : TEXCOORD3;
  				float3 worldPos : TEXCOORD4;
			};

			v2f vert (a2v v)
			{
				v2f o;

				o.pos = UnityObjectToClipPos(v.vertex);

				o.scrPos = ComputeGrabScreenPos(o.pos);

				o.uv = TRANSFORM_TEX(v.texcoord, _OffsetTex);

				o.uv.y += _Time.x;

				o.worldNormal = UnityObjectToWorldNormal(v.normal);

				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

				o.worldViewDir = UnityWorldSpaceViewDir(o.worldPos);

				return o;
			}

			float2 getPos(v2f i, fixed fresnel)
			{
				fixed3 bump = tex2D(_OffsetTex, i.uv).rgb;	
				
				float2 offset = bump.xy;

				i.scrPos.xy += lerp(offset, float2(0, 0), fresnel * 0.25);

				float2 pos = i.scrPos.xy / i.scrPos.w;

				return pos;
			}

			fixed getFresnel(v2f i)
			{
				float _FresnelScale = 0.2;

				fixed3 worldNormal = normalize(i.worldNormal);

				fixed3 worldViewDir = normalize(i.worldViewDir);

				fixed fresnel = _FresnelScale + (1 - _FresnelScale) * pow(1 - dot(worldViewDir, worldNormal), 0.5);

				return saturate(fresnel);
			}

			fixed4 frag (v2f i) : SV_Target
			{
				fixed fresnel = getFresnel(i);

				float2 pos = getPos(i, fresnel);

				float2 mirrorPos = pos;

				#if UNITY_UV_STARTS_AT_TOP
					mirrorPos.y = 1 - mirrorPos.y;
				#endif

				fixed3 mirroCol = tex2D(_MirroTex, mirrorPos).rgb;

				fixed3 evnCol = tex2D(_EnviromentTex, pos).rgb;

				fixed3 final = lerp(evnCol, mirroCol, fresnel);

				return fixed4(final * _RiverColor.rgb, 1);
			}
			ENDCG
		}
	}
}
