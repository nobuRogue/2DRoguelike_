/**
 * @file PartTitle.cs
 * @brief タイトルパート
 * @author yao
 * @date 2025/1/9
 */

using Cysharp.Threading.Tasks;

public class PartTitle : PartBase {
	public override async UniTask Initialize() {
		await MenuManager.instance.Get<MenuTitle>("Prefabs/Menu/CanvasTitle").Initialize();
	}

	public override async UniTask Execute() {
		UserDataHolder.SetCurrentData(new UserData());

		MenuTitle titleMenu = MenuManager.instance.Get<MenuTitle>();
		await titleMenu.Open();
		await titleMenu.Close();
		UniTask task = PartManager.instance.TransitionPart(eGamePart.MainGame);
	}

	public override async UniTask Setup() {
		await UniTask.CompletedTask;
	}

	public override async UniTask Teardown() {
		await UniTask.CompletedTask;
	}
}
