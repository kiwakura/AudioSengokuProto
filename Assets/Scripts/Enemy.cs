using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    int target_location = 1;
    static float[] target_y = {-5.0f, 0.0f, 5.0f};
    
    const float init_power = 10.0f;
    const float power_up_speed = 0.01f;
    float m_power;
    float m_power_up;

    const float min_duration = 0.5f;    // 最低待ち時間
    const float init_duration_range = 2.0f;  // 最低待ち時間への加算時間
    
    float m_duration;
    float m_duration_add;
    float m_last_time;
    
    // Start is called before the first frame update
    void Start()
    {
        int current_level = GameMain.getLevel();
        current_level -= 2; // 難度補正
        float level_ratio = 1f + 0.2f*(float)current_level;
        
        m_power = init_power * level_ratio;
        m_power_up = power_up_speed * level_ratio;
        m_duration = 0;
        m_duration_add = init_duration_range * (1.0f/level_ratio);
        m_last_time = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        GameMain master = GameObject.Find("Master").GetComponent<GameMain>();
        
        // 毎ループ処理
        m_power += m_power_up;
        // 残りパワー表示
        GameObject ui = GameObject.Find("EnemyUI");
        Text power_text = ui.GetComponent<Text>();
        int pow = Mathf.FloorToInt(m_power);
        power_text.text = "P:" + pow;
        
        // 一定時間毎に行動
        float delta_t = Time.time - m_last_time;
        m_last_time = Time.time;
        m_duration -= delta_t;
        if(m_duration > 0){
            return;
        }
        
        // ------------------------------------------------
        // 行動開始
        m_duration = min_duration + m_duration_add*Random.value;
        
        // 移動
        target_location = Random.Range(0, 2+1);
        if(master.isMultiLine() == 0){
            target_location = 1;
        }
        Vector3 v = transform.position;
        transform.position = new Vector3(v.x, target_y[target_location], v.z);
        
        // ユニット生成
        int unit_type = Random.Range(0, 3);

        if(m_power >= master.UnitCost(unit_type)){
            master.CreateUnit(unit_type, transform.root.gameObject, 1);
            m_power -= master.UnitCost(unit_type);
        }
        
    }
}
