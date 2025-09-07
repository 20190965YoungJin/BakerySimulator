using UnityEngine;
using TMPro;

/// <summary>
/// 손님의 머리 위에 이모트(생각풍선)를 관리합니다.
/// </summary>
public class EmoteController : MonoBehaviour
{
    public GameObject balloon;
    public GameObject breadEmote;
    public TMP_Text breadCountText;
    public GameObject takeoutEmote;
    public GameObject counterEmote;
    [HideInInspector]
    public ParticleSystem emojiSmile;

    private CustomerAi customerAi;

    private void Awake()
    {
        customerAi = GetComponent<CustomerAi>();
        emojiSmile = GetComponentInChildren<ParticleSystem>();
        HideAllEmote();
        balloon.SetActive(false);
    }

    private void Update()
    {
        WantBreadCountText();
    }

    public void UpdateEmote()
    {
        // 전부 끄기
        HideAllEmote();

        if (!customerAi.getBread) // 진열대에 도착했는데 빵을 다 못 집었을 때
        {
            balloon.SetActive(true);
            breadEmote.SetActive(true);
        }
        else // 빵 다 얻음
        {
            if (!customerAi.isBuy) // 계산 or 식사 안함
            {
                balloon.SetActive(true);

                if (customerAi.isTakeout) // 테이크아웃
                {
                    takeoutEmote.SetActive(true);
                }
                else // 매장 식사
                {
                    counterEmote.SetActive(true);
                }
            }
            else // 계산 or 식사 완료
            {
                balloon.SetActive(false);
            }
        }
    }


    private void HideAllEmote()
    {
        breadEmote.SetActive(false);
        takeoutEmote.SetActive(false);
        counterEmote.SetActive(false);
    }

    private void WantBreadCountText()
    {
        // breadCountText 업데이트 추가
        int remainingBread = customerAi.breadTargetCount - customerAi.breadCount;
        breadCountText.text = remainingBread.ToString();
    }
}
