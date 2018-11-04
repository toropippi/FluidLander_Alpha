using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSword : MonoBehaviour
{
    Sprite sprite;
    GameObject refObjScoreGUI;
    ScoreGUITexture scrguitexcomp;
    Texture2D tex;
    // Use this for initialization
    void Start()
    {
        refObjScoreGUI = GameObject.Find("ScoreGUI");
        scrguitexcomp = refObjScoreGUI.GetComponent<ScoreGUITexture>();//コンポーネント
        tex = scrguitexcomp.optintex;

        //Texture2DからSpriteを作成
        sprite = Sprite.Create(
          texture: tex,
          rect: new Rect(0, 8 * Const.CO.FONTPY, 152, Const.CO.FONTPY),
          pivot: new Vector2(0.5f, 0.5f)
        );
        GetComponent<SpriteRenderer>().sprite = sprite;
        GetComponent<SpriteRenderer>().material.SetVector("_Intensity", new Color(1.0f, 1.0f, 1.0f,1.0f));
    }
    // Update is called once per frame
    void Update()
    {
        if (scrguitexcomp.alfa_fps_flg == 1)
        {
            GetComponent<SpriteRenderer>().material.SetVector("_Intensity", new Color(1.0f, 1.0f, 1.0f, scrguitexcomp.alfa_fps));
        }
    }
}


