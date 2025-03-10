/**
 * @file CharacterManager.cs
 * @brief キャラクター管理
 * @author yao
 * @date 2025/1/21
 */

using System.Collections.Generic;
using UnityEngine;

using static GameConst;
using static CommonModule;
using Cysharp.Threading.Tasks;

public class CharacterManager : MonoBehaviour {
	[SerializeField]
	private CharacterObject _characterObjectOrigin = null;

	[SerializeField]
	private Transform _useObjectRoot = null;

	[SerializeField]
	private Transform _unuseObjectRoot = null;

	public static CharacterManager instance { get; private set; } = null;
	// 使用中のキャラクターリスト
	private List<CharacterBase> _useList = null;
	// 未使用状態のプレイヤー
	private List<PlayerCharacter> _unusePlayer = null;
	// 未使用状態のエネミーリスト
	private List<EnemyCharacter> _unuseEnemyList = null;

	// 使用中のキャラクターオブジェクトリスト
	private List<CharacterObject> _useObjectList = null;
	// 未使用状態のキャラクターオブジェクトリスト
	private List<CharacterObject> _unuseObjectList = null;

	public void Initialize() {
		instance = this;
		CharacterBase.SetGetObjectCallback(GetCharacterObject);
		// 必要なキャラクターとオブジェクトのインスタンスを生成して未使用状態にしておく
		_useList = new List<CharacterBase>(FLOOR_ENEMY_MAX + 1);
		_useObjectList = new List<CharacterObject>(FLOOR_ENEMY_MAX + 1);

		_unusePlayer = new List<PlayerCharacter>(1);
		_unusePlayer.Add(new PlayerCharacter());

		_unuseEnemyList = new List<EnemyCharacter>(FLOOR_ENEMY_MAX);
		for (int i = 0; i < FLOOR_ENEMY_MAX; i++) {
			_unuseEnemyList.Add(new EnemyCharacter());
		}
		_unuseObjectList = new List<CharacterObject>(FLOOR_ENEMY_MAX + 1);
		for (int i = 0; i < FLOOR_ENEMY_MAX + 1; i++) {
			_unuseObjectList.Add(Instantiate(_characterObjectOrigin, _unuseObjectRoot));
		}
	}

	/// <summary>
	/// プレイヤーキャラの生成
	/// </summary>
	/// <param name="squareData"></param>
	public void UsePlayer(MapSquareData squareData, int masterID) {
		// インスタンスの取得
		PlayerCharacter usePlayer = null;
		if (IsEmpty(_unusePlayer)) {
			usePlayer = new PlayerCharacter();
		} else {
			usePlayer = _unusePlayer[0];
			_unusePlayer.RemoveAt(0);
		}
		// 使用可能なIDを取得して使用リストに追加
		int useID = UseCharacter(usePlayer);
		usePlayer.Setup(useID, squareData, masterID);
	}

	/// <summary>
	/// エネミーの生成
	/// </summary>
	/// <param name="squareData"></param>
	public void UseEnemy(MapSquareData squareData, int masterID) {
		// インスタンスの取得
		EnemyCharacter useEnemy = null;
		if (IsEmpty(_unuseEnemyList)) {
			useEnemy = new EnemyCharacter();
		} else {
			useEnemy = _unuseEnemyList[0];
			_unuseEnemyList.RemoveAt(0);
		}
		// 使用可能なIDを取得して使用リストに追加
		int useID = UseCharacter(useEnemy);
		useEnemy.Setup(useID, squareData, masterID);
	}

	private int UseCharacter(CharacterBase useCharacter) {
		// 使用可能なIDを取得して使用リストに追加
		int useID = -1;
		for (int i = 0, max = _useList.Count; i < max; i++) {
			if (_useList[i] != null) continue;

			useID = i;
			_useList[i] = useCharacter;
			break;
		}
		if (useID < 0) {
			useID = _useList.Count;
			_useList.Add(useCharacter);
		}
		// オブジェクトの取得
		CharacterObject useObject = null;
		if (IsEmpty(_unuseObjectList)) {
			useObject = Instantiate(_characterObjectOrigin);
		} else {
			useObject = _unuseObjectList[0];
			_unuseObjectList.RemoveAt(0);
		}
		// オブジェクトの使用リストへの追加
		while (!IsEnableIndex(_useObjectList, useID)) _useObjectList.Add(null);

		_useObjectList[useID] = useObject;
		useObject.transform.SetParent(_useObjectRoot);
		return useID;
	}

	public void UnuseEnemy(EnemyCharacter unuseEnemy) {
		if (unuseEnemy == null) return;

		int unuseID = unuseEnemy.ID;
		// マス情報から取り除く
		MapSquareManager.instance.Get(unuseEnemy.positionX, unuseEnemy.positionY)?.RemoveCharacter();
		// 使用リストから取り除く
		if (IsEnableIndex(_useList, unuseID)) _useList[unuseID] = null;
		// 片付け処理を読んで未使用リストに加える
		unuseEnemy.Teardown();
		_unuseEnemyList.Add(unuseEnemy);
		// オブジェクトを未使用にする
		UnuseObject(unuseID);
	}

	private void UnuseObject(int unuseID) {
		CharacterObject unuseCharacterObject = GetCharacterObject(unuseID);
		// 使用リストから取り除く
		if (IsEnableIndex(_useObjectList, unuseID)) _useObjectList[unuseID] = null;
		// 見えない場所に置く
		unuseCharacterObject.transform.SetParent(_unuseObjectRoot);
		// 未使用リストに追加
		_unuseObjectList.Add(unuseCharacterObject);
	}

	private CharacterObject GetCharacterObject(int ID) {
		if (!IsEnableIndex(_useObjectList, ID)) return null;

		return _useObjectList[ID];
	}

	public CharacterBase Get(int ID) {
		if (!IsEnableIndex(_useList, ID)) return null;

		return _useList[ID];
	}

	/// <summary>
	/// プレイヤー取得
	/// </summary>
	/// <returns></returns>
	public PlayerCharacter GetPlayer() {
		if (IsEmpty(_useList)) return null;

		for (int i = 0, max = _useList.Count; i < max; i++) {
			if (!_useList[i].IsPlayer()) continue;

			return _useList[i] as PlayerCharacter;
		}
		return null;
	}

	/// <summary>
	/// 全ての使用中キャラクターに指定の処理を実行
	/// </summary>
	/// <param name="action"></param>
	public void ExecuteAll(System.Action<CharacterBase> action) {
		if (action == null || IsEmpty(_useList)) return;

		for (int i = 0, max = _useList.Count; i < max; i++) {
			if (_useList[i] == null) continue;

			action(_useList[i]);
		}
	}

	/// <summary>
	/// 全ての使用中キャラクターに指定のタスクを実行
	/// </summary>
	/// <param name="task"></param>
	/// <returns></returns>
	public async UniTask ExecuteAllTask(System.Func<CharacterBase, UniTask> task) {
		if (task == null || IsEmpty(_useList)) return;

		for (int i = 0, max = _useList.Count; i < max; i++) {
			if (_useList[i] == null) continue;

			await task(_useList[i]);
		}
	}

}
