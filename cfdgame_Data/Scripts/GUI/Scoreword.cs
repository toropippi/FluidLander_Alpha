using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scoreword: MonoBehaviour
{
    Sprite sprite;
    ScoreGUITexture scrguitexcomp;
    Texture2D tex;
    Gagemaster ggmstrcomp;
    // Use this for initialization
    void Start()
    {
        scrguitexcomp = GameObject.Find("ScoreGUI").GetComponent<ScoreGUITexture>();//コンポーネント
        ggmstrcomp = GameObject.Find("gage").GetComponent<Gagemaster>();//コンポーネント
        tex = scrguitexcomp.optintex;

        //Texture2DからSpriteを作成
        sprite = Sprite.Create(
          texture: tex,
          rect: new Rect(0, 12 * Const.CO.FONTPY, 480, Const.CO.FONTPY),
          pivot: new Vector2(0.5f, 0.5f)
        );
        GetComponent<SpriteRenderer>().sprite = sprite;
        transform.localScale = new Vector3(0.3f,0.3f,1.0f);
    }
    // Update is called once per frame
    void Update()
    {
        if (ggmstrcomp.alfa_gage_flg == 1)
        {
            GetComponent<SpriteRenderer>().material.SetVector("_Intensity", new Color(1.0f, 1.0f, 1.0f, ggmstrcomp.alfa_gage));
        }
    }
}