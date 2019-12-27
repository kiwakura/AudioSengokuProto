using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitBase : MonoBehaviour
{
    int m_unitType = 0;
    // 0:足軽　1:騎馬 2:鉄砲
    private int m_side = 0;
    // 0:→ 1:←
    float m_move_speed = 0;
    float[] move_speed_table = {0.02f, 0.03f, 0.015f};
    public void setType(int type){
        m_unitType = type;
    }
    public int unitType(){
        return m_unitType;
    }
    public void setSide(int side){
        m_side = side;
    }
    public int side(){
        return m_side;
    }
    
    const float battle_speed = 0.1f;
    // 戦闘中なら1
    int m_battling = 0;
    int m_uchitottari = 0;
    float m_hp;
    UnitBase m_battle_target;
    
    float[] hp_table = {100f, 120f, 70f};
    float[] soldier_atk_table = {10.0f, 7.0f, 8.0f};
    float[] knight_atk_table = {13.0f, 10.0f, 10.0f};
    float[] shooter_atk_table = {18.0f, 12.0f, 10.0f};
    float[] getAttackTable(int type){
        if(type == 0){
            return soldier_atk_table;
        }else if(type == 1){
            return knight_atk_table;
        }else{
            return shooter_atk_table;
        }
    }
    
    public int isBattling(){
        return m_battling;
    }
    public void setBattleTarget(UnitBase target){
        if(target == null){
            m_battling = 0;
        }else{
            m_battling = 1;
        }
        m_battle_target = target;
    }
    public UnitBase battleTarget(){
        return m_battle_target;
    }
    // このユニットにダメージ dmg を与える
    // 結果としてこのユニットを倒したら１を返す
    public int setDamage(float dmg){
        dmg *= battle_speed;
        
        m_hp -= dmg;
        if(m_hp <= 0f){
            return 1;
        }else{
            return 0;
        }
    }
    
    // ===================================================================
    // Start is called before the first frame update
    virtual protected void Start()
    {
        m_move_speed = move_speed_table[m_unitType];
        if(m_side > 0){
            m_move_speed *= -1f;
        }
        
        m_hp = hp_table[m_unitType];
    }

    // Update is called once per frame
    virtual protected void Update()
    {
        if(m_hp <= 0f){
            // 倒された
            Destroy(gameObject);
            return;
        }
        
        // 敵将打ち取ったり
        if(m_uchitottari > 0){
            Vector3 sc = transform.localScale;
            sc *= 1.03f;
            transform.localScale = sc;
            return;
        }
            
        if(isBattling() > 0){
            // 揺らす
            float ang = Time.time * 10f;
            ang = Mathf.Sin(ang) * 40f;
            transform.eulerAngles = new Vector3(0f,0f,ang);
            transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            
            UnitBase o_unit = battleTarget();
            int o_type = o_unit.unitType();
            float atk_rate = 0;
            if(unitType() == 0){
                atk_rate = soldier_atk_table[o_type];
            }else if(unitType() == 1){
                atk_rate = knight_atk_table[o_type];
            }else{
                atk_rate = shooter_atk_table[o_type];
            }
            if(o_unit.setDamage(atk_rate) > 0){
                // 勝った
                setBattleTarget(null);
            }
        }
        
        if(isBattling() == 0){
            transform.eulerAngles = new Vector3(0f,0f,0f);
            transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
            
            transform.Translate(m_move_speed, 0, 0);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision){
        // 戦闘中でなければ
        if(isBattling() == 0){
            UnitBase o_unit = collision.gameObject.GetComponent<UnitBase>();
            if(o_unit){
                if(o_unit.side() != side()){
                    // 相手と当たった
                    if(o_unit.isBattling() == 0){
                        o_unit.setBattleTarget(transform.root.GetComponent<UnitBase>());
                        setBattleTarget(o_unit);
                    }
                }
            }
        }
        
        // 反対側の壁にあたったら
        if((collision.gameObject.name == "Wall2D_L" && side()==1)
        || (collision.gameObject.name == "Wall2D_R" && side()==0)){
            GameMain master = GameObject.Find("Master").GetComponent<GameMain>();
            if(master.isGameFinished() == 0){
                m_uchitottari = 1;
                //Debug.Log("Goal:" + side() + ":" + m_side);
                if(side() == 0){
                    master.setWinner(1);
                }else{
                    master.setWinner(2);
                }
            }
        }
    }
    
}
