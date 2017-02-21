//ORIGINAL SHADER CODE FROM: https://willweissman.wordpress.com/tutorials/shaders/unity-shaderlab-object-outlines/

Shader "Custom/PostOutline"
{
    Properties
    {
		_MainTex("Main Texture", 2D) = "white" {}
        _SourceTex("Source Texture", 2D) = "white" {}
		_Color("Main Color", Color) = (1, 0, 1, 1)
		_Thickness("Thickness", Int) = 9
		
    }
    SubShader 
    {
     Tags {
        "Queue" = "Overlay" 
        "IgnoreProjector" = "True" 
        "RenderType" = "Transparent" 
     }
    ZWrite On
    ZTest Always
    Blend SrcAlpha OneMinusSrcAlpha
        Pass 
        {
            CGPROGRAM
     
            sampler2D _MainTex;
            sampler2D _SourceTex;
			half4 _Color;
			half _Thickness;
 
            //<SamplerName>_TexelSize is a float2 that says how much screen space a texel occupies.
            float2 _MainTex_TexelSize;
 
            #pragma vertex vert
            #pragma fragment frag
             
            struct appdata {
                float4 pos : POSITION;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float2 uvSource : TEXCOORD1;
            };
             
            v2f vert (appdata input) {
                v2f o;
                o.pos = mul(UNITY_MATRIX_MVP, input.pos);
                o.uv = o.pos.xy / 2.0 + 0.5;
                o.uvSource = o.pos.xy / 2.0f + 0.5;
                return o;
            }
             
             
            half4 frag(v2f input) : COLOR {
                if(tex2D(_MainTex, input.uv).r > 0) { // Potential performance issue: conditional
                    discard;
                }
                half ColorIntensityInRadius = 0.0f;
                for(half i = -_Thickness/2.0; i < _Thickness/2.0; ++i) {
                    for(half j = -_Thickness/2.0; j < _Thickness/2.0; ++j) {
                        ColorIntensityInRadius += 
                            tex2D(_MainTex, input.uv + float2(i*_MainTex_TexelSize.x, j*_MainTex_TexelSize.y)).r; // Potential performance issue: lots of lookups
                    }
                }

                half4 sourceColor = tex2D(_SourceTex, input.uvSource);
                return lerp(sourceColor, _Color, ceil(ColorIntensityInRadius));
            }
            ENDCG
        }
    }
}