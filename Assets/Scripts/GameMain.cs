using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace TurnBase01_Iwakura {

public class battleJudge 
{
    public UnitBase m_unit1;
    public UnitBase m_unit2;

    float DamageAmount(int atk_type, int def_type){
        float[] atk_table = GameSettings.GetAttackTable(atk_type);
        return atk_table[def_type];
    }
    
    public battleJudge(UnitBase unit1, UnitBase unit2){
        m_unit1 = unit1;
        m_unit2 = unit2;
    }
    
    // 戦闘処理：終わったら１を返す
    public int BattleProcess()
    {
        float a_to_b = DamageAmount(m_unit1.UnitType(), m_unit2.UnitType()) * GameSettings.BattleSpeed();
        float b_to_a = DamageAmount(m_unit2.UnitType(), m_unit1.UnitType()) * GameSettings.BattleSpeed();
        int battle_end = 0;
        battle_end += m_unit1.SetDamage(b_to_a);
        battle_end += m_unit2.SetDamage(a_to_b);
        if(battle_end > 0){
            // 戦闘終了。負けた方は自滅するので、setBattleTarget(null)しなくてもいい（よかった）
            m_unit1.SetBattleTarget(null);
            m_unit2.SetBattleTarget(null);
            return 1;
        }else{
            return 0;
        }
    }
}


public class GameMain : MonoBehaviour
{
    // 0:ゲーム中　1:プレイヤー勝ち 2:敵勝ち
    int m_winner = 0;
    public void SetWinner(int winner){
        // 上書きは禁止
        if(m_winner == 0){
            m_winner = winner;
        }
    }
    // 決着がついていたら 0 以外が返る
    public int IsGameFinished(){
        return m_winner;
    }
    
    // =========================================================================
    // ユニットまわり
    public GameObject soldierObjPrefab;
    public GameObject knightObjPrefab;
    public GameObject shooterObjPrefab;
    public GameObject messengerObjPrefab;

    public void CreateUnit(int unit_type, GameObject prnt, int side)
    {
        if(unit_type < 0 || unit_type > 2){
            return;
        }
        
        GameObject g;
        if(unit_type == 0){
            g = Instantiate(soldierObjPrefab, prnt.transform.position, Quaternion.identity);
        }else if(unit_type == 1){
            g = Instantiate(knightObjPrefab, prnt.transform.position, Quaternion.identity);
        }else{
            g = Instantiate(shooterObjPrefab, prnt.transform.position, Quaternion.identity);
        }
        UnitBase unit = g.GetComponent<UnitBase>();
        unit.SetType(unit_type);
        unit.SetSide(side);
    }
    
    // =========================================================================
    // 戦場の距離:Start() でアップデートする
    float m_battlefield_distance = 100f;
    public float BattleFieldUnit(){
        return m_battlefield_distance / GameSettings.NumCells();
    }
    // 自陣０、敵陣１
    public float CalcAdvanceRatio(float pos_x){
        // 他の GameObject から参照される値の初期化
        GameObject left = GameObject.Find("Wall2D_L");
        float x = pos_x - left.transform.position.x;
        float r = x / m_battlefield_distance;
        r = Mathf.Clamp(r, 0f, 1f);
        return r;
    }
    
    private List<battleJudge> m_battle_list = new List<battleJudge>();
    public void StartBattle(UnitBase unit1, UnitBase unit2){
        m_battle_list.Add(new battleJudge(unit1, unit2));
    }
    
    void ProcessAllBattle()
    {
        foreach(battleJudge item in m_battle_list.ToArray()){
            if(item.BattleProcess() > 0){
                m_battle_list.Remove(item);
            }
        }
    }
    // =========================================================================
    int m_step = 0; // 状態遷移
    float m_ftimer; // メインステップ用汎用タイマー
    float m_last_time;  // 前の時間：deltaTime を取得するため
    int m_num_turns;  // 現在のターン数
    public int CurrentTurn(){
        return m_num_turns;
    }
    float m_left_time;  // 今のターンの残り時間
    
    void PanelOnOff()
    {
        GameObject ui = GameObject.Find("GameUI");
        Text game_text = ui.GetComponent<Text>();
        game_text.text = "LEVEL:" + GameSettings.GetLevel() + " TURN:" + CurrentTurn();
        
        // パネルコントロール
        if(Input.GetKeyDown(KeyCode.Space)){
            PanelCtrl panel = GameObject.Find("Panel").GetComponent<PanelCtrl>();
            panel.TogglePanel();
        }
    }
    
    
    // Start is called before the first frame update
    void Start()
    {
        // 他の GameObject から参照される値の初期化
        GameObject right = GameObject.Find("Wall2D_R");
        GameObject left = GameObject.Find("Wall2D_L");
        m_battlefield_distance = right.transform.position.x - left.transform.position.x;
        // ゲーム進行処理の初期化
        m_last_time = Time.time;
        m_left_time = GameSettings.TurnDuration();
        m_num_turns = 0;
        m_battle_list.Clear();
    }

    // Update is called once per frame
    void Update()
    {
        // とりあえず
        PanelOnOff();
        
        // Update 開始処理
        float dtime = Time.time - m_last_time;
        m_last_time = Time.time;
        
        // ターンが進むチェック
        m_left_time -= dtime;
        bool turn_increase = false;
        if(m_left_time <= 0){
            turn_increase = true;
            m_left_time += GameSettings.TurnDuration();
        }
        
        switch(m_step){
            case 0:
            // ステージ初期設定 は Start() で実行
            SoundManager.Instance.PlaySe("Jingle_Start");
            m_ftimer = 1.0f;
            m_step++;
            break;
            case 1:
            // 開始デモ終了街
            m_ftimer -= dtime;
            if(m_ftimer < 0.0f){
                m_step++;
            }
            break;
            case 2:
            // ターン進行
            if(turn_increase){
                m_num_turns += 1;
                // 戦闘処理
                ProcessAllBattle();
            }
            
            // 勝者が決まったら
            if(m_winner > 0){
                if(m_winner == 1){
                    SoundManager.Instance.PlaySe("Jingle_End_Win");
                }else{
                    SoundManager.Instance.PlaySe("Jingle_End_Lose");
                }
                m_ftimer = 2.0f;
                m_step++;
            }
            break;
            
            case 3:
            // ゲーム終了デモ
            m_ftimer -= dtime;
            if(m_ftimer < 0.0f){
                m_step++;
            }
            break;
            
            case 4:
            //次のシーンへ
            if(m_winner == 1){
                SceneManager.LoadScene("YouWin");
            }else if(m_winner == 2){
                SceneManager.LoadScene("GameOver");
            }
            break;
        }
        
        // 終了チェック
        if(Input.GetKey(KeyCode.Escape)){
            UnityEngine.Application.Quit();
        }
    }
}

}

