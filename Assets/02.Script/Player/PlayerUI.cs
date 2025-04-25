using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Unity.VisualScripting;

public class PlayerUI : MonoBehaviour
{
    public static PlayerUI Instance;

    [SerializeField] private TextMeshProUGUI _bombText;
    [SerializeField] private TextMeshProUGUI _bulltetCountText;
    [SerializeField] private Slider _bombChargeSlider;
    [SerializeField] private Slider _steminaSlider;
    private Coroutine _chargeCoroutine;
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        Player.Instance.GetComponent<PlayerFire>().BombCountChange += ShowBombs;
        Player.Instance.GetComponent<PlayerFire>().BulletCountChange += ShowBullets;
        Player.Instance.GetComponent<PlayerMove>().SteminaChanged += ShowStemina;
    }

    private void ShowBombs(int remain, int max)
    {
        _bombText.text = $"{remain}개/{max}개";
    }

    public void BombChargeShow(float duration)
    {
        _bombChargeSlider.gameObject.SetActive(true); // 슬라이더 보이기
        _chargeCoroutine = StartCoroutine(ChargeBombCoroutine(duration));
    }

    public void BombChargeHide()
    {
        if (_chargeCoroutine != null)
        {
            StopCoroutine(_chargeCoroutine);
            _chargeCoroutine = null;
        }
        _bombChargeSlider.gameObject.SetActive(false); // 슬라이더 숨기기
    }

    private IEnumerator ChargeBombCoroutine(float duration)
    {

        float elapsed = 0f;
        // 초기화
        _bombChargeSlider.value = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            _bombChargeSlider.value = Mathf.Clamp01(elapsed / duration); // 0~1 사이 비율
            yield return null;
        }

        // 끝에 정확히 맞춰주기
        _bombChargeSlider.value = 1f;

        _bombChargeSlider.gameObject.SetActive(false); // 슬라이더 숨기기   
    }

    private void ShowBullets(int remain, int max)
    {
        _bulltetCountText.text = $"{remain}/{max}";
    }
    private void ShowStemina()
    {
        _steminaSlider.value = Player.Instance.Stemina / Player.Instance.PlayerSO.SteminaMax;
    }
}
