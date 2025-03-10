/**
 * @file ActionRange00_DirForward.cs
 * @brief キャラの向き前方の射程
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
		// とりあえず前方1マスで実装
		int sourceX = sourceCharacter.positionX, sourceY = sourceCharacter.positionY;
		MapSquareData sourceSquare = GetCharacterSquare(sourceCharacter);
		MapSquareData targetSquare = GetToDirSquare(sourceX, sourceY, sourceCharacter.direction);
		// 攻撃するマスにキャラが居るか判定
		if (!targetSquare.existCharacter) return;
		// 攻撃可能なマスか判定
		if (!CanAttack(sourceX, sourceY, targetSquare, sourceCharacter.direction)) return;

		CharacterBase targetCharacter = CharacterManager.instance.Get(targetSquare.characterID);
		if (IsRelativeEnemy(sourceCharacter, targetCharacter)) targetList.Add(targetCharacter.ID);

	}

	/// <summary>
	/// 使用可能か（対象が居るか）
	/// </summary>
	/// <param name="sourceCharacter"></param>
	/// <param name="dir"></param>
	/// <returns></returns>
	public override bool CanUse(CharacterBase sourceCharacter, ref eDirectionEight dir) {
		MapSquareData sourceSquare = GetCharacterSquare(sourceCharacter);
		int sourceX = sourceSquare.positionX, sourceY = sourceSquare.positionY;
		// 8方向の前方1マスを確認
		for (int i = 0, max = (int)eDirectionEight.Max; i < max; i++) {
			var checkDir = (eDirectionEight)i;
			MapSquareData targetSquare = GetToDirSquare(sourceSquare, checkDir);
			if (targetSquare == null ||
				!targetSquare.existCharacter) continue;

			if (!CanAttack(sourceX, sourceY, targetSquare, checkDir)) continue;
			// マスにいるキャラクターを対象に取るか判定
			CharacterBase targetCharacter = GetCharacter(targetSquare.characterID);
			if (!IsRelativeEnemy(sourceCharacter, targetCharacter)) continue;
			// 対象が居る
			dir = checkDir;
			return true;
		}
		return false;
	}

	/// <summary>
	/// 相対的な敵か否か
	/// </summary>
	/// <param name="source"></param>
	/// <param name="target"></param>
	/// <returns></returns>
	private bool IsRelativeEnemy(CharacterBase source, CharacterBase target) {
		return source.IsPlayer() != target.IsPlayer();
	}

}
