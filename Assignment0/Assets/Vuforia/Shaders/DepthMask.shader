// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

//===============================================================================
//Copyright (c) 2015 PTC Inc. All Rights Reserved.
//
//Confidential and Proprietary - Protected under copyright and other laws.
//Vuforia is a trademark of PTC Inc., registered in the United States and other
//countries.
//===============================================================================
//===============================================================================
//Copyright (c) 2010-2014 Qualcomm Connected Experiences, Inc.
//All Rights Reserved.
//Confidential and Proprietary - Qualcomm Connected Experiences, Inc.
//===============================================================================

Shader "DepthMask" {

	Properties {
		_MainTex ("Mask", 2D) = "white" {}
	}
   
    SubShader {
        // Render the mask after regular geometry, but before masked geometry and
        // transparent things.
       
        Tags {"Queue" = "Geometry-10" }
       
        // Turn off lighting, because it's expensive and the thing is supposed to be
        // invisible anyway.
       
        Lighting Off

        // Draw into the depth buffer in the usual way.  This is probably the default,
        // but it doesn't hurt to be explicit.

        ZTest LEqual
        ZWrite On

        // Don't draw anything into the RGBA channels. This is an undocumented
        // argument to ColorMask which lets us avoid writing to anything except
        // the depth buffer.

        ColorMask 0

        // Do nothing specific in the pass:

        Pass {

        	CGPROGRAM
	            #pragma vertex vert
	            #pragma fragment frag
	            #pragma multi_compile DUMMY PIXELSNAP_ON
	            #include "UnityCG.cginc"
	     
	            struct appdata_t
	            {
	                float4 vertex   : POSITION;
	                float4 color    : COLOR;
	                float2 texcoord : TEXCOORD0;
	            };
	 
	            struct v2f
	            {
	                float4 vertex   : SV_POSITION;
	                fixed4 color    : COLOR;
	                half2 texcoord  : TEXCOORD0;
	            };
	     
	            fixed4 _Color;
	 
	            v2f vert(appdata_t IN)
	            {
	                v2f OUT;
	                OUT.vertex = UnityObjectToClipPos(IN.vertex);
	                OUT.texcoord = IN.texcoord;
	                OUT.color = IN.color * _Color;
	                #ifdef PIXELSNAP_ON
	                OUT.vertex = UnityPixelSnap (OUT.vertex);
	                #endif
	 
	                return OUT;
	            }
	 
	            sampler2D _MainTex;
	 
	            fixed4 frag(v2f IN) : SV_Target
	            {
	                fixed4 c = tex2D(_MainTex, IN.texcoord);
	                if ((c.r * c.g * c.b) == 0) discard;            //Most IMPORTANT working Code
	                c.rgb *= c.a;
	                return c;
	            }
	        ENDCG

        }
    }
}
