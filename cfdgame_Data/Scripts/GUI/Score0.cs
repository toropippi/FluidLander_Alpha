using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Const;


public class Score0 : MonoBehaviour
{
    Sprite sprite;
    ScoreGUITexture scrguitexcomp;
    Gagemaster ggmstrcomp;
    int cnt;
    public int num;
    int prenum;
    Texture2D tex;
    // Use this for initialization
    //スコアカンストしたりマイナスになっても変にならないように(num+10)%10にしている
    void Start()
    {
        scrguitexcomp = GameObject.Find("ScoreGUI").GetComponent<ScoreGUITexture>();//コンポーネント
        ggmstrcomp = GameObject.Find("gage").GetComponent<Gagemaster>();//コンポーネント
        cnt = 0;
        prenum = -1;
        num = 0;
        tex = scrguitexcomp.numtex;
        sprite = Sprite.Create(
              texture: tex,
              rect: new Rect(0, (9 - (num+10)%10) * Const.CO.FONTPY, 48, Const.CO.FONTPY),
              pivot: new Vector2(0.5f, 0.5f)
            );
        GetComponent<SpriteRenderer>().sprite = sprite;

        Vector3 scale = transform.localScale;
        scale.x *= 0.4f;
        scale.y *= 0.4f;
        transform.localScale = scale;
    }
    // Update is called once per frame
    void Update()
    {
        if (prenum != num)
        {
            //Texture2DからSpriteを作成
            Sprite.Destroy(sprite);
            sprite = Sprite.Create(
              texture: tex,
              rect: new Rect(0, (9 - (num%10 + 10) % 10) * Const.CO.FONTPY, 48, Const.CO.FONTPY),
              pivot: new Vector2(0.5f, 0.5f)
            );
            GetComponent<SpriteRenderer>().sprite = sprite;
            num = prenum;
        }

        if ((ggmstrcomp.alfa_gage_flg == 1)|(cnt==1))
        {
            GetComponent<SpriteRenderer>().material.SetVector("_Intensity", new Color(24f/255f, 222f / 255f, 180f / 255f, ggmstrcomp.alfa_gage));
        }
        cnt++;
    }
}