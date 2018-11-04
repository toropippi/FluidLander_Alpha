using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MesIcon : MonoBehaviour
{
    public Texture2D tex;
    Sprite sprite;
    public int num;
    int mnum;
    int cnt;

    void Start () {
        sprite = Sprite.Create(
          texture: tex,
          rect: new Rect(0,0,100,100),
          pivot: new Vector2(0.5f, 0.5f)
        );
        mnum = -1;
        //num = 0;
        cnt = 0;
    }
	
	// Update is called once per frame
	void Update () {

        if (mnum != num)
        {
            //Texture2DからSpriteを作成
            Sprite.Destroy(sprite);
            sprite = Sprite.Create(
              texture: tex,
              rect: new Rect((num%13)*100, (2-num / 13)*100,100,100),
              pivot: new Vector2(0.5f, 0.5f)
            );
            GetComponent<SpriteRenderer>().sprite = sprite;
        }
        cnt++;
        mnum = num;//このあとほかのスクリプトでこのnumが書き換えられる可能背がある
    }
}