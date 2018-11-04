using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Yajirushi : MonoBehaviour {
    public int yajino;
    Ufo ucomp;
    Gagemaster ggmstr;
    MenyBullets mnybllts;
    // Use this for initialization
    void Start ()
    {
        mnybllts = GameObject.Find("nabie").GetComponent<MenyBullets>();//
        ggmstr = GameObject.Find("gage").GetComponent<Gagemaster>();//コンポーネント
        //yajino = 0;
    }
	
	// Update is called once per frame
	void Update () {
        float rad = Mathf.Atan2(-mnybllts.extf.x, -mnybllts.extf.y);
        float n = Mathf.Sqrt(mnybllts.extf.x * mnybllts.extf.x + mnybllts.extf.y * mnybllts.extf.y)*10.0f;
        n = Mathf.Clamp(n, -0.2f, 0.2f);
        transform.rotation = Quaternion.Euler(0, 0, rad*180.0f/Mathf.PI);
        Vector3 objpos;// = transform.position;
        objpos.x = ggmstr.gage_pos.x - 1.1f * yajino * Mathf.Sin(rad) * n-0.97f;
        objpos.y = ggmstr.gage_pos.y + 1.1f * yajino * Mathf.Cos(rad) * n;
        objpos.z = ggmstr.gage_pos.z;
        transform.position = objpos;
        if (ggmstr.alfa_gage_flg == 1)
        {
            GetComponent<SpriteRenderer>().material.SetVector("_Intensity", new Color(1.0f, 1.0f, 1.0f, ggmstr.alfa_gage));
        }
    }
}
