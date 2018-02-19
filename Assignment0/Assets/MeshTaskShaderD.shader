Shader "AR/ScreenSpaceTexture"
{
	//Exposed properties (similar to public members in MonoBehaviour scripts)
	Properties{
		_Texture("Texture", 2D) = "white" {}
		_TextureScale("Texture Scale", Range(0,10)) = 1
	}
	Subshader {
		
		//https://docs.unity3d.com/462/Documentation/Manual/SL-SubshaderTags.html
		Tags {"Queue"="Geometry"}

		Pass {

			//https://docs.unity3d.com/Manual/SL-Blend.html
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			//SHADER STARTS HERE

			#pragma vertex vertexShader
			#pragma fragment fragmentShader

			//Define input for each vertex. Received from the application / mesh data
			struct VertexIn {
				float4 position : POSITION;
			};

			//Define output of vertex, which is input for each fragment
			struct VertexOutFragmentIn {
				float4 position: SV_POSITION;
				float4 normDeviceCoords : TEXCOORD0;
			};

			sampler2D _Texture;
			float _TextureScale;
			
			//VERTEX SHADER
			VertexOutFragmentIn vertexShader(VertexIn IN) {
				VertexOutFragmentIn OUT;
				OUT.position = UnityObjectToClipPos(IN.position);
				OUT.normDeviceCoords = UnityObjectToClipPos(IN.position); //Alternatively, you can try the built-in ComputeScreenPos function
				return OUT;
			}

			//FRAGMENT SHADER
			fixed4 fragmentShader(VertexOutFragmentIn IN) : SV_Target{
				//Homogeneous coordinates, needs to be normalized by dividing by w
				float2 uv = float2(IN.normDeviceCoords.x,IN.normDeviceCoords.y) / IN.normDeviceCoords.w;
				//Map from normalized device to uv coordinate system
				uv = float2(
					uv.x * 0.5 + 0.5, 
					-uv.y * 0.5 + 0.5);
				//Get color from texture
				return tex2D(_Texture, uv * _TextureScale);
			}

			ENDCG
		}

	}

}
