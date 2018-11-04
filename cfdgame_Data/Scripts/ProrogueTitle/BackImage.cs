using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackImage : MonoBehaviour
{
    Sprite sprite;
    public Texture2D[] tex=new Texture2D[36];
    int flg;
    // Use this for initialization
    void Start()
    {
        flg = 0;
    }
    // Update is called once per frame
    void Update()
    {
    }
    public void Create(int id,int fullflg)
    {
        //Texture2DからSpriteを作成
        if (flg == 1) { Sprite.Destroy(sprite); }
        flg = 1;

        sprite = Sprite.Create(
            texture: tex[id],
            rect: new Rect(0,0, tex[id].width, tex[id].height),
            pivot: new Vector2(0.5f, 0.5f)
        );

        GetComponent<SpriteRenderer>().sprite = sprite;

        if (fullflg % 16 == 0)//ただの表示
        {
            transform.position = new Vector3(transform.position.x, 0.0f, -0.1f);
            transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        }


        if (fullflg % 16 == 1)//スクリーン全体に拡大
        {
            float scalex = tex[id].width / 100.0f;
            float scaley = tex[id].height / 100.0f;
            scalex = 10.0f / scalex;
            scaley = 10.0f / scaley;
            float scl=Mathf.Max(scalex, scaley);
            transform.localScale = new Vector3(scl, scl, 1.0f);
        }

        if (fullflg%16 == 2)//親父キャラの画像を標準化
        {
            transform.position = new Vector3(transform.position.x, 1.3f, -0.1f);
            transform.localScale = new Vector3(0.6f, 0.6f, 1.0f);
        }

        if (fullflg % 16 == 3)//真ん中に小さく表示
        {
            float scalex = tex[id].width / 100.0f;
            //float scaley = tex[id].height / 100.0f;
            scalex = 7.0f / scalex;
            //scaley = 5.6f / scalex;
            float scl = Mathf.Max(scalex, scalex);
            transform.localScale = new Vector3(scl, scl, 1.0f);
            transform.position = new Vector3(transform.position.x, 1.0f, -0.2f);
            GetComponent<SpriteRenderer>().material.SetVector("_Intensity", new Color(1.0f,1.0f,1.0f,1.0f));
        }

        if (fullflg % 16 == 4)//真ん中に小さく表示
        {
            float scalex = tex[id].width / 100.0f;
            //float scaley = tex[id].height / 100.0f;
            scalex = 5.0f / scalex;
            //scaley = 5.6f / scalex;
            float scl = Mathf.Max(scalex, scalex);
            transform.localScale = new Vector3(scl, scl, 1.0f);
            transform.position = new Vector3(transform.position.x, 1.0f, -0.2f);
            GetComponent<SpriteRenderer>().material.SetVector("_Intensity", new Color(1.0f, 1.0f, 1.0f, 1.0f));
        }

        if (fullflg >= 16)//左右反転
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
    }
}

