/**
 * @file CharacterObject.cs
 * @brief �L�����N�^�[�I�u�W�F�N�g
 * @author yao
 * @date 2025/1/21
 */

using Cysharp.Threading.Tasks;
using System.Text;
using UnityEngine;

using static CommonModule;

public class CharacterObject : MonoBehaviour {
	private static StringBuilder _fileNameBuilder = new StringBuilder();
	private static readonly int _ANIMATION_DELAY_MILLI_SEC = 150;

	[SerializeField]
	private SpriteRenderer _characterSprite = null;

	// �L�����N�^�[�X�v���C�g�̃t�@�C���p�X
	private static readonly string _CHARACTER_SPRITE_PATH = "Design/Sprites/Character/";
	// �L�����N�^�[�X�v���C�g�̃A�j���[�V����������
	private static readonly string[] _ANIMATION_SPRITE_NAME = new string[] { "wait", "walk", "attack", "damage" };

	private Sprite[][] _characterSpriteList = null;

	private UniTask _animTask;
	public eCharacterAnimation currentAnim { get; private set; } = eCharacterAnimation.Invalid;
	private int _animIndex = -1;

	public void Setup(Entity_CharacterData.Param characterMaster) {
		if (characterMaster == null) return;
		// �}�X�^�[�f�[�^����X�v���C�g�擾
		string spriteName = characterMaster.spriteName;
		int animMax = (int)eCharacterAnimation.Max;
		_characterSpriteList = new Sprite[animMax][];
		for (int i = 0; i < animMax; i++) {
			_fileNameBuilder.Append(_CHARACTER_SPRITE_PATH);
			_fileNameBuilder.Append(spriteName);
			_fileNameBuilder.Append(_ANIMATION_SPRITE_NAME[i]);
			_characterSpriteList[i] = Resources.LoadAll<Sprite>(_fileNameBuilder.ToString());
			_fileNameBuilder.Clear();
		}
		// �ҋ@�A�j���[�V������ݒ�
		SetAnimation(eCharacterAnimation.Wait);
		// �A�j���[�V�����Đ��^�X�N�����s
		if (_animTask.Status.IsCompleted()) _animTask = PlayAnimationTask();

	}

	public void Teardown() {

	}

	public void SetDirection(eDirectionEight dir) {
		switch (dir) {
			case eDirectionEight.UpRight:
			case eDirectionEight.Right:
			case eDirectionEight.DownRight:
			Vector3 scale = _characterSprite.transform.localScale;
			scale.x = 1.0f;
			_characterSprite.transform.localScale = scale;
			break;
			case eDirectionEight.DownLeft:
			case eDirectionEight.Left:
			case eDirectionEight.UpLeft:
			scale = _characterSprite.transform.localScale;
			scale.x = -1.0f;
			_characterSprite.transform.localScale = scale;
			break;
		}
	}

	public void SetPosition(Vector3 position) {
		transform.position = position;
	}

	public void SetAnimation(eCharacterAnimation setAnim) {
		// ���݂Ɠ����A�j���Ȃ瑖�点�Ȃ�
		if (setAnim == currentAnim) return;

		currentAnim = setAnim;
		_animIndex = 0;
	}

	/// <summary>
	/// �A�j���[�V�������Đ���������^�X�N
	/// </summary>
	/// <returns></returns>
	private async UniTask PlayAnimationTask() {
		while (true) {
			int currentAnimIndex = (int)currentAnim;
			if (IsEnableIndex(_characterSpriteList, currentAnimIndex)) {
				Sprite[] animSpriteList = _characterSpriteList[currentAnimIndex];
				// �A�j���[�V�����̃��[�v����
				if (!IsEnableIndex(animSpriteList, _animIndex)) AnimationLoopProcess();

				_characterSprite.sprite = animSpriteList[_animIndex];
			}
			_animIndex++;
			await UniTask.Delay(_ANIMATION_DELAY_MILLI_SEC);
		}
	}

	/// <summary>
	/// �A�j���[�V���������[�v����ۂ̏���
	/// </summary>
	private void AnimationLoopProcess() {
		// �U������_���[�W�Ȃ�1���[�v�őҋ@�ɖ߂�
		if (currentAnim == eCharacterAnimation.Attack ||
			currentAnim == eCharacterAnimation.Damage) {
			SetAnimation(eCharacterAnimation.Wait);
		} else {
			_animIndex = 0;
		}
	}

}
