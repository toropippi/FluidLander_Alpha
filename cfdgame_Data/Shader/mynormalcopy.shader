Shader "mynormalcopy"{
	Properties{
		_MainTex("テクスチャ", 2D) = "white" { }
		_Intensity("色の強さ", Vector) = (1.0,1.0,1.0,1.0)
	}

		SubShader{
		Tags{ "Queue" = "Transparent" }

		Pass{
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"

	uniform sampler2D _MainTex;
	uniform fixed4 _MainTex_ST;//これもなぜかいる
	uniform fixed4 _Intensity;
	

	struct appdata {
		float4 vertex   : POSITION;
		float4 texcoord : TEXCOORD0;
	};

	struct v2f {
		float4 pos : SV_POSITION;
		float2 uv  : TEXCOORD0;
	};

	v2f vert(appdata v) {
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
		return o;
	}

	fixed4 frag(v2f v) : SV_Target{
		return tex2D(_MainTex, v.uv)*_Intensity;
	}
		ENDCG
	}
	}
}