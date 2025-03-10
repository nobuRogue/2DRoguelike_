/**
 * @file RogueLog.cs
 * @brief 1つのログメッセ－ジ
 * @author yao
 * @date 2025/2/27
 */

using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;

public class RogueLog : MonoBehaviour {
	private static readonly float _FLOW_DURATION = 0.5f;

	[SerializeField]
	TextMeshProUGUI _logText = null;

	[SerializeField]
	RectTransform rectTransForm = null;

	public void Setup(string showText) {
		_logText.text = showText;
	}

	public void Teardown() {
		_logText.text = string.Empty;
	}

	/// <summary>
	/// ログ1行分自身を上に流す
	/// </summary>
	/// <returns></returns>
	public async UniTask FlowLog() {
		// スタートと目的地の決定
		float flowValue = rectTransForm.sizeDelta.y;
		Vector3 startPos = transform.position;
		Vector3 goalPos = startPos;
		goalPos.y += flowValue;
		// 規定の秒数をかけて移動
		float elapsedTime = 0.0f;
		while (elapsedTime < _FLOW_DURATION) {
			elapsedTime += Time.deltaTime;
			float t = elapsedTime / _FLOW_DURATION;
			transform.position = Vector3.Lerp(startPos, goalPos, t);

			await UniTask.DelayFrame(1);
		}
	}

}
