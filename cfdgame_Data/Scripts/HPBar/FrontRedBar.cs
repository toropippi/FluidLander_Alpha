using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrontRedBar : MonoBehaviour {
    public Texture2D tex;
    Sprite sprite;
    Ufo ucomp;
    float hitpoint_old;
    float hitpoint_old_view;
    //赤いやつ
    void Start()
    {
        //Texture2DからSpriteを作成//バックのやや黒いやつ
        sprite = Sprite.Create(
          texture: tex,
          rect: new Rect(0, 0, tex.width, tex.height),
          pivot: new Vector2(0.0f, 0.0f)
        );
        GetComponent<SpriteRenderer>().sprite = sprite;
        GetComponent<SpriteRenderer>().material.SetVector("_Intensity", new Color(1.0f, 0.0f, 0.0f, 1.0f));
        ucomp = GameObject.Find("ufo").GetComponent<Ufo>();//ufo コンポーネント
        hitpoint_old = ucomp.hitpoint;
        hitpoint_old_view= ucomp.hitpoint;
    }

    // Update is called once per frame
    void Update()
    {
        if (hitpoint_old!= ucomp.hitpoint){
            hitpoint_old = ucomp.hitpoint;
        }
        Vector3 scale;
        scale.x = hitpoint_old_view / 32.0f;
        scale.y = 1.5f;
        scale.z = 1.0f;
        transform.localScale = scale;

        if (hitpoint_old_view> hitpoint_old)
        {
            hitpoint_old_view -= 0.2f;
        }

        if (hitpoint_old_view < hitpoint_old)
        {
            hitpoint_old_view = hitpoint_old;
        }
    }
}
