// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "ChinesePaint/Object"
{
	Properties
	{
		_Outline("OutLine", float) = 0.1
		_FillRate("FillRate", range(0, 1)) = 1
	}
	SubShader
	{
	// 填充pass
		Pass
		{
			Cull Back

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "Lighting.cginc"

			float _FillRate;
			
			struct a2v
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float vdotn : TEXCOORD1;
        		float2 depth: TEXCOORD2;
				fixed3 color : COLOR;
			};

			v2f vert (a2v v)
			{
				v2f o;

				o.vertex = UnityObjectToClipPos(v.vertex);

				fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;

				fixed3 worldNormal = normalize(mul(v.normal, (float3x3)unity_WorldToObject));
				
				fixed3 worldLight = normalize(_WorldSpaceLightPos0.xyz);
				
				fixed3 diffuse = _LightColor0.rgb * fixed4(1, 1, 1, 1) * saturate(dot(worldNormal, worldLight));
				
				o.color = diffuse;//fixed4(0.5, 0.5, 0.5, 1);

				float3 viewDir = normalize( mul(unity_WorldToObject, float4(_WorldSpaceCameraPos.xyz, 1)).xyz - v.vertex);

        		o.vdotn = dot(normalize(viewDir),v.normal);

        		o.depth = UnityObjectToClipPos(v.vertex).zw;

				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed c = 0.2125 * i.color.r + 0.7154 * i.color.g + 0.0721 * i.color.b;

				float depth = Linear01Depth(i.depth.x/i.depth.y);

				float depthColor = depth ;//* i.vdotn;

				c = min(i.vdotn * 8, 1) - c + depthColor * 1;

				if (c < 0.2)
				{
					c = 0.2;
				}
				else if(c > 0.8)
				{
					c = 1;
				}

				c = c + _FillRate;

				if (c >= 1)
				{
					//discard;
				}

				return fixed4(c, c, c, 1);
			}
			ENDCG
		}

		// 描边pass
		Pass {
			NAME "OUTLINE"
			
			Cull Front
			
			CGPROGRAM
			
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			
			float _Outline;
			
			struct a2v {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			}; 
			
			struct v2f {
			    float4 pos : SV_POSITION;
			    fixed width : texcoord0;
			    float3 normal : texcoord1;
			    float4 vertex : texcoord2;
			};
			
			v2f vert (a2v v) {
				v2f o;
				
				float4 pos = mul(UNITY_MATRIX_MV, v.vertex); 

				float3 normal = mul((float3x3)UNITY_MATRIX_IT_MV, v.normal);

				//normal.z = -0.5;

				fixed4 fixwidth = abs(dot(v.normal, normalize(ObjSpaceViewDir(v.vertex))));

				fixed width = fixwidth.x * 0.5;

				pos = pos + float4(normalize(normal), 0) * (_Outline + width);

				o.pos = mul(UNITY_MATRIX_P, pos);

				o.width = width;

				o.normal = v.normal;

				o.vertex = v.vertex;

				return o;
			}
			
			float4 frag(v2f i) : SV_Target {

				float angle = abs(dot(i.normal, normalize(ObjSpaceViewDir(i.vertex)) ));

				if (angle > 0.2)
				{
					//discard;
				}

				fixed c = 1 - i.width * 15;

				return fixed4(c, c, c, 1);               
			}
			
			ENDCG
		}
	}
}
