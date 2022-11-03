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
    [SerializeField] private TMP_Text prize_name;
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
        gameObject.SetActive(false);
    }

    public void CloseWindows() {
        prize_image.gameObject.SetActive(false);
        continueWindow.SetActive(false);
        prize_image.sprite = null;
        prize_image.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    public WindowsType GetWindowType() {
        return _type;
    }

    public void OpenWindow() {
        gameObject.SetActive(true);
        prize_id.text = "";
        user_place.text = "not found";
        winner_account.text = "not found";

        t_error.text = "";

        b_Continue.interactable = true;

        if (AuthController.Instance) prizeInfo = AuthController.Instance.GetPrizeInfo();

        prize_id.text = prizeInfo.id.ToString();
        user_place.text = prizeInfo.place.ToString();
        prize_name.text = prizeInfo.prize.games_prizes_data.object_data.title;

        if (prizeInfo.win_user.user.phone != null)
            winner_account.text = prizeInfo.win_user.user.phone.Length > 0 ? prizeInfo.win_user.user.phone : "user" + prizeInfo.win_user.user_id;
        else {
            if (prizeInfo.win_user.user.email != null)
                winner_account.text = prizeInfo.win_user.user.email.Length > 0 ? prizeInfo.win_user.user.email : "user" + prizeInfo.win_user.user_id;
            else
                winner_account.text = "user" + prizeInfo.win_user.user_id;
        }


        if (prizeInfo.win_user.received == 0) {
            b_GivePrizeToClient.gameObject.SetActive(true);
            prize_out_away.SetActive(false);
        }
        else {
            b_GivePrizeToClient.gameObject.SetActive(false);
            prize_out_away.SetActive(true);
        }

        if (prizeInfo.prize.games_prizes_data.object_data.default_look_preview != null && prizeInfo.prize.games_prizes_data.object_data.default_look_preview.Length > 0)
                StartCoroutine(LoadTexture(prizeInfo.prize.games_prizes_data.object_data.default_look_preview));
        else
            if (prizeInfo.win_user.qr_code != null && prizeInfo.win_user.qr_code.Length > 0)
            StartCoroutine(LoadTexture(prizeInfo.win_user.qr_code));

        continueWindow.SetActive(false);
    }

    private IEnumerator LoadTexture(string url) {
        prize_image.gameObject.SetActive(true);

        Texture2D good_image;

        using (UnityWebRequest req = UnityWebRequestTexture.GetTexture(url)) {

            yield return req.SendWebRequest();
            if (req.isNetworkError || req.isHttpError) {
                req.Dispose();
                yield break;
            }

            good_image = DownloadHandlerTexture.GetContent(req);
            req.Dispose();
        }

        prize_image.sprite = Sprite.Create(good_image, new Rect(0, 0, good_image.width, good_image.height), Vector2.zero);
    }

    //https://cloudsgoods.com/api/games/stocks?mode=give_out_prize&coupon_number=1028689121635&user_id=7
    public IEnumerator GivePrizeToClient() {
        WWWForm form = new WWWForm();
        form.AddField("mode", "give_out_prize");
        form.AddField("coupon_number", prizeInfo.coupon_number);
        form.AddField("user_id", prizeInfo.win_user.user_id);

        using (UnityWebRequest www = UnityWebRequest.Post(WebData.GivePrizeOut, form)) {
            yield return www.SendWebRequest();
            if (www.isNetworkError || www.isHttpError) {
                Debug.LogError(www.error);
                www.Dispose();
                yield break;
            }

            t_error.text = www.downloadHandler.text.Length > 10 ? www.downloadHandler.text.Substring(0, 10) : www.downloadHandler.text;
           // PrizeCalResult result = JsonUtility.FromJson<PrizeCalResult>(www.downloadHandler.text);
            www.Dispose();

            //if (result.error.Length < 1) {
                b_GivePrizeToClient.gameObject.SetActive(false);
                prize_out_away.SetActive(true);
                continueWindow.SetActive(false);
                yield break;
            //}

            //yield return new WaitForFixedUpdate();
            //if (result.error.Length > 0) t_error.text = result.error[0];
            //else
            //    t_error.text = "Не удалось выдать купон";

            continueWindow.SetActive(false);
            b_Continue.interactable = true;
        }
    }
}
