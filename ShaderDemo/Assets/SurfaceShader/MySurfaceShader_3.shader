Shader "Custom/MySurfaceShader_3" {
	Properties {
		_MainTex("Main", 2D) = "white"{}
		_CubeMap("Cube", CUBE) = ""{}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		
		CGPROGRAM

		#pragma surface surf Model

		sampler2D _MainTex;
		samplerCUBE _CubeMap;

		struct Input {
			float2 uv_MainTex;
			float3 worldRefl;
		};

		struct Output{
			fixed3 Albedo;
			fixed Alpha;
			fixed3 Normal; 
			fixed3 Emission;
		};

		void surf (Input IN, inout Output o) {
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) ;
			o.Albedo = c.rgb;

//			o.Emission = texCUBE(_CubeMap, IN.worldRefl).rgb;
			o.Albedo = texCUBE(_CubeMap, IN.worldRefl).rgb;
		}

		inline float4 LightingModel(Output s, fixed3 lightDir, fixed3 viewDir, float atten) {

			return float4(s.Albedo, 1);
		}

		ENDCG
	}
}
