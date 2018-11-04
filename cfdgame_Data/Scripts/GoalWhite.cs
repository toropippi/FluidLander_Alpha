using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Const;

public class GoalWhite : MonoBehaviour {
    int cnt;
    public Vector3 pos=new Vector3(96.0f,72.0f,-0.025f);//自分の中心座標、流体側座標
    Vector3 subpos;//ufoとの差分
    //ここからは別ソースのload系
    GameObject refObjUfo;
    Ufo ucomp;
    public int goalflg=200;
    float alfa;
    float rad;
    public int scoreprefabflg=0;
    Stagemanager stgmngrcomp;
    SoundEffects SE;
    // Use this for initialization


    public void Start () {
        refObjUfo = GameObject.Find("ufo");
        ucomp = refObjUfo.GetComponent<Ufo>();//ufo コンポーネント
        stgmngrcomp = GameObject.Find("StageManager").GetComponent<Stagemanager>();//コンポーネント
        SE = GameObject.Find("SE").GetComponent<SoundEffects>();//コンポーネント
        cnt = 0;
        goalflg = Const.CO.GOALWAIT;
        scoreprefabflg = 0;
        alfa = 1.0f;
        GetComponent<SpriteRenderer>().material.SetVector("_Intensity", new Color(1.0f * alfa, 0.7f * alfa, 0.7f * alfa, 1.0f));
    }

    // Update is called once per frame
    void Update() {
        //まずは角度の設定
        cnt++;
        rad += 1.02f;//ラジアン角ではなく360度かくで計算
        rad -= 0.5f * (float)(Const.CO.GOALWAIT - goalflg);
        transform.rotation = Quaternion.Euler(0, 0, 0.01f * ((int)rad % 628) / Mathf.PI * 180.0f);
        //特殊ステージではゴールが移動している
        if (stgmngrcomp.nowstage == 18)
        {
            float mvrad = stgmngrcomp.mvocomp[0].obj_rad;
            pos.x = 90.0f - 44.0f * Mathf.Sin(mvrad + 3.33f);
            pos.y = 70.0f + 44.0f * Mathf.Cos(mvrad + 3.33f);
        }
        //次に座標確定 pos→objposに代入
        Setpos();

        //自分とufoの関係の処理
        subpos = ucomp.ufo_pos - pos;
        alfa = 1.0f;

        if ((subpos.x * subpos.x + subpos.y * subpos.y) < 49.0f)
        {//自分のgoalとufoが近ければ色が変わる
            alfa = Mathf.Clamp(0.03f * goalflg, 0.0f, alfa);
            GetComponent<SpriteRenderer>().material.SetVector("_Intensity", new Color(0.6f * alfa, 1.0f * alfa, 1.0f * alfa, 1.0f));
            //ufo減速
            ucomp.ufo_spd *= 0.96f;
            ucomp.ingoalflg = 1;//さらに風の影響を受けなくなる、ufoのゴールフラグたてる
            goalflg--;

            if (Mathf.Abs(ucomp.ufo_rad) < 1.3f)//ufoの角度が問題なければ
            {
                //ufoが正しい角度に近づいていく
                ucomp.ufo_rad = ucomp.ufo_rad * 0.92f;
            }
        }
        else
        {
            GetComponent<SpriteRenderer>().material.SetVector("_Intensity", new Color(1.0f * alfa, 0.7f * alfa, 0.7f * alfa, 1.0f));
            if (goalflg > 0) {
                goalflg = Const.CO.GOALWAIT;
            }
        }

        //フラグ管理、キー押してればフラグはConst.CO.GOALWAIT
        if (goalflg > 0) {
            if (Input.GetButton("Jump"))
            {
                goalflg = Const.CO.GOALWAIT;
            }
            if (Mathf.Abs(Input.GetAxis("Horizontal")) >= 0.8f)
            {
                goalflg = Const.CO.GOALWAIT;
            }
        }

        //キュイーンのSEならす
        if (goalflg == Const.CO.GOALWAIT-10)
        {
            SE.aS.PlayOneShot(SE.goal0);
        }


    }



    //クリアチェック
    public bool ClearCheck()
    {
        if (goalflg == 0 && scoreprefabflg == 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    public void Clear()
    {
        goalflg = -1;
        SE.aS.PlayOneShot(SE.goal1);
        //{//goalがscore画面を呼び出し、スコア画面がfadeoutを呼び出しfadeoutがstagemanagerを呼び出しステージ更新が行われる
        //Instantiate(scoreprefab, new Vector3(0.0f, 0.0f, -0.5f), Quaternion.identity);
        scoreprefabflg++;//念のため2回以上実行されないように
        //}
        //}
    }


    //自分のposを確定
    void Setpos()
    {
        Vector3 objpos;// = transform.position;
        objpos.x = (pos.x - 0.5f * Const.CO.WX) / 0.5f / Const.CO.WY * 5.0f;
        objpos.y = (0.5f * Const.CO.WY - pos.y) / 0.5f / Const.CO.WY * 5.0f;
        objpos.z = pos.z;
        transform.position = objpos;
    }

}
