/**
 * @file MenuPlayerStatus.cs
 * @brief �v���C���[�̃X�e�[�^�X�\��
 * @author yao
 * @date 2025/2/6
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;
using Cysharp.Threading.Tasks;

public class MenuPlayerStatus : MenuBase {
	public static MenuPlayerStatus instance { get; private set; } = null;

	// �t���A��
	[SerializeField]
	private TextMeshProUGUI _floorCountText = null;
	// �v���C���[��HP�\�L
	[SerializeField]
	private TextMeshProUGUI _HPText = null;
	// �v���C���[�̖����x
	[SerializeField]
	private TextMeshProUGUI _staminaText = null;

	// �v���C���[�̍U���͕\�L
	[SerializeField]
	private TextMeshProUGUI _attackText = null;
	// �v���C���[�̖h��\�L
	[SerializeField]
	private TextMeshProUGUI _defenseText = null;

	StringBuilder _textStringBuilder = null;

	public override async UniTask Initialize() {
		await base.Initialize();
		_textStringBuilder = new StringBuilder();
		instance = this;
	}

	public void SetFloorCount(int floorCount) {
		_textStringBuilder.Append(floorCount);
		_textStringBuilder.Append('F');
		_floorCountText.text = _textStringBuilder.ToString();
		_textStringBuilder.Clear();
	}

	public void SetHP(int HP, int maxHP) {
		_textStringBuilder.Append(HP);
		_textStringBuilder.Append('/');
		_textStringBuilder.Append(maxHP);
		_HPText.text = _textStringBuilder.ToString();
		_textStringBuilder.Clear();
	}

	public void SetStamina(int showStamina) {
		_textStringBuilder.Append(showStamina);
		_textStringBuilder.Append("%");
		_staminaText.text = _textStringBuilder.ToString();
		_textStringBuilder.Clear();
	}

	public void SetAttack(int attackValue) {
		_attackText.text = attackValue.ToString();
	}

	public void SetDefense(int defenseValue) {
		_defenseText.text = defenseValue.ToString();
	}

}
