// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "SiriWave/Wave" {
	Properties {
		_Index("index", int) = 0
		_Color ("Line Color", Color) = (1, 1, 1, 1)
		_Pos("position", Float) = 0
		_Height ("Height", Float) = 0
 		_WaveLength ("Wave Length", Float) = 0
	}
	SubShader {
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}

		GrabPass{"_EnviromentTex"}

		Pass {
			Tags { "LightMode"="ForwardBase" }
			
			ZWrite Off
			//Blend SrcAlpha OneMinusSrcAlpha
			Blend SrcAlpha One
			Cull Off
			
			CGPROGRAM  
			#pragma vertex vert 
			#pragma fragment frag

			#include "UnityCG.cginc"

			int _Index;
			fixed4 _Color;
			float _Height;
			float _Pos;
			float _WaveLength;
			sampler2D _EnviromentTex;
			float4 _EnviromentTex_ST;
			
			struct a2v {
				float4 vertex : POSITION;
				float4 texcoord : TEXCOORD0;
			};
			
			struct v2f {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
			};
			
			v2f vert(a2v v) {
				v2f o;
				
				float4 offset = float4(0, 0, 0, 0);
				if(v.vertex.x > _Pos - _WaveLength * 1.57f && v.vertex.x < _Pos + _WaveLength * 1.57f) {
					offset.y = cos((v.vertex.x - _Pos) / _WaveLength) * _Height;
				}
				if(v.vertex.y > 0){
					offset.y = -offset.y;
				}
				o.pos = UnityObjectToClipPos(v.vertex + offset);
				
				o.uv = TRANSFORM_TEX(v.texcoord, _EnviromentTex);

				return o;
			}
			
			fixed4 frag(v2f i) : SV_Target {
				
				fixed4 col = tex2D(_EnviromentTex, i.uv);
				fixed3 c = _Color.rgb + col.rgb ;//+ fixed3(.5f, .5f, .5f);
				float alpha = .3f;
				return fixed4(c, alpha);
			} 
			
			ENDCG
		}
	}
//	FallBack "Transparent/VertexLit"
}

