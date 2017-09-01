Shader "Unlit/KnotVertex"
{
	Properties
	{
		_Diffuse ("Diffuse", Color) = (1, 1, 1, 1)
		_HeightLight1("Height Light 1", Color) = (1, 1, 1, 1)
		_HeightLight2("Height Light 2", Color) = (1, 1, 1, 1)
		_Alpha("Alpha", range(0, 1)) = 1
	}
	SubShader
	{
		Pass
		{
			Tags { "LightMode"="ForwardBase" }
			Blend SrcAlpha OneMinusSrcAlpha
		
			CGPROGRAM
			
			#pragma vertex vert
			#pragma fragment frag
			
			#include "Lighting.cginc"
			
			fixed4 _Diffuse;
			fixed4 _HeightLight1;
			fixed4 _HeightLight2;
			float _Wide;
			float3 _Wave[500];//x-pos, y-height, z-length
			int _Wave_Num;
			float _Alpha;

			struct a2v {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float2 uv : TEXCOORD0;
			};
			
			struct v2f {
				float4 pos : SV_POSITION;
				float3 worldNormal : TEXCOORD0;
				float4 color : TEXCOORD1;
			};
			
			v2f vert(a2v v) {
				float4 color = _Diffuse;
				float pos = v.uv.x;
				float offset = 0;
				for(int j = 0; j < _Wave_Num; j++){
					float3 waveData = _Wave[j];
					if(pos > waveData.x - waveData.z * 1.57f && pos < waveData.x + waveData.z * 1.57f) {
						float curOffset = cos((pos - waveData.x) / waveData.z) * waveData.y;
						offset += curOffset;
					}
				}
				fixed lerpRate = (offset + _Wide) * 8;
				if(lerpRate < 1){
					color = lerp(_Diffuse, _HeightLight1, lerpRate);
				}else{
					color = lerp(_HeightLight1, _HeightLight2, saturate(lerpRate - 1));
				}

				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex + offset * v.normal + _Wide * v.normal);
				o.worldNormal = UnityObjectToWorldNormal(v.normal);
				o.color = color;

				return o;
			}
			
			fixed4 frag(v2f i) : SV_Target {
				fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;
				
				fixed3 worldNormal = normalize(i.worldNormal);
				fixed3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz);
				
				fixed halfLambert = dot(worldNormal, worldLightDir) * 0.5 + 0.5;
				fixed3 diffuse = _LightColor0.rgb * i.color.rgb * halfLambert;
				
				fixed3 color = ambient + diffuse;
				
				return fixed4(color, _Alpha);
			}
			
			ENDCG
		}
	}
//	FallBack "Specular"
}
