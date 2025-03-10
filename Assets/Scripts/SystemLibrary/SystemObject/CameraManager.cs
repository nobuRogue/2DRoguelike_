/**
 * @file CameraManager.cs
 * @brief ÉJÉÅÉâä«óù
 * @author yao
 * @date 2025/1/9
 */
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : SystemObject, PlayerMoveObserver {
	public Camera _camera { get; private set; } = null;
	private string _CAMERA_NAME = "Main Camera";

	public static CameraManager instance { get; private set; } = null;

	public override async UniTask Initialize() {
		instance = this;
		_camera = GameObject.Find(_CAMERA_NAME).GetComponent<Camera>();
		await UniTask.CompletedTask;
	}

	public void OnPlayerMove(Vector3 playerPosition) {
		playerPosition.z = -10.0f;
		_camera.transform.position = playerPosition;
	}
}
