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
                missionText.text = "빵 앞으로 가시오!";
                break;
            case MissionManager.MissionState.FillShelf:
                missionText.text = "진열대에 빵을 채우시오!";
                break;
            case MissionManager.MissionState.Completed:
                missionText.text = "미션 완료!";
                break;
        }
    }
}
