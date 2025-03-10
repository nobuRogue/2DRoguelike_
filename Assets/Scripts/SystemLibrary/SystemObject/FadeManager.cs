/**
 * @file FadeManager.cs
 * @brief 画面フェード処理
 * @author yao
 * @date 2025/2/4
 */
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeManager : SystemObject {
	[SerializeField]
	private Image _fadeImage = null;

	public static FadeManager instance { get; private set; } = null;

	private const float _DEFAULT_FADE_DURATION = 0.3f;

	public override async UniTask Initialize() {
		instance = this;
		await UniTask.CompletedTask;
	}

	/// <summary>
	/// フェードアウト、暗くする
	/// </summary>
	/// <param name="duration"></param>
	/// <returns></returns>
	public async UniTask FadeOut(float duration = _DEFAULT_FADE_DURATION) {
		await FadeTargetAlpha(1.0f, duration);
	}

	/// <summary>
	/// フェードイン、明るくする
	/// </summary>
	/// <param name="duration"></param>
	/// <returns></returns>
	public async UniTask FadeIn(float duration = _DEFAULT_FADE_DURATION) {
		await FadeTargetAlpha(0.0f, duration);
	}

	private async UniTask FadeTargetAlpha(float targetAlpha, float duration) {
		float elapsedTime = 0.0f;
		float startAlpha = _fadeImage.color.a;
		while (elapsedTime < duration) {
			elapsedTime += Time.deltaTime;
			Color fadeColor = _fadeImage.color;
			float t = elapsedTime / duration;
			fadeColor.a = Mathf.Lerp(startAlpha, targetAlpha, t);
			_fadeImage.color = fadeColor;

			await UniTask.DelayFrame(1);
		}
	}

}
