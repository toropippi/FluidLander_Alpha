using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Const;
using UnityEngine.SceneManagement;

public class Stagemanager : MonoBehaviour {
    public int nowstage;
    public int veryfaststageFlg=0;//激流ステージフラグ
    public int stage_clear_flg;
    public int keyflg;
    public int difficulty;
    public int stage_score;
    public float stagetime;
    public GameObject goalprefab;
    public GameObject rareeprefab;
    public GameObject[] objprefab=new GameObject[5];//浮遊objのプレはbu
    public GameObject startfab;
    public GameObject stagenameprefab;
    public GameObject textfab;//会話マネージャーのfab
    public GameObject scorefab;//スコア画面のマネージャーのfab
    public GameObject fadeoutfab;//フェードアウトのfab
    GoalWhite[] glcomp;
    public Moveobj[] mvocomp;
    string[] textdata;
    int fadeoutObjflg;//スコア画面がでているとき、フェードアウトが出る前かあとか
    int goalnum;//現在のステージにいくつゴールがあるか
    public int rareenum;//現在のステージにいくつレアアースがあるか
    public int fobjnum;//現在のステージにいくつ浮遊オブジェクトがあるか
    public int fadeinflg;
    GameObject[] goalObj;
    GameObject[] rareeObj;
    GameObject[] objObj;
    GameObject textObj;
    GameObject scoreObj;
    GameObject fadeoutObj;
    Ufo ucomp;
    Frontstage frntcomp;
    backstage bckcomp;
    DotParticle dtprtcomp;
    LoadWallFromBMP ldwllinfcomp;
    MenyBullets mnybllts;
    StageSound SS;
    SoundEffects SE;
    TextController Txtccomp;
    // Use this for initialization
    void Start ()
    {
        ucomp = GameObject.Find("ufo").GetComponent<Ufo>();//ufo コンポーネント
        frntcomp = GameObject.Find("frontpng").GetComponent<Frontstage>();//コンポーネント
        bckcomp = GameObject.Find("backpng").GetComponent<backstage>();//コンポーネント
        dtprtcomp = GameObject.Find("nabie").GetComponent<DotParticle>();//コンポーネント
        ldwllinfcomp = GameObject.Find("nabie").GetComponent<LoadWallFromBMP>();//コンポーネント
        mnybllts = GameObject.Find("nabie").GetComponent<MenyBullets>();//コンポーネント
        SS= GameObject.Find("StageSound").GetComponent<StageSound>();//コンポーネント
        SE = GameObject.Find("SE").GetComponent<SoundEffects>();//コンポーネント

        //nowstage = 0;//今遊んでるステージ
        nowstage = GameObject.Find("referobj").GetComponent<Referobj>().nowstage;//シーン切り替えで引き継ぐ変数の受け渡し
        veryfaststageFlg=1;
        stage_clear_flg = 0;//クリア時にのみ1になる
        stage_score = 0;
        keyflg = 0;//キー操作を受け付けるかどうか、1受け付ける、0受け付けない。受け付けないときはufoの座標系さんもとまる
        fadeinflg = 0;//フェードインカウンター、開始が0で毎フレーム1増えていく。fade in後操作をしばらく受け付けない
        //difficulty = 0;//0はイージー、1がノーマル、2がハード
        difficulty = GameObject.Find("referobj").GetComponent<Referobj>().difficulty;//シーン切り替えで引き継ぐ変数の受け渡し
        stagetime = 0.0f;
        fadeoutObjflg = 0;

        //ゴール系
        glcomp = new GoalWhite[4];//ゴール設定数最大4
        goalObj = new GameObject[4];//ゴール設定数最大4
        goalnum = 1;//最初のステージだけここからインスタンス化
        goalObj[0] = Instantiate(goalprefab, new Vector3(1.0f * (96 - Const.CO.WX / 2) / (Const.CO.WY / 2) * 5.0f, 1.0f * (Const.CO.WY - 1 - 72 - Const.CO.WY / 2) / (Const.CO.WY / 2) * 5.0f, -0.02f), Quaternion.identity);
        glcomp[0] = goalObj[0].GetComponent<GoalWhite>();//ゴール0のスクリプトデータ取得

        //レアアース系
        rareenum = 0;
        rareeObj = new GameObject[6];//ゴール設定数最大6

        //浮遊obj系
        fobjnum = 0;
        objObj = new GameObject[3];//浮遊obj設定数最大3
        mvocomp= new Moveobj[3];
        
        //最初のステージだけはここからインスタンス化、ステージ開始時のフェードイン
        Instantiate(startfab, new Vector3(0.0f, 0.0f, -0.5f), Quaternion.identity);
        //最初のステージだけはここからインスタンス化、ステージ開始時の文字
        Instantiate(stagenameprefab, new Vector3(1.6f, 0.0f, -0.032f), Quaternion.identity);

        //このスクリプトが最後に実行されるためこれが可能になる
        if (nowstage != 0)
        {
            nowstage--;
            stage_score = GameObject.Find("referobj").GetComponent<Referobj>().stage_score;//シーン切り替えで引き継ぐ変数の受け渡し
            stageclear();
        }
    }

    


    // Update is called once per frame
    void Update() {
        //keyflg管理、これは毎フレーム必要
        keyflg = 1;
        for (int i=0;i< goalnum; i++)
        {
            //ゴール完了してれば
            if (glcomp[i].goalflg<=0)
            {
                keyflg = 0;
            }
        }

        //フェードイン時０から毎フレーム増えていく値
        if (fadeinflg < 90)
        {
            keyflg = 0;
            if (fadeinflg == 89)
            {//ある時間になると会話が始まる
                textObj = Instantiate(textfab, new Vector3(0, 0, 0), Quaternion.identity);
                Txtccomp = textObj.GetComponent<TextController>();
            }
        }
        //もしまだ会話中だったら
        if (textObj != null)
        {
            keyflg = 0;
        }
        //死に戻りする最中ならば
        if (ucomp.deathcounter != 0)
        {
            keyflg = 0;
        }

        
        //デバッグ用、キー押すと次のステージに
        if (keyflg == 1)
        {
            if (Input.GetButton("1"))
            {
                stage_clear_flg = 1;
            }
            if (Input.GetButton("2"))
            {
                stage_clear_flg = 1;
                nowstage -= 2;
            }
        }
        



        //ほかのスクリプトからstage_clear_flgが更新されるとnowstageが次になる
        if (stage_clear_flg != 0)
        {
            stageclear();
        }

        //激流ステージだけ粒子の色に特殊効果が表れる
        if (veryfaststageFlg == 0)
        {
            if (fadeinflg>100 && Txtccomp.currentLine >=3)
            {
                veryfaststageFlg = 1;
            }
        }

        //タイム管理
        if (keyflg == 1)
        {
            stagetime += 0.0166666666666667f;
        }

        //レアアース全部取るとゴール出現






        //ゴールにゴールしていればスコア画面出す
        for (int i = 0; i < goalnum; i++)
        {
            if (glcomp[i].ClearCheck())
            {
                glcomp[i].Clear();
                if (nowstage != 0)
                {
                    scoreObj = Instantiate(scorefab, new Vector3(0.0f, 0.0f, -0.4f), Quaternion.identity);
                }
                else
                {
                    fadeoutObj = Instantiate(fadeoutfab, new Vector3(0.0f, 0.0f, -0.5f), Quaternion.identity);
                }
            }
        }

        //スコア画面が一定時間をすぎてボタンを押せばフェードアウト生成
        if (scoreObj != null)
        {
            if (scoreObj.GetComponent<ScoreMaker>().cnt >= 210 && Input.GetButtonDown("Jump"))
            {
                if (fadeoutObjflg==0)
                {
                    //こいつは死ぬとき自動でフェードインをインスタンス化する
                    fadeoutObj = Instantiate(fadeoutfab, new Vector3(0.0f, 0.0f, -0.5f), Quaternion.identity);
                    fadeoutObjflg = 1;
                }
            }
            //フェードアウトがおわったら消す
            if (scoreObj.GetComponent<ScoreMaker>().cnt > 210 && fadeoutObjflg == 1)
            {
                if (fadeoutObj == null)
                {
                    Destroy(scoreObj);
                    scoreObj = null;
                    fadeoutObjflg = 0;
                }
            }
        }







        //まだゴールが出現してなければ
        if (goalnum == 0 && rareenum != 0)
        {
            int couto = 0;
            for (int i = 0; i < rareenum; i++)
            {
                if (rareeObj[i] == null)
                {
                    couto++;
                }
            }
            if (couto == rareenum)//全部取っていたら
            {
                SE.aS.PlayOneShot(SE.puu);
                CreateGoal();//ゴール生成
            }
        }


        fadeinflg++;
    }



















    //ステージクリアして次ステージの読み込み
    void stageclear()
    {
        keyflg = 0;
        stage_clear_flg = 0;
        //まずは過去のstageのもろもろを削除
        for (int i = 0; i < goalnum; i++)//ゴール削除
        {
            Destroy(goalObj[i]);
        }
        goalnum = 0;
        for (int i = 0; i < rareenum; i++)//レアアース削除
        {
            Destroy(rareeObj[i]);
        }
        rareenum = 0;
        for (int i = 0; i < fobjnum; i++)//浮遊オブジェクト削除
        {
            Destroy(objObj[i]);
        }
        fobjnum = 0;



        //ステージ情報を読み込み
        nowstage++;if (nowstage == 9) { veryfaststageFlg = 0; }
        Save();
        if (nowstage == 19)
        {
            SceneManager.LoadScene("Ending");
            nowstage--;
        }
        
        stagetime = 0.0f;
        Debug.Log("newstage=" + nowstage + "");
        TextAsset textasset = Resources.Load<TextAsset>("data" + nowstage + "");
        string TextLines = textasset.text;
        textdata = TextLines.Split('\n');//1行ずつに分割

        //各スクリプトに新規stageのloadを
        mnybllts.FillComputeBuffer();
        ldwllinfcomp.stages_speed = float.Parse(textdata[92 + 2 * difficulty]);//ステージごとのスピード倍率設定
        ldwllinfcomp.LoadWallInfo("" + nowstage);//これで流体側は読み込まれた
        mnybllts.FillBuffer_F2(dtprtcomp.RYS);//RYSに0.0代入、色のほうはまだ
        dtprtcomp.ResetSomeofRYS(int.Parse(textdata[98]));//粒子座標初期化が何フレームで１周するかの倍速。中でsomeofRYSのvram(compute buffer)が再生成されている
        ldwllinfcomp.LoadWallInfo2("" + nowstage);//これで粒子側は読み込まれた
        dtprtcomp.someofRYS.Floatload();//登録座標をfloatに変換and展開
        dtprtcomp.someofRYS.ShuffleAndToVram();//クラス内部のvram変数に転送
                                               //dtprtcomp.SetRYSC();//これで粒子の初期化も官僚
        frntcomp.LoadNewStage(nowstage);//front stageも読み込み
        bckcomp.LoadNewStage(nowstage);//front stageも読み込み
                                       //粒子関連としてRYcを初期化
        mnybllts.FillBuffer_I(dtprtcomp.RYc);

        //ufoの位置初期化
        ucomp.ufo_pos.x = float.Parse(textdata[0]);
        ucomp.ufo_pos.y = float.Parse(textdata[2]);
        ucomp.ufo_restart_pos.x = ucomp.ufo_pos.x;
        ucomp.ufo_restart_pos.y = ucomp.ufo_pos.y;
        ucomp.ufo_spd = new Vector2(0.0f, 0.0f);
        ucomp.grav = float.Parse(textdata[74 + 2 * difficulty]);//重力
        ucomp.ufo_rad = 0.0f;//角度初期化
                             //噴射力の設定
        ucomp.jumpf = float.Parse(textdata[80 + 2 * difficulty]);//噴射力
                                                                 //流れの影響
        ucomp.pullf = float.Parse(textdata[86 + 2 * difficulty]);//流れのufoに対する影響
        ucomp.forcetoufolimit = float.Parse(textdata[100]);//ufoがうける外力最大値
        ucomp.hitpoint = 128.0f;
        ucomp.fastkey = 0;
        ucomp.hidamage = 0;

        //オブジェクト類
        CreateRareearth();
        if (rareenum == 0)//最初からレアアースがないステージなら
        {
            CreateGoal();
        }
        CreateFloatObj();

        //フェードインスプライトはフェードアウト側から直接インスタンス化される
        fadeinflg = 0;
        //stage1 なんとかなんとかっていうスプライトobj生成
        Instantiate(stagenameprefab, new Vector3(1.6f, 0.0f, -0.032f), Quaternion.identity);
        //こいつは自動で消える
    }


    //レアアース生成
    void CreateRareearth()
    {
        int rarex = 0;
        int rarey = 0;
        int rareflg = 0;
        rareenum = 0;
        for (int i = 0; i < 6; i++)
        {
            rareflg = int.Parse(textdata[20 + i * 6]);//生存フラhぐ//レアアースの状態(0は存在しない、1は存在する。0はセーブポイントでない、2はセーブポイントあり。0は銀色、4は金色。)
            rarex = int.Parse(textdata[22 + i * 6]);
            rarey = int.Parse(textdata[24 + i * 6]);
            if (rareflg != 0)
            {
                rareeObj[rareenum] = Instantiate(rareeprefab, new Vector3(1.0f * (rarex - Const.CO.WX / 2) / (Const.CO.WY / 2) * 5.0f, 1.0f * (Const.CO.WY - 1 - rarey - Const.CO.WY / 2) / (Const.CO.WY / 2) * 5.0f, -0.015f), Quaternion.identity);
                rareenum++;
            }
        }
    }


    //goal生成
    void CreateGoal()
    {
        int glx = 0;
        int gly = 0;
        for (int i = 0; i < 4; i++)
        {
            glx = int.Parse(textdata[4 + i * 4]);
            gly = int.Parse(textdata[6 + i * 4]);
            if (glx + gly != 0)
            {
                goalObj[goalnum] = Instantiate(goalprefab, new Vector3(1.0f * (glx - Const.CO.WX / 2) / (Const.CO.WY / 2) * 5.0f, 1.0f * (Const.CO.WY - 1 - gly - Const.CO.WY / 2) / (Const.CO.WY / 2) * 5.0f, -0.025f), Quaternion.identity);
                glcomp[goalnum] = goalObj[goalnum].GetComponent<GoalWhite>();//ゴール0のスクリプトデータ取得
                glcomp[goalnum].pos = new Vector3((float)glx, (float)gly, -0.025f);
                glcomp[goalnum].Start();
                goalnum++;
            }
        }
    }


    //浮遊obj生成
    void CreateFloatObj()
    {
        fobjnum = 0;
        int objflg = 0;
        int fobjx = 0;
        int fobjy = 0;
        for (int i = 0; i < 3; i++)
        {
            objflg = int.Parse(textdata[56 + i * 6]);//生存フラhぐ、かつid
            fobjx = int.Parse(textdata[58 + i * 6]);//生成x座標
            fobjy = int.Parse(textdata[60 + i * 6]);//生成y座標
            if (objflg != 0)
            {
                //最初は見えないところに生成
                objObj[fobjnum] = Instantiate(objprefab[objflg - 1], new Vector3(1.0f * (fobjx - Const.CO.WX / 2) / (Const.CO.WY / 2) * 5.0f, 1.0f * (Const.CO.WY - 1 - fobjy - Const.CO.WY / 2) / (Const.CO.WY / 2) * 5.0f, -0.001f), Quaternion.identity);
                mvocomp[fobjnum] = objObj[fobjnum].GetComponent<Moveobj>();//浮遊オブジェクトのスクリプトデータ取得
                mvocomp[fobjnum].obj_id = 4 + fobjnum;//set_val設定。これはkabeに書き込まれるidになる
                mvocomp[fobjnum].obj_bmp_id = objflg - 1;
                mvocomp[fobjnum].obj_pos.x = 1.0f * fobjx;
                mvocomp[fobjnum].obj_pos.y = 1.0f * fobjy;
                fobjnum++;
            }
        }
    }



    //ステージ交換時セーブ
    void Save()
    {
        int i = PlayerPrefs.GetInt("maxstage" + difficulty + "",-1);
        if (i< nowstage)
        {
            i = nowstage;
            PlayerPrefs.SetInt("maxstage" + difficulty + "", i);
        }

        i = PlayerPrefs.GetInt("score" + difficulty + "e" + nowstage + "", -1);
        if (i < stage_score)
        {
            i = stage_score;
            PlayerPrefs.SetInt("score" + difficulty + "e" + nowstage + "", stage_score);
        }
        PlayerPrefs.SetInt("difficulty", difficulty);
    }
    
}
