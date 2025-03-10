/**
 * @file ActionEffect000_Attack.cs
 * @brief �ʏ�U���̌��ʏ���
 * @author yao
 * @date 2025/2/18
 */

using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

using static CommonModule;

public class ActionEffect000_Attack : ActionEffectBase {

	private const int _ATTACK_HIT_SE_ID = 0;

	public override async UniTask Execute(CharacterBase sourceCharacter, ActionRangeBase range) {
		// �s���҂̍U���A�j���[�V�����Đ�
		sourceCharacter.SetAnimation(eCharacterAnimation.Attack);
		int sourceAttack = sourceCharacter.attack;
		List<int> targetList = range.targetList;
		int targetCount = targetList.Count;
		List<UniTask> taskList = new List<UniTask>(targetCount);
		// �Ώۂ��ƂɍU���̏���
		for (int i = 0; i < targetCount; i++) {
			CharacterBase target = CharacterManager.instance.Get(targetList[i]);
			if (target == null) continue;

			UniTask task = SoundManager.instance.PlaySE(_ATTACK_HIT_SE_ID);
			taskList.Add(ExecuteAttack(sourceAttack, target));
		}
		// �U���A�j���[�V�����̏I���҂�
		while (sourceCharacter.GetCurrentAnimation() == eCharacterAnimation.Attack) await UniTask.DelayFrame(1);

		await WaitTask(taskList);
	}

	private async UniTask ExecuteAttack(int sourceAttack, CharacterBase targetCharacter) {
		// �Ώۂ̔�_���[�W�A�j���[�V����
		targetCharacter.SetAnimation(eCharacterAnimation.Damage);
		// �_���[�W�v�Z
		int defense = targetCharacter.defense;
		int damage = (int)(sourceAttack * Mathf.Pow(15.0f / 16.0f, defense));
		// ���O�\��
		MenuRogueLog.instance.AddLog(string.Format(0.ToMessage(), damage));
		// HP�����炷
		targetCharacter.RemoveHP(damage);
		// �A�j���[�V�����̏I���҂�
		while (targetCharacter.GetCurrentAnimation() == eCharacterAnimation.Damage) await UniTask.DelayFrame(1);

		// ���S����A����
		if (!targetCharacter.IsDead()) return;

		await CharacterUtility.DeadCharacter(targetCharacter);
	}

}
