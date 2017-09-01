Shader "Unlit/TexAnim"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_AnimTex ("Anim", 2D) = "white"{}
	}
	SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 100

		Pass{

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing

			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _AnimTex;
			float4 _AnimTex_TexelSize;

			struct a2v{
				float4 vertex : POSITION;
				uint vid : SV_VertexID;
				float4 vcol : COLOR;
				float2 uv : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			v2f vert (a2v v)
			{
				UNITY_SETUP_INSTANCE_ID(v);

				float animX = (v.vid + 0.5) * _AnimTex_TexelSize.x;
				float animY = fmod(_Time.y, 1.0);
				float4 pos= tex2Dlod(_AnimTex, float4(animX, animY, 0, 0));

				v2f o;
				o.vertex = UnityObjectToClipPos(pos);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : COLOR
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				return col;
			}
			ENDCG
		}

	}
}
