Shader "Unlit/AdaptiveThreshold" {
Properties{
        _ContourColor("Contour Color", Color) = (1,1,1,1)
        _SurfaceColor("Surface Color", Color) = (0.5,0.5,0.5,1)
        _DepthThreshold("Depth Threshold", Float) = 0.002

        _Factor1 ("Factor 1", float) = 1
        _Factor2 ("Factor 2", float) = 1
        _Factor3 ("Factor 3", float) = 1
        _Random("Random", float) = 0.1
    }

    SubShader {
        Tags { "Queue" = "Geometry" "RenderType" = "Transparent" }

        Pass {
            Cull Back
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            uniform sampler2D _CameraDepthTexture;
            uniform float4 _ContourColor;
            uniform float4 _SurfaceColor;
            uniform float _DepthThreshold;

            float _Factor1;
            float _Factor2;
            float _Factor3;
            float _Random;

            struct v2f {
                float4 pos : SV_POSITION;
                float4 screenPos : TEXCOORD0;
                float depth : TEXCOORD1;
            };

            v2f vert(appdata_base v) 
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.screenPos = ComputeScreenPos(o.pos);
                
                COMPUTE_EYEDEPTH(o.depth);
                o.depth = (o.depth - _ProjectionParams.y) / (_ProjectionParams.z - _ProjectionParams.y);
                return o;
            }
 
            float noise(half2 uv, float fac1, float fac2, float fac3)
            {
                return frac(sin(dot(uv, float2(fac1, fac2))) * fac3);
            }

            half4 frag(v2f i) : COLOR 
            {
                float2 uv = i.screenPos.xy / i.screenPos.w;
                float du = 1.0 / _ScreenParams.x;
                float dv = 1.0 / _ScreenParams.y;
                float2 uv_X1 = uv + float2(du, 0.0);
                float2 uv_Y1 = uv + float2(0.0, dv);
                float2 uv_X2 = uv + float2(-du, 0.0);
                float2 uv_Y2 = uv + float2(0.0, -dv);

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

                float4 texcol = _SurfaceColor * (1.0 - contour) + _ContourColor * contour;


                float sample = noise(float2(unity_DeltaTime.x, unity_DeltaTime.y), 1, 1, 1);

                //if (sample > _Random) {
                	fixed4 col = noise(uv, _Factor1, _Factor2, _Factor3);
        			texcol = dot(dot(texcol, col.rgb), float3(1.0, 1.0, 1.0));
        		//}

        		return texcol;
            }

            ENDCG
        }
    }

    Fallback "Diffuse"
} 


