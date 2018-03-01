// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

 Shader "Unlit/Noise" {
	Properties {
		_MainTex ("Noise Texture", 2D) = "white" { } // The texture for the noise

		_NoiseIntensity ("_NoiseIntensity", Vector) = (1, 1, 1, 1) 
		_NoiseSample ("_NoiseSample", Vector) = (1, 1, 1, 1)
	}
	SubShader {
		Pass {
			CGPROGRAM
			
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			// main texture for the noise
			sampler2D _MainTex;
			sampler2D _NOISETex;

			uniform float4 _Intensity; // x=grain, y=scratch

			struct v2f {
				float4  pos : SV_POSITION; // Vertex position
				float2  uv  : TEXCOORD0; // The first uv coordinate
			};

			Vector _NoiseIntensity;
			Vector _NoiseSample;
			float4 _MainTex_ST;

      		v2f vert (appdata_base v) {
        		v2f o;
				// Convert to camera position
        		o.pos = UnityObjectToClipPos (v.vertex);
        		o.uv = TRANSFORM_TEX (v.texcoord, _MainTex);
				return o;
      		}

			half4 frag (v2f i) : COLOR {
				half4 texCol = tex2D(_MainTex, _NoiseSample * _NoiseIntensity );

        		texCol.r = 0;
        		texCol.g = 0;
        		texCol.b = 0;

        		return half4(0, 0, 0, 0);
      		}

			ENDCG  
		}
	}
	Fallback "VertexLit"
} 


