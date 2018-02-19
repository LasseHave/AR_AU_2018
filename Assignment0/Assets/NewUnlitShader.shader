// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'


Shader "custom/one sided plus lighting"{
   Properties {
     _Color ("Color Tint", Color) = (1.0,1.0,1.0,1.0)
     _MainTex ("Diffuse Texture", 2D) = "white" {}
     _SpecColor ("Specular Color", Color) = (1.0,1.0,1.0,1.0)
     _Shininess ("Shininess", Float) = 10
   }
   SubShader {
     Pass {
       Tags {"LightMode" = "ForwardBase"}
       CGPROGRAM
       #pragma vertex vert
       #pragma fragment frag
   
       //user defined variables
       uniform sampler2D _MainTex;
       uniform float4 _MainTex_ST;
       uniform float4 _Color;
       uniform float4 _SpecColor;
       uniform float _Shininess;
   
       //unity defined variables
       uniform float4 _LightColor0;
   
       //base input structs
       struct vertexInput{
         float4 vertex : POSITION;
         float3 normal : NORMAL;
         float4 texcoord : TEXCOORD0;
       };
       struct vertexOutput{
         float4 pos : SV_POSITION;
         float4 tex : TEXCOORD0;
         float4 posWorld : TEXCOORD1;
         float3 normalDir : TEXCOORD2;
       };
   
       //vertex Function
   
       vertexOutput vert(vertexInput v){
         vertexOutput o;
     
         o.posWorld = mul(unity_ObjectToWorld, v.vertex);
         o.normalDir = normalize( mul( float4( v.normal, 0.0 ), unity_WorldToObject ).xyz );
         o.pos = UnityObjectToClipPos(v.vertex);
         o.tex = v.texcoord;
     
         return o;
       }
   
       //fragment function
   
       float4 frag(vertexOutput i) : COLOR
       {
         float3 normalDirection = i.normalDir;
         float3 viewDirection = normalize( _WorldSpaceCameraPos.xyz - i.posWorld.xyz );
         float3 lightDirection;
         float atten;
     
         if(_WorldSpaceLightPos0.w == 0.0){ //directional light
           atten = 18.0;
           lightDirection = normalize(_WorldSpaceLightPos0.xyz);
         }
         else{
           float3 fragmentToLightSource = _WorldSpaceLightPos0.xyz - i.posWorld.xyz;
           float distance = length(fragmentToLightSource);
           atten = 1.0/distance;
           lightDirection = normalize(fragmentToLightSource);
         }
     
         //Lighting
         float3 diffuseReflection = atten * _LightColor0.xyz * saturate(dot(normalDirection, lightDirection));
         float3 specularReflection = diffuseReflection * _SpecColor.xyz * pow(saturate(dot(reflect(-lightDirection, normalDirection), viewDirection)) , _Shininess);
           
         float3 lightFinal = UNITY_LIGHTMODEL_AMBIENT.xyz + diffuseReflection + specularReflection;// + rimLighting;
     
         //Texture Maps
         float4 tex = tex2D(_MainTex, i.tex.xy * _MainTex_ST.xy + _MainTex_ST.zw);
     
         return float4(tex.xyz * lightFinal * _Color.xyz, 1.0);
       }
   
       ENDCG
   
     }
 
 
   }
   //Fallback "Specular"
}