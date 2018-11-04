using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreMaker : MonoBehaviour {
    public GameObject onespritefab;

    GameObject back;
    OneSprite backcomp;
    GameObject[] stageclear=new GameObject[6];//ステージクリア系
    OneSprite[] stageclearcomp = new OneSprite[6];//ステージクリア系コンポーネント
    GameObject[] numberobj = new GameObject[42];//数字系
    OneSprite[] numbercomp = new OneSprite[42];//数字系コンポーネント

    float[] posx = { 0.9f, -2.8f, -1.6f, -1.6f, -1.6f, -2.8f };
    float[] posy = { 3.8f, 2.5f, 1.1f, 0.2f, -0.7f, -2.5f };
    float[] scale = { 1.0f, 0.65f, 0.65f, 0.65f, 0.65f, 0.65f };
    float rightborder=4.0f;
    int index;
    int stagetime;
    public int cnt;
    Stagemanager stgmngrcomp;
    SoundEffects SE;

    // Use this for initialization
    void Start ()
    {
        stgmngrcomp = GameObject.Find("StageManager").GetComponent<Stagemanager>();//コンポーネント
        SE = GameObject.Find("SE").GetComponent<SoundEffects>();//コンポーネント
        index = 0;
        cnt = -30;

        //バックの半透明のやつ
        back = Instantiate(onespritefab, new Vector3(0f, 0f, -0.4f), Quaternion.identity);
        backcomp = back.GetComponent<OneSprite>();
        backcomp.Create(1,0,0,1300,923);
        backcomp.SetScaleF3(new Vector3(0.0f,0.93f,1.0f));
        SE.aS.PlayOneShot(SE.sscore0);
    }











    // Update is called once per frame
    void Update () {
        if (cnt>=0 && cnt <= 20)
        {
            backcomp.SetScaleF3(new Vector3(0.93f*cnt/20.0f, 0.93f, 1.0f));
        }

        if (cnt == 20)
        {
            SE.aS.PlayOneShot(SE.sscore1);
            Create0();
        }

        if (cnt == 40)
        {
            SE.aS.PlayOneShot(SE.sscore1);
            Create1();
        }
        if (cnt == 60)
        {
            SE.aS.PlayOneShot(SE.sscore1);
            Create2();
        }
        if (cnt == 80)
        {
            SE.aS.PlayOneShot(SE.sscore1);
            Create3();
        }
        if (cnt == 100)
        {
            SE.aS.PlayOneShot(SE.sscore1);
            Create4();
        }
        if (cnt == 130)
        {
            SE.aS.PlayOneShot(SE.sscore2);
            Create5();
        }

        cnt++;

    }






    void Create0()
    {
        //stageclear系
        for (int i = 0; i < 6; i++)
        {
            stageclear[i] = Instantiate(onespritefab, new Vector3(posx[i], posy[i], -0.41f), Quaternion.identity);
            stageclearcomp[i] = stageclear[i].GetComponent<OneSprite>();
            stageclearcomp[i].Create(2, 0, 126 * i, 800, 126);
            stageclearcomp[i].SetScale(scale[i]);
        }
        stageclearcomp[0].SetColor(new Color(24f / 255f, 222f / 255f, 180f / 255f, 1f));
        stageclearcomp[1].SetColor(new Color(1.0f, 129f / 255f, 0f, 1f));
        stageclearcomp[5].SetColor(new Color(1.0f, 129f / 255f, 0f, 1f));
    }


    void Create1()
    {
        //数字描画
        //クリアタイム
        stagetime = (int)(stgmngrcomp.stagetime * 1000.0f);
        int[] tmg = new int[6];
        tmg[0] = (stagetime / 10) % 10;
        tmg[1] = (stagetime / 100) % 10;
        tmg[2] = ((stagetime / 1000) % 60) % 10;
        tmg[3] = ((stagetime / 1000) % 60) / 10;
        tmg[4] = ((stagetime / 60000) % 60) % 10;
        tmg[5] = ((stagetime / 60000) % 60) / 10;
        //インスタンス化
        for (int i = 0; i < 6; i++)
        {
            numberobj[i] = Instantiate(onespritefab, new Vector3(rightborder - 0.3f * i - 0.18f * (int)(i / 2), posy[1], -0.41f), Quaternion.identity);
            numbercomp[i] = numberobj[i].GetComponent<OneSprite>();
            numbercomp[i].Create(0, 0, tmg[i] * 126, 100, 126);
            numbercomp[i].SetScale(0.75f);
            index++;
        }
        //「：」
        numberobj[6] = Instantiate(onespritefab, new Vector3(rightborder - 0.3f * 2 + 0.03f, posy[1], -0.41f), Quaternion.identity);
        numbercomp[6] = numberobj[6].GetComponent<OneSprite>();
        numbercomp[6].Create(3, 0, 0, 100, 126);
        numbercomp[6].SetScale(0.75f);
        index++;
        //「：」
        numberobj[7] = Instantiate(onespritefab, new Vector3(rightborder - 0.3f * 4 - 0.18f + 0.03f, posy[1], -0.41f), Quaternion.identity);
        numbercomp[7] = numberobj[7].GetComponent<OneSprite>();
        numbercomp[7].Create(3, 0, 0, 100, 126);
        numbercomp[7].SetScale(0.75f);
        index++;
    }

    void Create2()
    {
        //タイムボーナス
        int bonusscore = Mathf.Clamp(120000 - stagetime, 0, 120000);
        stgmngrcomp.stage_score += bonusscore;
        for (int i = 8; i < 14; i++)
        {
            numberobj[i] = Instantiate(onespritefab, new Vector3(rightborder - 0.3f * (i - 8), posy[2], -0.41f), Quaternion.identity);
            numbercomp[i] = numberobj[i].GetComponent<OneSprite>();
            numbercomp[i].Create(0, 0, (bonusscore % 10) * 126, 100, 126);
            numbercomp[i].SetScale(0.75f);
            bonusscore /= 10;
            if (bonusscore == 0)
            {
                index = i + 1;
                break;
            }
        }
        //「+」
        numberobj[index] = Instantiate(onespritefab, new Vector3(rightborder - 0.3f * (index - 8), posy[2], -0.41f), Quaternion.identity);
        numbercomp[index] = numberobj[index].GetComponent<OneSprite>();
        numbercomp[index].Create(3, 0, 378, 100, 126);
        numbercomp[index].SetScale(0.75f);
        index++;
    }

    void Create3()
    {
        //被ダメージ
        int hidamage = GameObject.Find("ufo").GetComponent<Ufo>().hidamage*250;//コンポーネント;
        stgmngrcomp.stage_score = Mathf.Clamp(stgmngrcomp.stage_score - hidamage, 0, 99999999);
        int ketasu = 0;
        //インスタンス化
        for (int i = 0; i < 8; i++)
        {
            numberobj[index] = Instantiate(onespritefab, new Vector3(rightborder - 0.3f * i, posy[3], -0.41f), Quaternion.identity);
            numbercomp[index] = numberobj[index].GetComponent<OneSprite>();
            numbercomp[index].Create(0, 0, (hidamage % 10) * 126, 100, 126);
            numbercomp[index].SetScale(0.75f);
            numbercomp[index].SetColor(new Color(1.0f, 0f, 0f, 1f));
            hidamage /= 10;
            index++;
            if (hidamage == 0)
            {
                ketasu = i + 1;
                break;
            }
        }
        //「-」
        numberobj[index] = Instantiate(onespritefab, new Vector3(rightborder - 0.3f * ketasu, posy[3], -0.41f), Quaternion.identity);
        numbercomp[index] = numberobj[index].GetComponent<OneSprite>();
        numbercomp[index].Create(3, 0, 504, 100, 126);
        numbercomp[index].SetScale(0.75f);
        numbercomp[index].SetColor(new Color(1.0f, 0f, 0f, 1f));
        index++;

    }

    void Create4()
    {
        //アイテムぼーなる
        int ketasu = 0;
        int itemscore = stgmngrcomp.rareenum * 100000;
        //インスタンス化
        for (int i = 0; i < 8; i++)
        {
            numberobj[index] = Instantiate(onespritefab, new Vector3(rightborder - 0.3f * i, posy[4], -0.41f), Quaternion.identity);
            numbercomp[index] = numberobj[index].GetComponent<OneSprite>();
            numbercomp[index].Create(0, 0, (itemscore % 10) * 126, 100, 126);
            numbercomp[index].SetScale(0.75f);
            itemscore /= 10;
            index++;
            if (itemscore == 0)
            {
                ketasu = i + 1;
                break;
            }
        }
        //「+」
        numberobj[index] = Instantiate(onespritefab, new Vector3(rightborder - 0.3f * ketasu, posy[4], -0.41f), Quaternion.identity);
        numbercomp[index] = numberobj[index].GetComponent<OneSprite>();
        numbercomp[index].Create(3, 0, 378, 100, 126);
        numbercomp[index].SetScale(0.75f);
        index++;

    }

    void Create5()
    {
        //トータルスコア
        int totalscore = stgmngrcomp.stage_score;
        for (int i = 0; i < 8; i++)
        {
            numberobj[index] = Instantiate(onespritefab, new Vector3(rightborder - 0.47f * i, posy[5] - 0.6f, -0.41f), Quaternion.identity);
            numbercomp[index] = numberobj[index].GetComponent<OneSprite>();
            numbercomp[index].Create(0, 0, (totalscore % 10) * 126, 100, 126);
            numbercomp[index].SetScale(1.1f);
            totalscore /= 10;
            index++;
            if (totalscore == 0)
            {
                break;
            }
        }

    }




    void OnDisable()
    {
        Destroy(back);
        for(int i = 0; i < 6; i++)
        {
            Destroy(stageclear[i]);
        }
        for (int i = 0; i < index; i++)
        {
            Destroy(numberobj[i]);
        }
    }


}
