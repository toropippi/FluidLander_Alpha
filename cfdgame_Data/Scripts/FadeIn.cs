using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Const;

public class FadeIn : MonoBehaviour {
    public Texture2D tex;
    Sprite sprite;
    int cnt;
    // Use this for initialization
    void Start () {
        tex = new Texture2D(Const.CO.WX, Const.CO.WX, TextureFormat.RGBA32, false);
        //Texture2DからSpriteを作成
        for (int i = 0; i < Const.CO.WX; i++)
        {
            for (int j = 0; j < Const.CO.WX; j++)
            {
                tex.SetPixel(i, j, new Color(0.0f, 0.0f, 0.0f, 1.0f));
            }
        }
        tex.Apply();

        sprite = Sprite.Create(
          texture: tex,
          rect: new Rect(0, 0, Const.CO.WX, Const.CO.WX),
          pivot: new Vector2(0.5f, 0.5f)
        );
        GetComponent<SpriteRenderer>().sprite = sprite;
        cnt = Const.CO.WX/2;

    }

    // Update is called once per frame
    void Update () {
        if (cnt > 1) { 
            for (int i = 0; i < Const.CO.WX; i++)
            {
                tex.SetPixel(i, cnt, new Color(0.0f, 0.0f, 0.0f, 0.0f));
                tex.SetPixel(i, Const.CO.WX - 1 - cnt, new Color(0.0f, 0.0f, 0.0f, 0.0f));
            }
            for (int i = 0; i < Const.CO.WX; i++)
            {
                tex.SetPixel(cnt, i, new Color(0.0f, 0.0f, 0.0f, 0.0f));
                tex.SetPixel(Const.CO.WX - 1 - cnt, i, new Color(0.0f, 0.0f, 0.0f, 0.0f));
            }
            cnt--;
            for (int i = 0; i < Const.CO.WX; i++)
            {
                tex.SetPixel(i, cnt, new Color(0.0f, 0.0f, 0.0f, 0.0f));
                tex.SetPixel(i, Const.CO.WX - 1 - cnt, new Color(0.0f, 0.0f, 0.0f, 0.0f));
            }
            for (int i = 0; i < Const.CO.WX; i++)
            {
                tex.SetPixel(cnt, i, new Color(0.0f, 0.0f, 0.0f, 0.0f));
                tex.SetPixel(Const.CO.WX - 1 - cnt, i, new Color(0.0f, 0.0f, 0.0f, 0.0f));
            }
            tex.Apply();
        }


        if (cnt ==0)//
        {
            Destroy(this.gameObject);//そのあとは自分は死ぬ。
        }
        cnt--;
    }
}
