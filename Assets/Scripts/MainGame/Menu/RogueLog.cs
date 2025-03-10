/**
 * @file RogueLog.cs
 * @brief 1�̃��O���b�Z�|�W
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
	/// ���O1�s�����g����ɗ���
	/// </summary>
	/// <returns></returns>
	public async UniTask FlowLog() {
		// �X�^�[�g�ƖړI�n�̌���
		float flowValue = rectTransForm.sizeDelta.y;
		Vector3 startPos = transform.position;
		Vector3 goalPos = startPos;
		goalPos.y += flowValue;
		// �K��̕b���������Ĉړ�
		float elapsedTime = 0.0f;
		while (elapsedTime < _FLOW_DURATION) {
			elapsedTime += Time.deltaTime;
			float t = elapsedTime / _FLOW_DURATION;
			transform.position = Vector3.Lerp(startPos, goalPos, t);

			await UniTask.DelayFrame(1);
		}
	}

}
