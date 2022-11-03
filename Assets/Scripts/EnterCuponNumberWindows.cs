using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;

public class EnterCuponNumberWindows : MonoBehaviour, IWindows {
    [SerializeField] private WindowsType _type = WindowsType.qr_read;

    [Header("UI elements")]
    [SerializeField] private Button b_Continue;
    [SerializeField] private Button b_StartScanningQR;

    [SerializeField] private TMP_InputField cupon_number;

    [SerializeField] private TMP_Text user_name;

    [SerializeField] private QRReader qr_reader;

    [SerializeField] private TMP_Text t_error;

    [Header("Description")]
    [SerializeField] private Button b_OpenDiscription;
    [SerializeField] private GameObject discriptionWindow;
    [SerializeField] private Button b_CloseDiscription;

    private IEnumerator Start() {
        while (!AuthController.Instance)
            yield return new WaitForFixedUpdate();

        AuthController.Instance.InitWindows(this);

        user_name.text = "";

        qr_reader.gameObject.SetActive(false);

        b_Continue.onClick.AddListener(() => {
            b_Continue.interactable = false;
            qr_reader.StopRead();
            StartCoroutine(RequestCuponInfo());
        });

        b_StartScanningQR.onClick.AddListener(() => {
            qr_reader.Init(cupon_number);
        });

        b_OpenDiscription.onClick.AddListener(() => discriptionWindow.SetActive(true));

        b_CloseDiscription.onClick.AddListener(() => discriptionWindow.SetActive(false));

        gameObject.SetActive(false);
    }

    public void CloseWindows() {
        gameObject.SetActive(false);
        cupon_number.caretColor = Color.white;
    }

    public WindowsType GetWindowType() {
        return _type;
    }

    public void OpenWindow() {
        gameObject.SetActive(true); 
        qr_reader.gameObject.SetActive(false);
        b_Continue.interactable = true;
        cupon_number.text = "";
        t_error.text = "";

        if (AuthController.Instance) user_name.text = "Аккаунт: " + AuthController.Instance.GetUserInfo().phone;
    }

    //https://cloudsgoods.com/api/games/stocks?mode=prizes_on_give_out&coupon_number=1028689483020
    private IEnumerator RequestCuponInfo() {

        if (cupon_number.text.Length < 1) {
            t_error.text = "Поле с номером купона не может быть пустым";
            yield return new WaitForFixedUpdate();
            b_Continue.interactable = true;
            yield break;
        }

        WWWForm form = new WWWForm();
        form.AddField("mode", "prizes_on_give_out");

        form.AddField("coupon_number", cupon_number.text);

        using (UnityWebRequest www = UnityWebRequest.Post(WebData.RequestPrizeInfo, form))
        {
            yield return www.SendWebRequest();
            if (www.isNetworkError || www.isHttpError) {
                Debug.LogError(www.error);
                www.Dispose();
                yield break;
            }

            PrizeCalResult result = JsonUtility.FromJson<PrizeCalResult>(www.downloadHandler.text);
            www.Dispose();

            if (result.error.Length < 1 && result.data.Length >= 1) {

                if (AuthController.Instance) {
                    AuthController.Instance.SetPrizeData(result.data[0]);
                    AuthController.Instance.OpenWindows(WindowsType.prize_demo);
                }
                yield break;
            }

            yield return new WaitForFixedUpdate();
            if (result.error.Length > 0) t_error.text = result.error[0];
            else
                t_error.text = "Купона с таким номером не существует";
            b_Continue.interactable = true;
        }

    }
}
