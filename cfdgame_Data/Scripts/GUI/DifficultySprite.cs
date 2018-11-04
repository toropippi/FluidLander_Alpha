using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultySprite : MonoBehaviour {

    Sprite sprite;
    GameObject refObjScoreGUI;
    ScoreGUITexture scrguitexcomp;
    Stagemanager stgmngrcomp;
    public int cnt;
    int diff;
    Texture2D tex;
    // Use this for initialization
    void Start () {
        refObjScoreGUI = GameObject.Find("ScoreGUI");
        scrguitexcomp = refObjScoreGUI.GetComponent<ScoreGUITexture>();//コンポーネント
        stgmngrcomp = GameObject.Find("StageManager").GetComponent<Stagemanager>();//コンポーネント
        cnt = 0;
        diff = 0;

        tex = scrguitexcomp.normaltex;
        //Texture2DからSpriteを作成
        sprite = Sprite.Create(
          texture: tex,
          rect: new Rect(0, (2-stgmngrcomp.difficulty) * 33, 192, 33),
          pivot: new Vector2(0.5f, 0.5f)
        );
        GetComponent<SpriteRenderer>().sprite = sprite;
    }
    // Update is called once per frame
    void Update () {
        if (stgmngrcomp.stagetime < 0.04f)
        {
            //Texture2DからSpriteを作成
            Destroy(sprite);
            sprite = Sprite.Create(
              texture: tex,
              rect: new Rect(0, (2 - stgmngrcomp.difficulty) * 33, 192, 33),
              pivot: new Vector2(0.5f, 0.5f)
            );
            GetComponent<SpriteRenderer>().sprite = sprite;
        }
        cnt++;

        if (scrguitexcomp.alfa_time_flg == 1)
        {
            GetComponent<SpriteRenderer>().material.SetVector("_Intensity", new Color(1.0f, 1.0f, 1.0f, scrguitexcomp.alfa_time));
        }
    }
}

