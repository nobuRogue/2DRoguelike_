/**
 * @file ActionEffect001_RecoveryHP.cs
 * @brief HP��
 * @author yao
 * @date 2025/3/10
 */

using Cysharp.Threading.Tasks;
using System.Collections.Generic;

public class ActionEffect001_RecoveryHP : ActionEffectBase {

	private enum eParamIndex {
		recoveryValue,  // �񕜗�
	}

	private static readonly int _RECOVERY_LOG_ID = 1;

	public override async UniTask Execute( CharacterBase sourceCharacter, Entity_ActionEffectData.Param effectMaster, ActionRangeBase range ) {
		int recoveryValue = effectMaster.param[(int)eParamIndex.recoveryValue];
		List<int> targetList = range.targetList;
		int targetCount = targetList.Count;
		// �Ώۂ��Ƃɉ񕜂̏���
		for (int i = 0; i < targetCount; i++) {
			CharacterBase target = CharacterManager.instance.Get( targetList[i] );
			if (target == null) continue;

			// ���O�\��
			MenuRogueLog.instance.AddLog( string.Format( _RECOVERY_LOG_ID.ToMessage(), recoveryValue ) );
			target.AddHP( recoveryValue );
		}
		await UniTask.Delay( 1000 );
	}

}
