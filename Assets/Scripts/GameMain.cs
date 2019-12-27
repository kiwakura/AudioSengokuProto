using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameMain : MonoBehaviour
{
    // ゲーム設定まわり
    // 3ライン有効かどうか
    [SerializeField] int MultiLineEnable = 1;
    
    public int isMultiLine(){
        return MultiLineEnable;
    }
    
    // 0:ゲーム中　1:プレイヤー勝ち 2:敵勝ち
    int m_winner = 0;
    public void setWinner(int winner){
        // 上書きは禁止
        if(m_winner == 0){
            m_winner = winner;
        }
    }
    // 決着がついていたら 0 以外が返る
    public int isGameFinished(){
        return m_winner;
    }
    
    // ゲームレベル
    public static int m_level;
    public static int getLevel(){
        return m_level;
    }
    public static void resetLevel(){
        m_level = 0;
    }
    
    // ユニットまわり
    // 生成するユニットのコスト
    static float[] unit_cost = {2.0f, 3.0f, 4.0f};
    public GameObject soldierObjPrefab;
    public GameObject knightObjPrefab;
    public GameObject shooterObjPrefab;

    public float UnitCost(int unit_type){
        if(unit_type < 0 || unit_type > 2){
            return 0f;
        }
        float cost = unit_cost[unit_type];
        return cost;
    }
    
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
        unit.setType(unit_type);
        unit.setSide(side);
    }

    void Awake()
    {
        //QualitySettings.vSyncCount = 0;
    }
    
    int m_step = 0; // 状態遷移
    float m_time;
    
    // Start is called before the first frame update
    void Start()
    {
        m_level += 1;   // スタート毎にレベルアップ
    }

    // Update is called once per frame
    void Update()
    {
        GameObject ui = GameObject.Find("GameUI");
        Text game_text = ui.GetComponent<Text>();
        game_text.text = "LEVEL:" + m_level;
        
        // パネルコントロール
        if(Input.GetKeyDown(KeyCode.Space)){
            PanelCtrl panel = GameObject.Find("Panel").GetComponent<PanelCtrl>();
            panel.TogglePanel();
        }
        
        switch(m_step){
            case 0:
            // 進行中
                if(m_winner > 0){
                    m_step++;
                    m_time = Time.time;
                }
            break;
            case 1:
                //勝負あった
                if(Time.time > m_time + 2f){
                    m_step++;
                }
            break;
            case 2:
            //次のシーンへ
                if(m_winner == 1){
                    SceneManager.LoadScene("YouWin");
                }else if(m_winner == 2){
                    SceneManager.LoadScene("GameOver");
                }
            break;
            
        }
   }
}
