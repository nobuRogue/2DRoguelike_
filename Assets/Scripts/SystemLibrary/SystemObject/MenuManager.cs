/**
 * @file MenuManager.cs
 * @brief ���j���[�Ǘ�
 * @author yao
 * @date 2025/2/6
 */

using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : SystemObject {
	private List<GameObject> _menuObjectList = null;
	public static MenuManager instance { get; private set; }

	public override async UniTask Initialize() {
		instance = this;
		_menuObjectList = new List<GameObject>(256);
		await UniTask.CompletedTask;
	}

	/// <summary>
	/// ���j���[�̎擾
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="name"></param>
	/// <returns></returns>
	public T Get<T>(string name = null) where T : MenuBase {
		// �L���b�V�������I�u�W�F�N�g����T��
		for (int i = 0, max = _menuObjectList.Count; i < max; i++) {
			T menu = _menuObjectList[i].GetComponent<T>();
			if (menu == null) continue;

			return menu;
		}
		// �Ȃ��̂Ő�������
		return Load<T>(name);
	}

	/// <summary>
	/// ���j���[�̓ǂݍ���
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="name"></param>
	/// <returns></returns>

	public T Load<T>(string name) where T : MenuBase {
		var loadObject = Resources.Load(name) as GameObject;
		if (loadObject == null) return null;

		GameObject createObject = Instantiate(loadObject, transform);
		T menu = createObject.GetComponent<T>();
		if (menu == null) return null;

		createObject.SetActive(false);
		_menuObjectList.Add(createObject);
		return menu;
	}

}
