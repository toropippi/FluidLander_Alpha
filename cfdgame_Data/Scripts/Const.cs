using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Const
{

    public static class CO
    {
        public const int WX = 192;
        public const int WY = 144;
        public const int TEXSCALE = 8;//流体側192:144に対しstageグラフィックは192*8:144*8なので
        public const int IM = WX - 1;
        public const int IJ = WY - 1;
        public const int IJM = (WX * WY);
        public const int IJM4 = (WX * WY * 4);
        public const int IJM2 = (IJM / 2);
        public const int IJM04 = (IJM / 4);
        public const float SPEED = 1.00f;//ステージ全体のマスタースピード。壁に設定した速度にこれが乗算される
        public const float PRESSURER = 0.0015f;//湧出壁に設定した圧力これが乗算される。
        public const float DT = 1.00f;//流体デルタタイム、なぜかわからんが0.74とか中途半端な値だと流体が変な挙動を起こすことが、たぶんcip法のところでDTをかけるのをどこかで忘れている
        public const float MU = 0.0000001f;//粘性項μ
        public const float arufa = MU * DT;//粘性項で使う値
        public const float ar1fa = 1.0f / (1.0f + 4.0f * arufa);//粘性項で使う値
        public const float ALPHA = 1.79f;//圧力計算の加速係数
        public const int CFDFRAME_PAR_GAMEFRAME = 12;//CFD処理を１フレーム何回行うか。
        public const int PARTICLEWRITEDIV = 16384;//何粒子フレームで粒子全体の座標初期化書き込みが１週するか。これはstageごとに等倍にかわるが、baseの値はここで設定
        public const int PARTICLEREADDIV = 16384;//1024粒子フレームで粒子初期化座標を記憶している変数への読み込みが１週する

        public const int PARTICLE_precision = 0;//0がVector2 1がushort*2。残念ながら実験でushortにしてバンド幅節約してもFPS改善しないことが判明したのでVector2採用

        public const int PARTICLECOLOR_NOZ1 = 255 + 22 * 256 + 5 * 65536;//バックファイヤの色
        public const float UFORADSPD = 0.053f;//ufoの回転速度
        public const float SCOREPOSX = -3.90f;
        public const float SCOREPOSY = -3.72f;
        //public const int PARTICLECOLOR_NOZ1 = 8 + 12 * 256 + 255 * 65536;

        //フォント関連
        public const int FONTPY=126;

        //画像、その他関係
        public const int GOALWAIT = 120;

    }

}

//スパゲッティソース直したいところ
//PARTICLE_precision = 0と=1のときで処理が完全にわかれてコードが二重になっているところをなんとかまとめたい。ただ=1はもう使わないだろう・・
//各スクリプトが各スクリプトを参照して、それぞれがどの変数をどこでどのclassの関数で実行しているかがわかりづらい。
//ただ実装の都合上、一部の関数は実行するタイミングが非常に大事なので、MenyBulletsの中に処理自体が多めにかいてある