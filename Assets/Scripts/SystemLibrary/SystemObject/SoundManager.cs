/**
 * @file SoundManager.cs
 * @brief �T�E���h�Ǘ�
 * @author yao
 * @date 2025/2/25
 */

using Cysharp.Threading.Tasks;
using UnityEngine;

using static CommonModule;

public class SoundManager : SystemObject {
	[SerializeField]
	private AudioSource _bgmAudioSource = null;
	[SerializeField]
	private AudioSource[] _seAudioSourceArray = null;

	[SerializeField]
	private BGMAssign _bgmAssign = null;
	[SerializeField]
	private SEAssign _seAssign = null;

	public static SoundManager instance { get; private set; } = null;

	public override async UniTask Initialize() {
		instance = this;
		await UniTask.CompletedTask;
	}

	/// <summary>
	/// BGM�Đ�
	/// </summary>
	/// <param name="bgmID"></param>
	public void PlayBGM(int bgmID) {
		if (!IsEnableIndex(_bgmAssign.bgmArray, bgmID)) return;

		_bgmAudioSource.clip = _bgmAssign.bgmArray[bgmID];
		_bgmAudioSource.Play();
	}

	/// <summary>
	/// BGM���~�߂�
	/// </summary>
	public void StopBGM() {
		_bgmAudioSource.Stop();
	}

	/// <summary>
	/// SE�Đ�
	/// </summary>
	/// <param name="seID"></param>
	/// <returns>�Đ������I�[�f�B�I�\�[�X�̃C���f�N�X</returns>
	public async UniTask<int> PlaySE(int seID, bool isLoop = false) {
		if (!IsEnableIndex(_seAssign.seArray, seID)) return -1;
		// �Đ����łȂ��I�[�f�B�I�\�[�X��T���Ă���ōĐ�
		for (int i = 0, max = _seAudioSourceArray.Length; i < max; i++) {
			AudioSource seAudioSource = _seAudioSourceArray[i];
			if (seAudioSource.isPlaying) continue;
			// �Đ����łȂ��I�[�f�B�\�[�X�����������̂ōĐ�
			seAudioSource.clip = _seAssign.seArray[seID];
			seAudioSource.loop = isLoop;
			seAudioSource.Play();
			// �Đ��̏I���҂�������
			while (seAudioSource.isPlaying) await UniTask.DelayFrame(1);

			return i;
		}
		return -1;
	}

	/// <summary>
	/// SE���~�߂�
	/// </summary>
	/// <param name="audioSourceIndex">�~�߂�I�[�f�B�I�\�[�X�̃C���f�N�X</param>
	public void StopSE(int audioSourceIndex) {
		if (!IsEnableIndex(_seAudioSourceArray, audioSourceIndex)) return;

		_seAudioSourceArray[audioSourceIndex].Stop();
	}
}
