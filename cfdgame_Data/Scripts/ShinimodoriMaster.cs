using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Const;

public class ShinimodoriMaster : MonoBehaviour {
    int cnt;
    public float x;
    public float y;
    public GameObject gobj;//一つの丸い輪っか
    // Use this for initialization
    void Start () {
        cnt = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (cnt>70)
        {
            if (cnt % 6 == 0)
            {
                GameObject gameObj = Instantiate(gobj, new Vector3(1.0f * (x - Const.CO.WX / 2) / (Const.CO.WY / 2) * 5.0f, 1.0f * (Const.CO.WY - 1 - y - Const.CO.WY / 2) / (Const.CO.WY / 2) * 5.0f, -0.01f), Quaternion.identity);
                Destroy(gameObj, 0.44f);//爆発アニメーションは0.3秒後に自動で消える
            }
        }
        cnt++;
        if (cnt == 129)
        {
            Destroy(this.gameObject);
        }
    }
}
