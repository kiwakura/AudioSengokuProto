using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Controller : MonoBehaviour
{
    int target_location = 1;
    static float[] target_y = {-5.0f, 0.0f, 5.0f};
    float move_speed = 0.3f;
    
    const float init_power = 10.0f;
    const float power_up_speed = 0.01f;
    float m_power;
    float m_power_up;
    
    void Move()
    {
        float d = target_y[target_location] - transform.position.y;
        if(Math.Abs(d) > move_speed*2f){
            if(d > 0){
                transform.Translate(0, move_speed, 0);
            }else if(d < 0){
                transform.Translate(0, -move_speed, 0);
            }
            
            // ワープ
            Vector3 v = transform.position;
            transform.position = new Vector3(v.x, target_y[target_location], v.z);
        }
        
    }
        
    // Start is called before the first frame update
    void Start()
    {
        m_power = init_power;
        m_power_up = power_up_speed;
    }

    // Update is called once per frame
    void Update()
    {
        GameMain master = GameObject.Find("Master").GetComponent<GameMain>();
        
        //移動
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            target_location += 1;
        }
        else if(Input.GetKeyDown(KeyCode.DownArrow))
        {
            target_location -= 1;
        }
        target_location = Mathf.Clamp(target_location, 0, 2);
        if(master.isMultiLine() == 0){
            target_location = 1;
        }
        Move();
        
        // ユニット生成
        m_power += m_power_up;
        int unit_type = -1;
        if(Input.GetKeyDown(KeyCode.Alpha1)){
            unit_type = 0;
        }else if(Input.GetKeyDown(KeyCode.Alpha2)){
            unit_type = 1;
        }else if(Input.GetKeyDown(KeyCode.Alpha3)){
            unit_type = 2;
        }
        
        if(m_power >= master.UnitCost(unit_type)){
            master.CreateUnit(unit_type, transform.root.gameObject, 0);
            m_power -= master.UnitCost(unit_type);
        }
        
        // 残りパワー表示
        GameObject ui = GameObject.Find("PlayerUI");
        Text power_text = ui.GetComponent<Text>();
        int pow = Mathf.FloorToInt(m_power);
        power_text.text = "P:" + pow;
        
        // 生成可能ユニット表示
        {
            GameObject icon;
            float base_position = 30f;
            icon = GameObject.Find("SoldierEnable");
            Vector3 pos = icon.transform.position;
            if(m_power > master.UnitCost(0)){
                icon.transform.position = new Vector3(pos.x, base_position, pos.z);
            }else{
                icon.transform.position = new Vector3(pos.x, base_position-200f, pos.z);
            }
            icon = GameObject.Find("KnightEnable");
            pos = icon.transform.position;
            if(m_power > master.UnitCost(1)){
                icon.transform.position = new Vector3(pos.x, base_position, pos.z);
            }else{
                icon.transform.position = new Vector3(pos.x, base_position-200f, pos.z);
            }
            icon = GameObject.Find("ShooterEnable");
            pos = icon.transform.position;
            if(m_power > master.UnitCost(2)){
                icon.transform.position = new Vector3(pos.x, base_position, pos.z);
            }else{
                icon.transform.position = new Vector3(pos.x, base_position-200f, pos.z);
            }
        }
        
    }
}
