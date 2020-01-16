using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurnBase01_Iwakura {
public class Messenger : MonoBehaviour
{
    int m_step = 0;
    int m_side;
    int m_message = 0;
    float m_move_speed = 0;
    float m_target_position_x;
    
    public void SetSide(int side){
        m_side = side;
    }
    public int Side(){
        return m_side;
    }
    public void SetMessage(int lose_unit_type){
        m_message = lose_unit_type;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        m_move_speed = -GameSettings.m_messenger_speed;
        if(m_side > 0){
            m_move_speed *= -1f;
        }
        GameObject left = GameObject.Find("Wall2D_L");
        m_target_position_x = left.transform.position.x;
        
        m_step = 0;
    }

    // Update is called once per frame
    void Update()
    {
        switch(m_step){
            case 0:
            // 自分の陣地へ移動
	        transform.Translate(m_move_speed, 0, 0);
            if(transform.position.x < m_target_position_x){
                m_step++;
            }
            break;
            case 1:
            int pan = GameSettings.IdentifyLine(transform.position.y);
            SoundManager.Instance.PlaySeVolPan("t_lose",1.0f, GameSettings.pan_values[pan]);
            Destroy(gameObject);
            break;
        }
    }
}

}
