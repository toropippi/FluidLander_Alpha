using UnityEngine;
using System.Runtime.InteropServices;
using System.Linq;
using System;
using System.IO;
using Const;

public class DotParticle : MonoBehaviour {
    
    //ここではRYS以外に、RYSに書き込み初期化座標を記憶した変数も用意する
    //これは毎フレーム書き込みoffsetをずらしていくが、読み込みoffsetもずらしていく
    //読み込みのoffsetがずれないと毎フレーム全く同じ座標から粒子が発生することになり見栄えが悪い
    //書き込みは1024フレームで1週、読み込みはXフレームで1週
    public class SomeofRYS
    {
        struct xyn//12byteずつのアラインメントは少し気持ち悪いが仕方なし
        {
            public int X;
            public int Y;
            public int N;//湧出頻度
        }
        xyn[] dataxy;//湧出座標を整数で保存しておく変数、かつそこの座標で湧出頻度がどのくらいかも
        Vector2[] datap;//実際の湧出座標を少数で記録しておく変数
        Vector2[] dataq;//pのシャッフル版
        int datap_len;
        public int dataxy_len;
        int reqminnum;
        public int read_idx;
        public int funcloop_per_rysframe;//１粒子フレームあたり何回粒子座標初期化関数を実行するか
        public ComputeBuffer vram;//湧出座標を保存したVARM

        //コンストラクタ
        public SomeofRYS(int i0)
        {
            funcloop_per_rysframe = i0;
            read_idx = 0;
            datap = new Vector2[Const.CO.WX * Const.CO.WY * 128 / 4];//4MB,この数字に特に意味はない。だいたいWX*WY*128あれば十分すぎるので/4している
            datap_len = 0;
            dataxy = new xyn[Const.CO.WX * Const.CO.WY];//だいたいWX*WYあれば十分すぎる
            dataxy_len = 0;
            reqminnum = GameObject.Find("referobj").GetComponent<Referobj>().PARTICLERONEFRAME * Const.CO.PARTICLEREADDIV* funcloop_per_rysframe;//1粒子フレームあたりConst.CO.PARTICLERONEFRAME*funcloop_per_rysframe個の粒子が座標初期化される。初期化位置の参照箇所はConst.CO.PARTICLEREADDIV回で１週する
            dataq = new Vector2[reqminnum];//
            vram = new ComputeBuffer(reqminnum, 2 * Marshal.SizeOf(typeof(float)));//粒子の座標ｘ、ｙ
        }
        //
        //int座標をここで登録。重複なしだが、重複回数はNで記憶しておく
        public void Inset(int x, int y, int num)
        {
            xyn tmpxyn;
            tmpxyn.X = x;
            tmpxyn.Y = y;
            tmpxyn.N = num;
            dataxy[dataxy_len] = tmpxyn;
            dataxy_len++;
        }

        //座標を指定して登録削除できる
        public void Delset(int x, int y)
        {
            xyn tmpxyn;
            for (int i = 0; i < dataxy_len; i++)
            {
                tmpxyn = dataxy[i];
                if ((tmpxyn.X == x) & (tmpxyn.Y == y))
                {
                    //i番目を消して上に詰める
                    for (int j = i; j < dataxy_len - 1; j++)
                    {
                        dataxy[j] = dataxy[j + 1];
                    }
                    //詰め終わったら
                    dataxy_len--;
                    i--;
                }
            }
        }
        //insetしたあとに呼び出すと、N重複も考慮しfloatに展開。int→float変換時にランダムで小数点以下が付与される
        public void Floatload()
        {
            datap_len = 0;
            for (int i = 0; i < dataxy_len; i++)
            {
                xyn tmpxyn = dataxy[i];
                for (int g = 0; g < tmpxyn.N; g++)
                {
                    datap[datap_len] = new Vector2(0.01f * UnityEngine.Random.Range(0, 99) + tmpxyn.X, 0.01f * UnityEngine.Random.Range(0, 99) + tmpxyn.Y);
                    datap_len++;
                }
            }
            //これでfloat展開できた
        }

        //datap（少数座標）のをshuffleしてdataqに格納。そのさい、dataq全部が必要data数に満たない場合適宜複製する。
        //さらにVRAMに転送する
        public void ShuffleAndToVram()
        {
            //重複なし抽出
            var ary = Enumerable.Range(0, datap_len).OrderBy(n => Guid.NewGuid()).Take(datap_len).ToArray();
            for (int s = 0; s < reqminnum; s++)
            {
                dataq[s] = datap[ary[s % datap_len]];//ここでdatap_len<reqminnumなら実質複製していることになる。逆にdatap_len>reqminnumなら間引きしていることになる
            }
            vram.SetData(dataq, 0, 0, reqminnum);//ここでcpu→gpu
        }
    }




    public SomeofRYS someofRYS;
    public ComputeBuffer RYS;
    public ComputeBuffer RYc;
    public int write_idx;
    MenyBullets menybulletscomp;
    Stagemanager stgmngrcomp;
    Referobj refer;
    int cnt;
    int ryc_read_idx;
    int[] rycolor;
    


    // Use this for initialization
    void Start () {
        menybulletscomp = GetComponent<MenyBullets>();//コンポーネント
        stgmngrcomp = GameObject.Find("StageManager").GetComponent<Stagemanager>();//コンポーネント
        refer = GameObject.Find("referobj").GetComponent<Referobj>();//コンポーネント
        someofRYS = new SomeofRYS(1); //毎フレーム粒子が一部初期化されていくやつ
        write_idx=0;
        //someofRYSがインスタンス化できているのでload wallする
        GetComponent<LoadWallFromBMP>().LoadWallInfo2("0");
        cnt = 0;
        RYcmaker_start();//RYc関係
    }

    // Update is called once per frame
    void Update()
    {
        if (cnt == 0) { 
            // そのあとRYS配列とRYcに初期値を代入する//ただしMenybuleetsのstartがすべて完了しているのが前提
            SetRYSC();
        }
        cnt++;
    }


    //RYSの中身を更新して、書き込みindexと読み込みindexを更新
    public void UpdateOFST()
    {
        int col = RYcmaker();
        int copysize = refer.PARTICLERONEFRAME * someofRYS.funcloop_per_rysframe;
        
        //ここで、粒子の一部の座標を初期化
        if ((RYS.count-write_idx)>= copysize)//書き込みでオーバーフローしなければ
        {
            menybulletscomp.CopyBufferToBuffer_f2(RYS, someofRYS.vram, copysize, write_idx, someofRYS.read_idx);
            menybulletscomp.FillMem_ui(RYc, copysize, write_idx, col);
        }
        else//書き込みでオーバーフローしそうなら
        {
            int dsize = RYS.count - write_idx;
            menybulletscomp.CopyBufferToBuffer_f2(RYS, someofRYS.vram, dsize, write_idx, someofRYS.read_idx);
            menybulletscomp.CopyBufferToBuffer_f2(RYS, someofRYS.vram, copysize - dsize, 0, someofRYS.read_idx+dsize);
            menybulletscomp.FillMem_ui(RYc, dsize, write_idx, col);
            menybulletscomp.FillMem_ui(RYc, copysize - dsize, 0 , col);
        }//なお読み込みのほうでオーバーフローはしない前提で設計されている

        write_idx          = (write_idx          + copysize) % (refer.PARTICLENUM);
        someofRYS.read_idx = (someofRYS.read_idx + copysize) % (copysize * Const.CO.PARTICLEREADDIV);
    }
    

    // RYS配列とRYcに初期値を代入する
    public void SetRYSC()
    {
        //SomeofRYSも生成
        someofRYS.Floatload();//登録座標をfloatに変換and展開
        someofRYS.ShuffleAndToVram();//クラス内部のvram変数に転送
        //1024回、RYSに適応して全RYS vramを埋め尽くす
        for (int i = 0; i < Const.CO.PARTICLEWRITEDIV; i++)
        {
            UpdateOFST();
        }

        uint[] rycs = new uint[RYc.count];
        for (int i = 0; i < RYS.count; i++)
        {
            rycs[i] = (uint)(RYcmaker());
        }
        // バッファに適応
        RYc.SetData(rycs);
    }

    //のずる噴射口からでる粒子の設定
    public void NozzleRYS(float leftx,float upy,float scalex,float scaley)
    {
        //RYｃのほう。//書き込みでオーバーフローしなければ
        if ((RYS.count - write_idx) >= refer.NOZZLEPARTICLENUM){
            menybulletscomp.FillMem_ui(RYc, refer.NOZZLEPARTICLENUM, write_idx, Const.CO.PARTICLECOLOR_NOZ1);
        }else{//書き込みでオーバーフローしそうなら
            int dsize = RYS.count - write_idx;
            menybulletscomp.FillMem_ui(RYc, dsize, write_idx, Const.CO.PARTICLECOLOR_NOZ1);
            menybulletscomp.FillMem_ui(RYc, refer.NOZZLEPARTICLENUM - dsize,0, Const.CO.PARTICLECOLOR_NOZ1);
        }//なお読み込みのほうでオーバーフローはしない前提で設計されている
        write_idx = (write_idx + refer.NOZZLEPARTICLENUM) % refer.PARTICLENUM;
    }

    //ufoが爆発した時の粉生成
    public void ExpRYS(float ufox,float ufoy)
    {
        int copysize = refer.EXPPARTICLE;
        Vector2[] exp_xy = new Vector2[copysize];//粒子発生座標をcpuで計算
        for(int i=0;i< copysize; i++)
        {
            exp_xy[i] = new Vector2(ufox - 3.0f + 6.0f * UnityEngine.Random.Range(0.0f, 1.0f), ufoy - 3.0f + 6.0f * UnityEngine.Random.Range(0.0f, 1.0f));
        }

        //ここで、粒子の一部の座標を初期化
        if ((RYS.count - write_idx) >= copysize)//書き込みでオーバーフローしなければ
        {
            RYS.SetData(exp_xy, 0, write_idx, copysize);
            menybulletscomp.FillMem_ui(RYc, copysize, write_idx, Const.CO.PARTICLECOLOR_NOZ1);
        }
        else//書き込みでオーバーフローしそうなら
        {
            int dsize = RYS.count - write_idx;
            RYS.SetData(exp_xy, 0, write_idx, dsize);
            RYS.SetData(exp_xy, dsize, 0, copysize - dsize);
            menybulletscomp.FillMem_ui(RYc, dsize, write_idx, Const.CO.PARTICLECOLOR_NOZ1);
            menybulletscomp.FillMem_ui(RYc, copysize - dsize, 0, Const.CO.PARTICLECOLOR_NOZ1);
        }//なお読み込みのほうでオーバーフローはしない前提で設計されている

        write_idx = (write_idx + copysize) % (refer.PARTICLENUM);
    }

    //粒子の色メーカー
    void RYcmaker_start() {
        ryc_read_idx = 0;
        rycolor=new int[1024];
        int r = 0;
        int g = 0;
        int b = 0;
        for (int i = 0; i < 1024; i++)
        {
            b = UnityEngine.Random.Range(2, 256);
            g = UnityEngine.Random.Range(2, 256- b*6/7);
            r = UnityEngine.Random.Range(2, 256- (g+b)*2/3);
            rycolor[i] = b*65536+ g*256+ r;
        }
    }

    //粒子の色メーカー、適宜色を返す
    int RYcmaker()
    {
        int c;
        ryc_read_idx++;
        if (ryc_read_idx >= 1024*600) { ryc_read_idx = 0; }
        c= rycolor[ryc_read_idx / 600];
        //ただし激流ステージのある条件を満たす場合色が透明になる
        c *= stgmngrcomp.veryfaststageFlg;
        return c;
    }

    //someofRYSをリセット再生成
    public void ResetSomeofRYS(int i0)
    {
        someofRYS.vram.Release();
        someofRYS = null;
        someofRYS = new SomeofRYS(i0); //毎フレーム粒子が一部初期化されていくやつ
    }


    void OnDisable()
    {
        // コンピュートバッファは明示的に破棄しないと怒られます
        RYS.Release();
        RYc.Release();
        someofRYS.vram.Release();
    }
}

