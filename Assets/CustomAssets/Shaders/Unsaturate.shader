
Shader "Custom/Unsaturate"
{
    Properties
    {
		_MainTex("Main Texture", 2D) = "white" {}
		_Color("Main Color", Color) = (1, 0, 1, 1)
        _Intensity("Intensity", Float) = 0.0
    }
    SubShader 
    {
    ZWrite Off
    Blend SrcAlpha OneMinusSrcAlpha
        Pass 
        {
            CGPROGRAM
     
            sampler2D _MainTex;
			half4 _Color;
            half _Intensity;
 
 
            #pragma vertex vert
            #pragma fragment frag
             
            struct appdata {
                float4 pos : POSITION;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };
             
            v2f vert (appdata input) {
                v2f o;
                o.pos = mul(UNITY_MATRIX_MVP, input.pos);
                o.uv = o.pos.xy / 2.0 + 0.5;
                return o;
            }
             
             
            half4 frag(v2f input) : COLOR {
                half4 color = tex2D(_MainTex, input.uv);
                half c = color.r*0.21f + color.g*0.72f + color.b*0.07;
                return lerp(color, half4(c,c,c,1), _Intensity);
            }
            ENDCG
        }
    }
}