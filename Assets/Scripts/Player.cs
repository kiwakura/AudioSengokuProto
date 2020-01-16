using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace TurnBase01_Iwakura {

public class Player : MonoBehaviour
{
    int m_target_location = 1;
    static float[] target_y = {-5.0f, 0.0f, 5.0f};
    float move_speed = 0.3f;
    
    void Move()
    {
        float d = target_y[m_target_location] - transform.position.y;
        if(Math.Abs(d) > move_speed*2f){
            if(d > 0){
                transform.Translate(0, move_speed, 0);
            }else if(d < 0){
                transform.Translate(0, -move_speed, 0);
            }
            
            // ワープ
            Vector3 v = transform.position;
            transform.position = new Vector3(v.x, target_y[m_target_location], v.z);
        }
        
    }
    
    // ====================================================
    int m_drawn_unit;
    
	bool MakeUnit(){
	    GameMain master = GameObject.Find("Master").GetComponent<GameMain>();
		
        // ユニット生成
        int unit_type = -1;
        if(Input.GetKeyDown(KeyCode.UpArrow)){
            unit_type = m_drawn_unit;
        }
        
        if(unit_type >= 0){
            master.CreateUnit(unit_type, transform.root.gameObject, 0);
			return true;
        }else{
			return false;
		}
	}
	
    // 生成可能ユニット表示
    void DisplayActiveUnit(){
        PanelCtrl panel = GameObject.Find("SoldierEnable").GetComponent<PanelCtrl>();
        if(m_drawn_unit == 0){
            panel.On();
        }else{
            panel.Off();
        }
        panel = GameObject.Find("KnightEnable").GetComponent<PanelCtrl>();
        if(m_drawn_unit == 1){
            panel.On();
        }else{
            panel.Off();
        }
        panel = GameObject.Find("ShooterEnable").GetComponent<PanelCtrl>();
        if(m_drawn_unit == 2){
            panel.On();
        }else{
            panel.Off();
        }
    }
    
    // ====================================================================
	int m_step = 0;
	int m_turn_done = 0;
    // Start is called before the first frame update
    void Start()
    {
		m_step = 0;
		m_turn_done = 0;
    }

    // Update is called once per frame
    void Update()
    {
        GameMain master = GameObject.Find("Master").GetComponent<GameMain>();
        
        // ====================================================
        // 移動（常に受付）
        bool play_se = false;
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            play_se = true;
            m_target_location += 1;
        }
        else if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            play_se = true;
            m_target_location -= 1;
        }
        m_target_location = Mathf.Clamp(m_target_location, 0, 2);
        if(GameSettings.MultiLineEnable() == 0){
            m_target_location = 1;
        }
        if(play_se){
            string[] location_se = {"decision1","decision2","decision3"};
            float[] pans = {-0.8f, 0f, 0.8f};
            SoundManager.Instance.PlaySeVolPan(location_se[m_target_location],0.7f,GameSettings.pan_values[m_target_location]);
        }
        Move();
        
        // ====================================================
		switch(m_step){
			case 0:
            // 生成ユニットを抽選する
            m_drawn_unit = GameSettings.DrawUnit();
            SoundManager.Instance.PlaySeVol(GameSettings.spawn_se[m_drawn_unit],1.0f);
            m_step++;
            break;
            case 1:
			// 入力受付前
			if(MakeUnit()){
                m_drawn_unit = -1;
				m_turn_done = master.CurrentTurn();
				m_step++;
			}
			break;
			case 2:
			// 次ターンまで待機
			if(master.CurrentTurn() > m_turn_done){
				m_step = 0;
			}
			break;
		}
        
        // プレイヤー情報表示
        GameObject ui = GameObject.Find("PlayerUI");
        Text player_info = ui.GetComponent<Text>();
        player_info.text = "P:" + m_drawn_unit;
        
        DisplayActiveUnit();
    }
}

}
