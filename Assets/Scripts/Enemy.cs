using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TurnBase01_Iwakura {

public class Enemy : MonoBehaviour
{
    int target_location = 1;
    static float[] target_y = {-5.0f, 0.0f, 5.0f};
    
    float m_last_time;
 	int m_step = 0;
	int m_turn_done = 0;
   
    // Start is called before the first frame update
    void Start()
    {
        int current_level = GameSettings.GetLevel();
        current_level -= 2; // 難度補正
        float level_ratio = 1f + 0.2f*(float)current_level;
        
        m_last_time = Time.time;
		m_step = 0;
		m_turn_done = 0;
    }

	void MakeUnit(){
        GameMain master = GameObject.Find("Master").GetComponent<GameMain>();
		
        // 移動
        target_location = Random.Range(0, 2+1);
        if(GameSettings.MultiLineEnable() == 0){
            target_location = 1;
        }
        Vector3 v = transform.position;
        transform.position = new Vector3(v.x, target_y[target_location], v.z);
        
        // ユニット生成
        int unit_type = GameSettings.DrawUnit();
        master.CreateUnit(unit_type, transform.root.gameObject, 1);
	}
	
    // Update is called once per frame
    void Update()
    {
        GameMain master = GameObject.Find("Master").GetComponent<GameMain>();
        
        // 毎ループ処理
        GameObject ui = GameObject.Find("EnemyUI");
        Text enemy_text = ui.GetComponent<Text>();
        enemy_text.text = "Enemy Info";
        
		switch(m_step){
			case 0:
			// 入力受付前
			MakeUnit();
			m_turn_done = master.CurrentTurn();
			m_step = 1;
			break;
			
			case 1:
			// 次ターンまで待機
			if(master.CurrentTurn() > m_turn_done){
				m_step = 0;
			}
			break;
		}
        
    }
}

}

