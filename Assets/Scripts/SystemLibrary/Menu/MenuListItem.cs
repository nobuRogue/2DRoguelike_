/**
 * @file MenuListItem.cs
 * @brief ���X�g���ڂ̊��N���X
 * @author yao
 * @date 2025/3/6
 */
using UnityEngine;
using UnityEngine.UI;

public abstract class MenuListItem : MonoBehaviour {
	/// <summary>
	///	�J�[�\�������Ă�ꂽ�ۂ̕\���摜
	/// </summary>
	[SerializeField]
	private Image _selectImage = null;

	/// <summary>
	/// �J�[�\�������Ă�ꂽ�ۂ̏���
	/// </summary>
	public virtual void Select() {
		if (_selectImage == null) return;

		_selectImage.enabled = true;
	}

	/// <summary>
	/// �J�[�\�����O�ꂽ�ۂ̏���
	/// </summary>
	public virtual void Deselect() {
		if (_selectImage == null) return;

		_selectImage.enabled = false;
	}

}
