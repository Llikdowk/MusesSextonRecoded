Shader "Custom/DrawSimple"
{
    Properties
    {
    }

    SubShader 
    {
        ZWrite Off
        ZTest Always
        Lighting Off
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

			struct appdata {
				float4 pos : POSITION;
			};
 
            struct v2f {
                float4 pos : SV_POSITION;
            };
 
            v2f vert(appdata input) {
                v2f output;
                output.pos = mul(UNITY_MATRIX_MVP, input.pos);
                return output;
            }
 
            half4 frag() : COLOR {
                return half4(1,0,0,0);
            }
 
            ENDCG
        }
    }
}