/**
 * @file PlayerMoveObserver.cs
 * @brief �v���C���[�ړ��p�I�u�U�[�o
 * @author yao
 * @date 2025/1/21
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface PlayerMoveObserver {
	void OnPlayerMove(Vector3 playerPosition);
}
