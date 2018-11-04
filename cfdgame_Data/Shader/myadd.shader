Shader "myadd" {
	Properties{
		_MainTex("テクスチャ", 2D) = "white" { }
		_Intensity("色の強さ", Vector) = (1.0,1.0,1.0,1.0)
	}

		SubShader{
		Tags{ "Queue" = "Transparent" }

		Pass{
		ZWrite Off
		Blend One One
		CGPROGRAM
#pragma vertex vert_img
#pragma fragment frag
#include "UnityCG.cginc"

	uniform sampler2D _MainTex;
	uniform fixed4 _Intensity;

	float4 frag(v2f_img i) : COLOR{
		return tex2D(_MainTex, i.uv)*_Intensity;
	}
		ENDCG
	}
	}
}