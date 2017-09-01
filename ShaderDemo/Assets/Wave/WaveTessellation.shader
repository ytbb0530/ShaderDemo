Shader "Custom/MyTessellation" {
 Properties {
            _Tess ("Tessellation", Range(1,32)) = 4
            _MainTex ("Base (RGB)", 2D) = "white" {}
            _DispTex ("Disp Texture", 2D) = "gray" {}
            _NormalMap ("Normalmap", 2D) = "bump" {}
            _Displacement ("Displacement", Range(0, 1.0)) = 0.3
            _Color ("Color", color) = (1,1,1,0)
            _SpecColor ("Spec color", color) = (0.5,0.5,0.5,0.5)
        }
        SubShader {
            Tags { "RenderType"="Opaque" }
            LOD 300
            
            CGPROGRAM
            #pragma surface surf BlinnPhong addshadow fullforwardshadows vertex:disp tessellate:tessFixed nolightmap
            #pragma target 5.0

            struct appdata {
                float4 vertex : POSITION;
                float4 tangent : TANGENT;
                float3 normal : NORMAL;
                float2 texcoord : TEXCOORD0;
            };

            float _Tess;

            float4 tessFixed()
            {
                return _Tess;
            }

            sampler2D _WaveParamTex;

            float4 _PosX;
            float4 _PosY;
            float4 _Scale;

            sampler2D _DispTex;
            float _Displacement;

            void disp (inout appdata v)
            {
            	for(int i = 0; i < 4; i++)
            	{
            		float x;
            		float y;
            		float s;

            		if(i == 0){
            			x = _PosX.r;
            			y = _PosY.r;
            			s = _Scale.r;
            		}else if(i == 1){
            			x = _PosX.g;
            			y = _PosY.g;
            			s = _Scale.g;
            		}else if(i == 2){
            			x = _PosX.b;
            			y = _PosY.b;
            			s = _Scale.b;
            		}else if(i == 3){
            			x = _PosX.a;
            			y = _PosY.a;
            			s = _Scale.a;
            		}

            		if(s < 0) continue;

	            	float2 uv = v.texcoord.xy - float2(0.5, 0.5);

	            	uv += float2(x, y);

	            	uv /= s;

	            	uv += float2(0.5, 0.5);

	                float d = tex2Dlod(_DispTex, float4(uv,0,0)).r * _Displacement;

	                v.vertex.xyz += v.normal * d;
            	}

            }

            struct Input {
                float2 uv_MainTex;
            };

            sampler2D _MainTex;
            sampler2D _NormalMap;
            fixed4 _Color;

            void surf (Input IN, inout SurfaceOutput o) {
                half4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
                o.Albedo = c.rgb;
                o.Specular = 0.2;
                o.Gloss = 1.0;
                o.Normal = UnpackNormal(tex2D(_NormalMap, IN.uv_MainTex));
            }
            ENDCG
        }
//        FallBack "Diffuse"
    }