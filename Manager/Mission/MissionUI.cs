using UnityEngine;
using UnityEngine.UI;

public class MissionUI : MonoBehaviour
{
    public Text missionText;

    private void OnEnable()
    {
        MissionManager.OnMissionChanged += UpdateMissionText;
    }

    private void OnDisable()
    {
        MissionManager.OnMissionChanged -= UpdateMissionText;
    }

    void UpdateMissionText(MissionManager.MissionState state)
    {
        switch (state)
        {
            case MissionManager.MissionState.GoToBread:
                missionText.text = "�� ������ ���ÿ�!";
                break;
            case MissionManager.MissionState.FillShelf:
                missionText.text = "�����뿡 ���� ä��ÿ�!";
                break;
            case MissionManager.MissionState.Completed:
                missionText.text = "�̼� �Ϸ�!";
                break;
        }
    }
}
