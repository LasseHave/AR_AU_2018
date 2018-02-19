Shader "Unlit/MeshTaskShaderA"
{
	Properties
	{
		//_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
	/*	Tags { "RenderType"="Opaque" }
		LOD 100
		*/
		Pass
		{
			CGPROGRAM
			// Shader start
			#pragma vertex vertexShader
			#pragma fragment fragmentShader
			
			//Define input for each vertex. Received from the application / mesh data
			struct VertexIn {
				float4 position : POSITION;
				fixed4 color : COLOR; // Add color from mesh
			};

			//Define output of vertex, which is input for each fragment
			struct VertexOutFragmentIn {
				float4 position: SV_POSITION;
				fixed4 color : COLOR; // Add color
			};

	
		//	sampler2D _MainTex;
	//		float4 _MainTex_ST;
			//VERTEX SHADER
			VertexOutFragmentIn vertexShader(VertexIn IN) {
				VertexOutFragmentIn OUT;
				OUT.position = UnityObjectToClipPos(IN.position);
				OUT.color = IN.color; // Forward colors
				return OUT;
			}

					//FRAGMENT SHADER
			fixed4 fragmentShader(VertexOutFragmentIn IN) : SV_Target{
					//Get color from texture
				return IN.color;
			}

			ENDCG
		}
	}
}
