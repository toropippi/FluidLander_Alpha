using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneSprite : MonoBehaviour {
    Sprite sprite;
    public Texture2D tex0;
    public Texture2D tex1;
    public Texture2D tex2;
    public Texture2D tex3;
    int flg=0;
    // Use this for initialization
    void Start () {

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    //画像生成
    public void Create(int texno, int left_px, int up_px, int sizex, int sizey)
    {
        Texture2D tex=tex0;
        if (texno == 0) { tex = tex0; }
        if (texno == 1) { tex = tex1; }
        if (texno == 2) { tex = tex2; }
        if (texno == 3) { tex = tex3; }
        if (flg == 1) { Destroy(sprite);}
        sprite = Sprite.Create(
              texture: tex,
              rect: new Rect(left_px,tex.height-up_px- sizey, sizex,sizey),
              pivot: new Vector2(0.5f, 0.5f)
            );
        GetComponent<SpriteRenderer>().sprite = sprite;
        flg = 1;
    }

    public void SetScale(float f1)
    {
        transform.localScale = new Vector3(f1, f1, f1);
    }

    public void SetScaleF3(Vector3 v3)
    {
        transform.localScale = v3;
    }

    public void SetColor(Color rgba)
    {
        GetComponent<SpriteRenderer>().material.SetVector("_Intensity", new Color(rgba.r, rgba.g, rgba.b, rgba.a));
    }
}
