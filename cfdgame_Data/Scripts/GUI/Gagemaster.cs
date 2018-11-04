using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gagemaster : MonoBehaviour
{
    public GameObject prefab;//矢印のやつ
    public GameObject scorefab;
    public GameObject prefab_scoreword;//スコアワードのほう
    Ufo ucomp;
    public float alfa_gage;
    public float alfa_gage_pa;
    public int alfa_gage_flg;
    // Use this for initialization
    public Vector3 gage_pos;

    GameObject[] scoreobj;
    Stagemanager stgmngrcomp;

    void Start()
    {
        alfa_gage = 0.0f;
        ucomp = GameObject.Find("ufo").GetComponent<Ufo>();//ufo コンポーネント
        stgmngrcomp = GameObject.Find("StageManager").GetComponent<Stagemanager>();//コンポーネント
        gage_pos = gameObject.transform.position+new Vector3(0.15f,0.17f,-0.0001f);
        // プレハブからインスタンスを生成
        for(int i = 0; i < 5; i++)
        {
            GameObject obj = (GameObject)Instantiate(prefab, gage_pos, Quaternion.identity);
            // 作成したオブジェクトを子として登録
            obj.transform.parent = transform;
            obj.GetComponent<Yajirushi>().yajino = i;
        }
        //スコアの8桁
        //スコアプレハブ生成
        scoreobj = new GameObject[8];
        for (int i = 0; i < 8; i++)
        {
            scoreobj[i] = Instantiate(scorefab, new Vector3(Const.CO.SCOREPOSX + 0.2f * i - 0.24f, Const.CO.SCOREPOSY, -0.0303f), Quaternion.identity);
            scoreobj[i].transform.parent = transform;
        }
        //スコアワード
        GameObject obj_scoreword = Instantiate(prefab_scoreword, new Vector3(-3.76f,-4.12f, -0.0303f), Quaternion.identity);
        obj_scoreword.transform.parent = transform;// 作成したオブジェクトを子として登録


    }
    // Update is called once per frame
    void Update()
    {
        float x = ucomp.ufo_pos.x + 10.0f * Mathf.Sin(ucomp.ufo_rad);
        float y = ucomp.ufo_pos.y - 10.0f * Mathf.Cos(ucomp.ufo_rad);
        alfa_gage = Mathf.Min(
            Mathf.Clamp(Mathf.Sqrt(
            (x - 40.0f) * (x - 40.0f) + (y - 125.0f) * (y - 125.0f)
            ) / 23.0f - 1.0f, 0.11f, 0.8f),
            Mathf.Clamp(Mathf.Sqrt(
            (ucomp.ufo_pos.x - 40.0f) * (ucomp.ufo_pos.x - 40.0f) + (ucomp.ufo_pos.y - 125.0f) * (ucomp.ufo_pos.y - 125.0f)
            ) / 23.0f - 1.0f, 0.11f, 0.8f)
        );

        if (alfa_gage != alfa_gage_pa) { alfa_gage_flg = 1; } else { alfa_gage_flg = 0; }
        alfa_gage_pa = alfa_gage;


        if (alfa_gage_flg == 1)
        {
            GetComponent<SpriteRenderer>().material.SetVector("_Intensity", new Color(1.0f, 1.0f, 1.0f, alfa_gage));
        }

        ///////ここからはスコア
        int sc = stgmngrcomp.stage_score;
        for (int i = 7; i >= 0; i--)
        {
            scoreobj[i].GetComponent<Score0>().num = sc % 10;
            sc = sc / 10;
        }
        ///////ここまで
    }
}