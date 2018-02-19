Shader "Unlit/SpaceShipRenderv2"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_HeightMap ("Height Map", 2D) = "white" {}
        _HeightMapScale ("Height", Float) = 1
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION; // Get position from mesh
				float2 uv : TEXCOORD0; // Get texture coordinates from mesh
				fixed4 color : COLOR; // Add color from mesh
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
				fixed4 color : COLOR; // Add color
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _HeightMap;
			half _HeightMapScale;
			
			v2f vert (appdata v) 
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex); // Transform from local to normalized coordinates
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				v.vertex.y += tex2Dlod(_MainTex, float4(v.uv.xy, 0.0, 0.0)) * _HeightMapScale;
				UNITY_TRANSFER_FOG(o,o.vertex);
				o.color = v.color; // Set color
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target  // Fragment Shader
			{
			/*
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				*/
				return i.color; // DO nothing but return color instead of col
			}
			ENDCG
		}
	}
}
