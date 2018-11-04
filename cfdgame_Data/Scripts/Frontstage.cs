using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Const;

public class Frontstage : MonoBehaviour {
    public Texture2D tex;
    //Texture2D tex1;
    Sprite sprite;
    public GameObject wreck;

    // Use this for initialization
    void Start () {
        tex = GetComponent<Loadpngs>().PicloadPng(Application.dataPath+"\\stage\\0\\stage.png");
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

    public void DelPixcelTex(int rndx,int rndy)
    {
        GameObject gameObj = Instantiate(wreck, new Vector3(1.0f*(rndx-Const.CO.WX/2)/(Const.CO.WY / 2) *5.0f, 1.0f*(Const.CO.WY-1 - rndy- Const.CO.WY/2) / (Const.CO.WY / 2) * 5.0f, -0.03f), Quaternion.identity);
        Destroy(gameObj,1.0f);//爆発アニメーションは１秒後に自動で消える
        for (int i = 0; i< Const.CO.TEXSCALE; i++)
        {
            for (int j = 0; j< Const.CO.TEXSCALE; j++)
            {
                tex.SetPixel(rndx* Const.CO.TEXSCALE + i, (Const.CO.WY-1- rndy)* Const.CO.TEXSCALE + j, new Color(1.0f, 1.0f, 1.0f, 0.0f));
            }
        }
        //tex.Apply();
    }

    //ステージ更新時に読み込まれる
    public void LoadNewStage(int stage)
    {
        Destroy(tex);
        Destroy(sprite);
        tex= GetComponent<Loadpngs>().PicloadPng(Application.dataPath + "\\stage\\"+stage+"\\stage.png");
        sprite = Sprite.Create(
          texture: tex,
          rect: new Rect(0, 0, tex.width, tex.height),
          pivot: new Vector2(0.5f, 0.5f)
        );
        GetComponent<SpriteRenderer>().sprite = sprite;
    }

}
