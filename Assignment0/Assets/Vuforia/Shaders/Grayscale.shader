// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

 Shader "Unlit/GreyScale" {
	Properties {
		_MainTex ("Texture", 2D) = "white" { }
		_Res("Noise Resolution",Float)=128
	}
	SubShader {
			CGPROGRAM
			// #pragma vertex vert
			// #pragma fragment frag
			#pragma surface surf Lambert

          	float4 _Color;
         	float _Res;
			
			#include "UnityCG.cginc"
			
			sampler2D _MainTex;
			
			struct v2f {
				float4  pos : SV_POSITION;
				float2  uv : TEXCOORD0;
			};

			struct Input {
            	float2 uv: TEXCOORD;
            	float3 worldPos;
         	};
			
			float4 _MainTex_ST;

			float rand(float3 myVector)  {
            	return frac(sin( dot(myVector ,float3(12.9898,78.233,45.5432) )) * 43758.5453);
         	}
			
			v2f vert (appdata_base v) {
				v2f o;
				o.pos = UnityObjectToClipPos (v.vertex);
				o.uv = TRANSFORM_TEX (v.texcoord, _MainTex);
				return o;
			}
			
			half4 frag (v2f i) : COLOR {
				half4 texcol = tex2D (_MainTex, i.uv);
				// texcol.rgb = dot(texcol.rgb,noisecol.rgb);
				return texcol;
			}

			void surf (Input IN, inout SurfaceOutput o) {
				float3 vWPos=IN.worldPos;
				//float3 vTimeOffset=float3(1,1,1)*_Time[0];
				//vWPos+=vTimeOffset;
				vWPos*=_Res;
				
				float Rand1=rand(round(vWPos));
				vWPos+=float3(.5,.5,.5);
				
				Rand1+=rand(round(vWPos));
				vWPos-=float3(1,1,1);
				//Rand1+=rand(round(vWPos));
				Rand1/=2;
				o.Albedo =float3(Rand1,Rand1,Rand1)*_Color.xyz;
				o.Alpha = 1;
         	}

			ENDCG	
	}
	Fallback "VertexLit"
 } 