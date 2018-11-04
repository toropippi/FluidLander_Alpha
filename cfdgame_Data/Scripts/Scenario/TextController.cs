using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TextController : MonoBehaviour
{
    string[] scenarios;
    int scenarios_Length;
    public Text uiTexto;
    public GameObject meswinobj;
    public GameObject iconobj;
    GameObject insmeswin;
    GameObject insicon;
    MesIcon iconcomp;
    SoundEffects SE;
    public int stage;

    float intervalForCharacterDisplay = 0.05f;//テキスト表示の速さ

    private string currentText = string.Empty;//その時に表示する数行分だけのテキスト
    private float timeUntilDisplay;//1.0でテキスト全部表示するやつ
    private float timeElapsed;//実時間ベース
    public int currentLine;//ステージｎでm番目のテキストのmに相当するやつ
    private int lastUpdateCharacter;//テキスト表示の処理で表示内容が変わらない場合処理を間引きするため？
    int[] kaoid;
    int deathflg;

    // 文字の表示が完了しているかどうか
    public bool IsCompleteDisplayText
    {
        get { return Time.time > timeElapsed + timeUntilDisplay; }
    }


    // 最後の文だけはボタンアップで会話終了となる
    public bool ButtonUpDownfunc
    {
        //最後の分なら
        get
        {
            if (currentLine == scenarios.Length - 1)
            {
                return Input.GetButtonUp("Jump");
            }
            else
            {
                return Input.GetButtonDown("Jump");
            }
        }
    }

    void Start()
    {
        uiTexto = GameObject.Find("Text").GetComponent<Text>();//コンポーネント
        stage = 200;
        if (GameObject.Find("StageManager") != null)//流体シーンなら
        {
            stage = GameObject.Find("StageManager").GetComponent<Stagemanager>().nowstage;//コンポーネント。もしプロローグならstage１００を読み込み
        }
        /*
        if (GameObject.Find("proroguemaneger") != null)//プロローグなら他スクリプトからstage指定
        {
            stage = 100;
        }
        */
        if (stage < 100)
        {
            Create();
        }
    }

	void Update () 
	{
		// 文字の表示が完了してるならクリック時に次の行を表示する
		if( IsCompleteDisplayText ){
			if (currentLine < scenarios.Length && (ButtonUpDownfunc| Input.GetButton("Cancel")))
            {
                SE.aS.PlayOneShot(SE.pi);
				SetNextLine();
			}
		}else{
		// 完了してないなら文字をすべて表示する
			if (Input.GetButtonDown("Jump") | Input.GetButton("Cancel"))
            {
                SE.aS.PlayOneShot(SE.pi);
                timeUntilDisplay = 0;
			}
        }

        if (deathflg == 0)
        {
            int displayCharacterCount = (int)(Mathf.Clamp01((Time.time - timeElapsed) / timeUntilDisplay) * currentText.Length);
            if (displayCharacterCount != lastUpdateCharacter)
            {
                uiTexto.text = currentText.Substring(0, displayCharacterCount);
                lastUpdateCharacter = displayCharacterCount;
            }
        }
	}

    public void Create()
    {
        SE = GameObject.Find("SE").GetComponent<SoundEffects>();//コンポーネント
        //ここでメッセージウィンドウのインスタンス生成、あと画面アイコン
        insmeswin = Instantiate(meswinobj, new Vector3(1.25f, -3.75f, -1.0f), Quaternion.identity);
        insicon = Instantiate(iconobj, new Vector3(-5.41666666667f, -3.75f, -1.0f), Quaternion.identity);
        insmeswin.transform.localScale = new Vector3(5.0f / 3.0f, 5.0f / 3.0f, 5.0f / 3.0f);
        insicon.transform.localScale = new Vector3(2.5f, 2.5f, 2.5f);
        insmeswin.transform.parent = transform;//生成したスプライトを子オブジェクトとして登録
        insicon.transform.parent = transform;//生成したスプライトを子オブジェクトとして登録
        iconcomp = insicon.GetComponent<MesIcon>();//コンポーネント
        //ここからはTEXTとアイコンID。1行ずつ交互に設定してあるのを解凍読み込み
        var all_scenarioText = Resources.Load<TextAsset>("Scenario/" + stage);
        scenarios = all_scenarioText.text.Split(new string[] { "@br" }, System.StringSplitOptions.None);
        scenarios_Length = scenarios.Length/2;
        kaoid = new int[scenarios_Length];
        for (int i = 0; i < scenarios_Length; i++)
        {
            kaoid[i] = int.Parse(scenarios[i*2]);//各テキストごとの顔id
        }
        currentLine = -1;
        //その他
        deathflg = 0;
        SetNextLine();//ここでcurrentLine 0になる
    }

	void SetNextLine()
    {
        currentLine++;
        if (scenarios_Length <= currentLine)
        {
            Destroy(insmeswin);
            Destroy(insicon);
            Destroy(this.gameObject);
            uiTexto.text = string.Empty;
            deathflg++;
        }
        else
        {
            currentText = scenarios[currentLine*2+1];
		    timeUntilDisplay = currentText.Length * intervalForCharacterDisplay;
		    timeElapsed = Time.time;
		    lastUpdateCharacter = -1;
            iconcomp.num = kaoid[currentLine];
        }
    }
}