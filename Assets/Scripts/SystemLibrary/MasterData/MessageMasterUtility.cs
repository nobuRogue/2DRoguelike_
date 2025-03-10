/**
 * @file MessageMasterUtility.cs
 * @brief ���b�Z�[�W�}�X�^�[�f�[�^���s����
 * @author yao
 * @date 2025/2/27
 */

using System.Collections.Generic;

public class MessageMasterUtility {

	/// <summary>
	/// �}�X�^�[���烁�b�Z�[�W�擾
	/// </summary>
	/// <param name="ID"></param>
	/// <returns></returns>
	public static string GetMessage(int ID) {
		//�V�X�e���f�[�^����g�p����C���f�N�X���擾
		int languageIndex = 0;
		List<Entity_MessageData.Param> messageList = MasterDataManager.messageData[0];
		for (int i = 0, max = messageList.Count; i < max; i++) {
			if (messageList[i].ID != ID) continue;

			return messageList[i].Message[languageIndex];
		}
		return string.Empty;
	}
}
