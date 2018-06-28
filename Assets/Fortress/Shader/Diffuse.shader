 
Shader "Mobile/DiffuseFrontFace" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
    _MainTex ("Base (RGB)", 2D) = "white" {}
}
SubShader {
 Tags {  "Queue"="Geometry+2000" "IgnoreProjector"="True" "RenderType"="Geometry" }

 	Blend SrcAlpha OneMinusSrcAlpha
	//ColorMask RGB
	Cull Off Lighting On 
 
    LOD 200
    
 Pass {
        ZWrite On
        ColorMask 0
    }
CGPROGRAM
#pragma surface surf Lambert noforwardadd
 
sampler2D _MainTex;
float4 _Color;
struct Input {
    float2 uv_MainTex;
};
 
void surf (Input IN, inout SurfaceOutput o) {
    fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
    o.Albedo = c.rgb*_Color.rgb;
    o.Alpha = c.a;
}
ENDCG
}
 Fallback "Mobile/VertexLit"
}