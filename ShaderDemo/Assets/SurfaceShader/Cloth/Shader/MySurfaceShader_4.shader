// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/MySurfaceShader_4" {
	Properties {
		_MainTint ("Global Tint", Color) = (1,1,1,1)  
        _BumpMap ("Normal Map", 2D) = "bump" {}  
        _DetailBump ("Detail Normal Map", 2D) = "bump" {}  
        _DetailTex ("Fabric Weave", 2D) = "white" {}  
        _FresnelColor ("Fresnel Color", Color) = (1,1,1,1)  
        _FresnelPower ("Fresnel Power", Range(0, 12)) = 3  
        _RimPower ("Rim FallOff", Range(0, 12)) = 3  
        _SpecIntesity ("Specular Intensiity", Range(0, 1)) = 0.2  
        _SpecWidth ("Specular Width", Range(0, 1)) = 0.2  
        _WetTex("Wet Map", 2D) = "black"{}    
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		
		CGPROGRAM
		#pragma surface surf Model vertex:vertexFunc

		sampler2D _BumpMap;  
        sampler2D _DetailBump;  
        sampler2D _DetailTex;  
        float4 _MainTint;  
        float4 _FresnelColor;  
        float _FresnelPower;  
        float _RimPower;  
        float _SpecIntesity;  
        float _SpecWidth;  
        sampler2D _WetTex;

		struct Input {
			float2 uv_BumpMap;  
            float2 uv_DetailBump;  
            float2 uv_DetailTex;
            float3 worldPos;
		};

		struct Output{
			fixed3 Albedo;
			fixed Alpha;
			fixed3 Emission;
			fixed3 Normal;
			fixed Gloss;
			fixed Specular;
			fixed Mask;
			float3 WorldPos;
		};

		void vertexFunc(inout appdata_full v, out Input IN) {

			UNITY_INITIALIZE_OUTPUT(Input, IN);
            IN.worldPos = mul(unity_ObjectToWorld, v.vertex);
        }

		void surf (Input IN, inout Output o) {

			half4 c = tex2D (_DetailTex, IN.uv_DetailTex); 
			fixed4 packedNormal = tex2D(_BumpMap, IN.uv_BumpMap);
            fixed3 normals = UnpackNormal(packedNormal).rgb;  
            fixed3 detailNormals = UnpackNormal(tex2D(_DetailBump, IN.uv_DetailBump)).rgb;  
            fixed3 finalNormals = float3(normals.x + detailNormals.x,   
                                        normals.y + detailNormals.y,   
                                        normals.z + detailNormals.z);  
              
            o.Normal = normalize(finalNormals);  
            o.Specular = _SpecWidth;  
            o.Gloss = _SpecIntesity;  
            o.Albedo = c.rgb * _MainTint;  
            o.Alpha = c.a;  
            o.Mask = packedNormal.r;
            o.WorldPos = IN.worldPos;
		}

		fixed4 dry(Output s, fixed3 lightDir, float3 viewDir, float atten)
		{
			//Create lighting vectors here  
            viewDir = normalize(viewDir);  
            lightDir = normalize(lightDir);  
            half3 halfVec = normalize (lightDir + viewDir);  
            fixed NdotL = max (0, dot (s.Normal, lightDir));  
              
            //Create Specular   
            float NdotH = max (0, dot (s.Normal, halfVec));  
            float spec = pow (NdotH, s.Specular*128.0) * s.Gloss;  
              
            //Create Fresnel  
            float HdotV = pow(1-max(0, dot(halfVec, viewDir)), _FresnelPower);  
            float NdotE = pow(1-max(0, dot(s.Normal, viewDir)), _RimPower);  
            float finalSpecMask = NdotE * HdotV;  
              
            //Output the final color  
            fixed4 c;  
            c.rgb = (s.Albedo * NdotL * _LightColor0.rgb)  
                     + (spec * (finalSpecMask * _FresnelColor)) * (atten * 2);  
            c.a = 1.0;  
            return c;  
		}

		fixed4 wet(Output s, fixed3 lightDir, float3 viewDir, float atten)
		{
			float diff = saturate(dot(s.Normal, lightDir));
			float3 refV = normalize(s.Normal * 2 * diff - lightDir);
			float spec = pow (max (0, dot (s.Normal, normalize (lightDir + viewDir))), s.Specular * 32.0);  
			fixed3 col = s.Albedo * diff + spec * s.Mask.r * 2;

			return float4(col, s.Alpha);
		}

		inline fixed4 LightingModel(Output s, fixed3 lightDir, float3 viewDir, float atten)
		{
			fixed4 dryColor = dry(s, lightDir, viewDir, atten);
			fixed4 wetColor = wet(s, lightDir, viewDir, atten);

			float wetPower = tex2D(_WetTex, float2(0.5, s.WorldPos.y / 1.8)).r;

			return lerp(dryColor, wetColor, wetPower);
		}

		ENDCG
	}
}
