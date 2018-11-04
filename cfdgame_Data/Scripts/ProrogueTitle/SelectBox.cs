using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectBox : MonoBehaviour
{
    public Texture2D tex;
    Sprite sprite;
    int cnt;
    public int select_x_id;
    int buttondown_now;
    int buttondown_log;
    float[] scalex = { 6.5f, 6.5f, 4.0f, 4.9f, 6.4f, 4.9f ,-1.0f};
    float[] scaley = { 1.8f,1.8f, 1.8f, 1.7f, 1.7f, 1.7f, -1.0f };
    float[] boxx = { -3.95f,0.9f,4.41f, -4.4f, -3.9f, -4.4f, -100.0f };
    float[] boxy = { -3.72f, -3.72f, -3.72f, 0.5f, -0.75f, -1.83f, -100.0f };
    GameObject gameobj_easy;
    GameObject gameobj_norm;
    GameObject gameobj_hard;
    public GameObject fadeOutfab;
    TitleFadeOut fadeoutcomp;
    Referobj robj;
    SoundEffects SE;
    int flg;
    int counter;//裏コマンド
    // Use this for initialization
    void Start()
    {
        SE = GameObject.Find("SE").GetComponent<SoundEffects>();//コンポーネント
        tex = new Texture2D(64,64, TextureFormat.RGBA32, false);
        //Texture2DからSpriteを作成
        sprite = Sprite.Create(
          texture: tex,
          rect: new Rect(0, 0, 64,64),
          pivot: new Vector2(0.5f, 0.5f)
        );
        for (int i = 0; i < 64; i++)
        {
            for (int j = 0; j < 64; j++)
            {
                tex.SetPixel(i, j, new Color(1.0f,1.0f,1.0f, 1.0f));
            }
        }
        tex.Apply();
        GetComponent<SpriteRenderer>().sprite = sprite;

        gameobj_easy = GameObject.Find("font02_96px_11");
        gameobj_easy.SetActive(false);
        gameobj_norm = GameObject.Find("font02_96px_12");
        gameobj_norm.SetActive(false);
        gameobj_hard = GameObject.Find("font02_96px_13");
        gameobj_hard.SetActive(false);

        cnt = 0;
        counter = 0;
        flg = 0;
        select_x_id = 0;
        buttondown_now = 0;
        buttondown_log = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //はじめから、つづきから、EXITの選択
        if (select_x_id<3 && select_x_id >= 0)
        {
            if ((Input.GetAxis("Horizontal") < -0.8f))
            {
                buttondown_now = -1;
                if (buttondown_log != -1)
                {
                    select_x_id--;
                    SE.aS.PlayOneShot(SE.pi1);
                    if (select_x_id == -1)
                    {
                        select_x_id = 2;
                    }
                }
            }
            else
            {
                if ((Input.GetAxis("Horizontal") > 0.8f))
                {
                    buttondown_now = 1;
                    if (buttondown_log != 1)
                    {
                        select_x_id++;
                        SE.aS.PlayOneShot(SE.pi1);
                        if (select_x_id == 3)
                        {
                            select_x_id = 0;
                        }
                    }
                }
                else
                {
                    buttondown_now = 0;
                }
            }
        }







        //難易度の選択
        if (select_x_id < 6 && select_x_id >= 3)
        {
            if ((Input.GetAxis("Vertical") < -0.8f))
            {
                buttondown_now = -1;
                if (buttondown_log != -1)
                {
                    select_x_id++;
                    SE.aS.PlayOneShot(SE.pi1);
                    if (select_x_id == 6)
                    {
                        select_x_id = 5;
                        counter++;
                        if (counter == 100)//100回HARDから下を入力するとでーたが消える
                        {
                            SE.aS.PlayOneShot(SE.exp1);
                            PlayerPrefs.DeleteAll();
                        }
                    }
                }
            }
            else
            {
                if ((Input.GetAxis("Vertical") > 0.8f))
                {
                    buttondown_now = 1;
                    if (buttondown_log != 1)
                    {
                        select_x_id--;
                        SE.aS.PlayOneShot(SE.pi1);
                        if (select_x_id == 2)
                        {
                            select_x_id = 3;
                        }
                    }
                }
                else
                {
                    buttondown_now = 0;
                }
            }
        }







        //決定
        if (Input.GetButtonDown("Jump"))
        {
            //つづきから
            if (select_x_id == 1)
            {
                if (CheckData()){
                    SE.aS.PlayOneShot(SE.pi2);
                    select_x_id = 6;
                    GameObject obj = Instantiate(fadeOutfab, new Vector3(0f, 0f, -1.2f), Quaternion.identity);
                    fadeoutcomp = obj.GetComponent<TitleFadeOut>();//
                    flg = 3;
                }
                else//セーブデータがないなら
                {
                    //ぶぶーの音ならす
                    SE.aS.PlayOneShot(SE.bubu);
                }
            }
            //EXIT
            if (select_x_id == 2)
            {
                SE.aS.PlayOneShot(SE.pi2);
                select_x_id = 6;
                GameObject obj = Instantiate(fadeOutfab, new Vector3(0f, 0f, -1.2f), Quaternion.identity);
                fadeoutcomp = obj.GetComponent<TitleFadeOut>();//
                flg = 2;
            }

            //EASY
            if (select_x_id == 3)
            {
                SE.aS.PlayOneShot(SE.pi2);
                select_x_id = 6;
                GameObject obj = Instantiate(fadeOutfab, new Vector3(0f, 0f, -1.2f), Quaternion.identity);
                fadeoutcomp = obj.GetComponent<TitleFadeOut>();//
                robj = GameObject.Find("referobj").GetComponent<Referobj>();//
                robj.difficulty = 0;
                flg = 1;
            }
            //NORMAL
            if (select_x_id == 4)
            {
                SE.aS.PlayOneShot(SE.pi2);
                select_x_id = 6;
                GameObject obj = Instantiate(fadeOutfab, new Vector3(0f, 0f, -1.2f), Quaternion.identity);
                fadeoutcomp = obj.GetComponent<TitleFadeOut>();//
                robj = GameObject.Find("referobj").GetComponent<Referobj>();//
                robj.difficulty = 1;
                flg = 1;
            }
            //HARD
            if (select_x_id == 5)
            {
                SE.aS.PlayOneShot(SE.pi2);
                select_x_id = 6;
                GameObject obj = Instantiate(fadeOutfab, new Vector3(0f, 0f, -1.2f), Quaternion.identity);
                fadeoutcomp=obj.GetComponent<TitleFadeOut>();//
                robj = GameObject.Find("referobj").GetComponent<Referobj>();//
                robj.difficulty = 2;
                flg = 1;
            }

            //はじめから
            if (select_x_id == 0)
            {
                SE.aS.PlayOneShot(SE.pi2);
                gameobj_easy.SetActive(true);
                gameobj_norm.SetActive(true);
                gameobj_hard.SetActive(true);
                select_x_id = 4;
            }

        }



        //キャンセルボタンの時
        if (Input.GetButtonDown("Cancel"))
        {
            if (select_x_id < 6 && select_x_id >= 3)
            {
                SE.aS.PlayOneShot(SE.pi2);
                gameobj_easy.SetActive(false);
                gameobj_norm.SetActive(false);
                gameobj_hard.SetActive(false);
                select_x_id = 0;
            }
        }




        //flg = 2のときはEXIT終了
        if (flg == 2)
        {
            if (fadeoutcomp.cnt > 90)
            {
                Application.Quit();
            }
        }

        //flg = 1のときはシーン終了
        if (flg == 1)
        {
            if (fadeoutcomp.cnt > 90)
            {
                SceneManager.LoadScene("prorogue");
            }
        }

        //3のときはつづきから
        if (flg == 3)
        {
            if (fadeoutcomp.cnt > 90)
            {
                SceneManager.LoadScene("stageselect");
            }
        }

        buttondown_log = buttondown_now;
        setpos();
        cnt++;
    }





    //つづきから始めるで
    //セーブデータが有効なものが一つでもあるかどうか
    bool CheckData()
    {
        int ckflg = 0;
        for (int i = 0; i < 3; i++)
        {
            if (PlayerPrefs.GetInt("maxstage" + i + "", -1) != -1)
            {
                ckflg = 1;
            }
            
            for (int j = 0; j < 18; j++)
            {
                if (PlayerPrefs.GetInt("score" + i + "e" + j + "", -1) != -1)
                {
                    ckflg = 1;
                }
            }
        }
        if (ckflg == 1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }





    void setpos()
    {
        Vector3 objpos;// = transform.position;
        objpos.x = boxx[select_x_id];
        objpos.y = boxy[select_x_id];
        objpos.z = -0.9f;
        transform.position = objpos;
        transform.localScale = new Vector3(scalex[select_x_id]+0.5f*Mathf.Sin(0.1f*cnt), scaley[select_x_id], 1.0f);
        GetComponent<SpriteRenderer>().material.SetVector("_Intensity", new Color(0.4f, 0.5f, 0.9f, 0.5f+0.2f* Mathf.Cos(0.167234f * cnt)));
    }
}
