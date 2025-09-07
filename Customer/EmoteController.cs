using UnityEngine;
using TMPro;

/// <summary>
/// �մ��� �Ӹ� ���� �̸�Ʈ(����ǳ��)�� �����մϴ�.
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
        // ���� ����
        HideAllEmote();

        if (!customerAi.getBread) // �����뿡 �����ߴµ� ���� �� �� ������ ��
        {
            balloon.SetActive(true);
            breadEmote.SetActive(true);
        }
        else // �� �� ����
        {
            if (!customerAi.isBuy) // ��� or �Ļ� ����
            {
                balloon.SetActive(true);

                if (customerAi.isTakeout) // ����ũ�ƿ�
                {
                    takeoutEmote.SetActive(true);
                }
                else // ���� �Ļ�
                {
                    counterEmote.SetActive(true);
                }
            }
            else // ��� or �Ļ� �Ϸ�
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
        // breadCountText ������Ʈ �߰�
        int remainingBread = customerAi.breadTargetCount - customerAi.breadCount;
        breadCountText.text = remainingBread.ToString();
    }
}
