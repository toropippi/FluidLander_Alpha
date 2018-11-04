using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Const;

public class AfterGoalFadeout : MonoBehaviour {
    public Texture2D tex;
    Sprite sprite;
    int cnt;
    Stagemanager stgmngrcomp;
    public GameObject startfab;

    void Start () {
        stgmngrcomp = GameObject.Find("StageManager").GetComponent<Stagemanager>();
        tex = new Texture2D(Const.CO.WX, Const.CO.WX, TextureFormat.RGBA32, false);
        //Texture2DからSpriteを作成
        for (int i = 0; i < Const.CO.WX; i++)
        {
            for (int j = 0; j < Const.CO.WX; j++)
            {
                tex.SetPixel(i,j, new Color(0.0f, 0.0f, 0.0f, 0.0f));
            }
        }
        tex.Apply();

        sprite = Sprite.Create(
          texture: tex,
          rect: new Rect(0, 0, Const.CO.WX, Const.CO.WX),
          pivot: new Vector2(0.5f, 0.5f)
        );
        GetComponent<SpriteRenderer>().sprite = sprite;
        cnt = 0;
    }

    //フェードアウトして死ぬだけ
    void Update () {
        if (cnt < (Const.CO.WX / 2+1))
        {
            for (int i = 0; i < Const.CO.WX; i++)
            {
                tex.SetPixel(i, cnt, new Color(0.0f, 0.0f, 0.0f, 1.0f));
                tex.SetPixel(i, Const.CO.WX - 1 - cnt, new Color(0.0f, 0.0f, 0.0f, 1.0f));
            }
            for (int i = 0; i < Const.CO.WX; i++)
            {
                tex.SetPixel(cnt, i, new Color(0.0f, 0.0f, 0.0f, 1.0f));
                tex.SetPixel(Const.CO.WX - 1 - cnt, i, new Color(0.0f, 0.0f, 0.0f, 1.0f));
            }
            cnt++;
            for (int i = 0; i < Const.CO.WX; i++)
            {
                tex.SetPixel(i, cnt, new Color(0.0f, 0.0f, 0.0f, 1.0f));
                tex.SetPixel(i, Const.CO.WX - 1 - cnt, new Color(0.0f, 0.0f, 0.0f, 1.0f));
            }
            for (int i = 0; i < Const.CO.WX; i++)
            {
                tex.SetPixel(cnt, i, new Color(0.0f, 0.0f, 0.0f, 1.0f));
                tex.SetPixel(Const.CO.WX - 1 - cnt, i, new Color(0.0f, 0.0f, 0.0f, 1.0f));
            }
            tex.Apply();
        }


        if (cnt > (Const.CO.WX/2+10))//stage更新
        {
            stgmngrcomp.stage_clear_flg = 1;//更新フラグをたてて
            //フェードイン画面生成して
            Instantiate(startfab, new Vector3(0.0f, 0.0f, -0.5f), Quaternion.identity);

            Destroy(this.gameObject);//そのあとは自分は死ぬ。
        }

        cnt++;
    }
}
