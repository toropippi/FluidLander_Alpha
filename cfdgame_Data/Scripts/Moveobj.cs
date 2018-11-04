using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using Const;


//流体の中で可動する物体
//定義を満たせば各々独立していくらでも生成できる(ただし4つ以上配置する場合はシェーダー側定数バッファの拡張が必要)
//ObjMapping(ucomp.nozzle_vram, ucomp.nozzle_spd, ucomp.nozzle_pos, ucomp.ufo_rad,3);
//のように座標記憶vram、速度、中心座標、回転角度、obj_idさえあればよい
public class Moveobj : MonoBehaviour {
    public ComputeBuffer obj_vram;
    public Vector3 obj_pos;
    public Vector2 obj_spd;
    public float obj_rad;
    public float obj_radspd;
    public int obj_id;
    public int obj_bmp_id;//obj_idとは違うid。これはすべてのステージで使うすべてのobjに割り当てられている
    public int w;
    public int h;
    public uint[] hostdata;

    int cnt;
    // Use this for initialization
    void Start () {
        obj_rad = 0.0f;
        obj_radspd = 0.0f;
        //obj_bmp_id//これはtextのほうで指定。このidが違えばそれぞれ動きが違う、それぞれのスクリプトに対応する感じ
        //obj_id = 4;//これはインスタンス化してくれたスクリプトで設定する
        //obj_pos.x = 50.0f;
        //obj_pos.y = 50.0f;
        obj_pos.z = -0.001f;
        obj_spd.x = 0.0f;
        obj_spd.y = 0.0f;
        cnt = 0;
        LoadObjData();
    }
	
	// Update is called once per frame
	void Update () {
        obj_pos.x += obj_spd.x * Const.CO.DT * Const.CO.CFDFRAME_PAR_GAMEFRAME;
        obj_pos.y += obj_spd.y * Const.CO.DT * Const.CO.CFDFRAME_PAR_GAMEFRAME;
        obj_rad += obj_radspd * Const.CO.DT * Const.CO.CFDFRAME_PAR_GAMEFRAME;
        obj_rad = (obj_rad + 3.0f * Mathf.PI) % (2.0f * Mathf.PI) - Mathf.PI;

        if (obj_bmp_id == 0)
        {
            obj_spd.y = 0.1f*Mathf.Sin(0.06f*cnt);
        }

        if (obj_bmp_id == 1)
        {
            obj_spd.x = 0.09f * Mathf.Sin(0.19f * cnt);
        }
        if (obj_bmp_id == 2)
        {
            obj_spd.x = -0.15f * Mathf.Sin(0.19f * cnt);
        }
        if (obj_bmp_id == 3)
        {
            obj_spd.y = -0.54f * Mathf.Cos(0.16f * cnt);
        }

        if (obj_bmp_id == 4)
        {
            obj_radspd=0.0018f;
        }

        if (obj_bmp_id == 5)
        {
            obj_radspd = 0.0036f;
        }

        if (obj_bmp_id == 6)
        {
            obj_radspd = -0.0015f;
        }

        if (obj_bmp_id == 7)
        {
            obj_radspd = -0.0015f;
        }

        if (obj_bmp_id == 8)
        {
            obj_spd.y = -0.015f * Mathf.Cos(0.01f * cnt);
        }
        Setpos();
        cnt++;
    }

    
    void LoadObjData()//bmpからload
    {
        int obj_count = 0;
        Loadpngs loadpngs = GetComponent<Loadpngs>();
        int[,] tmpbmp = loadpngs.LoadBmp(Application.dataPath + "\\Textures\\stageobjs\\" + obj_bmp_id + "cfdcoli.bmp");
        w = tmpbmp.GetLength(0);
        h = tmpbmp.GetLength(1);
        hostdata = new uint[w*h];
        for (int i = 0; i < w; i++)
        {
            for (int j = 0; j < h; j++)
            {
                if ((tmpbmp[i, j] % 256) == 0)
                {//黒いところならCFD上で壁になる
                    hostdata[obj_count] = (uint)(i - w/2 + 2048) + (uint)(j - h/2 + 2048) * 4096;
                    obj_count++;
                }
            }
        }
        obj_vram = new ComputeBuffer(obj_count, Marshal.SizeOf(typeof(uint)));
        obj_vram.SetData(hostdata, 0, 0, obj_count);
    }

    //自分のposを確定
    void Setpos()
    {
        Vector3 objpos;// = transform.position;
        objpos.x = (obj_pos.x - 0.5f * Const.CO.WX) / 0.5f / Const.CO.WY * 5.0f;
        objpos.y = (0.5f * Const.CO.WY - obj_pos.y) / 0.5f / Const.CO.WY * 5.0f;
        objpos.z = obj_pos.z;
        transform.position = objpos;
        transform.rotation = Quaternion.Euler(0, 0, -obj_rad / Mathf.PI * 180.0f);
    }

    void OnDisable()
    {
        // コンピュートバッファは明示的に破棄しないと怒られます
        obj_vram.Release();
    }
}
