/**
 * @file ActionEffectBase.cs
 * @brief 行動の効果の基底
 * @author yao
 * @date 2025/2/18
 */

using Cysharp.Threading.Tasks;
using System;

public abstract class ActionEffectBase {
	protected static Action<eDungeonEndReason> _EndDungeon = null;

	public static void SetEndCallback( Action<eDungeonEndReason> setDungeonProcess ) {
		_EndDungeon = setDungeonProcess;
	}

	public abstract UniTask Execute( CharacterBase sourceCharacter, Entity_ActionEffectData.Param effectMaster, ActionRangeBase range );

	public virtual void TearDown() {

	}
}
