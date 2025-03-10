/**
 * @file MenuListItem.cs
 * @brief リスト項目の基底クラス
 * @author yao
 * @date 2025/3/6
 */
using UnityEngine;
using UnityEngine.UI;

public abstract class MenuListItem : MonoBehaviour {
	/// <summary>
	///	カーソルが当てられた際の表示画像
	/// </summary>
	[SerializeField]
	private Image _selectImage = null;

	/// <summary>
	/// カーソルが当てられた際の処理
	/// </summary>
	public virtual void Select() {
		if (_selectImage == null) return;

		_selectImage.enabled = true;
	}

	/// <summary>
	/// カーソルが外れた際の処理
	/// </summary>
	public virtual void Deselect() {
		if (_selectImage == null) return;

		_selectImage.enabled = false;
	}

}
