using UnityEngine;
using System.Runtime.InteropServices;
using System;
using System.IO;
using Const;
using UnityEngine.Rendering;

public class MenyBullets : MonoBehaviour
{
    public Shader bulletsShader;///MenyBulletsShader.shader 粒子をレンダリングするシェーダー//
    Material bulletsMaterial;///粒子のマテリアル bulletsShaderと紐づけされる
    public ComputeShader bulletsComputeShader;///NS.compute 流体の更新を行うコンピュートシェーダー 
    CommandBuffer commandb;
    public ComputeBuffer YU;
    public ComputeBuffer YUN;
    public ComputeBuffer GXU;
    public ComputeBuffer GYU;
    public ComputeBuffer YV;
    public ComputeBuffer YVN;
    public ComputeBuffer GXV;
    public ComputeBuffer GYV;
    public ComputeBuffer YE;//爆破に関係する熱量の移動
    public ComputeBuffer YEN;//爆破に関係する熱量の移動
    public ComputeBuffer GXE;//爆破に関係する熱量の移動
    public ComputeBuffer GYE;//爆破に関係する熱量の移動
    public ComputeBuffer GXd0;
    public ComputeBuffer GYd0;
    public ComputeBuffer GXd1;
    public ComputeBuffer GYd1;
    public ComputeBuffer GXd2;
    public ComputeBuffer GYd2;
    public ComputeBuffer YPN;
    public ComputeBuffer YUV;
    public ComputeBuffer YVU;
    public ComputeBuffer YTT;
    public ComputeBuffer DIV;
    public ComputeBuffer DIVexplorer;
    public ComputeBuffer kabePori;
    public ComputeBuffer kabeP;
    public ComputeBuffer kabeX;
    public ComputeBuffer kabeY;
    public ComputeBuffer RES108;
    public ComputeBuffer UFOE;
    public ComputeBuffer StructureOfFloatObj;
    int kernelnewgrad;
    int kernelnewgrad2;
    int kerneldcip0;
    int kerneldcip1;
    int kerneldcip2;
    int kernelpressure0;
    int kernelpressure1;
    int kerneldiv;
    int kernelrhs;
    int kernelveloc;
    int kernelnensei0;
    int kernelnensei1;
    int kernelryuusi;
    int kernelryuusi_fire;
    int kernelexpkabe;
    int kernelcomputebuffermemcopy_YE;
    int kernelcomputebuffermemcopy_i;
    int kernelcomputebuffermemcopy_f;
    int kernelcomputebuffermemcopy_f2;
    int kernelfillmem_f;
    int kernelfillmem_ui;
    int kernelobjmapping;
    int kernelkabemapping;
    int kernelufopressure;
    int kernelVorticityReduce;
    uint cnt;
    int[] res;//配列数108の配列変数
    public Vector2 extf;//表示用の外力

    //ここからは別ソースのload系
    Ufo ucomp;
    Frontstage frntcomp;
    DotParticle dtprtcomp;
    Stagemanager stgmngrcomp;
    SoundEffects SE;
    Referobj refer;
    public uint[] kbp;//流体関連の圧力壁定義点のvalではなく種類を記憶するほう
    public uint[] kbpori;//流体関連の圧力壁定義点のvalではなく種類を記憶するほう
    public uint[] kbx;//流体関連のｘ速度壁定義点のvalではなく種類を記憶するほう
    public uint[] kby;//流体関連のｙ速度壁定義点のvalではなく種類を記憶するほう
    public float[] kkx;//流体関連のｘ速度壁定義点のval
    public float[] kky;//流体関連のｙ速度壁定義点のval
    public float[] kkp;//流体関連の圧力壁定義点のval

    void Start()
    {
        ucomp = GameObject.Find("ufo").GetComponent<Ufo>();//ufo コンポーネント
        frntcomp = GameObject.Find("frontpng").GetComponent<Frontstage>();//コンポーネント
        dtprtcomp = GetComponent<DotParticle>();//コンポーネント
        stgmngrcomp = GameObject.Find("StageManager").GetComponent<Stagemanager>();//コンポーネント
        SE = GameObject.Find("SE").GetComponent<SoundEffects>();//コンポーネント
        refer = GameObject.Find("referobj").GetComponent<Referobj>();//コンポーネント
        kbp = new uint[Const.CO.WX * Const.CO.WY];
        kbpori = new uint[Const.CO.WX * Const.CO.WY];
        kbx = new uint[Const.CO.WX * Const.CO.WY];
        kby = new uint[Const.CO.WX * Const.CO.WY];
        kkx = new float[Const.CO.WX * Const.CO.WY];
        kky = new float[Const.CO.WX * Const.CO.WY];
        kkp = new float[Const.CO.WX * Const.CO.WY];
        res = new int[108];
        bulletsMaterial = new Material(bulletsShader);
        Debug.Log(Mathf.Sqrt(Mathf.Clamp((65536f * 4f) / refer.PARTICLENUM, 0.0f, 1.0f)));
        bulletsMaterial.SetFloat("_Intensity", Mathf.Sqrt(Mathf.Clamp((65536f*4f)/refer.PARTICLENUM,0.0f,1.0f)));
        FindKernelInit();//
        InitializeComputeBuffer();//ここで流体関連と粒子等のvram生成
        SetKernels();//カーネル全部作成＆引数セット
        //コマンドバッファ系
        commandb = new CommandBuffer();
        Camera cam = GameObject.Find("Main Camera").GetComponent<Camera>();//コンポーネント
        commandb.name = "gpu instanse";
        commandb.DrawProcedural(cam.cameraToWorldMatrix, bulletsMaterial, 0, MeshTopology.Points, dtprtcomp.RYS.count, 1);
        cam.AddCommandBuffer(CameraEvent.AfterForwardOpaque, commandb);

        GetComponent<LoadWallFromBMP>().stages_speed = 0.0f;
        GetComponent<LoadWallFromBMP>().LoadWallInfo("0");//ここで壁情報を画像データbmpやpngから読み込み壁情報をvramに転送
        cnt = 0;
        extf = new Vector2(0.0f,0.0f);
        
    }

    /// <summary>
    /// 更新処理
    /// デバッグにて１フレームあたりの餌やり0.09ms、計算自体は33msとなっている
    /// </summary>
    void Update()
    {
        //自分が死んだときに発生する粒子は、移流の前でないと見た目がしょぼいことになる
        if (((ucomp.deathcounter > 2) & (ucomp.deathcounter < 19)) & (ucomp.deathcounter % 2 == 1))
        {
            if (ucomp.deathcounter == 3) { SE.aS.PlayOneShot(SE.exp0); }
            dtprtcomp.ExpRYS(ucomp.death_posx, ucomp.death_posy);
        }


        //ここで壁mapping
        CopyBufferToBuffer_f(kabeP, kabePori);//壁初期値をまず設定
        //すべての壁objをmapping、ufo含む
        AnotherObj();
        //マッピング情報からkabe情報を生成
        bulletsComputeShader.Dispatch(kernelkabemapping, 1, Const.CO.WY, 1);
        
        //CFD解析本編
        for (int loop = 0; loop < Const.CO.CFDFRAME_PAR_GAMEFRAME; loop++)
        {
            CopyBufferToBuffer_f(YU, YUN);
            CopyBufferToBuffer_f(YV, YVN);
            //veloc//ブロッキングモードoff
            bulletsComputeShader.Dispatch(kernelveloc, 1, Const.CO.WY, 1);
            CopyBufferToBuffer_f(GXd0, GXU);
            CopyBufferToBuffer_f(GYd0, GYU);
            CopyBufferToBuffer_f(GXd1, GXV);
            CopyBufferToBuffer_f(GYd1, GYV);
            CopyBufferToBuffer_f(GXd2, GXE);
            CopyBufferToBuffer_f(GYd2, GYE);
            //cip移流//ブロッキングモードoff
            bulletsComputeShader.Dispatch(kerneldcip0, 1, Const.CO.WY, 1);
            bulletsComputeShader.Dispatch(kerneldcip1, 1, Const.CO.WY, 1);
            bulletsComputeShader.Dispatch(kerneldcip2, 1, Const.CO.WY, 1);

            //過剰な渦度平滑化
            //bulletsComputeShader.Dispatch(kernelVorticityReduce, 1, Const.CO.WY, 1);

            CopyBufferToBuffer_f(YU, YUN);//これはいる！！しかしなぜなのかはわからない
            CopyBufferToBuffer_f(YV, YVN);//これはいる！！しかしなぜなのかはわからない
            CopyBufferYE_YEN();//YE←YEN//これはただのコピーではない。熱量減量、噴射時マッピング
            
            //粘性//ブロッキングモードoff
            //CopyBufferToBuffer_f(YU, YUN);
            //CopyBufferToBuffer_f(YV, YVN);//もしかしたらnewgradのためいるのかもしれない
            //CopyBufferToBuffer_f(GXd0, YUN);
            //CopyBufferToBuffer_f(GYd0, YVN);
            //bulletsComputeShader.Dispatch(kernelnensei0, 1, Const.CO.WY, 1);
            //bulletsComputeShader.Dispatch(kernelnensei1, 1, Const.CO.WY, 1);

            //DIV//ブロッキングモードoff
            bulletsComputeShader.Dispatch(kerneldiv, 1, Const.CO.WY, 1);

            //pressure//ブロッキングモードoff
            for (int i = 0; i < (refer.POISSONLOOPNUM+ BoolToInt(stgmngrcomp.fadeinflg<2 && loop==0)*1000); i++)
            {
                bulletsComputeShader.Dispatch(kernelpressure0, Const.CO.WX* Const.CO.WY/128 / 2, 1, 1);
                bulletsComputeShader.Dispatch(kernelpressure1, Const.CO.WX * Const.CO.WY / 128 / 2, 1, 1);
            }

            //ufoのうける外力
            //UFOのうける外力計算。1スレッドで144ループしてもらう。リダクション面倒くさいしどうせメモリ律速になるので・・
            bulletsComputeShader.SetFloat("OBJ0POSX", ucomp.ufo_pos.x);
            bulletsComputeShader.SetFloat("OBJ0POSY", ucomp.ufo_pos.y);
            bulletsComputeShader.Dispatch(kernelufopressure, 1, 1, 1);

            //rhs//ブロッキングモードoff
            bulletsComputeShader.Dispatch(kernelrhs, 1, Const.CO.WY, 1);

            //newgrad//ブロッキングモードoff
            bulletsComputeShader.Dispatch(kernelnewgrad, 1, Const.CO.WY, 1);
            bulletsComputeShader.Dispatch(kernelnewgrad2, 1, Const.CO.WY, 1);

            //粒子移流ブロッキングモードoff//一定フレームで更新
            if (loop % refer.RYRATIO == refer.RYRATIO -1)
            {
                dtprtcomp.UpdateOFST();//粒子場所一部初期化ブロッキングモードoff
                if ((Input.GetButton("Jump")) & (stgmngrcomp.keyflg > 0))
                { //粒子場所、噴射している場合赤い粒子発生ブロッキングモードoff
                    NozzleFire();
                }
                //粒子移動処理ブロッキングモードoff
                bulletsComputeShader.Dispatch(kernelryuusi, dtprtcomp.RYS.count / 64, 1, 1);
            }
        }


        //爆発判定ブロッキングモードoff
        bulletsComputeShader.Dispatch(kernelexpkabe, 1, Const.CO.WY, 1);
        //あとのブロッキングモードonになる処理は都合によりon Renderingで
        cnt++;
        extf *= 0.96f;
        

    }



    /// <summary>
    /// レンダリング
    /// ただのレンダリングではなくすべてのスクリプトのupdateが施行されたフレーム処理最後のプログラムになるので
    /// 今までGPUに投げていたタスクをCPUに戻すブロッキングをいれる
    /// ここでCPUはGPU待ちを喰らうので、重いCPU処理はここに到達するまでに行うべき
    /// </summary>
    /// 順番は
    /// OnPreRenderカメラがシーンのレンダリングを開始する前	1回
    /// OnRenderObjectすべてのシーンレンダリング終了後	1回
    /// OnPostRenderカメラがシーンのレンダリングを終了した後	1回
    /// OnRenderImage(Pro only)画面レンダリングが完了し画面画像の処理が可能になった後	1回
    /// //有効なものは２つOnRenderObjectかOnGUI
    void OnRenderObject()
    {
        //GPU→CPUなのでブロッキングが発生する。
        //まずはufoのほう
        float[] f2=new float[2];
        UFOE.GetData(f2);
        extf += ucomp.ExtForce(f2[0], f2[1]); ;//外力
        f2[0] = 0.0f;f2[1] = 0.0f;
        UFOE.SetData(f2);

        //次に爆発のほう
        RES108.GetData(res);
        //CPU側で判定check→位置復元、front画像の処理を
        //と思ったけど不具合あったのでkabePoriを全loadして比較することに
        if (res[0] != 0)
        { //elseはそもそも爆発してない
            kabePori.GetData(kbpori);//kabePoriはすでにカーネル側で更新されている
            for (int j = 0; j < Const.CO.WX * Const.CO.WY; j++)
            {
                if (kbpori[j] != kbp[j])
                {
                    int expx = j % Const.CO.WX;
                    int expy = j / Const.CO.WX;
                    frntcomp.DelPixcelTex(expx, expy);
                    kbp[expx + expy * Const.CO.WX] = 255;
                }
            }
            //爆発しているのでapply、ほかRESのリセットも必要
            frntcomp.tex.Apply();
            FillMem_ui(RES108, 108, 0, 0);
            SE.Play_exp0();
        }


        //もともと粒子のレンダリング前にブロッキングを↑に挟んだほうが早くなることが実験的にわかっていた
        //そのごいろいろリファクタして結局変わらないくらいになった。どっちかというとOnpostのほうが早そうだった
        //bulletsMaterial.renderQueue=4;
        // レンダリングを開始
        //bulletsMaterial.SetPass(0);
        // 1万個のオブジェクトをレンダリング
        //Graphics.DrawProcedural(MeshTopology.Points, dtprtcomp.RYS.count);
    }




































    void FindKernelInit()//1
    {
        kernelnewgrad = bulletsComputeShader.FindKernel("newgrad");
        kernelnewgrad2 = bulletsComputeShader.FindKernel("newgrad2");
        kerneldcip0 = bulletsComputeShader.FindKernel("dcip0");
        kerneldcip1 = bulletsComputeShader.FindKernel("dcip1");
        kerneldcip2 = bulletsComputeShader.FindKernel("dcip2");
        kernelpressure0 = bulletsComputeShader.FindKernel("pressure0");
        kernelpressure1 = bulletsComputeShader.FindKernel("pressure1");
        kerneldiv = bulletsComputeShader.FindKernel("div");
        kernelrhs = bulletsComputeShader.FindKernel("rhs");
        kernelveloc = bulletsComputeShader.FindKernel("veloc");
        kernelnensei0 = bulletsComputeShader.FindKernel("nensei0");
        kernelnensei1 = bulletsComputeShader.FindKernel("nensei1");
        kernelryuusi = bulletsComputeShader.FindKernel("ryuusi");
        kernelryuusi_fire= bulletsComputeShader.FindKernel("ryuusi_fire");
        kernelexpkabe = bulletsComputeShader.FindKernel("expkabe");
        kernelcomputebuffermemcopy_YE = bulletsComputeShader.FindKernel("computebuffermemcopy_YE");
        kernelcomputebuffermemcopy_i = bulletsComputeShader.FindKernel("computebuffermemcopy_i");
        kernelcomputebuffermemcopy_f = bulletsComputeShader.FindKernel("computebuffermemcopy_f");
        kernelcomputebuffermemcopy_f2 = bulletsComputeShader.FindKernel("computebuffermemcopy_f2");
        kernelobjmapping = bulletsComputeShader.FindKernel("objmapping");
        kernelkabemapping = bulletsComputeShader.FindKernel("kabemapping");
        kernelfillmem_f = bulletsComputeShader.FindKernel("fillmem_f");
        kernelfillmem_ui = bulletsComputeShader.FindKernel("fillmem_ui");
        kernelufopressure = bulletsComputeShader.FindKernel("ufopressure");
        kernelVorticityReduce = bulletsComputeShader.FindKernel("VorticityReduce");
    }

    /// <summary>
    /// コンピュートバッファの初期化
    /// </summary>
    void InitializeComputeBuffer()//2
    {
        YU = new ComputeBuffer(Const.CO.IJM, Marshal.SizeOf(typeof(float)));
        YUN = new ComputeBuffer(Const.CO.IJM, Marshal.SizeOf(typeof(float)));
        GXU = new ComputeBuffer(Const.CO.IJM, Marshal.SizeOf(typeof(float)));
        GYU = new ComputeBuffer(Const.CO.IJM, Marshal.SizeOf(typeof(float)));
        YV = new ComputeBuffer(Const.CO.IJM, Marshal.SizeOf(typeof(float)));
        YVN = new ComputeBuffer(Const.CO.IJM, Marshal.SizeOf(typeof(float)));
        GXV = new ComputeBuffer(Const.CO.IJM, Marshal.SizeOf(typeof(float)));
        GYV = new ComputeBuffer(Const.CO.IJM, Marshal.SizeOf(typeof(float)));
        YE = new ComputeBuffer(Const.CO.IJM, Marshal.SizeOf(typeof(float)));
        YEN = new ComputeBuffer(Const.CO.IJM, Marshal.SizeOf(typeof(float)));
        GXE = new ComputeBuffer(Const.CO.IJM, Marshal.SizeOf(typeof(float)));
        GYE = new ComputeBuffer(Const.CO.IJM, Marshal.SizeOf(typeof(float)));
        GXd0 = new ComputeBuffer(Const.CO.IJM, Marshal.SizeOf(typeof(float)));
        GYd0 = new ComputeBuffer(Const.CO.IJM, Marshal.SizeOf(typeof(float)));
        GXd1 = new ComputeBuffer(Const.CO.IJM, Marshal.SizeOf(typeof(float)));
        GYd1 = new ComputeBuffer(Const.CO.IJM, Marshal.SizeOf(typeof(float)));
        GXd2 = new ComputeBuffer(Const.CO.IJM, Marshal.SizeOf(typeof(float)));
        GYd2 = new ComputeBuffer(Const.CO.IJM, Marshal.SizeOf(typeof(float)));
        YPN = new ComputeBuffer(Const.CO.IJM, Marshal.SizeOf(typeof(float)));
        YUV = new ComputeBuffer(Const.CO.IJM, Marshal.SizeOf(typeof(float)));
        YVU = new ComputeBuffer(Const.CO.IJM, Marshal.SizeOf(typeof(float)));
        YTT = new ComputeBuffer(Const.CO.IJM, Marshal.SizeOf(typeof(Vector2)));
        DIV = new ComputeBuffer(Const.CO.IJM, Marshal.SizeOf(typeof(float)));
        DIVexplorer = new ComputeBuffer(Const.CO.IJM, Marshal.SizeOf(typeof(float)));
        kabeP = new ComputeBuffer(Const.CO.IJM, Marshal.SizeOf(typeof(uint)));//computebufferのstrideには4未満が指定できないらしい。openclではstride=1でやっていた
        kabePori = new ComputeBuffer(Const.CO.IJM, Marshal.SizeOf(typeof(uint)));//computebufferのstrideには4未満が指定できないらしい。openclではstride=1でやっていた
        kabeX = new ComputeBuffer(Const.CO.IJM, Marshal.SizeOf(typeof(uint)));
        kabeY = new ComputeBuffer(Const.CO.IJM, Marshal.SizeOf(typeof(uint)));
        RES108 = new ComputeBuffer(108, Marshal.SizeOf(typeof(uint)));//192*144/256=108という説。関係ない。１フレームに爆発するドットはせいぜい16個が限度なのでこれで十分
        UFOE = new ComputeBuffer(2, Marshal.SizeOf(typeof(float)));
        StructureOfFloatObj = new ComputeBuffer(64, 5*Marshal.SizeOf(typeof(float)));//浮遊objは1ステージあたり最大でも64個までだろう。あとfloat5の構造体なので*5
        FillComputeBuffer();
        //変則的だがこれしないとうちのスクリプトのstartでRYSが使えない
        dtprtcomp.RYS = new ComputeBuffer(refer.PARTICLENUM, 2 * Marshal.SizeOf(typeof(float)));//粒子の座標ｘ、ｙ
        dtprtcomp.RYc = new ComputeBuffer(refer.PARTICLENUM, Marshal.SizeOf(typeof(uint)));//粒子の色 uchar,uchar,uchar,ucharをまとめたもの
    }

    public void FillComputeBuffer() { 
        FillBuffer_F(YU);
        FillBuffer_F(YUN);
        FillBuffer_F(GXU);
        FillBuffer_F(GYU);
        FillBuffer_F(YV);
        FillBuffer_F(YVN);
        FillBuffer_F(GXV);
        FillBuffer_F(GYV);
        FillBuffer_F(YE);
        FillBuffer_F(YEN);
        FillBuffer_F(GXE);
        FillBuffer_F(GYE);
        FillBuffer_F(GXd0);
        FillBuffer_F(GYd0);
        FillBuffer_F(GXd1);
        FillBuffer_F(GYd1);
        FillBuffer_F(GXd2);
        FillBuffer_F(GYd2);
        FillBuffer_F(YPN);
        FillBuffer_F(YUV);
        FillBuffer_F(YVU);
        FillBuffer_F(DIV);
        FillBuffer_F(DIVexplorer);
        FillBuffer_F(UFOE);
        FillBuffer_I(kabeP);
        FillBuffer_I(kabePori);
        FillBuffer_I(kabeX);
        FillBuffer_I(kabeY);
        FillMem_ui(RES108, 108, 0, 0);
        FillHostData();//cpu側でnewしたほうに0をfillする
    }

    void FillHostData() {
        FillHD_I(kbp);
        FillHD_I(kbpori);
        FillHD_I(kbx);
        FillHD_I(kby);
        FillHD_F(kkx);
        FillHD_F(kky);
        FillHD_F(kkp);
    }
    void FillHD_I(uint[] data)
    {
        for(int i=0;i< Const.CO.WX * Const.CO.WY; i++)
        {
            data[i] = 0;
        }
    }
    void FillHD_F(float[] data)
    {
        for (int i = 0; i < Const.CO.WX * Const.CO.WY; i++)
        {
            data[i] = 0.0f;
        }
    }

    void SetKernels()//5
    {
        bulletsMaterial.SetBuffer("RYS", dtprtcomp.RYS);
        bulletsMaterial.SetBuffer("RYc", dtprtcomp.RYc);

        bulletsComputeShader.SetFloat("DT", Const.CO.DT);
        bulletsComputeShader.SetFloat("RYRATIO", (float)refer.RYRATIO);

        bulletsComputeShader.SetBuffer(kernelnewgrad, "yn", YVN);
        bulletsComputeShader.SetBuffer(kernelnewgrad, "y", YV);
        bulletsComputeShader.SetBuffer(kernelnewgrad, "GX", GXV);
        bulletsComputeShader.SetBuffer(kernelnewgrad, "GY", GYV);
        bulletsComputeShader.SetBuffer(kernelnewgrad, "kabe", kabeY);

        bulletsComputeShader.SetBuffer(kernelnewgrad2, "yn", YUN);
        bulletsComputeShader.SetBuffer(kernelnewgrad2, "y", YU);
        bulletsComputeShader.SetBuffer(kernelnewgrad2, "GX", GXU);
        bulletsComputeShader.SetBuffer(kernelnewgrad2, "GY", GYU);
        bulletsComputeShader.SetBuffer(kernelnewgrad2, "kabe", kabeX);

        bulletsComputeShader.SetBuffer(kerneldcip0, "fn", YUN);
        bulletsComputeShader.SetBuffer(kerneldcip0, "gxn", GXU);
        bulletsComputeShader.SetBuffer(kerneldcip0, "gyn", GYU);
        bulletsComputeShader.SetBuffer(kerneldcip0, "u", YU);
        bulletsComputeShader.SetBuffer(kerneldcip0, "v", YVU);
        bulletsComputeShader.SetBuffer(kerneldcip0, "GXd", GXd0);
        bulletsComputeShader.SetBuffer(kerneldcip0, "GYd", GYd0);
        bulletsComputeShader.SetBuffer(kerneldcip0, "kabe", kabeX);

        bulletsComputeShader.SetBuffer(kerneldcip1, "fn", YVN);
        bulletsComputeShader.SetBuffer(kerneldcip1, "gxn", GXV);
        bulletsComputeShader.SetBuffer(kerneldcip1, "gyn", GYV);
        bulletsComputeShader.SetBuffer(kerneldcip1, "u", YUV);
        bulletsComputeShader.SetBuffer(kerneldcip1, "v", YV);
        bulletsComputeShader.SetBuffer(kerneldcip1, "GXd", GXd1);
        bulletsComputeShader.SetBuffer(kerneldcip1, "GYd", GYd1);
        bulletsComputeShader.SetBuffer(kerneldcip1, "kabe", kabeY);

        bulletsComputeShader.SetBuffer(kerneldcip2, "fn", YEN);//EはV定義点と同じ
        bulletsComputeShader.SetBuffer(kerneldcip2, "YE", YE);//EはV定義点と同じ
        bulletsComputeShader.SetBuffer(kerneldcip2, "gxn", GXE);
        bulletsComputeShader.SetBuffer(kerneldcip2, "gyn", GYE);
        bulletsComputeShader.SetBuffer(kerneldcip2, "YTT", YTT);
        bulletsComputeShader.SetBuffer(kerneldcip2, "GXd", GXd2);
        bulletsComputeShader.SetBuffer(kerneldcip2, "GYd", GYd2);
        bulletsComputeShader.SetBuffer(kerneldcip2, "kabe", kabeP);

        bulletsComputeShader.SetBuffer(kernelnensei0, "YU", YU);
        bulletsComputeShader.SetBuffer(kernelnensei0, "YUN", YUN);
        bulletsComputeShader.SetBuffer(kernelnensei0, "YV", YV);
        bulletsComputeShader.SetBuffer(kernelnensei0, "YVN", YVN);
        bulletsComputeShader.SetBuffer(kernelnensei0, "GXd", GXd0);
        bulletsComputeShader.SetBuffer(kernelnensei0, "GYd", GYd0);
        bulletsComputeShader.SetBuffer(kernelnensei0, "kabeX", kabeX);
        bulletsComputeShader.SetBuffer(kernelnensei0, "kabeY", kabeY);
        bulletsComputeShader.SetFloat("arufa", Const.CO.arufa);
        bulletsComputeShader.SetFloat("ar1fa", Const.CO.ar1fa);

        bulletsComputeShader.SetBuffer(kernelnensei1, "YU", YU);
        bulletsComputeShader.SetBuffer(kernelnensei1, "YUN", YUN);
        bulletsComputeShader.SetBuffer(kernelnensei1, "YV", YV);
        bulletsComputeShader.SetBuffer(kernelnensei1, "YVN", YVN);
        bulletsComputeShader.SetBuffer(kernelnensei1, "GXd", GXd0);
        bulletsComputeShader.SetBuffer(kernelnensei1, "GYd", GYd0);
        bulletsComputeShader.SetBuffer(kernelnensei1, "kabeX", kabeX);
        bulletsComputeShader.SetBuffer(kernelnensei1, "kabeY", kabeY);

        bulletsComputeShader.SetBuffer(kernelpressure0, "DIV", DIV);
        bulletsComputeShader.SetBuffer(kernelpressure0, "YPN", YPN);
        bulletsComputeShader.SetBuffer(kernelpressure0, "kabeP", kabeP);
        bulletsComputeShader.SetFloat("ALPHA", Const.CO.ALPHA);

        bulletsComputeShader.SetBuffer(kernelpressure1, "DIV", DIV);
        bulletsComputeShader.SetBuffer(kernelpressure1, "YPN", YPN);
        bulletsComputeShader.SetBuffer(kernelpressure1, "kabeP", kabeP);

        bulletsComputeShader.SetBuffer(kerneldiv, "DIV", DIV);
        bulletsComputeShader.SetBuffer(kerneldiv, "DIVexplorer", DIVexplorer);
        bulletsComputeShader.SetBuffer(kerneldiv, "YUN", YUN);
        bulletsComputeShader.SetBuffer(kerneldiv, "YVN", YVN);

        bulletsComputeShader.SetBuffer(kernelrhs, "YUN", YUN);
        bulletsComputeShader.SetBuffer(kernelrhs, "YVN", YVN);
        bulletsComputeShader.SetBuffer(kernelrhs, "YPN", YPN);
        bulletsComputeShader.SetBuffer(kernelrhs, "kabeX", kabeX);
        bulletsComputeShader.SetBuffer(kernelrhs, "kabeY", kabeY);

        bulletsComputeShader.SetBuffer(kernelveloc, "YUN", YUN);
        bulletsComputeShader.SetBuffer(kernelveloc, "YVN", YVN);
        bulletsComputeShader.SetBuffer(kernelveloc, "YVU", YVU);
        bulletsComputeShader.SetBuffer(kernelveloc, "YUV", YUV);
        bulletsComputeShader.SetBuffer(kernelveloc, "YTT", YTT);

        bulletsComputeShader.SetBuffer(kernelryuusi, "RYS", dtprtcomp.RYS);
        bulletsComputeShader.SetBuffer(kernelryuusi, "YUN", YUN);
        bulletsComputeShader.SetBuffer(kernelryuusi, "YVN", YVN);


        bulletsComputeShader.SetBuffer(kernelexpkabe, "kabePori", kabePori);
        bulletsComputeShader.SetBuffer(kernelexpkabe, "RES108", RES108);
        bulletsComputeShader.SetBuffer(kernelexpkabe, "YE", YE);
        bulletsComputeShader.SetBuffer(kernelexpkabe, "DIVexplorer", DIVexplorer);

        bulletsComputeShader.SetBuffer(kernelcomputebuffermemcopy_YE, "DATADST", YE);//書き込まれる側
        bulletsComputeShader.SetBuffer(kernelcomputebuffermemcopy_YE, "DATASRC", YEN);//書き込む参照側

        bulletsComputeShader.SetBuffer(kernelobjmapping, "StructureOfFloatObj", StructureOfFloatObj);
        bulletsComputeShader.SetBuffer(kernelobjmapping, "kabeP", kabeP);

        bulletsComputeShader.SetBuffer(kernelkabemapping, "kabeP_ro", kabeP);//read onlyの意味
        bulletsComputeShader.SetBuffer(kernelkabemapping, "StructureOfFloatObj_ro", StructureOfFloatObj);//read onlyの意味
        bulletsComputeShader.SetBuffer(kernelkabemapping, "kabeX", kabeX);
        bulletsComputeShader.SetBuffer(kernelkabemapping, "kabeY", kabeY);
        bulletsComputeShader.SetBuffer(kernelkabemapping, "YUN", YUN);
        bulletsComputeShader.SetBuffer(kernelkabemapping, "YVN", YVN);
        bulletsComputeShader.SetBuffer(kernelkabemapping, "GXU", GXU);
        bulletsComputeShader.SetBuffer(kernelkabemapping, "GXV", GXV);
        bulletsComputeShader.SetBuffer(kernelkabemapping, "GYU", GYU);
        bulletsComputeShader.SetBuffer(kernelkabemapping, "GYV", GYV);


        bulletsComputeShader.SetBuffer(kernelufopressure, "kabeP", kabeP);
        bulletsComputeShader.SetBuffer(kernelufopressure, "YPN", YPN);
        bulletsComputeShader.SetBuffer(kernelufopressure, "UFOE", UFOE);

        bulletsComputeShader.SetBuffer(kernelVorticityReduce, "YUN", YUN);
        bulletsComputeShader.SetBuffer(kernelVorticityReduce, "YVN", YVN);
    }

    //全部0.0fで埋める関数 at CPU
    public void FillBuffer_F(ComputeBuffer data)
    {
        float[] a = new float[data.count];
        for (int i = 0; i < data.count; i++)
        {
            a[i] = 0.0f;
        }
        data.SetData(a);
    }
    //全部0.0fで埋める関数 at CPU
    public void FillBuffer_F2(ComputeBuffer data)
    {
        Vector2[] a = new Vector2[data.count];
        for (int i = 0; i < data.count; i++)
        {
            a[i] = new Vector2(0.0f, 0.0f);
        }
        data.SetData(a);
    }
    //全部0(int32)で埋める関数 at CPU
    public void FillBuffer_I(ComputeBuffer data)
    {
        uint[] a = new uint[data.count];
        for (int i = 0; i < data.count; i++)
        {
            a[i] = 0;
        }
        data.SetData(a);
    }


    //vram同士のコピーuint限定
    //offset次第では書き込みオーバーフローも発生する
    public void CopyBufferToBuffer_i(ComputeBuffer datadst, ComputeBuffer datasrc, int size = 0, int dstoffset = 0, int srcoffset = 0)//この場合のsizeはbyteではなく配列数、offsetもそう
    {
        if (size == 0)
        {
            size = datadst.count;
        }
        bulletsComputeShader.SetInt("OFSET0", size);
        bulletsComputeShader.SetInt("OFSETDST", dstoffset);
        bulletsComputeShader.SetInt("OFSETSRC", srcoffset);
        bulletsComputeShader.SetBuffer(kernelcomputebuffermemcopy_i, "DATADSTI", datadst);
        bulletsComputeShader.SetBuffer(kernelcomputebuffermemcopy_i, "DATASRCI", datasrc);
        bulletsComputeShader.Dispatch(kernelcomputebuffermemcopy_i, (size + 63) / 64, 1, 1);
    }

    //vram同士のコピーfloat限定
    //offset次第では書き込みオーバーフローも発生する
    public void CopyBufferToBuffer_f(ComputeBuffer datadst, ComputeBuffer datasrc, int size = 0, int dstoffset = 0, int srcoffset = 0)//この場合のsizeはbyteではなく配列数、offsetもそう
    {
        if (size == 0)
        {
            size = datadst.count;
        }
        bulletsComputeShader.SetInt("OFSET0", size);
        bulletsComputeShader.SetInt("OFSETDST", dstoffset);
        bulletsComputeShader.SetInt("OFSETSRC", srcoffset);
        bulletsComputeShader.SetBuffer(kernelcomputebuffermemcopy_f, "DATADST", datadst);
        bulletsComputeShader.SetBuffer(kernelcomputebuffermemcopy_f, "DATASRC", datasrc);
        bulletsComputeShader.Dispatch(kernelcomputebuffermemcopy_f, (size + 63) / 64, 1, 1);
    }
    //vram同士のコピーfloat2限定
    //offset次第では書き込みオーバーフローも発生する
    public void CopyBufferToBuffer_f2(ComputeBuffer datadst, ComputeBuffer datasrc, int size = 0, int dstoffset = 0, int srcoffset = 0)//この場合のsizeはbyteではなく配列数、offsetもそう
    {
        if (size == 0)
        {
            size = datadst.count;
        }
        bulletsComputeShader.SetInt("OFSET0", size);
        bulletsComputeShader.SetInt("OFSETDST", dstoffset);
        bulletsComputeShader.SetInt("OFSETSRC", srcoffset);
        bulletsComputeShader.SetBuffer(kernelcomputebuffermemcopy_f2, "DATADST2", datadst);
        bulletsComputeShader.SetBuffer(kernelcomputebuffermemcopy_f2, "DATASRC2", datasrc);
        bulletsComputeShader.Dispatch(kernelcomputebuffermemcopy_f2, (size + 63) / 64, 1, 1);
    }

    //fill float版
    //offset次第では書き込みオーバーフローも発生する
    public void FillMem_f(ComputeBuffer datadst,int size,int dstoffset,float fcolor)//この場合のsizeはbyteではなく配列数、offsetもそう
    {
        bulletsComputeShader.SetInt("OFSET0", size);
        bulletsComputeShader.SetInt("OFSETDST", dstoffset);
        bulletsComputeShader.SetFloat("FCOLOR", fcolor);
        bulletsComputeShader.SetBuffer(kernelfillmem_f, "DATADST", datadst);
        bulletsComputeShader.Dispatch(kernelfillmem_f, (size + 63) / 64, 1, 1);
    }
    //fill uint版
    //offset次第では書き込みオーバーフローも発生する
    public void FillMem_ui(ComputeBuffer datadst, int size, int dstoffset,int uicolor)//この場合のsizeはbyteではなく配列数、offsetもそう
    {
        bulletsComputeShader.SetInt("OFSET0", size);
        bulletsComputeShader.SetInt("OFSETDST", dstoffset);
        bulletsComputeShader.SetInt("UICOLOR", uicolor);
        bulletsComputeShader.SetBuffer(kernelfillmem_ui, "DATADSTUI", datadst);
        bulletsComputeShader.Dispatch(kernelfillmem_ui, (size + 63) / 64, 1, 1);
    }




















    //YEにYENを配列コピー、なのだがただのコピーではなく減算処理、噴射時の熱量発生処理など施している。
    //配列コピーの後に上記処理施すより、メモリアクセス低減になっている
    void CopyBufferYE_YEN()
    {
        if ((Input.GetButton("Jump")) & (stgmngrcomp.keyflg > 0))
        {
            bulletsComputeShader.SetFloat("OBJ0POSX", ucomp.nozzle_rys_pos.x);
            bulletsComputeShader.SetFloat("OBJ0POSY", ucomp.nozzle_rys_pos.y);
        }else{
            bulletsComputeShader.SetFloat("OBJ0POSX", 900.0f);//実質噴射しないことと同じ
            bulletsComputeShader.SetFloat("OBJ0POSY", 900.0f);//実質噴射しないことと同じ
        }
        bulletsComputeShader.Dispatch(kernelcomputebuffermemcopy_YE, 1, Const.CO.WY, 1);
    }

    //スペース押したときに噴射する赤いやつ
    void NozzleFire()
    {
        //VRAMの更新
        bulletsComputeShader.SetBuffer(kernelryuusi_fire, "RYS", dtprtcomp.RYS);
        bulletsComputeShader.SetFloat("OBJ0POSX", ucomp.nozzle_rys_pos.x);
        bulletsComputeShader.SetFloat("OBJ0POSY", ucomp.nozzle_rys_pos.y);
        bulletsComputeShader.SetInt("OFSET0", refer.NOZZLEPARTICLENUM);
        bulletsComputeShader.SetInt("OFSET1", dtprtcomp.RYS.count);
        bulletsComputeShader.SetInt("OFSET2", dtprtcomp.write_idx);
        bulletsComputeShader.SetInt("OFSET3", (int)((cnt)%1234567)+ UnityEngine.Random.Range(0,423));//seed
        bulletsComputeShader.Dispatch(kernelryuusi_fire, (refer.NOZZLEPARTICLENUM+63)/64, 1, 1);
        //RYcの更新とindexの更新
        dtprtcomp.NozzleRYS(ucomp.nozzle_rys_pos.x - 1.0f, ucomp.nozzle_rys_pos.y - 1.0f, 2.0f, 2.0f);
    }

    void AnotherObj()
    {
        //まずufo
        ObjMapping(ucomp.ufo_vram, ucomp.ufo_spd, ucomp.ufo_pos, ucomp.ufo_rad, ucomp.ufo_radspd, 2);//次にufo mappin
        //キー入力で燃料噴射するなら噴射口もmappingしたい
        if ((Input.GetButton("Jump")) & (stgmngrcomp.keyflg > 0))
        {
            ObjMapping(ucomp.nozzle_vram, ucomp.nozzle_spd, ucomp.nozzle_pos, ucomp.ufo_rad, ucomp.ufo_radspd, 3);
        }
        //3つめ以降の壁
        for(int i=0;i< stgmngrcomp.fobjnum; i++)
        {
            Moveobj mcomp = stgmngrcomp.mvocomp[i];
            ObjMapping(mcomp.obj_vram, mcomp.obj_spd, mcomp.obj_pos, mcomp.obj_rad, mcomp.obj_radspd, mcomp.obj_id);
        }
        
    }

    void ObjMapping(ComputeBuffer vram,Vector2 spd,Vector3 pos, float rad, float rad_spd, int setval)
    {

        //bulletsComputeShader.SetBuffer(kernelobjmapping, "StructureOfFloatObj", StructureOfFloatObj);
        //bulletsComputeShader.SetBuffer(kernelobjmapping, "kabeP", kabeP);
        bulletsComputeShader.SetBuffer(kernelobjmapping, "OBJ0XY",vram);
        bulletsComputeShader.SetFloat("OBJ0POSX", pos.x);
        bulletsComputeShader.SetFloat("OBJ0POSY", pos.y);
        bulletsComputeShader.SetFloat("OBJ0SPDX", spd.x);
        bulletsComputeShader.SetFloat("OBJ0SPDY", spd.y);
        bulletsComputeShader.SetFloat("RAD", rad);
        bulletsComputeShader.SetFloat("RAD_SPD", rad_spd);
        bulletsComputeShader.SetInt("OFSET0", vram.count);
        bulletsComputeShader.SetInt("OFSET1", setval);
        bulletsComputeShader.Dispatch(kernelobjmapping, (vram.count + 63) / 64, 1, 1);

    }




    void OnDisable()
    {
        // コンピュートバッファは明示的に破棄しないと怒られます
        YU.Release();
        YUN.Release();
        GXU.Release();
        GYU.Release();
        YV.Release();
        YVN.Release();
        GXV.Release();
        GYV.Release();
        YE.Release();
        YEN.Release();
        GXE.Release();
        GYE.Release();
        GXd0.Release();
        GYd0.Release();
        GXd1.Release();
        GYd1.Release();
        GXd2.Release();
        GYd2.Release();
        YPN.Release();
        YUV.Release();
        YVU.Release();
        YTT.Release();
        DIV.Release();
        DIVexplorer.Release();
        kabeP.Release();
        kabePori.Release();
        kabeX.Release();
        kabeY.Release();
        RES108.Release();
        UFOE.Release();
        StructureOfFloatObj.Release();
    }



    int BoolToInt(bool b0)
    {
        if (b0 == true)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }

}


//壁仕様
//x速度、y速度を記憶するkabeX,kabeYは<=128で「壁」、>128で「流体」

//圧力を格納するkabePは <=64で壁なので内部の圧力は参照されない、>64かつ<=128で参照されるかつ自分の更新がない つまり圧力固定の吸収湧出壁となる、>128では参照も書き込みもされる普通の流体部分


//最適化するところ、cip法、ifをもっと大きくとる、壁マージンなんとかす
//マルチグリッド