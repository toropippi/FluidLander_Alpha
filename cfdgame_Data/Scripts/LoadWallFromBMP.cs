using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadWallFromBMP : MonoBehaviour {
    // Use this for initialization
    public float stages_speed;//ステージごとに設定されるスピード倍率
    void Start() {
        stages_speed = 1.0f;
    }

    // Update is called once per frame
    void Update() {

    }

    //kabeP.pngなどから情報を取得。
    public void LoadWallInfo(string stagename)
    {
        MenyBullets menybulletscomp = GetComponent<MenyBullets>();//コンポーネント
        Loadpngs loadpngs = GetComponent<Loadpngs>();// コンポーネント
        //壁xと速度を画像から取得
        int[,] tmpbmp;
        tmpbmp = loadpngs.LoadBmp(Application.dataPath + "\\stage\\" + stagename + "\\kabex.bmp");
            TexToKb(menybulletscomp.kbx, menybulletscomp.kkx, tmpbmp);
        //壁xと速度を画像から取得
        tmpbmp = loadpngs.LoadBmp(Application.dataPath + "\\stage\\" + stagename + "\\kabey.bmp");
            TexToKb(menybulletscomp.kby, menybulletscomp.kky, tmpbmp);
        //kabePの設定
        tmpbmp = loadpngs.LoadBmp(Application.dataPath + "\\stage\\" + stagename + "\\kabep.bmp");
            TexToKb_p(menybulletscomp.kbp, menybulletscomp.kkp, tmpbmp);
        tmpbmp = loadpngs.LoadBmp(Application.dataPath + "\\stage\\" + stagename + "\\kabew.bmp");
            TexToKb_w(menybulletscomp.kbx, menybulletscomp.kby, menybulletscomp.kkx, menybulletscomp.kky, menybulletscomp.kbp, tmpbmp);
        menybulletscomp.kabePori.SetData(menybulletscomp.kbp);
        menybulletscomp.kabeX.SetData(menybulletscomp.kbx);
        menybulletscomp.kabeY.SetData(menybulletscomp.kby);
        menybulletscomp.YUN.SetData(menybulletscomp.kkx);
        menybulletscomp.YVN.SetData(menybulletscomp.kky);
        menybulletscomp.YPN.SetData(menybulletscomp.kkp);
        menybulletscomp.YPN.GetData(menybulletscomp.kbpori);//実質kbpori=kbp 配列子ぴ
    }
    //texから壁xyデータに変換
    void TexToKb(uint[] kb, float[] kk, int[,] tmpb)
    {
        int ginfo_r;
        for (int y = 0; y < Const.CO.WY; y++)
        {
            for (int x = 0; x < Const.CO.WX; x++)
            {
                ginfo_r = tmpb[x, y] % 256;
                //壁仕様にのっとり速度決定
                if (ginfo_r > 128)
                {
                    kb[y * Const.CO.WX + x] = (uint)255;
                }
                else
                {
                    kb[y * Const.CO.WX + x] = (uint)0;
                    kk[y * Const.CO.WX + x] = (1.0f/64.0f) * (ginfo_r - 64) * Const.CO.SPEED*stages_speed;
                }
            }
        }
    }
    //texから壁pデータに変換。これは主に吸収境界の設定
    void TexToKb_p(uint[] kbp, float[] kkp, int[,] tmpb)
    {
        int ginfo_r;
        for (int x = 0; x < Const.CO.WX; x++)
        {
            for (int y = 0; y < Const.CO.WY; y++)
            {
                ginfo_r = tmpb[x, y] % 256;
                if (ginfo_r == 0) { ginfo_r = 1; }//壊れる壁は壊れない壁に強制設定
                //壁仕様にのっとり壁か流体部分か決定
                kbp[y * Const.CO.WX + x] = (uint)ginfo_r;
                if (ginfo_r != 255) { 
                    kkp[y * Const.CO.WX + x] = Const.CO.PRESSURER * ((float)(ginfo_r - 96));//固定圧力も画像から読み込み
                }
                //カーネルのほうで自動的に、kbpが0-64のとき65-127のとき128以上のときで場合分けされ計算される
            }
        }
    }
    //texから壁pデータに変換。これは主に壁の設定
    //壁ならpに0を設定しxとyも0にクランプ、また速度も矯正0に
    void TexToKb_w(uint[] kbx, uint[] kby, float[] kkx, float[] kky, uint[] kbp, int[,] tmpb)
    {
        int ginfo_r;
        int ginfo_b;
        for (int y = 0; y < Const.CO.WY; y++)
        {
            for (int x = 0; x < Const.CO.WX; x++)
            {
                ginfo_r = tmpb[x, y] % 256;
                ginfo_b = tmpb[x, y] / 65536;
                if (ginfo_r == 0)
                {
                    if (kbp[x + y * Const.CO.WX] == 255) { kbp[x + y * Const.CO.WX] = 0; }
                    kbx[x + y * Const.CO.WX] = 0;
                    kkx[x + y * Const.CO.WX] = 0.0f;
                    kbx[(x + 1) % Const.CO.WX + y * Const.CO.WX] = 0;
                    kkx[(x + 1) % Const.CO.WX + y * Const.CO.WX] = 0.0f;
                    kby[x + y * Const.CO.WX] = 0;
                    kky[x + y * Const.CO.WX] = 0.0f;
                    kby[x + ((y + 1) % Const.CO.WY) * Const.CO.WX] = 0;
                    kky[x + ((y + 1) % Const.CO.WY) * Const.CO.WX] = 0.0f;
                }
                
                if (ginfo_b == 0)
                {
                    kbp[x + y * Const.CO.WX] = 1;//1は壊れない壁という意味
                }
            }
        }
    }


    //RYS側からみたload。画像のうち255,0,255のところが粒子発生点になるのでそれを登録
    public void LoadWallInfo2(string stagename)
    {
        DotParticle dtprtcomp = GetComponent<DotParticle>();//コンポーネント
        dtprtcomp.someofRYS.dataxy_len = 0;//何度もloadwallするためこれは毎回リセット必要
        Loadpngs loadpngs = GetComponent<Loadpngs>();// コンポーネント
        //壁xと速度を画像から取得
        int[,] tmpbmp;
        tmpbmp = loadpngs.LoadBmp(Application.dataPath + "\\stage\\"+ stagename + "\\kabew.bmp");
        int ginfo_g;
        for (int y = 0; y < Const.CO.WY; y++)
        {
            for (int x = 0; x < Const.CO.WX; x++)
            {
                ginfo_g = (tmpbmp[x, y] / 256) % 256;
                if (ginfo_g < 128)
                {//色の度合いによって粒子の出る割合が異なる。見た目をわかりやすくするためにg=128を0として色が濃いほどたくさん粒子がでる仕様
                    dtprtcomp.someofRYS.Inset(x, y, 128 - ginfo_g);//粒子湧出点を定義、あとで変更、追加もできる
                }
            }
        }
    }






}