using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffects : MonoBehaviour {

    public AudioSource aS;
    public AudioClip exp0;//壁が壊れるとき
    public AudioClip exp1;
    public AudioClip hit0;//ufoが壁にぶつかったとき
    public AudioClip hit1;
    public AudioClip hit2;//壁にぶつかりつつダメージを受けないとき
    public AudioClip goal0;
    public AudioClip goal1;
    public AudioClip jump;//ufo噴射
    public AudioClip restart;//リスタート時の音
    public AudioClip rareget;//レアアースゲット時
    public AudioClip pi;//会話の音
    public AudioClip pi1;//決定の音
    public AudioClip pi2;//キャンセルの音
    public AudioClip puu;//ゴールが出現する音
    public AudioClip sscore0;//スコアの音
    public AudioClip sscore1;//スコアの音
    public AudioClip sscore2;//スコアの音
    public AudioClip bubu;//ぶっぶーのおと
    int[] exparray;
    // Use this for initialization
    void Start ()
    {
        aS = GetComponent<AudioSource>();
        exparray = new int[16];
        for(int i=0; i < 16; i++)
        {
            exparray[i] = 0;
        }
    }

    private void Update()
    {
        for (int i = 0; i < 16; i++)
        {
            if (exparray[i] != 0)
            {
                exparray[i]--;
            }
        }
    }

    public void Play_exp0()
    {
        for (int i = 0; i < 16; i++)
        {
            if (exparray[i] == 0)
            {
                exparray[i] = 40;
                aS.PlayOneShot(exp0);
                break;
            }
        }
    }
}
