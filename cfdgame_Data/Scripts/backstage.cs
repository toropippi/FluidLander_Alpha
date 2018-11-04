using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class backstage : MonoBehaviour {

    public Texture2D tex;
    Sprite sprite;
    
    // Use this for initialization
    void Start () {

        tex = GetComponent<Loadpngs>().PicloadPng(Application.dataPath + "\\stage\\0\\haikei.png");
        //Texture2DからSpriteを作成
        sprite = Sprite.Create(
          texture: tex,
          rect: new Rect(0, 0, tex.width, tex.height),
          pivot: new Vector2(0.5f, 0.5f)
        );
        GetComponent<SpriteRenderer>().sprite = sprite;
    }

    // Update is called once per frame
    void Update ()
    {
    }

    public void DelPixcelTex(int rndx, int rndy)
    {
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                tex.SetPixel(rndx * 4 + i, rndy * 4 + j, new Color(1.0f, 1.0f, 1.0f, 0.0f));
            }
        }
        tex.Apply();
    }


    //ステージ更新時に読み込まれる
    public void LoadNewStage(int stage)
    {
        Destroy(tex);
        Destroy(sprite);
        tex = GetComponent<Loadpngs>().PicloadPng(Application.dataPath + "\\stage\\" + stage + "\\haikei.png");
        sprite = Sprite.Create(
          texture: tex,
          rect: new Rect(0, 0, tex.width, tex.height),
          pivot: new Vector2(0.5f, 0.5f)
        );
        GetComponent<SpriteRenderer>().sprite = sprite;
    }

}
