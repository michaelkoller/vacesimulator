Shader "Custom/RimLightTransparent"
{
	Properties
	{
		_TrueColor("Rim Light", Color) = (0, 1, 0, 0)
		_RimPower("Rim Pow", Range(0.2, 8)) = 1
	}
		SubShader
		{
			Tags {"Queue" = "Transparent"}

			Pass
			{
				ZWrite On
				ColorMask 0
			}

			CGPROGRAM
			#pragma surface surf Lambert alpha:fade

			struct Input
			{
				float3 viewDir;
			};

			float4 _TrueColor;
			
			float _RimPower;
			

			void surf(Input IN, inout SurfaceOutput o)
			{
				//saturate returns a value between 0 and 1
				half rim = 1 - saturate(dot(normalize(IN.viewDir), o.Normal));
				
				o.Emission = _TrueColor.rgb * pow(rim, _RimPower) * 10;
				o.Alpha = pow(rim, _RimPower)*_TrueColor.a;
			}
			ENDCG
		}
			FallBack "Diffuse"
}