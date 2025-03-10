/**
 * @file MessageMasterUtility.cs
 * @brief メッセージマスターデータ実行処理
 * @author yao
 * @date 2025/2/27
 */

using System.Collections.Generic;

public class MessageMasterUtility {

	/// <summary>
	/// マスターからメッセージ取得
	/// </summary>
	/// <param name="ID"></param>
	/// <returns></returns>
	public static string GetMessage(int ID) {
		//システムデータから使用言語インデクスを取得
		int languageIndex = 0;
		List<Entity_MessageData.Param> messageList = MasterDataManager.messageData[0];
		for (int i = 0, max = messageList.Count; i < max; i++) {
			if (messageList[i].ID != ID) continue;

			return messageList[i].Message[languageIndex];
		}
		return string.Empty;
	}
}
