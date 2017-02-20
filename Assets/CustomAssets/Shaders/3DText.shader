Shader "Custom/3DText" {

    Properties{
        _MainTex("Font Texture", 2D) = "white" {}
		_Color("Text Color", Color) = (1,1,1,1)
	}

	SubShader{
		Tags {
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
		}

		Lighting Off 
		Cull Back 
        ZWrite Off
        ZTest LEqual
		Fog { Mode Off }
		Blend SrcAlpha OneMinusSrcAlpha

		Pass {

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			sampler2D _MainTex;
			half4 _Color;

			struct appdata {
				float4 pos : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f {
				float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
			};

			v2f vert(appdata input) {
				v2f output;
				output.pos = mul(UNITY_MATRIX_MVP, input.pos);
                output.uv = input.uv;
				return output;
			}

			half4 frag(v2f input) : COLOR {
				return tex2D(_MainTex, input.uv).aaaa * _Color;
			}
			
			ENDCG
		}
	}
}