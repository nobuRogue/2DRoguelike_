/**
 * @file GameConst.cs
 * @brief �萔��`
 * @author yao
 * @date 2025/1/9
 */

public class GameConst {
	// �}�b�v�֘A
	public static readonly int MAP_SQUARE_HEIGHT_COUNT = 32;
	public static readonly int MAP_SQUARE_WIDTH_COUNT = 32;

	// �����T�C�Y
	public static readonly int MIN_ROOM_SIZE = 3;
	public static int MAX_ROOM_SIZE { get { return (MIN_ROOM_SIZE + 1) * 2; } }

	// �G���A������
	public static readonly int AREA_DEVIDE_COUNT = 8;
	// �G�l�~�[�ő吔
	public static readonly int FLOOR_ENEMY_MAX = 8;
	// 1�}�X�̈ړ��ɂ�����b��
	public static readonly float MOVE_DURATION = 0.1f;

	// �ʏ�U���̍s��ID
	public static readonly int NORMAL_ATTACK_ACTION_ID = 0;

	// �A�C�e���摜�t�@�C��
	public static string ITEM_SPRITE_FILE_NAME = "Design/Sprites/Item/itemIcons";

}
