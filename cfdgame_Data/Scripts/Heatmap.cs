using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Const;

public class Heatmap : MonoBehaviour {

    Texture2D tex;
    Sprite sprite;
    MenyBullets mnybllts;
    void Start() {
        mnybllts = GameObject.Find("nabie").GetComponent<MenyBullets>();//
        tex = new Texture2D(192, 144);
        for (int i = 0; i < 192 * 144; i++)
        {
            tex.SetPixel(i%192, i / 192, new Color(0.0f, 1.0f, 0.0f, 1.0f));
        }
        tex.Apply();
        sprite = Sprite.Create(
          texture: tex,
          rect: new Rect(0, 0, 192,144),
          pivot: new Vector2(0.5f, 0.5f)
        );
        GetComponent<SpriteRenderer>().sprite = sprite;
    }
	
	// Update is called once per frame
	void Update () {

        
        float[] kkx = new float[Const.CO.WX * Const.CO.WY];
        mnybllts.YE.GetData(kkx);

        for (int i = 0; i < Const.CO.WX * Const.CO.WY; i++)
        {
            tex.SetPixel(i % Const.CO.WX, Const.CO.WY-1-(i / Const.CO.WX), new Color(Limit(kkx[i]*1.1f), Limit((kkx[i]-0.6f) * 2.0f), 0.0f, 1.0f));
        }
        tex.Apply();
    }
    
    float Limit(float p1)
    {
        if (p1 > 1.0) { return 1.0f; } else
        {
            return p1;
        }
    }
}
