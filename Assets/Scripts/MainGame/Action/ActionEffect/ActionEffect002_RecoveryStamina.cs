/**
 * @file ActionEffect002_RecoveryStamina.cs
 * @brief –• “x‰ñ•œ
 * @author yao
 * @date 2025/3/10
 */

using Cysharp.Threading.Tasks;
using System.Collections.Generic;

public class ActionEffect002_RecoveryStamina : ActionEffectBase {

	private enum eParamIndex {
		recoveryValue,  // ‰ñ•œ—Ê
	}

	private static readonly int _RECOVERY_LOG_ID = 2;

	public override async UniTask Execute( CharacterBase sourceCharacter, Entity_ActionEffectData.Param effectMaster, ActionRangeBase range ) {
		int recoveryValue = effectMaster.param[(int)eParamIndex.recoveryValue];
		List<int> targetList = range.targetList;
		int targetCount = targetList.Count;
		// ‘ÎÛ‚²‚Æ‚É‰ñ•œ‚Ìˆ—
		for (int i = 0; i < targetCount; i++) {
			CharacterBase target = CharacterManager.instance.Get( targetList[i] );
			if (target == null || !target.IsPlayer()) continue;
			// ƒƒO•\¦
			MenuRogueLog.instance.AddLog( string.Format( _RECOVERY_LOG_ID.ToMessage(), recoveryValue ) );
			(target as PlayerCharacter).AddStamina( recoveryValue * 100 );
		}
		await UniTask.Delay( 1000 );
	}

}
