/**
 * @file GameEnum.cs
 * @brief �񋓑̒�`
 * @author yao
 * @date 2025/1/9
 */


public enum eGamePart {
	Invalid = -1,//�s���l
	Standby,    // �����p�[�g 
	Title,      // �^�C�g���p�[�g
	MainGame,   // ���C���p�[�g
	Ending,     // �G���f�B���O�p�[�g
	Max,        // 
}

public enum eTerrain {
	Invalid = -1,   // �s���l
	Passage,        // �ʘH
	Room,           // ����
	Wall,           // ��
	Stair,          // �K�i
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
	Dead,           // �v���C���[���S
	Clear,          // �_���W�����N���A
}

public enum eFloorEndReason {
	Invalid = -1,   // 
	Dead,           // �v���C���[���S
	Stair,          // �K�i�ňړ�
}

/// <summary>
/// �L�����N�^�[�̃A�j���[�V�����C���f�N�X��\��
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
	Potion, // ��
	Food,   // �H�ו�
	Wand,   // ��
	Max
}