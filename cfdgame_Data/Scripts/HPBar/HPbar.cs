using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPbar : MonoBehaviour {
    Ufo ucomp;

    void Start () {
        ucomp = GameObject.Find("ufo").GetComponent<Ufo>();//ufo コンポーネント
    }
	
	// Update is called once per frame
	void Update () {
        Vector3 objpos;// = transform.position;
        objpos.x = (ucomp.ufo_pos.x - 0.5f * Const.CO.WX) / 0.5f / Const.CO.WY * 5.0f - 0.64f;
        objpos.y = (0.5f * Const.CO.WY - ucomp.ufo_pos.y) / 0.5f / Const.CO.WY * 5.0f + 0.5f;
        objpos.z = -0.03f;
        transform.position = objpos;
    }
}
