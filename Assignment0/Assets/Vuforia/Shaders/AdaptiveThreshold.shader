Shader "Unlit/AdaptiveThreshold" {
Properties{
        _ContourColor("Contour Color", Color) = (1,1,1,1) // Define color of contours
        _SurfaceColor("Surface Color", Color) = (0.5,0.5,0.5,1) // Define color of surfaces (not contours)
        _DepthThreshold("Depth Threshold", Float) = 0.002 // Define color of contours

        _Factor1 ("Factor 1", float) = 1 // Factors for noise, used in pseudorandom noise generation
        _Factor2 ("Factor 2", float) = 1
        _Factor3 ("Factor 3", float) = 1
        _Random("Random", float) = 0.1
    }

    SubShader {
        Tags { "Queue" = "Geometry" "RenderType" = "Transparent" }

        // The pass represents an execution of the vertex.
        Pass {
            Cull Back
            Blend SrcAlpha OneMinusSrcAlpha 
            // SrcAlpha: The value of this stage is multiplied by the source alpha value.
            // OnMinusSrcAlpha: The value of this stage is multiplied by (1 - source alpha).

            //HLSL Code
            // Pragmas are compilation directives
            CGPROGRAM
            #pragma vertex vert // the vertex shader
            #pragma fragment frag // the pixel shader
            #include "UnityCG.cginc"

            // We define the variables here, which is the same as from the GUI in matlab
            uniform sampler2D _CameraDepthTexture;
            uniform float4 _ContourColor;
            uniform float4 _SurfaceColor;
            uniform float _DepthThreshold;

            float _Factor1;
            float _Factor2;
            float _Factor3;
            float _Random;

            // struct of v2f (close to class)
            // Includes, position, and coordinates for screen and depth
            // UV coordinates are telling which pixels that is being shown
            struct v2f {
                float4 pos : SV_POSITION; // Vertex position
                float4 screenPos : TEXCOORD0; // This first UV Coordinate
                float depth : TEXCOORD1; // The second UV Coordinate
            };

            v2f vert(appdata_base v) {
                // The vertex input appdata_base is position noraml and texture coordinate
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex); // BuiltIn func from object space to camera space
                o.screenPos = ComputeScreenPos(o.pos); // BuiltIn func computes texture coord 
                
                COMPUTE_EYEDEPTH(o.depth); // Computes eye space depth of vertex, not rendring into depth texture, so usefull for colors
                o.depth = (o.depth - _ProjectionParams.y) / (_ProjectionParams.z - _ProjectionParams.y); // sync with projection
                return o;
            }
 
            float noise(half2 uv, float fac1, float fac2, float fac3) {
                // Uses the three factors to generate pseudorandom noise
                // Seems the way most people do it in unity
                return frac(sin(dot(uv, float2(fac1, fac2))) * fac3);
            }

            // The pixel shader implementation
            half4 frag(v2f i) : COLOR {
            	// Define variables
                float2 uv = i.screenPos.xy / i.screenPos.w;
                // _ScreenParams is built in Unity, .x is render width :: .y is render height
                float du = 1.0 / _ScreenParams.x;
                float dv = 1.0 / _ScreenParams.y;
                float2 uv_X1 = uv + float2(du, 0.0);
                float2 uv_Y1 = uv + float2(0.0, dv);
                float2 uv_X2 = uv + float2(-du, 0.0);
                float2 uv_Y2 = uv + float2(0.0, -dv);

                // Linear01Depth = given high precision value from depth texture i, returns corresponding linear depth in range between 0 and 1.
                // Create multiple depth samples from buffer
                float depth0 = Linear01Depth(UNITY_SAMPLE_DEPTH(tex2D(_CameraDepthTexture, uv)));
                float depthX1 = Linear01Depth(UNITY_SAMPLE_DEPTH(tex2D(_CameraDepthTexture, uv_X1)));
                float depthY1 = Linear01Depth(UNITY_SAMPLE_DEPTH(tex2D(_CameraDepthTexture, uv_Y1)));
                float depthX2 = Linear01Depth(UNITY_SAMPLE_DEPTH(tex2D(_CameraDepthTexture, uv_X2)));
                float depthY2 = Linear01Depth(UNITY_SAMPLE_DEPTH(tex2D(_CameraDepthTexture, uv_Y2)));

                float farDist = _ProjectionParams.z;
                float refDepthStep = _DepthThreshold / farDist;
                float depthStepX = max(abs(depth0 - depthX1), abs(depth0 - depthX2));
                float depthStepY = max(abs(depth0 - depthY1), abs(depth0 - depthY2));
                float maxDepthStep = length(float2(depthStepX, depthStepY));
                half contour = (maxDepthStep > refDepthStep) ? 1.0 : 0.0;

                // If contour is 0, we show the surfaceColor, otherwise show contourColor.
                float4 texcol = _SurfaceColor * (1.0 - contour) + _ContourColor * contour;

                // Add noise to texColor, create noise from DeltaTime
            	fixed4 col = noise(uv, _Factor1, _Factor2, _Factor3);
                // Dot (multiply) the noise with the depth shader
                if (contour == 0) {
    				texcol = dot(dot(texcol, col), float3(1.0, 1.0, 1.0));
    			}
    			//texcol.a = 1;

        		return texcol;
            }

            ENDCG
        }
    }

    Fallback "Diffuse"
} 


