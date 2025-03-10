/**
 * @file CharacterManager.cs
 * @brief �L�����N�^�[�Ǘ�
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
	// �g�p���̃L�����N�^�[���X�g
	private List<CharacterBase> _useList = null;
	// ���g�p��Ԃ̃v���C���[
	private List<PlayerCharacter> _unusePlayer = null;
	// ���g�p��Ԃ̃G�l�~�[���X�g
	private List<EnemyCharacter> _unuseEnemyList = null;

	// �g�p���̃L�����N�^�[�I�u�W�F�N�g���X�g
	private List<CharacterObject> _useObjectList = null;
	// ���g�p��Ԃ̃L�����N�^�[�I�u�W�F�N�g���X�g
	private List<CharacterObject> _unuseObjectList = null;

	public void Initialize() {
		instance = this;
		CharacterBase.SetGetObjectCallback(GetCharacterObject);
		// �K�v�ȃL�����N�^�[�ƃI�u�W�F�N�g�̃C���X�^���X�𐶐����Ė��g�p��Ԃɂ��Ă���
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
	/// �v���C���[�L�����̐���
	/// </summary>
	/// <param name="squareData"></param>
	public void UsePlayer(MapSquareData squareData, int masterID) {
		// �C���X�^���X�̎擾
		PlayerCharacter usePlayer = null;
		if (IsEmpty(_unusePlayer)) {
			usePlayer = new PlayerCharacter();
		} else {
			usePlayer = _unusePlayer[0];
			_unusePlayer.RemoveAt(0);
		}
		// �g�p�\��ID���擾���Ďg�p���X�g�ɒǉ�
		int useID = UseCharacter(usePlayer);
		usePlayer.Setup(useID, squareData, masterID);
	}

	/// <summary>
	/// �G�l�~�[�̐���
	/// </summary>
	/// <param name="squareData"></param>
	public void UseEnemy(MapSquareData squareData, int masterID) {
		// �C���X�^���X�̎擾
		EnemyCharacter useEnemy = null;
		if (IsEmpty(_unuseEnemyList)) {
			useEnemy = new EnemyCharacter();
		} else {
			useEnemy = _unuseEnemyList[0];
			_unuseEnemyList.RemoveAt(0);
		}
		// �g�p�\��ID���擾���Ďg�p���X�g�ɒǉ�
		int useID = UseCharacter(useEnemy);
		useEnemy.Setup(useID, squareData, masterID);
	}

	private int UseCharacter(CharacterBase useCharacter) {
		// �g�p�\��ID���擾���Ďg�p���X�g�ɒǉ�
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
		// �I�u�W�F�N�g�̎擾
		CharacterObject useObject = null;
		if (IsEmpty(_unuseObjectList)) {
			useObject = Instantiate(_characterObjectOrigin);
		} else {
			useObject = _unuseObjectList[0];
			_unuseObjectList.RemoveAt(0);
		}
		// �I�u�W�F�N�g�̎g�p���X�g�ւ̒ǉ�
		while (!IsEnableIndex(_useObjectList, useID)) _useObjectList.Add(null);

		_useObjectList[useID] = useObject;
		useObject.transform.SetParent(_useObjectRoot);
		return useID;
	}

	public void UnuseEnemy(EnemyCharacter unuseEnemy) {
		if (unuseEnemy == null) return;

		int unuseID = unuseEnemy.ID;
		// �}�X��񂩂��菜��
		MapSquareManager.instance.Get(unuseEnemy.positionX, unuseEnemy.positionY)?.RemoveCharacter();
		// �g�p���X�g�����菜��
		if (IsEnableIndex(_useList, unuseID)) _useList[unuseID] = null;
		// �Еt��������ǂ�Ŗ��g�p���X�g�ɉ�����
		unuseEnemy.Teardown();
		_unuseEnemyList.Add(unuseEnemy);
		// �I�u�W�F�N�g�𖢎g�p�ɂ���
		UnuseObject(unuseID);
	}

	private void UnuseObject(int unuseID) {
		CharacterObject unuseCharacterObject = GetCharacterObject(unuseID);
		// �g�p���X�g�����菜��
		if (IsEnableIndex(_useObjectList, unuseID)) _useObjectList[unuseID] = null;
		// �����Ȃ��ꏊ�ɒu��
		unuseCharacterObject.transform.SetParent(_unuseObjectRoot);
		// ���g�p���X�g�ɒǉ�
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
	/// �v���C���[�擾
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
	/// �S�Ă̎g�p���L�����N�^�[�Ɏw��̏��������s
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
	/// �S�Ă̎g�p���L�����N�^�[�Ɏw��̃^�X�N�����s
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
