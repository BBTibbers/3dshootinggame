using System;
using TMPro;
using UnityEngine;
using DG.Tweening;
using System.Security.Cryptography;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class UI_LoginScene : MonoBehaviour
{
    [Serializable]
    public class UI_InputField
    {
        public TMP_InputField IDInputField;
        public TMP_InputField PasswordInputField;
        public TMP_InputField PasswordConfirmField;
        public TextMeshProUGUI OutputText;
        public Button ConfirmButton;
    }

    private const string SALT = "10043420";

    public GameObject LoginPanel;
    public GameObject RegisterPanel;

    [Header("로그인")]
    public UI_InputField LoginInputField;
    [Header("회원가입")]
    public UI_InputField RegisterInputField;


    private void Start()
    {
        LoginPanel.SetActive(true);
        RegisterPanel.SetActive(false);
        LoginCheck();
    }

    public void GotoRegister()
    {
        LoginPanel.SetActive(false);
        RegisterPanel.SetActive(true);
    }
    public void GotoLogin()
    {
        LoginPanel.SetActive(true);
        RegisterPanel.SetActive(false);
    }

    public void Register()
    {
        string id = RegisterInputField.IDInputField.text;
        if (string.IsNullOrEmpty(id))
        {
            RegisterInputField.OutputText.text = "아이디를 입력하세요.";
            RegisterInputField.OutputText.transform.DOShakeScale(0.5f, 0.1f, 10, 90, true);
            return;
        }
        string password = RegisterInputField.PasswordInputField.text;
        if (string.IsNullOrEmpty(password))
        {
            RegisterInputField.OutputText.text = "비밀번호를 입력하세요.";
            RegisterInputField.OutputText.transform.DOShakeScale(0.5f, 0.1f, 10, 90, true);
            return;
        }
        string passwordConfirm = RegisterInputField.PasswordConfirmField.text;
        if (string.Compare(password, passwordConfirm) != 0)
        {
            RegisterInputField.OutputText.text = "비밀번호가 일치하지 않습니다.";
            RegisterInputField.OutputText.transform.DOShakeScale(0.5f, 0.1f, 10, 90, true);
            return;
        }

        // playerPrefs에 저장
        PlayerPrefs.SetString(id, Encryption(password+SALT));
        PlayerPrefs.Save();

        GotoLogin();
        LoginInputField.IDInputField.text = id;
    }

    private string Encryption(string text)
    {
        SHA256 sha256 = SHA256.Create();

        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(text);
        byte[] hash = sha256.ComputeHash(bytes);
        string hashString = BitConverter.ToString(hash).Replace("-", "").ToLower();
        return hashString;
    }

    public void Login()
    {
        string id = LoginInputField.IDInputField.text;
        if (string.IsNullOrEmpty(id))
        {
            LoginInputField.OutputText.text = "아이디를 입력하세요.";
            LoginInputField.OutputText.transform.DOShakeScale(0.5f, 0.1f, 10, 90, true);
            return;
        }
        string password = LoginInputField.PasswordInputField.text;
        if (string.IsNullOrEmpty(password))
        {
            LoginInputField.OutputText.text = "비밀번호를 입력하세요.";
            LoginInputField.OutputText.transform.DOShakeScale(0.5f, 0.1f, 10, 90, true);
            return;
        }
        // playerPrefs에서 가져오기
        string savedPassword = PlayerPrefs.GetString(id);
        if (string.IsNullOrEmpty(savedPassword))
        {
            LoginInputField.OutputText.text = "아이디와 비밀번호 정보가 일치하지 않습니다.";
            LoginInputField.OutputText.transform.DOShakeScale(0.5f, 0.1f, 10, 90, true);
            return;
        }
        if (string.Compare(savedPassword, Encryption(password + SALT)) != 0)
        {
            LoginInputField.OutputText.text = "아이디와 비밀번호 정보가 일치하지 않습니다.";
            LoginInputField.OutputText.transform.DOShakeScale(0.5f, 0.1f, 10, 90, true);
            return;
        }
        // 로그인 성공
        Debug.Log("로그인 성공");
        SceneManager.LoadScene(1);
    }

    public void LoginCheck()
    {
        string id = LoginInputField.IDInputField.text;
        string password = LoginInputField.PasswordInputField.text; 

        LoginInputField.ConfirmButton.enabled = ! string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(password);
    }
}
