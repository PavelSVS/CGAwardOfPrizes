using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;

public class PrizeCuponInfoWindows : MonoBehaviour, IWindows
{
    [SerializeField] private WindowsType _type = WindowsType.qr_read;

    [Header("UI elements")]
    [SerializeField] private Button b_GivePrizeToClient;
    [SerializeField] private Button b_BackToScanWindow;

    [SerializeField] private GameObject prize_out_away;

    [SerializeField] private Image prize_image;

    [SerializeField] private TMP_Text prize_id;
    [SerializeField] private TMP_Text user_place;
    [SerializeField] private TMP_Text winner_account;

    [SerializeField] private TMP_Text t_error;

    [Header("Continue window")]
    [SerializeField] private GameObject continueWindow;
    [SerializeField] private Button b_Continue;
    [SerializeField] private Button b_Close;

    private PrizeInfo prizeInfo;

    private IEnumerator Start() {
        while (!AuthController.Instance)
            yield return new WaitForFixedUpdate();

        AuthController.Instance.InitWindows(this);

        prize_id.text = "";
        user_place.text = "not found";
        winner_account.text = "not found";

        t_error.text = "";

        b_GivePrizeToClient.onClick.AddListener(() => {
            continueWindow.SetActive(true);
        });

        b_BackToScanWindow.onClick.AddListener(() => {
            if (AuthController.Instance) AuthController.Instance.OpenWindows(WindowsType.qr_read);
        });

        b_Continue.onClick.AddListener(() => {
            b_Continue.interactable = false;
            StartCoroutine(GivePrizeToClient());
        });

        b_Close.onClick.AddListener(() => {
            continueWindow.SetActive(false);
        });

        continueWindow.SetActive(false);
    }

    public void CloseWindows() {
        gameObject.SetActive(false);
    }

    public WindowsType GetWindowType() {
        return _type;
    }

    public void OpenWindow() {
        prize_id.text = "";
        user_place.text = "not found";
        winner_account.text = "not found";

        t_error.text = "";

        if (AuthController.Instance) prizeInfo = AuthController.Instance.GetPrizeInfo();

        prize_id.text = prizeInfo.id.ToString();
        user_place.text = "1";
        winner_account.text = prizeInfo.win_user.user.phone.Length > 0 ? prizeInfo.win_user.user.phone : prizeInfo.win_user.user.email;

        if (prizeInfo.win_user.received == 0) {
            b_GivePrizeToClient.gameObject.SetActive(true);
            prize_out_away.SetActive(false);
        }
        else {
            b_GivePrizeToClient.gameObject.SetActive(false);
            prize_out_away.SetActive(true);
        }
    }

    public IEnumerator GivePrizeToClient() {
        yield return new WaitForFixedUpdate();
    }
}
