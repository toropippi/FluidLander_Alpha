using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using Const;

public class Ufo : MonoBehaviour {
    public ComputeBuffer ufo_vram;
    public ComputeBuffer nozzle_vram;
    public Vector2 ufo_spd;
    public Vector2 nozzle_spd;
    public Vector3 ufo_pos;
    public float death_posx;
    public float death_posy;
    public Vector2 ufo_restart_pos;
    public Vector3 nozzle_pos;
    public Vector2 nozzle_rys_pos;
    public float ufo_rad;
    public float ufo_radspd;
    public float grav;
    public float jumpf;
    public float pullf;//ufoが風におされる係数
    public int ingoalflg;//ゴール内にいる場合1になるフラグ
    public int pre_ingoalflg;//1フレーム前のingoalflgを記憶する変数
    public float hitpoint;
    public float forcetoufolimit;
    public int fastkey;//最初にスペースキーを押さないと動かない
    public int hidamage;

    Vector2[] ufocoli;
    int ufocoli_count;
    int cnt;
    int setcnt4;
    int downkey;
    int p_downkey;
    public int deathcounter;

    public GameObject shinimodoriMasterObj;
    MenyBullets mnybllts;
    Stagemanager stgmngrcomp;
    SoundEffects SE;

    // Use this for initialization
    void Start () {
        //ufo_count = 0;//UFOの流体側の当たり判定の、あたりにあたる領域の数
        ufo_spd.x = 0.00f;
        ufo_spd.y = 0.00f;
        ufo_pos.x = 23.0f;//これは流体基準の座標
        ufo_pos.y = 43.0f;//これは流体基準の座標
        ufo_pos.z = -0.02f;
        ufo_restart_pos.x = ufo_pos.x;
        ufo_restart_pos.y = ufo_pos.y;
        ufo_rad = 0.0f;
        ufo_radspd = 0.0f;
        grav =0.00f;
        jumpf = 0.0015f;//ジャンプキーを押したときのufoの進み具合
        pullf = 0.0f;//流れの影響
        hidamage = 0;
        forcetoufolimit = 0.0f;//ufoが受ける外力の最大値
        hitpoint = 128.0f;
        fastkey = 0;
        //ufo当たり判定用画像load
        LoadUfoData();//bmpからload生成

        ///////こっからはノズル
        //nozzle_count = 0;//ノズルの当たり判定の、あたりにあたる領域の数
        nozzle_spd.x = 0.00f;
        nozzle_spd.y = 0.00f;
        nozzle_pos.x = 11.0f;//これは流体基準の座標
        nozzle_pos.y = 11.0f;//これは流体基準の座標
        nozzle_pos.z = -0.02f;
        nozzle_rys_pos.x = 0.00f;//これはノズルの粒子がでてくる座標を記憶する変数GPUでも同じ計算をしている
        nozzle_rys_pos.y = 0.00f;//これはノズルの粒子がでてくる座標を記憶する変数GPUでも同じ計算をしている
        LoadNozzleData();//ノズルデータ生成

        //こっからは見た目上の当たり判定
        ufocoli_count = 0;
        LoadUfoColi();//当たり判定データload生成

        //コンポーネント
        mnybllts = GameObject.Find("nabie").GetComponent<MenyBullets>();//
        stgmngrcomp = GameObject.Find("StageManager").GetComponent<Stagemanager>();//コンポーネント
        SE = GameObject.Find("SE").GetComponent<SoundEffects>();//コンポーネント

        cnt = 0;
        setcnt4 = 0;
        downkey = 0;
        p_downkey=0;
        deathcounter = 0;
    }

    // Update is called once per frame
    void Update()
    {
        ufo_radspd = 0.0f;
        if ((Input.GetAxis("Horizontal") < -0.8f) & (stgmngrcomp.keyflg > 0)) 
        {
            ufo_rad -= Const.CO.UFORADSPD;
            ufo_radspd = -Const.CO.UFORADSPD / (Const.CO.DT * Const.CO.CFDFRAME_PAR_GAMEFRAME);
            ufo_rad = (ufo_rad + 3.0f*Mathf.PI) % (2.0f * Mathf.PI)- Mathf.PI;
        }
        if ((Input.GetAxis("Horizontal") > 0.8f) & (stgmngrcomp.keyflg > 0))
        {
            ufo_rad += Const.CO.UFORADSPD;
            ufo_radspd = Const.CO.UFORADSPD / (Const.CO.DT * Const.CO.CFDFRAME_PAR_GAMEFRAME);
            ufo_rad = (ufo_rad + 3.0f * Mathf.PI) % (2.0f * Mathf.PI) - Mathf.PI;
        }


        p_downkey = downkey;
        if ((Input.GetButton("Jump")) & (stgmngrcomp.keyflg > 0))
        {
            ufo_spd.x += jumpf * Mathf.Sin(ufo_rad);
            ufo_spd.y -= jumpf * Mathf.Cos(ufo_rad);
            downkey = 1;
            if (p_downkey == 0) { setcnt4 = cnt % 4; }
            if (cnt % 4 == setcnt4) { SE.aS.PlayOneShot(SE.jump); }
            fastkey = 1;
        }
        else
        {
            downkey = 0;
        }


        ufo_spd.y += grav*fastkey;//重力、最初のキーを押してないと重力は発生しない
        ufo_spd.x *= 0.993f;//地味に減速
        ufo_spd.y *= 0.993f;//地味に減速
        Coli();
        
        if (stgmngrcomp.keyflg > 0 | stgmngrcomp.nowstage ==18)
        { //キー操作を受け付けるかどうか、1受け付ける、0受け付けない。受け付けないときはufoの座標系さんもとまる
            ufo_pos.x += ufo_spd.x * Const.CO.DT * Const.CO.CFDFRAME_PAR_GAMEFRAME;
            ufo_pos.y += ufo_spd.y * Const.CO.DT * Const.CO.CFDFRAME_PAR_GAMEFRAME;
        }
        else
        {//座標系さんが止まっている場合は速度強制0
            ufo_spd.x = 0.0f;
            ufo_spd.y = 0.0f;
        }
        nozzle_pos.x = ufo_pos.x;
        nozzle_pos.y = ufo_pos.y;
        nozzle_spd.x = -0.71f * Mathf.Sin(ufo_rad) + ufo_spd.x;
        nozzle_spd.y = 0.71f * Mathf.Cos(ufo_rad) + ufo_spd.y;
        nozzle_rys_pos.x= -3.0f * Mathf.Sin(ufo_rad) + nozzle_pos.x;
        nozzle_rys_pos.y= 3.5f * Mathf.Cos(ufo_rad) + nozzle_pos.y;

        Setpos();//自分のposとrotationを確定
        Death();//死亡関係

        pre_ingoalflg = ingoalflg;
        ingoalflg = 0;
        cnt++;
    }













    void OnDisable()
    {
        // コンピュートバッファは明示的に破棄しないと怒られます
        ufo_vram.Release();
        nozzle_vram.Release();
    }

    //自分のposを確定
    void Setpos() {
        Vector3 objpos;// = transform.position;
        objpos.x = (ufo_pos.x - 0.5f*Const.CO.WX) / 0.5f / Const.CO.WY * 5.0f;
        objpos.y = (0.5f * Const.CO.WY - ufo_pos.y) / 0.5f / Const.CO.WY * 5.0f;
        objpos.z = ufo_pos.z;
        transform.position = objpos;
        transform.rotation = Quaternion.Euler(0, 0, -ufo_rad / Mathf.PI * 180.0f);
    }

    void LoadUfoData()//bmpからload
    {
        int ufo_count = 0;
        int[,] tmpbmp;
        Loadpngs loadpngs = GetComponent<Loadpngs>();
        tmpbmp = loadpngs.LoadBmp(Application.dataPath + "\\Textures\\ufo\\cfdcoli.bmp");
        uint[] hostdata = new uint[144];
        for (int i = 0; i < 12; i++)
        {
            for (int j = 0; j < 12; j++)
            {
                if ((tmpbmp[i, j] % 256) == 0)
                {//黒いところならCFD上で壁になる
                    hostdata[ufo_count] = (uint)(i - 5 + 2048) + (uint)(j - 5 + 2048) * 4096;
                    ufo_count++;
                }
            }
        }
        ufo_vram = new ComputeBuffer(ufo_count, Marshal.SizeOf(typeof(uint)));
        ufo_vram.SetData(hostdata, 0, 0, ufo_count);
    }

    void LoadNozzleData()//ノズルデータはソースから手動で入力
    {
        int nozzle_count = 0;
        //ノズル当たり判定設定。これは実質噴射速度を壁に固定するためにやっていて当たり判定として機能することは考えていない
        uint[] hostdata = new uint[144];
        hostdata[nozzle_count] = (uint)(4 - 5 + 2048) + (uint)4096 * (8 - 5 + 2048);
        nozzle_count++;
        hostdata[nozzle_count] = (uint)(5 - 5 + 2048) + (uint)4096 * (8 - 5 + 2048);
        nozzle_count++;
        hostdata[nozzle_count] = (uint)(4 - 5 + 2048) + (uint)4096 * (7 - 5 + 2048);
        nozzle_count++;
        hostdata[nozzle_count] = (uint)(5 - 5 + 2048) + (uint)4096 * (7 - 5 + 2048);
        nozzle_count++;
        nozzle_vram = new ComputeBuffer(nozzle_count, Marshal.SizeOf(typeof(uint)));
        nozzle_vram.SetData(hostdata, 0, 0, nozzle_count);
    }

    void LoadUfoColi()//bmpから見た目当たり判定の定義点をload
    {
        int[,] tmpbmp;
        Loadpngs loadpngs = GetComponent<Loadpngs>();
        tmpbmp = loadpngs.LoadBmp(Application.dataPath + "\\Textures\\ufo\\ufocoli.bmp");
        uint[] hostdata = new uint[1600];
        for (int i = 0; i < 40; i++)
        {
            for (int j = 0; j < 40; j++)
            {
                int pixc = tmpbmp[i, j];
                if (((pixc % 256) == 0)&((pixc / 256 % 256) == 255)& ((pixc / 65536% 256) == 0))
                {//緑いろのところなら当たり判定定義点になる
                    hostdata[ufocoli_count] = (uint)(i - 20 + 2048) + (uint)(j - 20 + 2048) * 4096;
                    ufocoli_count++;
                }
            }
        }
        ufocoli = new Vector2[ufocoli_count];
        //
        for(int i = 0; i < ufocoli_count; i++)
        {
            float fx = 0.25f * ((float)(hostdata[i] % 4096)-2048.0f + 0.5f);
            float fy = 0.25f * ((float)((hostdata[i] / 4096) % 4096) - 2048.0f + 0.5f);
            ufocoli[i] = new Vector2(fx, fy);
        }
    }





    //ufoと壁の当たり判定。流体側ではなく見た目用。
    void Coli() {
        // ufoの周りの各点で当たりかどうか計算。計算に使う座標はやはり流体側での座標。0～192、0～144
        // 認識すべきはkbpの0か1の壁部分。255は流体だし64-128は湧出壁になっているので当たらない
        // 2と3はufo自身なので当たり判定しない。4-63については後程
        Vector2 refvec=new Vector2(0.0f,0.0f);//衝突時の反射ベクトル
        Vector2 rotatedvec;
        int flg = 0;
        uint kb = 0;

        //まずは壁0,1のほう,ufoの当たり判定定義点すべてを解析
        foreach (var vecval in ufocoli)
        {
            rotatedvec.x = vecval.x * Mathf.Cos(ufo_rad) - vecval.y * Mathf.Sin(ufo_rad);
            rotatedvec.y = vecval.x * Mathf.Sin(ufo_rad) + vecval.y * Mathf.Cos(ufo_rad);
            int colix = (int)(rotatedvec.x + ufo_pos.x);
            int coliy = (int)(rotatedvec.y + ufo_pos.y);
            if ((colix >0)&(colix < Const.CO.WX-1)& (coliy > 0) & (coliy < Const.CO.WY-1))
            {
                kb = mnybllts.kbp[colix + coliy * Const.CO.WX];
                if ((kb <= 1)|(kb < 64))//0は非破壊壁、1は破壊壁、3以降は浮遊物
                {//この中は本当に衝突しているところ
                    flg++;
                    refvec += rotatedvec;//衝突している点をすべて加算
                }

            }
            else
            {//画面の端っこも強制反射
                flg++;
                refvec += rotatedvec;//衝突している点をすべて加算
            }
        }

        if (flg != 0)//衝突していたら
        {
            refvec.Normalize();//単位ベクトルに変換
            float naiseki = refvec.x * ufo_spd.x + refvec.y * ufo_spd.y;//内積=|vecA|×|VecB|*COS(sita)
            if (naiseki > 0.0f) {
                refvec *= -2.0f * naiseki;//反射準備
                ufo_spd.x += refvec.x;//反射
                ufo_spd.y += refvec.y;//反射

                if (pre_ingoalflg == 0) { //ゴールのわっかないでなければ
                    if (Mathf.Abs(naiseki) > 0.01)
                    {
                        //大ダメージ
                        if (Mathf.Abs(naiseki) > 0.088)
                        {
                            SE.aS.PlayOneShot(SE.hit1);
                            float damage = 70.0f * Mathf.Abs(naiseki) * (stgmngrcomp.difficulty + 2);
                            hitpoint -= damage;
                            hidamage += (int)(damage);
                        }
                        else//小ばめーじ
                        {
                            SE.aS.PlayOneShot(SE.hit0);
                            float damage = 40.0f * Mathf.Abs(naiseki) * (stgmngrcomp.difficulty + 2);
                            hitpoint -= damage;
                            hidamage += (int)(damage);
                        }
                    }
                }
            }
        }
        





        //次は可動壁との解析
        //上記とは逆に壁側から当たりにいく。ほとんどは距離で計算カットアウト
        //浮遊物の個数だけ全部当たり判定とる。浮遊物の個数ごとに内積計算
        for (int i = 0; i < stgmngrcomp.fobjnum; i++)//浮遊objの回数だけループ
        {
            flg = 0;
            refvec = new Vector2(0.0f, 0.0f);//衝突時の反射ベクトル
            uint[] hostdata;
            int datanum, obx, oby, rotateobjx, rotateobjy;
            uint xyscore;
            int objposx, objposy;
            float outspdx = 0.0f, outspdy=0.0f;
            datanum = stgmngrcomp.mvocomp[i].w * stgmngrcomp.mvocomp[i].h;
            hostdata = stgmngrcomp.mvocomp[i].hostdata;
            objposx = (int)stgmngrcomp.mvocomp[i].obj_pos.x;
            objposy = (int)stgmngrcomp.mvocomp[i].obj_pos.y;
            float objrad = stgmngrcomp.mvocomp[i].obj_rad;
            for (int j = 0; j < datanum; j++)//浮遊objの各ドット数だけループ
            {
                xyscore = hostdata[j];
                obx = (int)(xyscore % 4096) - 2048;// + objposx;//解凍
                oby = (int)(xyscore / 4096) - 2048;// + objposy;//解凍
                rotateobjx = (int)(obx * Mathf.Cos(objrad) - oby * Mathf.Sin(objrad)) + objposx;
                rotateobjy = (int)(obx * Mathf.Sin(objrad) + oby * Mathf.Cos(objrad)) + objposy;
                if (Mathf.Abs((int)ufo_pos.x - rotateobjx) < 9)//もしufoと近ければ計算、それいがいはskip
                {
                    if (Mathf.Abs((int)ufo_pos.y - rotateobjy) < 9)//もしufoと近ければ計算、それいがいはskip
                    {
                        foreach (var vecval in ufocoli)//一応ufoの全衝突定義点分だけループ
                        {
                            rotatedvec.x = vecval.x * Mathf.Cos(ufo_rad) - vecval.y * Mathf.Sin(ufo_rad);
                            rotatedvec.y = vecval.x * Mathf.Sin(ufo_rad) + vecval.y * Mathf.Cos(ufo_rad);
                            int colix = (int)(rotatedvec.x + ufo_pos.x);
                            int coliy = (int)(rotatedvec.y + ufo_pos.y);
                            if ((colix > 0) & (colix < Const.CO.WX - 1) & (coliy > 0) & (coliy < Const.CO.WY - 1))//ufoが変な場所じゃ苗kれバ
                            {
                                if ((colix == rotateobjx) & (coliy == rotateobjy))
                                {//この中は本当に衝突しているところ
                                    flg = i + 1;
                                    refvec += rotatedvec;//衝突している点をすべて加算.衝突している個所の壁の速度を覚えておく
                                    float f5sx = stgmngrcomp.mvocomp[i].obj_spd.x;
                                    float f5sy = stgmngrcomp.mvocomp[i].obj_spd.y;
                                    float f5px = rotateobjx - objposx;
                                    float f5py = rotateobjy - objposy;
                                    float f5radspd= stgmngrcomp.mvocomp[i].obj_radspd;
                                    outspdx = f5sx + (f5px * Mathf.Cos(f5radspd) - f5py * Mathf.Sin(f5radspd) - f5px);
                                    outspdy = f5sy + (f5px * Mathf.Sin(f5radspd) + f5py * Mathf.Cos(f5radspd) - f5py);
                                }
                            }
                        }
                    }
                }
            }
            //各objごとに衝突時の内積を計算して反射処理をする
            if (flg != 0)//可動壁に衝突していたら
            {
                refvec.Normalize();//単位ベクトルに変換
                float naiseki = refvec.x * (ufo_spd.x - outspdx) + refvec.y * (ufo_spd.y - outspdy);//内積=|vecA|×|VecB|*COS(sita)
                if (naiseki > 0.0f)
                {
                    refvec *= -2.0f * naiseki;//反射準備
                    ufo_spd.x += refvec.x;//反射
                    ufo_spd.y += refvec.y;//反射

                    if (pre_ingoalflg == 0)//ゴールのわっかないでなければ
                    {
                        if (Mathf.Abs(naiseki) > 0.01)
                        {
                            //大ダメージ
                            if (Mathf.Abs(naiseki) > 0.088)
                            {
                                
                                float damage = 70.0f * Mathf.Abs(naiseki) * (stgmngrcomp.difficulty + 2);
                                if (stgmngrcomp.nowstage == 16)
                                {
                                    damage = 0.0f;
                                    SE.aS.PlayOneShot(SE.hit2);
                                }
                                else
                                {
                                    SE.aS.PlayOneShot(SE.hit1);
                                }
                                hitpoint -= damage;
                                hidamage += (int)(damage);
                            }
                            else//小ばめーじ
                            {
                                SE.aS.PlayOneShot(SE.hit0);
                                float damage = 40.0f * Mathf.Abs(naiseki) * (stgmngrcomp.difficulty + 2);
                                if (stgmngrcomp.nowstage == 16)
                                {
                                    damage = 0.0f;
                                    SE.aS.PlayOneShot(SE.hit2);
                                }
                                else
                                {
                                    SE.aS.PlayOneShot(SE.hit0);
                                }
                                hitpoint -= damage;
                                hidamage += (int)(damage);
                            }
                        }
                    }
                }
            }
        }
        




    }








    //自分が死んだとき指定座標から開始
    void Death()
    {
        if (hitpoint < 0.0f)
        {
            deathcounter = 1;
            hitpoint = 128.0f;
            //まず爆発の圧力設定
            float[] divexplorer=new float[Const.CO.WX * Const.CO.WY];
            mnybllts.DIVexplorer.GetData(divexplorer);
            for (int i = (int)ufo_pos.x - 1; i < (int)ufo_pos.x + 1; i++)
            {
                for (int j = (int)ufo_pos.y - 1; j < (int)ufo_pos.y + 1; j++)
                {
                    divexplorer[i + j * Const.CO.WX] += 780.0f;
                }
            }
            mnybllts.DIVexplorer.SetData(divexplorer);
            //次に死に戻りの設定
            GameObject gameObj = Instantiate(shinimodoriMasterObj, new Vector3(0f,0f, 0f), Quaternion.identity);
            gameObj.GetComponent<ShinimodoriMaster>().x = ufo_restart_pos.x;
            gameObj.GetComponent<ShinimodoriMaster>().y = ufo_restart_pos.y;
            //Destroy(gameObj, 2.2f);//爆発アニメーション自動で消える
            //爆発粒子の設定
            death_posx = ufo_pos.x;
            death_posy = ufo_pos.y;
            //dtprtcomp.ExpRYS(death_posx, death_posy);
            //その他
            ufo_pos.x = 280.0f;
            ufo_pos.y = 50.0f;
            ufo_spd.x = 0.0f;
            ufo_spd.y = 0.0f;
            ufo_rad = 0.0f;
            hidamage += 120;
            SE.aS.PlayOneShot(SE.exp0);
            SE.aS.PlayOneShot(SE.exp1);
        }


        /*
        if (((deathcounter > 2) & (deathcounter < 19)) & (deathcounter % 2 == 1))
        {
            if (deathcounter == 3) { SE.aS.PlayOneShot(SE.exp0); }
            dtprtcomp.ExpRYS(death_posx, death_posy);
        }
        */

        if (deathcounter == 64)
        {
            SE.aS.PlayOneShot(SE.restart);//リスタートの音
        }
    
        //死に戻り中の動作
        if (deathcounter != 0)
        {
            deathcounter++;
            if (deathcounter == 147)
            {
                deathcounter = 0;
                ufo_pos.x = ufo_restart_pos.x;
                ufo_pos.y = ufo_restart_pos.y;
                fastkey = 0;
            }
        }
    }

    //ufoに働く外力計算
    public Vector2 ExtForce(float fx,float fy)
    {
        Vector2 ret=new Vector2(0.0f,0.0f);
        if (!float.IsNaN(fx) && !float.IsNaN(fy))//NaNでなければ
        {
            if (fastkey!=0){
                fx = Mathf.Clamp(fx, -forcetoufolimit, forcetoufolimit);
                fy = Mathf.Clamp(fy, -forcetoufolimit, forcetoufolimit);
                ufo_spd.x += pullf * (1 - pre_ingoalflg) * fx;
                ufo_spd.y += pullf * (1 - pre_ingoalflg) * fy;
                ret.x = pullf  * fx;//表示用の外力
                ret.y = pullf  * fy;//表示用の外力
            }
        }
        return ret;
    }


}
