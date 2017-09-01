Shader "Custom/MySurfaceShader_2" {
	Properties {
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_MaskMap("Mask", 2D) = "white"{}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Spec 

		#include "Lighting.cginc"

		sampler2D _MainTex;
		sampler2D _MaskMap;

		struct Input {
			float2 uv_MainTex;
			float2 uv_MaskMap;
		};
		struct Output{
			fixed3 Albedo;
			fixed Alpha;
			fixed3 Normal;
			fixed3 Emission;
			float Mask;
		};

		void surf (Input IN, inout Output o) {
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb;
			o.Alpha = c.a;

			o.Mask = tex2D(_MaskMap, IN.uv_MaskMap).r;
		}

		inline float4 LightingSpec(Output s, fixed3 lightDir, half3 viewDir, float atten)
		{
			lightDir = normalize( fixed3(1, 0, 0));

			fixed diff = dot(s.Normal, lightDir) + 1;

			float sPower = (s.Mask * 10) + 6;
			float spec = 0;

			if(s.Mask < 0.5)
			{
				spec = pow(max(0, dot(s.Normal, normalize(viewDir + lightDir))), sPower);
			}
			float specCol = (s.Albedo + (fixed3(1, 1, 1) - s.Albedo) * 0.6) * spec * 0.8;
//			float specCol = fixed3(0.7, 0.7, 0.7) * spec;

			fixed3 col = s.Albedo * diff + specCol;

			return fixed4(col, 1);
		}

		ENDCG
	}
}
