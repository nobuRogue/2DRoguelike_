/**
 * @file GameConst.cs
 * @brief 定数定義
 * @author yao
 * @date 2025/1/9
 */

public class GameConst {
	// マップ関連
	public static readonly int MAP_SQUARE_HEIGHT_COUNT = 32;
	public static readonly int MAP_SQUARE_WIDTH_COUNT = 32;

	// 部屋サイズ
	public static readonly int MIN_ROOM_SIZE = 3;
	public static int MAX_ROOM_SIZE { get { return (MIN_ROOM_SIZE + 1) * 2; } }

	// エリア分割回数
	public static readonly int AREA_DEVIDE_COUNT = 8;
	// エネミー最大数
	public static readonly int FLOOR_ENEMY_MAX = 8;
	// 1マスの移動にかかる秒数
	public static readonly float MOVE_DURATION = 0.1f;

	// 通常攻撃の行動ID
	public static readonly int NORMAL_ATTACK_ACTION_ID = 0;

	// アイテム画像ファイル
	public static string ITEM_SPRITE_FILE_NAME = "Design/Sprites/Item/itemIcons";

}
