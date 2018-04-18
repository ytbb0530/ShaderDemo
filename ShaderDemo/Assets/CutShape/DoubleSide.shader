Shader "Unlit/DoubleSide"
{
	Properties
	{
		_FrontColor ("Front", color) = (0, 0, 1, 1)
		_BackColor ("Back", color) = (1, 1, 0, 1)
	}
	SubShader
	{
		Pass
		{
			Cull Front

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct a2v
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
			};

			float4 _BackColor;
			
			v2f vert (a2v v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				return _BackColor;
			}
			ENDCG
		}

		Pass
		{
			Cull Back

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct a2v
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
			};

			float4 _FrontColor;
			
			v2f vert (a2v v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				return _FrontColor;
			}
			ENDCG
		}
	}
}
