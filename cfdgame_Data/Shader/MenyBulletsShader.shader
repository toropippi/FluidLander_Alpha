Shader "MenyBulletsShader" {
	Properties{
		_Intensity("色の強さ", float) = 1.0
	}

	SubShader{
		Pass{
		ZWrite Off
		Blend One One
		CGPROGRAM

		
#pragma target 5.0// シェーダーモデルは5.0を指定
#pragma vertex vert
//#pragma geometry geom
#pragma fragment frag

#define WX (uint)(192)
#define WY (uint)(144)

#include "UnityCG.cginc"

	uniform fixed _Intensity;

	// 弾の構造体
	struct Bullet
	{
		float2 pos;
		uint col;
	};
	
	// 弾の構造化バッファ
	StructuredBuffer<Bullet> Bullets;
	//粒子の位置
	StructuredBuffer<float2> RYS;
	//StructuredBuffer<uint> RYS;
	//粒子の色
	StructuredBuffer<uint> RYc;

	// 頂点シェーダからの出力
	struct VSOut {
		float4 pos : SV_POSITION;
		float4 col : COLOR;
	};

	// 頂点シェーダ
	VSOut vert(uint id : SV_VertexID)
	{
		// idを元に、弾の情報を取得
		VSOut output;
		uint di = RYS[id];
		//float xx = 0.00390625*(float)(di % 65536);
		//float yy = 0.00390625*(float)(di / 65536);
		float2 rysxy=RYS[id];
		float xx=rysxy.x;
		float yy=rysxy.y;

		float3 worldPos = float3(10.0f / WY *(xx- 0.5f*WX),10.0f/WY*(0.5f*WY -yy),0.0);//0.078125は10.0/128。RYSを0を中心に+-5以内に収める処理
		output.pos = mul(UNITY_MATRIX_VP, float4(worldPos, 1.0f));
		uint uintcol = RYc[id];
		output.col.r =  (float)(uintcol % 256) / 255.0;
		output.col.g =  (float)((uintcol / 256) % 256) / 255.0;
		output.col.b =  (float)((uintcol / 65536) % 256) / 255.0;
		output.col.a = 1.0f;//加算合成でのレンダリングのせいなのかαは0.0でも1.0として計算される
		return output;
	}
	/*
	// ジオメトリシェーダ
	[maxvertexcount(1)]
	void geom(point VSOut input[1], inout TriangleStream<VSOut> outStream)
	{
		VSOut output;
		// 全ての頂点で共通の値を計算しておく
		outStream.RestartStrip();
	}
	*/
	// ピクセルシェーダー
	float4 frag(VSOut i) : COLOR
	{
		return i.col*_Intensity;
	}

		ENDCG
	}




	}
}