using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleFadeIn : MonoBehaviour {

    Texture2D tex;
    Sprite sprite;
    public int cnt;
    // Use this for initialization
    void Start()
    {
        tex = new Texture2D(64, 64, TextureFormat.RGBA32, false);
        //Texture2DからSpriteを作成
        for (int i = 0; i < 64; i++)
        {
            for (int j = 0; j < 64; j++)
            {
                tex.SetPixel(i, j, new Color(0.0f, 0.0f, 0.0f, 1.0f));
            }
        }
        tex.Apply();

        sprite = Sprite.Create(
          texture: tex,
          rect: new Rect(0, 0, 64,64),
          pivot: new Vector2(0.5f, 0.5f)
        );
        GetComponent<SpriteRenderer>().sprite = sprite;
        cnt = 0;
        transform.localScale = new Vector3(40.0f,20.0f,1.0f);
    }

    // Update is called once per frame
    void Update()
    {
        for(int x = 0; x < 64; x++)
        {
            for (int y = 0; y < 64; y++)
            {
                tex.SetPixel(x,y, new Color(0.0f, 0.0f, 0.0f, Mathf.Clamp(1.0f*(60-cnt)/60f,0.0f,1.0f)));
            }
        }
        tex.Apply();

        /*デストロイは別のスクリプトでおkなう
        if (cnt == 121)//
        {
            Destroy(this.gameObject);//そのあとは自分は死ぬ。
        }
        */
        cnt++;
    }
}
