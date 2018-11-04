using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Const;

public class TIMEGUI4 : MonoBehaviour
{
    Sprite sprite;
    GameObject refObjScoreGUI;
    ScoreGUITexture scrguitexcomp;
    int cnt;
    public int num;
    Texture2D tex;
    // Use this for initialization
    void Start()
    {
        refObjScoreGUI = GameObject.Find("ScoreGUI");
        scrguitexcomp = refObjScoreGUI.GetComponent<ScoreGUITexture>();//コンポーネント
        cnt = 0;
        num = 0;
        tex = scrguitexcomp.numtex;
    }
    // Update is called once per frame
    void Update()
    {
        if (cnt != 0)
        {
            //Texture2DからSpriteを作成
            if (cnt != 1) { Sprite.Destroy(sprite); }
            sprite = Sprite.Create(
              texture: tex,
              rect: new Rect(0, (9 - num) * Const.CO.FONTPY, 48, Const.CO.FONTPY),
              pivot: new Vector2(0.5f, 0.5f)
            );
            GetComponent<SpriteRenderer>().sprite = sprite;
            if ((scrguitexcomp.alfa_time_flg == 1)| (cnt == 1))
            {
                GetComponent<SpriteRenderer>().material.SetVector("_Intensity", new Color(24f / 255f, 222f / 255f, 180f / 255f, scrguitexcomp.alfa_time));
            }
        }
        cnt++;
    }
}

