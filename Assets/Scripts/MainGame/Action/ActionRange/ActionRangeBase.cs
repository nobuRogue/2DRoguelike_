/**
 * @file ActionRangeBase.cs
 * @brief 行動の射程の基底
 * @author yao
 * @date 2025/2/13
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActionRangeBase {
	public List<int> targetList = null;

	/// <summary>
	/// 射程の実行処理
	/// </summary>
	/// <param name="sourceCharacter"></param>
	public abstract void Setup(CharacterBase sourceCharacter);

	/// <summary>
	/// 射程が使用可能か否か
	/// </summary>
	/// <returns></returns>
	public virtual bool CanUse(CharacterBase sourceCharacter, ref eDirectionEight dir) {
		return true;
	}

}
