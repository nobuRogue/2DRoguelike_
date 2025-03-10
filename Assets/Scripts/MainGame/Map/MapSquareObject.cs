/**
 * @file MapSquareObject.cs
 * @brief 1�}�X�̃I�u�W�F�N�g
 * @author yao
 * @date 2025/1/9
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSquareObject : MonoBehaviour {
	[SerializeField]
	private SpriteRenderer _terrainSprite = null;

	/// <summary>
	/// �L�����N�^�[���ړ�������W
	/// </summary>
	[SerializeField]
	private Transform _characterRoot = null;

	[SerializeField]
	private SpriteRenderer _mark = null;

	public void Setup(int setX, int setY) {
		Vector3 position = transform.position;
		position.x = setX * 0.32f;
		position.y = setY * 0.32f;
		position.z = setY * 0.1f;
		transform.position = position;
		HideMark();
	}

	public void SetTerrain(eTerrain setTerrain, int spriteIndex = -1) {
		// �n�`�ɉ������X�v���C�g�̐ݒ�
		_terrainSprite.sprite = TerrainSpriteAssignor.GetTerrainSprite(setTerrain, spriteIndex);
	}

	public Transform GetCharacterRoot() {
		return _characterRoot;
	}

	public void ShowMark(Color color) {
		_mark.color = color;
		_mark.enabled = true;
	}

	public void HideMark() {
		_mark.enabled = false;
	}

}
