using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Frontbar : MonoBehaviour {
    public Texture2D tex;
    Sprite sprite;
    Ufo ucomp;

    //緑のやつ
    void Start()
    {
        //Texture2DからSpriteを作成//バックのやや黒いやつ
        sprite = Sprite.Create(
          texture: tex,
          rect: new Rect(0, 0, tex.width, tex.height),
          pivot: new Vector2(0.0f, 0.0f)
        );
        GetComponent<SpriteRenderer>().sprite = sprite;
        ucomp = GameObject.Find("ufo").GetComponent<Ufo>();//ufo コンポーネント
        GetComponent<SpriteRenderer>().material.SetVector("_Intensity", new Color(0.2f, 1.0f, 0.1f, 1.0f));
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 scale;
        scale.x = ucomp.hitpoint / 32.0f;
        scale.y = 1.5f;
        scale.z = 1.0f;
        transform.localScale = scale;
    }
}
