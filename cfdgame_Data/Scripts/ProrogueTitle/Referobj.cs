using System.Collections;
using System.Collections.Generic;
using Const;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Referobj : MonoBehaviour {
    public int nowstage;
    public int difficulty;
    public int stage_score;
    public int BGMVOL;//BGMのボリューム
    public int SEVOL;//SEのボリューム
    public int RYRATIO;//粒子の移動処理を何CFDステップに1回行うか。コンフィグにより可変
    public int PARTICLENUM;//全パーティクル数。コンフィグにより可変
    public int POISSONLOOPNUM;//ポアソン方程式の反復回数
    public int PARTICLERONEFRAME;//1粒子フレームに何個の粒子が更新されるか。これはstageごとに等倍にかわるが、baseの値はここで設定
    public int NOZZLEPARTICLENUM;//適当。UFO噴射で1粒子フレームにでる粒子の数
    public int EXPPARTICLE;//自分が爆発した時の発生する粒子

    int cnt;
    // Use this for initialization
    void Start () {
        nowstage = 0;
        difficulty = 0;
        stage_score = 0;
        //粒子の移動処理を何CFDステップに1回行うか。
        //RYRATIO = 2;
        //PARTICLENUM = 65536 * 4;//全パーティクル数
        
        //破壊不能オブジェクト設定
        Object.DontDestroyOnLoad(this.gameObject);
        Object.DontDestroyOnLoad(GameObject.Find("SE"));
        cnt =0;
    }
	
	// Update is called once per frame
	void Update () {
        if (cnt == 0){
            ConfigLoad();
            SceneManager.LoadScene("TitleScene");
        }
        cnt ++;
	}

    //コンフィグ設定を読み込む
    void ConfigLoad()
    {
        int[,] tmpbmp;
        tmpbmp = GetComponent<Loadpngs>().LoadBmp(Application.dataPath + "\\Textures\\pp.bmp");
        BGMVOL = (10-tmpbmp[0, 0] % 256) * 10;
        SEVOL  = (10-tmpbmp[1, 0] % 256) * 10;
        PARTICLENUM = 65536 * (1<< (tmpbmp[2, 0] % 256));
        int[] dt = { 12, 6, 4, 3, 2, 1 };
        RYRATIO = dt[tmpbmp[3, 0] % 256];
        POISSONLOOPNUM = 32 << (tmpbmp[4, 0] % 256);

        PARTICLERONEFRAME = PARTICLENUM / Const.CO.PARTICLEWRITEDIV * RYRATIO;//1粒子フレームに何個の粒子が更新されるか。これはstageごとに等倍にかわるが、baseの値はここで設定
        NOZZLEPARTICLENUM = PARTICLERONEFRAME * 4;//適当。UFO噴射で1粒子フレームにでる粒子の数
        EXPPARTICLE = PARTICLENUM / 32;//自分が爆発した時の発生する粒子
        GameObject.Find("SE").GetComponent<AudioSource>().volume=0.01f*(float)SEVOL;
    }
}
