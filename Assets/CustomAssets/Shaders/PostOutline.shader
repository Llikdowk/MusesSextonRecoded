//ORIGINAL SHADER CODE FROM: https://willweissman.wordpress.com/tutorials/shaders/unity-shaderlab-object-outlines/

Shader "Custom/PostOutline"
{
    Properties
    {
		_MainTex("Main Texture", 2D) = "white" {}
		_Color("Main Color", Color) = (1, 0, 1, 1)
		_Thickness("Thickness", Int) = 9
		
    }
    SubShader 
    {
    Blend SrcAlpha OneMinusSrcAlpha
        Pass 
        {
            CGPROGRAM
     
            sampler2D _MainTex;
			half4 _Color;
			int _Thickness;
 
            //<SamplerName>_TexelSize is a float2 that says how much screen space a texel occupies.
            float2 _MainTex_TexelSize;
 
            #pragma vertex vert
            #pragma fragment frag
             
            struct appdata {
                float4 pos : POSITION;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uvs : TEXCOORD0;
            };
             
            v2f vert (appdata input) {
                v2f o;
                o.pos = mul(UNITY_MATRIX_MVP, input.pos);
                o.uvs = o.pos.xy / 2.0 + 0.5;
                return o;
            }
             
             
            half4 frag(v2f input) : COLOR {
                if(tex2D(_MainTex, input.uvs.xy).r > 0) { // Potential performance issue: conditional
                    discard;
                }

                half ColorIntensityInRadius = 0.0f;
                for(int i = -_Thickness/2.0; i < _Thickness/2.0; ++i) {
                    for(int j = -_Thickness/2.0; j < _Thickness/2.0; ++j) {
                        ColorIntensityInRadius += 
                            tex2D(_MainTex, input.uvs.xy + float2(i*_MainTex_TexelSize.x, j*_MainTex_TexelSize.y)).r; // Potential performance issue: lots of lookups
                    }
                }
                return ColorIntensityInRadius * _Color;
            }
            ENDCG
        }
    }
}