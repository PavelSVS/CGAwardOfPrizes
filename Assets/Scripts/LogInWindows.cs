using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LogInWindows : MonoBehaviour, IWindows {
    [SerializeField] private WindowsType _type = WindowsType.login;

    [Header("UI elements")]
    [SerializeField] private Button[] b_toggleButtons = new Button[2];

    [SerializeField] private GameObject mobileBlock;
    [SerializeField] private GameObject emailBlock;

    [SerializeField] private TMP_InputField mobileNumber_inputField;
    [SerializeField] private TMP_Text mobileVisualNumber;
    private string mobileTextFormat = "{0, -1} ({1,-3})-{2,-3}-{3,-2}-{4,-2}";

    [SerializeField] private TMP_InputField email_inputField;
    [SerializeField] private TMP_InputField password_inputField;

    [SerializeField] private Button b_Continue;

    [SerializeField] private TMP_Text t_error;


    private IEnumerator Start() {
        while (!AuthController.Instance)
            yield return new WaitForFixedUpdate();

        AuthController.Instance.InitWindows(this);

        for (int i = 0; i < b_toggleButtons.Length; i++)
            b_toggleButtons[i].onClick.AddListener(() => {
                mobileNumber_inputField.text = "";
                email_inputField.text = "";
            });

        b_Continue.onClick.AddListener(() => {

        });


        mobileNumber_inputField.onValueChanged.AddListener(value => {
            if (mobileNumber_inputField.text.Length <= 0) {
                mobileVisualNumber.text = "";
                return;
            }

            WriteRUNumber();

            if (mobileNumber_inputField.text.Length > 11) // проверка на длину номера, по международному стандарту. максимальное количество цифр в номере телефона не может привышать 18
                mobileNumber_inputField.text = mobileNumber_inputField.text.Substring(0, 11);
        });

        gameObject.SetActive(false);
    }

    private void WriteRUNumber() {
        string text = (mobileNumber_inputField.text.Length >= 1) ? mobileNumber_inputField.text.Substring(1, mobileNumber_inputField.text.Length - 1) : "";
        string[] number = new string[4];

        for (int i = 0; i < 10; i++) {
            if (i < 3)
                number[0] = number[0] + ((i < text.Length) ? text[i] : '_');
            else
                if (i < 6)
                number[1] = number[1] + ((i < text.Length) ? text[i] : '_');
            else
                if (i < 8)
                number[2] = number[2] + ((i < text.Length) ? text[i] : '_');
            else
                if (i < 10)
                number[3] = number[3] + ((i < text.Length) ? text[i] : '_');
        }

        mobileVisualNumber.text = "+" + System.String.Format(mobileTextFormat, 7, number[0], number[1], number[2], number[3]);
    }

    public void CloseWindows() {
        gameObject.SetActive(false);
    }

    public WindowsType GetWindowType() {
        return _type;
    }

    public void Init() {
        gameObject.SetActive(true);

        mobileNumber_inputField.text = "";
        email_inputField.text = "";
        t_error.text = "";

        if (PlayerPrefs.HasKey("MobileLogin"))
            mobileNumber_inputField.text = PlayerPrefs.GetString("MobileLogin");

        mobileBlock.SetActive(true);
        emailBlock.SetActive(false);
    }
}
