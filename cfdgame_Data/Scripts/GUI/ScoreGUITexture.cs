using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Const;

public class ScoreGUITexture : MonoBehaviour
{
    public Texture2D normaltex;//easy normalなどのテキスト
    public Texture2D numtex;//数字のほう
    public Texture2D optintex;//続きからなど
    
    GameObject refObj0;
    TIMEGUI0 tmg0;
    GameObject refObj1;
    TIMEGUI1 tmg1;
    GameObject refObj3;
    TIMEGUI3 tmg3;
    GameObject refObj4;
    TIMEGUI4 tmg4;
    GameObject refObj6;
    TIMEGUI6 tmg6;
    GameObject refObj7;
    TIMEGUI7 tmg7;

    GameObject refObjf0;
    FPS0 fps0;
    GameObject refObjf1;
    FPS1 fps1;

    Ufo ucomp;
    Stagemanager stgmngrcomp;

    private int stagetime;

    float alfa_time_pa;
    public float alfa_time;
    public int alfa_time_flg;

    float alfa_fps_pa;
    public float alfa_fps;
    public int alfa_fps_flg;

    //float alfa_scr_pa;
    //public float alfa_scr;
    //public int alfa_scr_flg;

    private float m_updateInterval = 0.5f;
    int cnt;

    private float m_accum;
    private int m_frames;
    private float m_timeleft;
    private float m_fps;
    SpriteRenderer gage_time_sprite;

    void Start()
    {
        refObj0 = GameObject.Find("TIMEGUI0");
        tmg0 = refObj0.GetComponent<TIMEGUI0>();//コンポーネント
        refObj1 = GameObject.Find("TIMEGUI1");
        tmg1 = refObj1.GetComponent<TIMEGUI1>();//コンポーネント
        refObj3 = GameObject.Find("TIMEGUI3");
        tmg3 = refObj3.GetComponent<TIMEGUI3>();//コンポーネント
        refObj4 = GameObject.Find("TIMEGUI4");
        tmg4 = refObj4.GetComponent<TIMEGUI4>();//コンポーネント
        refObj6 = GameObject.Find("TIMEGUI6");
        tmg6 = refObj6.GetComponent<TIMEGUI6>();//コンポーネント
        refObj7 = GameObject.Find("TIMEGUI7");
        tmg7 = refObj7.GetComponent<TIMEGUI7>();//コンポーネント
        refObjf0 = GameObject.Find("FPS0");
        fps0 = refObjf0.GetComponent<FPS0>();//コンポーネント
        refObjf1 = GameObject.Find("FPS1");
        fps1 = refObjf1.GetComponent<FPS1>();//コンポーネント
        ucomp = GameObject.Find("ufo").GetComponent<Ufo>();//ufo コンポーネント
        stgmngrcomp = GameObject.Find("StageManager").GetComponent<Stagemanager>();//コンポーネント
        alfa_time = 1.0f;
        alfa_fps = 1.0f;
        alfa_time_pa = 0.0f;
        alfa_fps_pa = 0.0f;
        cnt = 0;
        gage_time_sprite = GameObject.Find("gage_time").GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    //ここで時間スプライト更新
    void Update()
    {
        //まずufoと近ければ薄くする
        float x = ucomp.ufo_pos.x + 10.0f * Mathf.Sin(ucomp.ufo_rad);
        float y = ucomp.ufo_pos.y - 10.0f * Mathf.Cos(ucomp.ufo_rad);
        alfa_time = Mathf.Min(
            Mathf.Clamp(Mathf.Sqrt(
            (x - 170.0f) * (x - 170.0f) + 8.0f * (y - 10.0f) * (y - 10.0f)
            ) / 27.0f - 0.8f, 0.17f, 1.0f),
            Mathf.Clamp(Mathf.Sqrt(
            (ucomp.ufo_pos.x - 170.0f) * (ucomp.ufo_pos.x - 170.0f) + 8.0f * (ucomp.ufo_pos.y - 10.0f) * (ucomp.ufo_pos.y - 10.0f)
            ) / 27.0f - 1.0f, 0.17f, 1.0f)
        );

        alfa_fps = Mathf.Min(
            Mathf.Clamp(Mathf.Sqrt(
            (x - 177.0f) * (x - 177.0f) + (y - 138.0f) * (y - 138.0f)
            ) / 15.0f - 0.1f, 0.2f, 1.0f),
            Mathf.Clamp(Mathf.Sqrt(
            (ucomp.ufo_pos.x - 177.0f) * (ucomp.ufo_pos.x - 177.0f) + (ucomp.ufo_pos.y - 138.0f) * (ucomp.ufo_pos.y - 138.0f)
            ) / 15.0f - 0.1f, 0.2f, 1.0f)
        );
        /*
        alfa_scr = Mathf.Clamp(Mathf.Sqrt(
            (ucomp.ufo_pos.x - 20.0f) * (ucomp.ufo_pos.x - 20.0f) + 11.0f * (ucomp.ufo_pos.y - 8.0f) * (ucomp.ufo_pos.y - 8.0f)
            ) / 28.0f - 0.4f, 0.34f, 1.0f);
        */
        if (alfa_time!= alfa_time_pa){alfa_time_flg = 1;}else{alfa_time_flg = 0; }
        if (alfa_fps != alfa_fps_pa) { alfa_fps_flg = 1; } else { alfa_fps_flg = 0; }
        //if (alfa_scr != alfa_scr_pa) { alfa_scr_flg = 1; } else { alfa_scr_flg = 0; }
        alfa_time_pa = alfa_time;
        alfa_fps_pa = alfa_fps;
        //alfa_scr_pa = alfa_scr;
        //ここまで

        //ここからは時間計算,適応
        stagetime = (int)(stgmngrcomp.stagetime*1000.0f);
        tmg7.num = (stagetime / 10) % 10;
        tmg6.num = (stagetime / 100) % 10;
        tmg4.num = ((stagetime / 1000) % 60) % 10;
        tmg3.num = ((stagetime / 1000) % 60) / 10;
        tmg1.num = ((stagetime / 60000) % 60) % 10;
        tmg0.num = ((stagetime / 60000) % 60) / 10;
        //ここまで時間計算

        ///////ここからはfps計算
        m_timeleft -= Time.deltaTime;
        m_accum += Time.timeScale / Time.deltaTime;
        m_frames++;
        if (0 >= m_timeleft)
        {
            m_fps = m_accum / m_frames;
            m_timeleft = m_updateInterval;
            m_accum = 0;
            m_frames = 0;
        }
        fps0.num = (int)(m_fps) / 10;
        fps1.num = (int)(m_fps) % 10;
        //////ここまで


        ////ここからは自分以外のスクリプトがついていないobjの透明度を設定するルーチン
        if ((alfa_time_flg == 1) | (cnt == 1))
        {
            gage_time_sprite.material.SetVector("_Intensity", new Color(1.0f, 1.0f, 1.0f, alfa_time));
        }
        ////ここまで

        cnt++;
    }

}
