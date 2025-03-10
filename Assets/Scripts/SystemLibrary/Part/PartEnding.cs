/**
 * @file PartEnding.cs
 * @brief エンディングパート
 * @author yao
 * @date 2025/1/9
 */

using Cysharp.Threading.Tasks;

public class PartEnding : PartBase {
	public override async UniTask Initialize() {
		await MenuManager.instance.Get<MenuEnding>("Prefabs/Menu/CanvasEnding").Initialize();
	}

	public override async UniTask Setup() {
		await UniTask.CompletedTask;
	}

	public override async UniTask Execute() {
		MenuEnding endMenu = MenuManager.instance.Get<MenuEnding>();
		await endMenu.Open();
		await endMenu.Close();
		UniTask task = PartManager.instance.TransitionPart(eGamePart.Title);
	}

	public override async UniTask Teardown() {
		await UniTask.CompletedTask;
	}
}
