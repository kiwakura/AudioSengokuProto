using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurnBase01_Iwakura {

public class UnitBase : MonoBehaviour
{
    int m_unit_type = 0;
    // 0:足軽　1:騎馬 2:鉄砲
    private int m_side = 0;
    // 0:→ 1:←
    float m_move_speed = 0;
    float[] move_speed_table = {1f, 1.5f, 0.5f};
    public void SetType(int type){
        m_unit_type = type;
    }
    public int UnitType(){
        return m_unit_type;
    }
    public void SetSide(int side){
        m_side = side;
    }
    public int Side(){
        return m_side;
    }
    int m_my_line = 1;  // 0：左、1:中央、2:右
    public int GetLine(){
        return m_my_line;
    }
    
    int m_uchitottari = 0;
    float m_hp;
    UnitBase m_battle_target;

    public bool IsBattling(){
        if(m_battle_target){
            return true;
        }else{
            return false;
        }
    }
    public void SetBattleTarget(UnitBase target){
        m_battle_target = target;
    }
    public UnitBase BattleTarget(){
        return m_battle_target;
    }
    // このユニットにダメージ dmg を与える
    // 結果としてこのユニットを倒したら１を返す
    public int SetDamage(float dmg){
        m_hp -= dmg;
        if(m_hp <= 0f){
            return 1;
        }else{
            return 0;
        }
    }

    void CreateMessenger(){
        GameObject obj = (GameObject)Resources.Load ("Messenger");
        GameObject g = Instantiate(obj, transform.position, Quaternion.identity);
        Messenger unit = g.GetComponent<Messenger>();
        unit.SetSide(Side());
        unit.SetMessage(UnitType());
    }
    
    // ===================================================================
	// ステップ
	int m_unit_turn;
	int m_move_count; // この値の回数前に進む（てきとう）
	int m_step = 0;
    
    // Start is called before the first frame update
    virtual protected void Start()
    {
        GameMain master = GameObject.Find("Master").GetComponent<GameMain>();
        // 自分のラインを判定
        float y = transform.position.y;
        int m_my_line = GameSettings.IdentifyLine(y);
        
        float speed_ratio = move_speed_table[m_unit_type];
        m_move_speed = speed_ratio * master.BattleFieldUnit();
        //Debug.Log("Unit Speeed: " + m_move_speed);
        if(m_side == 0){
            SoundManager.Instance.PlaySeVolPan(GameSettings.go_se[m_unit_type],0.5f, GameSettings.pan_values[m_my_line]);
        }
        if(m_side > 0){
            m_move_speed *= -1f;
        }
        
        m_hp = GameSettings.hp_table[m_unit_type];
		
		m_unit_turn = master.CurrentTurn();
		m_step = 0;
    }
	
    // Update is called once per frame
    virtual protected void Update()
    {
		// ゲームシステムから各ユニットの動作指令が出る感じ
        GameMain master = GameObject.Find("Master").GetComponent<GameMain>();
		
        if(m_hp <= 0f){
            // 倒された
            if(Side() == 0){
                CreateMessenger();
            }
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
        
		switch(m_step){
			case 0:
			// 待機
			if(master.CurrentTurn() > m_unit_turn){
				m_unit_turn = master.CurrentTurn();
				//移動へ
				m_move_count = move_frames;	// 1sec
				m_step = 1; // 移動へ
			}
            TurnWait();
			break;
			// 移動
			case 1:
			if(IsBattling()){
				// 戦闘へ
				m_step = 2;
			}
			if(TurnMove() > 0){
				// 待機へ
				m_step = 0;
			}
			break;
			case 2:
			// 戦闘
            if(TurnBattle() > 0){
				m_step = 0; // もしくは移動へ？
            }
			break;
			case 3:
			// ターンエンド
			break;
		}
    }

    // Turn なんちゃらは終わると１
    // 待機中 : 待機は終わらないけど 0 
    int TurnWait(){
        transform.eulerAngles = new Vector3(0f,0f,0f);
        transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
        return 0;
    }
    
	// 1ターン分の移動:終わったら１を返す
    const int move_frames = 60;
	int TurnMove(){
		// 姿勢とスケールをリセット
        transform.eulerAngles = new Vector3(0f,0f,0f);
        transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
		
		if(m_move_count >= 0){
            float move_1f = m_move_speed/move_frames;
	        transform.Translate(move_1f, 0, 0);
			m_move_count -= 1;
		}
		if(m_move_count <= 0){
			return 1;
		}else{
			return 0;
		}
	}
	
	// 1ターン分の戦闘
	int TurnBattle(){
        // 揺らす
        float ang = Time.time * 10f;
        ang = Mathf.Sin(ang) * 40f;
        transform.eulerAngles = new Vector3(0f,0f,ang);
        transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
		if(!IsBattling()){
    		return 1;
        }else{
            return 0;
        }
	}

    private void OnTriggerEnter2D(Collider2D collision){
        // 戦闘中でなければ
        if(!IsBattling()){
            UnitBase o_unit = collision.gameObject.GetComponent<UnitBase>();
            UnitBase me = transform.root.GetComponent<UnitBase>();
            if(o_unit){
                // 相手と当たった
                if((o_unit.Side() != Side()) && (!o_unit.IsBattling()) ){
                    // 必ず１対１なので、相互に登録する
                    // 1対多がOKならbattleJudgeに任せる手もある
                    me.SetBattleTarget(o_unit);
                    o_unit.SetBattleTarget(me);
                    
                    GameMain master = GameObject.Find("Master").GetComponent<GameMain>();
                    master.StartBattle(me, o_unit);
                    // 戦闘開始効果音
                    float vol = 1.0f - master.CalcAdvanceRatio(transform.position.x); // 近い方が大きい
                    vol = vol * vol;    // 距離による減衰を大きくする
                    float pan = GameSettings.pan_values[GameSettings.IdentifyLine(transform.position.y)];
                    SoundManager.Instance.PlaySeVolPan("Armor_combat", vol, pan);
                }
            }
        }
        
        // 反対側の壁にあたったら
        if((collision.gameObject.name == "Wall2D_L" && Side()==1)
        || (collision.gameObject.name == "Wall2D_R" && Side()==0)){
            GameMain master = GameObject.Find("Master").GetComponent<GameMain>();
            if(master.IsGameFinished() == 0){
                m_uchitottari = 1;
                //Debug.Log("Goal:" + side() + ":" + m_side);
                if(Side() == 0){
                    master.SetWinner(1);
                }else{
                    master.SetWinner(2);
                }
            }
        }
    }
    
}

}

