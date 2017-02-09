Shader "Custom/UniformSpriteFaceCamera"
{
	
	Properties {
		_MainTex ("Texture Image", 2D) = "white" {}
		_Color("Color", Color) = (1, 1, 1, 1)
		_ScaleX("Scale X", Float) = 1.0
		_ScaleY("Scale Y", Float) = 1.0
	}
		SubShader{

		 ZWrite On
		 ZTest False
		 Tags {"Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "TransparentCutout"}
		 Blend SrcAlpha OneMinusSrcAlpha

	   Pass {
		  CGPROGRAM

		  #pragma vertex vert  
		  #pragma fragment frag 

		 uniform sampler2D _MainTex;        
		 uniform half4 _Color;
         uniform float _ScaleX;
         uniform float _ScaleY;

         struct vertexInput {
            float4 vertex : POSITION;
            float4 tex : TEXCOORD0;
         };
         struct v2f {
            float4 pos : SV_POSITION;
            float4 tex : TEXCOORD0;
         };
 
         v2f vert(vertexInput input) 
         {
            v2f output;

			float w = mul(UNITY_MATRIX_MVP, float4(0, 0, 0, 1)).w;
			float screenAdjust = (_ScaleX * 64) / _ScreenParams.x;
			w *= screenAdjust;

			output.pos = mul(UNITY_MATRIX_P, 
				mul(UNITY_MATRIX_MV, float4(0.0, 0.0, 0.0, 1.0)) + float4(input.vertex.xyz * w, 0.0) * float4(_ScaleX, _ScaleY, 1.0, 1.0));
 
            output.tex = input.tex;

            return output;
         }
 
         float4 frag(v2f input) : COLOR
         {
            return tex2D(_MainTex, float2(input.tex.xy)) * _Color;   
         }
 
         ENDCG
      }
   }
}
