using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Unity.VisualScripting;
using UnityEditor.Search;

public class PlayerUI : MonoBehaviour
{
    public static PlayerUI Instance;

    [SerializeField] private TextMeshProUGUI _bombText;
    [SerializeField] private TextMeshProUGUI _bulltetCountText;
    [SerializeField] private Slider _bombChargeSlider;
    [SerializeField] private Slider _steminaSlider;
    [SerializeField] private Image _bloodScreen;
    [SerializeField] private float BloodTime = 3;
    [SerializeField] private Slider _playerHealthSlider;
    private Coroutine _chargeCoroutine;
    private Coroutine _bloodCoroutine;

    private void Awake()
    {
        Instance = this;
        GameObject[] guns = GameObject.FindGameObjectsWithTag("Gun");

        foreach (GameObject gun in guns)
        {
            PlayerFire fire = gun.GetComponent<PlayerFire>();
            if (fire != null)
            {
                fire.BombCountChange += ShowBombs;
                fire.BulletCountChange += ShowBullets;
            }
        }
        Player.Instance.GetComponent<PlayerMove>().SteminaChanged += ShowStemina;
        Player.Instance.GetComponent<Player>().PlayerHealthChanged += ChangedPlayerHealth;
        ChangedPlayerHealth();
    }
    private void Start()
    {

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
    }

    private void ShowBullets(int remain, int max)
    {
        _bulltetCountText.text = $"{remain}/{max}";
    }
    private void ShowStemina()
    {
        _steminaSlider.value = Player.Instance.Stemina / Player.Instance.PlayerSO.SteminaMax;
    }
    public void ShowBloodScreen()
    {
        if (_bloodCoroutine != null)
        {
            StopCoroutine(_bloodCoroutine);
            _bloodCoroutine = null;
        }
        _bloodCoroutine = StartCoroutine(BloodScreenCoroutine());
    }
    private IEnumerator BloodScreenCoroutine()
    {
        float duration = BloodTime; // 총 5초
        float elapsed = 0f;

        Color color = _bloodScreen.color; // 시작 색 가져오기
        color.a = 1f; // 알파 1로 시작
        _bloodScreen.color = color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Lerp(1f, 0f, elapsed / duration); // 1 → 0 선형 보간
            _bloodScreen.color = color;

            yield return null; // 매 프레임 대기
        }

        // 마지막으로 확실히 0으로 세팅
        color.a = 0f;
        _bloodScreen.color = color;
    }

    public void ChangedPlayerHealth()
    {
        _playerHealthSlider.value = Player.Instance.Health / Player.Instance.PlayerSO.HealthMax;
    }
}
