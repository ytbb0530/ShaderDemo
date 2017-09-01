Shader "Custom/MySurfaceShader" {
	Properties {
		_MainTex("Main", 2D) = "white"{}
		_BumpMap("Bump", 2D) = "white"{}
		_SpacPower("Power", range(0, 30)) = 1
		_MaskMap("Mask", 2D) = "white"{}
	}
	SubShader 
	{
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf LightModel alpha//Phong

		sampler2D _MainTex;
		sampler2D _BumpMap;
		float _SpacPower;
		sampler2D _MaskMap;

		struct Input 
		{
			fixed2 uv_MainTex;
			fixed2 uv_BumpMap;
			fixed2 uv_MaskMap;
		};

		struct Output
		{
			fixed3 Albedo;
			fixed3 Normal;
			fixed3 Emission;
			fixed Alpha;
			fixed Mask;
		};

		void surf (Input IN, inout Output o) 
		{
			half4 c = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb;

			fixed3 normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
			normal = fixed3(normal.x * 2, normal.y * 2, normal.z);
			o.Normal = normal;

			o.Mask = tex2D(_MaskMap, IN.uv_MaskMap).r * 0.5 + 0.5;

			o.Alpha = 0;
		}

		inline float4 LightingLightModel (Output s, fixed3 lightDir, fixed atten)
		{
			lightDir = fixed3(1, 0, 0);
			float difLight = dot (s.Normal, lightDir) + 1;

			float3 col = s.Albedo * difLight;
			return float4(col, s.Alpha);
		}

		inline float4 LightingPhong (Output s, fixed3 lightDir, half3 viewDir, fixed atten)
		{
			lightDir = fixed3(1, 0, 0);

			float diff = dot(s.Normal, lightDir) + 1;

			float3 refV = normalize(s.Normal * 2 * diff - lightDir);

			float sPower = _SpacPower * s.Mask;

			float spec = pow(max(0, dot(refV, viewDir)), 20 / sPower);

			fixed3 col = s.Albedo * diff + spec;

			return float4(col, s.Alpha);
		}

		ENDCG
	} 
	//FallBack "Diffuse"
}
