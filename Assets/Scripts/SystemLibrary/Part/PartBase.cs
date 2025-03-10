/**
 * @file PartBase.cs
 * @brief �Q�[���p�[�g�̊��
 * @author yao
 * @date 2025/1/9
 */

using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PartBase : MonoBehaviour {
	/// <summary>
	/// ������
	/// </summary>
	/// <returns></returns>
	public abstract UniTask Initialize();
	/// <summary>
	/// ���s�O�̏���
	/// </summary>
	/// <returns></returns>
	public abstract UniTask Setup();
	/// <summary>
	/// ���s
	/// </summary>
	/// <returns></returns>
	public abstract UniTask Execute();
	/// <summary>
	/// �Еt��
	/// </summary>
	/// <returns></returns>
	public abstract UniTask Teardown();
}
