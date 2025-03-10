/**
 * @file CharacterUtility.cs
 * @brief キャラクター関連実行処理
 * @author yao
 * @date 2025/2/20
 */

using Cysharp.Threading.Tasks;
using System;

public class CharacterUtility {

	private static Action<eDungeonEndReason> _EndDungeon = null;

	public static void SetEndDungeonCallback(Action<eDungeonEndReason> setDungeonProcess) {
		_EndDungeon = setDungeonProcess;
	}

	/// <summary>
	/// プレイヤー取得
	/// </summary>
	/// <returns></returns>
	public static PlayerCharacter GetPlayer() {
		return CharacterManager.instance.GetPlayer();
	}

	/// <summary>
	/// キャラクターデータ取得
	/// </summary>
	/// <param name="ID"></param>
	/// <returns></returns>
	public static CharacterBase GetCharacter(int ID) {
		return CharacterManager.instance.Get(ID);
	}

	/// <summary>
	/// 全てのキャラクターに処理実行
	/// </summary>
	/// <param name="action"></param>
	public static void ExecuteAllCharacter(System.Action<CharacterBase> action) {
		CharacterManager.instance.ExecuteAll(action);
	}

	/// <summary>
	/// 全てのキャラクターにタスク実行
	/// </summary>
	/// <param name="task"></param>
	/// <returns></returns>
	public static async UniTask ExecuteTaskAllCharacter(System.Func<CharacterBase, UniTask> task) {
		await CharacterManager.instance.ExecuteAllTask(task);
	}

	/// <summary>
	/// キャラクターの死亡処理
	/// </summary>
	/// <param name="deadCharacter"></param>
	/// <returns></returns>
	public static async UniTask DeadCharacter(CharacterBase deadCharacter) {
		if (deadCharacter.IsPlayer()) {
			// プレイヤー死亡の処理
			_EndDungeon(eDungeonEndReason.Dead);
		} else {
			// エネミー死亡の処理
			CharacterManager.instance.UnuseEnemy(deadCharacter as EnemyCharacter);
		}
		await UniTask.CompletedTask;
	}

}
