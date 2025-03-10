/**
 * @file ActionEffect000_Attack.cs
 * @brief 通常攻撃の効果処理
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
		// 行動者の攻撃アニメーション再生
		sourceCharacter.SetAnimation(eCharacterAnimation.Attack);
		int sourceAttack = sourceCharacter.attack;
		List<int> targetList = range.targetList;
		int targetCount = targetList.Count;
		List<UniTask> taskList = new List<UniTask>(targetCount);
		// 対象ごとに攻撃の処理
		for (int i = 0; i < targetCount; i++) {
			CharacterBase target = CharacterManager.instance.Get(targetList[i]);
			if (target == null) continue;

			UniTask task = SoundManager.instance.PlaySE(_ATTACK_HIT_SE_ID);
			taskList.Add(ExecuteAttack(sourceAttack, target));
		}
		// 攻撃アニメーションの終了待ち
		while (sourceCharacter.GetCurrentAnimation() == eCharacterAnimation.Attack) await UniTask.DelayFrame(1);

		await WaitTask(taskList);
	}

	private async UniTask ExecuteAttack(int sourceAttack, CharacterBase targetCharacter) {
		// 対象の被ダメージアニメーション
		targetCharacter.SetAnimation(eCharacterAnimation.Damage);
		// ダメージ計算
		int defense = targetCharacter.defense;
		int damage = (int)(sourceAttack * Mathf.Pow(15.0f / 16.0f, defense));
		// ログ表示
		MenuRogueLog.instance.AddLog(string.Format(0.ToMessage(), damage));
		// HPを減らす
		targetCharacter.RemoveHP(damage);
		// アニメーションの終了待ち
		while (targetCharacter.GetCurrentAnimation() == eCharacterAnimation.Damage) await UniTask.DelayFrame(1);

		// 死亡判定、処理
		if (!targetCharacter.IsDead()) return;

		await CharacterUtility.DeadCharacter(targetCharacter);
	}

}
