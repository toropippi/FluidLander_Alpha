using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stagename : MonoBehaviour
{
    public Texture2D tex;
    Sprite sprite;
    Stagemanager stgmngrcomp;
    SpriteRenderer backsprite;
    SpriteRenderer moyasprite;
    SpriteRenderer mymysprite;
    public float alfa;
    int cnt;
    void Start ()
    {
        stgmngrcomp = GameObject.Find("StageManager").GetComponent<Stagemanager>();//コンポーネント
        moyasprite = GameObject.Find("moyasheet").GetComponent<SpriteRenderer>();//コンポーネント
        backsprite = GameObject.Find("Backname").GetComponent<SpriteRenderer>();//コンポーネント
        mymysprite = GetComponent<SpriteRenderer>();

        //自分の画像生成スプライト設定
        sprite = Sprite.Create(
          texture: tex,
          rect: new Rect(0, 1178-(stgmngrcomp.nowstage*62)-62, 1024,62),
          pivot: new Vector2(0.5f, 0.5f)
        );
        mymysprite.sprite = sprite;

        //backの画像生成スプライト設定
        sprite = Sprite.Create(
          texture: tex,
          rect: new Rect(0, 1178 - (stgmngrcomp.nowstage * 62) - 62, 1024, 62),
          pivot: new Vector2(0.5f, 0.5f)
        );
        backsprite.sprite = sprite;
        cnt = 0;
    }
	
	// Update is called once per frame
	void Update () {
        if (cnt == 0)
        {
            stgmngrcomp = GameObject.Find("StageManager").GetComponent<Stagemanager>();//コンポーネント
            moyasprite = GameObject.Find("moyasheet").GetComponent<SpriteRenderer>();//コンポーネント
            backsprite = GameObject.Find("Backname").GetComponent<SpriteRenderer>();//コンポーネント
            mymysprite = GetComponent<SpriteRenderer>();
        }
        alfa = Mathf.Clamp(0.03f*(88-cnt), 0.0f, 1.0f);
        mymysprite.material.SetVector("_Intensity", new Color(1.0f, 0.9f, 0.91f, 1.0f * alfa));
        moyasprite.material.SetVector("_Intensity", new Color(0.5f, 1.0f, 0.5f, 1.0f * alfa));
        backsprite.material.SetVector("_Intensity", new Color(0.1f, 0.1f, 0.1f, 1.0f * alfa));

        cnt++;

        if (cnt > 90)//stage更新
        {
            Destroy(this.gameObject);//そのあとは自分は死ぬ。
        }
    }
}
