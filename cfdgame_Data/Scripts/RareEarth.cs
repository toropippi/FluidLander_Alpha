using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Const;

public class RareEarth : MonoBehaviour {
    Vector3 objpos;//自分の中心座標
    Vector3 subpos;//ufoとの差分
    Ufo ucomp;
    Stagemanager stgmngrcomp;
    SoundEffects SE;
    // Use this for initialization
    void Start () {
        ucomp = GameObject.Find("ufo").GetComponent<Ufo>();//ufo コンポーネント
        stgmngrcomp = GameObject.Find("StageManager").GetComponent<Stagemanager>();//コンポーネント
        SE = GameObject.Find("SE").GetComponent<SoundEffects>();//コンポーネント
        objpos = transform.position;
        objpos.x = objpos.x * Const.CO.WY * 0.1f + 0.5f * Const.CO.WX;
        objpos.y = -(objpos.y * Const.CO.WY * 0.1f - 0.5f * Const.CO.WY);
    }

    // Update is called once per frame
    void Update()
    {
        subpos = ucomp.ufo_pos - objpos;
        //自分とufoの関係の処理
        if ((subpos.x * subpos.x + subpos.y * subpos.y) < 56.0f)
        {//自分の位置とufoが近ければスコア加算、自分死ぬのはstagemanagerからも呼ばれる
            stgmngrcomp.stage_score += 100000;
            SE.aS.PlayOneShot(SE.rareget);
            Destroy(this.gameObject);
        }
    }
}
