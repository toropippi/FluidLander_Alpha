using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Const;

public class FPS0 : MonoBehaviour {
    Sprite sprite;
    GameObject refObjScoreGUI;
    ScoreGUITexture scrguitexcomp;
    int cnt;
    public int num;
    Texture2D tex;
    // Use this for initialization
    void Start () {
        refObjScoreGUI = GameObject.Find("ScoreGUI");
        scrguitexcomp = refObjScoreGUI.GetComponent<ScoreGUITexture>();//コンポーネント
        cnt = 0;
        num = 0;
        tex = scrguitexcomp.numtex;
    }
	
	// Update is called once per frame
	void Update () {
        if (cnt != 0)
        {
            //Texture2DからSpriteを作成
            if (cnt != 1) { Sprite.Destroy(sprite); }
            sprite = Sprite.Create(
              texture: tex,
              rect: new Rect(0, (9 - (num%10+10)%10) * Const.CO.FONTPY, 48, Const.CO.FONTPY),
              pivot: new Vector2(0.5f, 0.5f)
            );
            GetComponent<SpriteRenderer>().sprite = sprite;
            if ((scrguitexcomp.alfa_fps_flg == 1)|(cnt==1))
            {
                GetComponent<SpriteRenderer>().material.SetVector("_Intensity", new Color(1.0f, 1.0f, 1.0f, scrguitexcomp.alfa_fps));
            }
        }
        cnt++;
    }
}
