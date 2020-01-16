using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurnBase01_Iwakura {

public class GameSettings : MonoBehaviour
{
    // 1ターンの時間
    [SerializeField] static float m_turn_duration = 5.0f;
    // 敵味方間のマス数
    [SerializeField] static int m_num_cells = 6;

    public static float TurnDuration(){
        return m_turn_duration;
    }
    public static int NumCells(){
        return m_num_cells;
    }

    public static int unit_kinds = 3;
    // 出現比率
    [SerializeField] public static int[] m_unit_rates = {7, 2, 1};
    // ユニットガチャ
    public static int DrawUnit(){
        int unit = 0;
        int total = 0;
        for(int i=0; i<unit_kinds; i++){
            total += m_unit_rates[i];
        }
        
        int draw = Random.Range(0, total);
        for(int i=0; i<unit_kinds; i++){
            draw -= m_unit_rates[i];
            if(draw < 0){
                unit = i;
                break;
            }
        }
        return unit;
    }
    
    // HP
    [SerializeField] public static float[] hp_table = {10f, 10f, 10f};
    
    static float[] soldier_atk_table = {11.0f, 4.0f, 8.0f};
    static float[] knight_atk_table = {15.0f, 11.0f, 9.0f};
    static float[] shooter_atk_table = {28.0f, 21.0f, 10.0f};
    public static float[] GetAttackTable(int type){
        if(type == 0){
            return soldier_atk_table;
        }else if(type == 1){
            return knight_atk_table;
        }else{
            return shooter_atk_table;
        }
    }
    static float m_battle_speed = 0.5f;
    public static float BattleSpeed(){
        return m_battle_speed;
    }
    // 
    public static float m_messenger_speed = 0.10f;
    
    // ゲーム設定まわり
    // 3ライン有効かどうか
    [SerializeField] static int m_multi_line_enable = 1;
    
    public static int MultiLineEnable(){
        return m_multi_line_enable;
    }
    
    // ゲームレベル
    public static int m_level;
    public static int GetLevel(){
        return m_level;
    }
    public static void IncreaseLevel(){
        m_level += 1;
    }
    public static void ResetLevel(){
        m_level = 0;
    }
    public static int IdentifyLine(float y_pos){
        int line;
        if(y_pos < -2f){
            line = 0;
        }else if(y_pos > 2f){
            line = 2;
        }else{
            line = 1;
        }
        return line;
    }
    // SE 関連
    // 左右のpan値
    static public float[] pan_values = {0.8f, 0f, -0.8f}; 
    // ガチャ
    static public string[] spawn_se = {"t_pawn","t_knight","t_archer"};
    // 出陣
    static public string[] go_se = {"Armor_spawn","Horse_spawn","Canon_spawn"};
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

}
