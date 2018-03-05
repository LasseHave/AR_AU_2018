 Shader "Unlit/GreyScale" {
	Properties {
		_MainTex ("Texture", 2D) = "white" { } // The texture for the noise
		_NoiseTex ("Noise Texture", 2D) = "white" { } // The texture for the noise

		_NoiseIntensity ("_NoiseIntensity", Vector) = (1, 1, 1, 1) 
		_NoiseSample ("_NoiseSample", Vector) = (1, 1, 1, 1)
		_NoiseSampleSize ("_NoiseSampleSize", Vector) = (1, 1, 1, 1)
	}
	SubShader {
		Pass {
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			// main texture for the noise
			sampler2D _MainTex;
			sampler2D _NoiseTex;

			float2 _NoiseIntensity;
			float2 _NoiseSampleSize;
			float2 _NoiseSample;

			float4 _MainTex_ST;

			struct v2f {
				float4  pos : SV_POSITION; // Vertex position
				float2  uv : TEXCOORD0; // The first uv coordinate
			};

			struct Input {
				float3 worldPos;
			};

      		v2f vert (appdata_base v) {
        		v2f o;
				// Convert to camera position
        		o.pos = UnityObjectToClipPos (v.vertex);
        		o.uv = TRANSFORM_TEX (v.texcoord, _MainTex);
				return o;
      		}

			half4 frag (v2f i) : COLOR {
        		half4 texcol = tex2D (_MainTex, i.uv);
				// Multiply the noise with the texture
        		texcol.rgb = dot(texcol.rgb, float3(0.3, 0.59, 0.11));


        		half4 noiseSample = tex2D(_NoiseTex, (i.uv + (_NoiseSample) ) * (_NoiseSampleSize) );
        		texcol.rgb += noiseSample.rgb * _NoiseIntensity.x;

        		return texcol;
      		}

			ENDCG  
		}
	}
	Fallback "VertexLit"
} 


