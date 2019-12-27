・Master の Multi Line Enable を0にすると１ライン、１にすると３ライン

・矢印の上下：移動（３ライン時のみ）

・数字キー１：足軽作成
・数字キー２：騎馬隊作成
・数字キー３：大筒作成

自分のパワーが左下に表示される。
パワーがたまっていると、ユニットのコストを支払いユニットを作成できる。
パワーは時間で増える。

作成されたユニットは敵方向に進む。
敵と接触すると戦闘開始。
お互いに攻撃して、HPが０になった方が負ける（消える）。

戦闘中のユニットは他のユニットと接触しない。

ユニットが一つでも相手の陣地にたどり着くと勝ち。

足軽：コスト２。馬、大筒に弱い。
騎馬：コスト３。足軽に強い。早い。
大筒：コスト４。遅い。攻撃力が強い。

〇調整項目
・ユニットの移動速度
UnitBase.cs の move_speed_table

・ユニットの体力
UnitBase.cs の hp_table

・ユニットの攻撃力
UnitBase.cs の soldier_atk_table, knight_atk_table, shooter_atk_table

・戦闘速度
UnitBase.cs の battle_speed

・ユニットのコスト
GameMain.cs の unit_cost

・パワーの初期値と増え方
Controller.cs と Enemy.cs の power 関連の値

・敵のレベルの上がり方
Enemy.cs の Start() の中とか。


