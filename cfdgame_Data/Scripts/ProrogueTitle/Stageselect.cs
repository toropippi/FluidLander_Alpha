using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Stageselect : MonoBehaviour {

    Texture2D tex;
    Sprite sprite;
    int[,] stage_score = new int[3, 18];
    int[] maxstage = new int[3];
    int[] activedif;
    int cnt;
    int x;
    int y;
    int num;
    int flg = 0;
    int buttondown_now;
    int buttondown_log;
    GameObject obj;
    public GameObject fadeOutfab;
    TitleFadeOut fadeoutcomp;
    SoundEffects SE;
    // Use this for initialization
    void Start ()
    {
        SE = GameObject.Find("SE").GetComponent<SoundEffects>();//コンポーネント
        num =0;
        for(int i = 0; i < 3; i++)
        {
            maxstage[i]= PlayerPrefs.GetInt("maxstage" + i + "", 0);
            if (maxstage[i] != 0) { num++; }
            maxstage[i] = Mathf.Clamp(maxstage[i], 0, 18);
            for (int j = 0; j < 18; j++)
            {
                int score = PlayerPrefs.GetInt("score" + i + "e" + j + "", -1);
                stage_score[i, j] = score;
            }
        }
        activedif = new int[num];
        num = 0;
        for (int i = 0; i < 3; i++)
        {
            if (maxstage[i] != 0)
            {
                activedif[num] = i;
                num++;
            }
        }


        //Texture2DからSpriteを作成
        tex = new Texture2D(64, 64, TextureFormat.RGBA32, false);
        sprite = Sprite.Create(
          texture: tex,
          rect: new Rect(0, 0, 64, 64),
          pivot: new Vector2(0.5f, 0.5f)
        );
        for (int i = 0; i < 64; i++)
        {
            for (int j = 0; j < 64; j++)
            {
                tex.SetPixel(i, j, new Color(1.0f, 1.0f, 1.0f, 1.0f));
            }
        }
        tex.Apply();
        GetComponent<SpriteRenderer>().sprite = sprite;

        cnt = 0;
        x = 0;
        y = 0;
        flg = 0;
        buttondown_now = 0;
        buttondown_log = 0;
    }

	
	// Update is called once per frame
	void Update () {
        if (flg == 0)
        {
            buttondown_now = 0;
            if ((Input.GetAxis("Horizontal") < -0.8f))
            {
                buttondown_now = 1;
                if (buttondown_log != 1)
                {
                    SE.aS.PlayOneShot(SE.pi1);
                    x--;
                }
            }
            if ((Input.GetAxis("Horizontal") > 0.8f))
            {
                buttondown_now = 1;
                if (buttondown_log != 1)
                {
                    SE.aS.PlayOneShot(SE.pi1);
                    x++;
                }
            }
            x = (x+num) % num;

            if ((Input.GetAxis("Vertical") < -0.8f))
            {
                buttondown_now = 1;
                if (buttondown_log != 1)
                {
                    SE.aS.PlayOneShot(SE.pi1);
                    y++;
                }
            }
            if ((Input.GetAxis("Vertical") > 0.8f))
            {
                buttondown_now = 1;
                if (buttondown_log != 1)
                {
                    SE.aS.PlayOneShot(SE.pi1);
                    y--;
                }
            }
            y = (y+ maxstage[activedif[x]]) % maxstage[activedif[x]];


            //決定
            if (Input.GetButtonDown("Jump"))
            {
                SE.aS.PlayOneShot(SE.pi2);
                GameObject.Find("referobj").GetComponent<Referobj>().nowstage = y+1;//シーン切り替えで引き継ぐ変数の受け渡し
                GameObject.Find("referobj").GetComponent<Referobj>().difficulty = activedif[x];
                GameObject.Find("referobj").GetComponent<Referobj>().stage_score = stage_score[activedif[x], y];
                GameObject obj = Instantiate(fadeOutfab, new Vector3(0f, 0f, -1.2f), Quaternion.identity);
                fadeoutcomp = obj.GetComponent<TitleFadeOut>();//
                flg = 1;
            }

            //キャンセルボタンの時
            if (Input.GetButtonDown("Cancel"))
            {
                SE.aS.PlayOneShot(SE.pi2);
                flg = 2;
                GameObject obj = Instantiate(fadeOutfab, new Vector3(0f, 0f, -1.2f), Quaternion.identity);
                fadeoutcomp = obj.GetComponent<TitleFadeOut>();//
            }

            buttondown_log = buttondown_now;
        }
        else//すでに決定orキャンセルしていたら
        {
            //フェードアウト後にステージ変異
            if (fadeoutcomp.cnt == 91)
            {
                if (flg == 1) { SceneManager.LoadScene("SampleScene"); }
                if (flg == 2) { SceneManager.LoadScene("TitleScene"); }
            }
        }

        setpos();
        cnt++;
    }


    void setpos()
    {
        Vector3 objpos;// = transform.position;
        objpos.x = (activedif[x] * 200 + 170) / 60f + 0.6f- 20.0f / 3.0f;
        objpos.y = -(y * 28 + 67) / 60f - 0.25f + 5.0f; 
        objpos.z = -0.6f;
        transform.position = objpos;
        transform.localScale = new Vector3(3.9f + 0.5f * Mathf.Sin(0.1f * cnt), 0.9f, 1.0f);
        GetComponent<SpriteRenderer>().material.SetVector("_Intensity", new Color(0.4f, 0.5f, 0.9f, 0.5f + 0.2f * Mathf.Cos(0.167234f * cnt)));
    }

}
