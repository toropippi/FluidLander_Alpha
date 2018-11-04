using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TIMEGUI5 : MonoBehaviour
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
          rect: new Rect(0, 13 * Const.CO.FONTPY, 48, Const.CO.FONTPY),
          pivot: new Vector2(0.5f, 0.5f)
        );
        GetComponent<SpriteRenderer>().sprite = sprite;
    }
    // Update is called once per frame
    void Update()
    {
        if (scrguitexcomp.alfa_time_flg == 1)
        {
            GetComponent<SpriteRenderer>().material.SetVector("_Intensity", new Color(24f / 255f, 222f / 255f, 180f / 255f, scrguitexcomp.alfa_time));
        }
    }
}

