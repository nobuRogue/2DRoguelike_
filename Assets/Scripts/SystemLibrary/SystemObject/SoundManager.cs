/**
 * @file SoundManager.cs
 * @brief サウンド管理
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
	/// BGM再生
	/// </summary>
	/// <param name="bgmID"></param>
	public void PlayBGM(int bgmID) {
		if (!IsEnableIndex(_bgmAssign.bgmArray, bgmID)) return;

		_bgmAudioSource.clip = _bgmAssign.bgmArray[bgmID];
		_bgmAudioSource.Play();
	}

	/// <summary>
	/// BGMを止める
	/// </summary>
	public void StopBGM() {
		_bgmAudioSource.Stop();
	}

	/// <summary>
	/// SE再生
	/// </summary>
	/// <param name="seID"></param>
	/// <returns>再生したオーディオソースのインデクス</returns>
	public async UniTask<int> PlaySE(int seID, bool isLoop = false) {
		if (!IsEnableIndex(_seAssign.seArray, seID)) return -1;
		// 再生中でないオーディオソースを探してそれで再生
		for (int i = 0, max = _seAudioSourceArray.Length; i < max; i++) {
			AudioSource seAudioSource = _seAudioSourceArray[i];
			if (seAudioSource.isPlaying) continue;
			// 再生中でないオーディソースが見つかったので再生
			seAudioSource.clip = _seAssign.seArray[seID];
			seAudioSource.loop = isLoop;
			seAudioSource.Play();
			// 再生の終了待ちをする
			while (seAudioSource.isPlaying) await UniTask.DelayFrame(1);

			return i;
		}
		return -1;
	}

	/// <summary>
	/// SEを止める
	/// </summary>
	/// <param name="audioSourceIndex">止めるオーディオソースのインデクス</param>
	public void StopSE(int audioSourceIndex) {
		if (!IsEnableIndex(_seAudioSourceArray, audioSourceIndex)) return;

		_seAudioSourceArray[audioSourceIndex].Stop();
	}
}
