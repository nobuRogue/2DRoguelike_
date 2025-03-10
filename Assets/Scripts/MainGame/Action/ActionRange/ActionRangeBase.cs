/**
 * @file ActionRangeBase.cs
 * @brief �s���̎˒��̊��
 * @author yao
 * @date 2025/2/13
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActionRangeBase {
	public List<int> targetList = null;

	/// <summary>
	/// �˒��̎��s����
	/// </summary>
	/// <param name="sourceCharacter"></param>
	public abstract void Setup(CharacterBase sourceCharacter);

	/// <summary>
	/// �˒����g�p�\���ۂ�
	/// </summary>
	/// <returns></returns>
	public virtual bool CanUse(CharacterBase sourceCharacter, ref eDirectionEight dir) {
		return true;
	}

}
