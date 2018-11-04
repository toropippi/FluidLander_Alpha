using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ProrogueManeger : MonoBehaviour
{
    public GameObject textfab;//会話マネージャーのfab
    GameObject textObj;
    TextController textcomp;
    public GameObject fadeoutfab;
    public GameObject fadeinfab;
    GameObject foobj;
    GameObject fiobj;
    BackImage backimagecomp;
    BackImage chara0comp;
    BackImage chara1comp;
    BackImage imageboxcomp;
    int changeflg;
    int cl;
    int mcl;
    int prrgcount;//プロローグ内でのフレーム的な0-2


    void Start()
    {
        backimagecomp = GameObject.Find("backimage").GetComponent<BackImage>();
        chara0comp = GameObject.Find("chara0").GetComponent<BackImage>();
        chara1comp = GameObject.Find("chara1").GetComponent<BackImage>();
        imageboxcomp = GameObject.Find("imagebox").GetComponent<BackImage>();
        textObj = null;
        //textObj = Instantiate(textfab, new Vector3(0, 0, 0), Quaternion.identity);
        //textcomp = textObj.GetComponent<TextController>();//コンポーネント
        //textcomp.stage = 100;
        //textcomp.Create();
        changeflg = 0;
        cl = -1;
        mcl = -1;
        if (SceneManager.GetActiveScene().name == "Ending")
        {//エンディングなら
            prrgcount = 100;
            backimagecomp.Create(3, 1);//0の背景を表示
        }
        else
        {//プロローグなら
            prrgcount = 0;
            backimagecomp.Create(0, 1);//0の背景を表示
        }
        foobj = null;
        fiobj = Instantiate(fadeinfab, new Vector3(0f, 0f, -1.2f), Quaternion.identity);
    }


    // Update is called once per frame
    void Update()
    {
        //もし会話が終わったら、フェードアウト開始
        if (fiobj == null)
        {
            if ((foobj == null) & (textObj == null))
            {
                chara1comp.Create(9, 0);
                chara0comp.Create(9, 0);
                foobj = Instantiate(fadeoutfab, new Vector3(0f, 0f, -1.2f), Quaternion.identity);
            }
        }
        //フェードアウトが終わるとき
        if (foobj != null)
        {
            if (foobj.GetComponent<TitleFadeOut>().cnt == 70)
            {
                Destroy(foobj);
                foobj = null;
                fiobj = Instantiate(fadeinfab, new Vector3(0f, 0f, -1.2f), Quaternion.identity);
                prrgcount++;
                if (prrgcount == 1)//プロローグ側
                {
                    backimagecomp.Create(2, 1);//0の背景を表示
                }
                if (prrgcount == 2)//プロローグ側
                {
                    SceneManager.LoadScene("SampleScene");
                }
                if (prrgcount == 101)//エンディング側
                {
                    SceneManager.LoadScene("TitleScene");
                }
            }
        }
        //フェードインが終わるとき
        if (fiobj != null)
        {
            if (fiobj.GetComponent<TitleFadeIn>().cnt == 61)
            {
                Destroy(fiobj);
                fiobj = null;
                textObj = Instantiate(textfab, new Vector3(0, 0, 0), Quaternion.identity);
                textcomp = textObj.GetComponent<TextController>();//コンポーネント
                textcomp.stage = 100 + prrgcount;
                textcomp.Create();
            }
        }


        //会話の位置によって背景変わるための準備
        if (textObj != null)
        {
            changeflg = 0;
            mcl = cl;
            cl = textcomp.currentLine;
            if (mcl != cl) { changeflg = 1; }
        }

        //会話の位置によって背景変わる
        if (changeflg == 1)
        {
            if (prrgcount == 0)//プロローグ前半
            {
                if (cl == 1)
                {
                    chara0comp.Create(0, 0);
                    chara1comp.Create(0, 0);
                }

                if (cl == 2)
                {
                    chara0comp.Create(1, 0);
                }

                if (cl == 3)
                {
                    chara0comp.Create(6, 0);
                }

                if (cl == 4)
                {
                    chara0comp.Create(1, 0);
                }

                if (cl == 5)
                {
                    chara0comp.Create(4, 0);
                    imageboxcomp.Create(0, 3);
                }
                if (cl == 6)
                {
                    imageboxcomp.Create(4, 3);
                }
                if (cl == 7)
                {
                    chara1comp.Create(6, 0);
                    imageboxcomp.Create(1, 3);
                }
                if (cl == 8)
                {
                    imageboxcomp.Create(2, 3);
                }

                if (cl == 9)
                {
                    imageboxcomp.Create(9, 3);
                }

                if (cl == 10)
                {
                    chara1comp.Create(1, 0);
                }
                if (cl == 11)
                {
                    chara0comp.Create(1, 0);
                }
                if (cl == 12)
                {
                    chara0comp.Create(2, 0);
                    chara1comp.Create(0, 0);
                }
                if (cl == 14)
                {
                    chara0comp.Create(3, 0);
                }
                if (cl == 15)
                {
                    chara1comp.Create(5, 0);
                }

                if (cl == 16)
                {
                    chara0comp.Create(9, 0);
                    chara1comp.Create(9, 0);
                }
                if (cl == 17)
                {
                    backimagecomp.Create(1, 1);//0の背景を表示
                }

                if (cl == 18)
                {
                    chara0comp.Create(4, 0);
                }

                if (cl == 19)
                {
                    chara0comp.Create(9, 0);
                    imageboxcomp.Create(3, 3);//肉ジュージュー
                }
                if (cl == 20)
                {
                    chara1comp.Create(3, 2);
                    imageboxcomp.Create(9, 3);
                }

                if (cl == 21)
                {
                    chara0comp.Create(10, 0 + 16);//いた
                }

                if (cl == 22)
                {
                    chara0comp.Create(6, 0);//最近グラボが品薄
                }

                if (cl == 24)
                {
                    chara0comp.Create(1, 0);//なんだがっかり
                }

                if (cl == 25)
                {
                    chara1comp.Create(9, 0);
                    chara0comp.Create(11, 0 + 16);//博士んちって
                }

                if (cl == 26)
                {
                    chara1comp.Create(3, 2);//そうじゃよ
                }

                if (cl == 27)
                {
                    chara0comp.Create(10, 0 + 16);//はぁ
                }

                if (cl == 28)
                {
                    chara0comp.Create(9, 0);
                    chara1comp.Create(9, 0);
                    imageboxcomp.Create(5, 4);//続いて次のニュースで
                }

                if (cl == 31)
                {
                    chara0comp.Create(6, 0);
                    imageboxcomp.Create(9, 3);//フーン隕石か
                }

                if (cl == 32)
                {
                    chara0comp.Create(9, 0);//プルる
                }

                if (cl == 33)
                {
                    chara0comp.Create(12, 2 + 16);//はいもしもし
                }

                if (cl == 34)
                {
                    chara1comp.Create(7, 0+16);//どうしたの？
                }

                if (cl == 36)
                {
                    chara1comp.Create(9, 0);
                    chara0comp.Create(9, 0);
                }
            }

            
            if (prrgcount == 1)//プロローグ後半
            {
                if (cl == 1)
                {
                    chara0comp.Create(13, 0 + 16);//博士これはいったい？
                }

                if (cl == 2)
                {
                    chara0comp.Create(9,0);//さこちらへ
                }

                if (cl == 3)
                {
                    chara1comp.Create(3, 2);//何を隠そう
                }

                if (cl == 5)
                {
                    chara0comp.Create(7, 0);//なんだって
                }

                if (cl == 6)
                {
                    chara0comp.Create(6, 0);//でもきがするってのは
                }

                if (cl == 14)
                {
                    chara0comp.Create(9, 0);//そして
                    chara1comp.Create(9, 0);//そして
                }

                if (cl == 20)
                {
                    imageboxcomp.Create(6, 3);//ですマーチ
                }
                if (cl == 21)
                {
                    imageboxcomp.Create(9, 3);//エンターX・・・CUDAです
                }
                if (cl == 23)
                {
                    chara1comp.Create(3, 2);//わしはアセンブリも
                }
                if (cl == 24)
                {
                    chara0comp.Create(4, 0);//どっちもできるよ私
                }
            }


            //
            if (prrgcount == 100)
            {
                if (cl == 1)
                {
                    chara1comp.Create(3, 2);
                    chara0comp.Create(1, 0);
                }
                if (cl == 4)
                {
                    chara0comp.Create(3, 0);
                    chara1comp.Create(0, 0);
                }
                //‥…ああ
                if (cl == 5)
                {
                    imageboxcomp.Create(7, 3);
                }
            }
            


        }
    }
}