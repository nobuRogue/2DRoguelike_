/**
 * @file ActionRange00_DirForward.cs
 * @brief �L�����̌����O���̎˒�
 * @author yao
 * @date 2025/1/9
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static CharacterUtility;
using static MapSquareUtility;
using static CommonModule;

public class ActionRange00_DirForward : ActionRangeBase {
	public override void Setup(CharacterBase sourceCharacter) {
		InitializeList(ref targetList);
		// �Ƃ肠�����O��1�}�X�Ŏ���
		int sourceX = sourceCharacter.positionX, sourceY = sourceCharacter.positionY;
		MapSquareData sourceSquare = GetCharacterSquare(sourceCharacter);
		MapSquareData targetSquare = GetToDirSquare(sourceX, sourceY, sourceCharacter.direction);
		// �U������}�X�ɃL���������邩����
		if (!targetSquare.existCharacter) return;
		// �U���\�ȃ}�X������
		if (!CanAttack(sourceX, sourceY, targetSquare, sourceCharacter.direction)) return;

		CharacterBase targetCharacter = CharacterManager.instance.Get(targetSquare.characterID);
		if (IsRelativeEnemy(sourceCharacter, targetCharacter)) targetList.Add(targetCharacter.ID);

	}

	/// <summary>
	/// �g�p�\���i�Ώۂ����邩�j
	/// </summary>
	/// <param name="sourceCharacter"></param>
	/// <param name="dir"></param>
	/// <returns></returns>
	public override bool CanUse(CharacterBase sourceCharacter, ref eDirectionEight dir) {
		MapSquareData sourceSquare = GetCharacterSquare(sourceCharacter);
		int sourceX = sourceSquare.positionX, sourceY = sourceSquare.positionY;
		// 8�����̑O��1�}�X���m�F
		for (int i = 0, max = (int)eDirectionEight.Max; i < max; i++) {
			var checkDir = (eDirectionEight)i;
			MapSquareData targetSquare = GetToDirSquare(sourceSquare, checkDir);
			if (targetSquare == null ||
				!targetSquare.existCharacter) continue;

			if (!CanAttack(sourceX, sourceY, targetSquare, checkDir)) continue;
			// �}�X�ɂ���L�����N�^�[��ΏۂɎ�邩����
			CharacterBase targetCharacter = GetCharacter(targetSquare.characterID);
			if (!IsRelativeEnemy(sourceCharacter, targetCharacter)) continue;
			// �Ώۂ�����
			dir = checkDir;
			return true;
		}
		return false;
	}

	/// <summary>
	/// ���ΓI�ȓG���ۂ�
	/// </summary>
	/// <param name="source"></param>
	/// <param name="target"></param>
	/// <returns></returns>
	private bool IsRelativeEnemy(CharacterBase source, CharacterBase target) {
		return source.IsPlayer() != target.IsPlayer();
	}

}
