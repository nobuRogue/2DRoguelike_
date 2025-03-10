/**
 * @file GameEnum.cs
 * @brief 列挙体定義
 * @author yao
 * @date 2025/1/9
 */


public enum eGamePart {
	Invalid = -1,//不正値
	Standby,    // 準備パート 
	Title,      // タイトルパート
	MainGame,   // メインパート
	Ending,     // エンディングパート
	Max,        // 
}

public enum eTerrain {
	Invalid = -1,   // 不正値
	Passage,        // 通路
	Room,           // 部屋
	Wall,           // 壁
	Stair,          // 階段
	Max,
}

public enum eDirectionFour {
	Invalid = -1,
	Up,
	Right,
	Down,
	Left,
	Max
}

public enum eDirectionEight {
	Invalid = -1,
	Up,
	UpRight,
	Right,
	DownRight,
	Down,
	DownLeft,
	Left,
	UpLeft,
	Max
}

public enum eDungeonEndReason {
	Invalid = -1,   // 
	Dead,           // プレイヤー死亡
	Clear,          // ダンジョンクリア
}

public enum eFloorEndReason {
	Invalid = -1,   // 
	Dead,           // プレイヤー死亡
	Stair,          // 階段で移動
}

/// <summary>
/// キャラクターのアニメーションインデクスを表す
/// </summary>
public enum eCharacterAnimation {
	Invalid = -1,
	Wait,
	Walk,
	Attack,
	Damage,
	Max,
}

public enum eItemCategory {
	Potion, // 薬
	Food,   // 食べ物
	Wand,   // 杖
	Max
}