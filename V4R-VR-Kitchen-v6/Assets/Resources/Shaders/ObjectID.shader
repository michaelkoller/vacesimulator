Shader "Unlit/ObjectID"
{
    Properties
    {
        _Index ("Index", Int) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
           
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
            };

            int _Index;
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                //UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            
            float4 frag (v2f i) : SV_Target
            {
                // sample the texture
                float4 col;
                col.x = _Index%10 *0.1f;
                col.y = _Index%100 * 0.01;
                col.z = _Index*0.00390625f;
                // col.x = (_Index %256) * (1.0f/255.0f)
                // col.y = (_Index /256) * (1.0f/255.0f)
                col.w = 1.0f;
                // apply fog
                //UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
