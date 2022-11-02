using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZXing;
using ZXing.QrCode;
using UnityEngine.UI;

public class QRReader : MonoBehaviour
{
    private WebCamTexture camTexture;
    [SerializeField] private RawImage image;

    private TMPro.TMP_InputField qr_code_visual;

    Coroutine scanning;

    public void Init(TMPro.TMP_InputField text) {
        gameObject.SetActive(true);

        qr_code_visual = text;

        camTexture = new WebCamTexture();
        camTexture.requestedHeight = (int)image.rectTransform.sizeDelta.y;
        camTexture.requestedWidth = (int)image.rectTransform.sizeDelta.x;

        image.texture = camTexture;
        image.material.mainTexture = camTexture;

        if (camTexture != null) {
            camTexture.Play();
            if (scanning != null)
                StopCoroutine(scanning);

            scanning = StartCoroutine(Scanning_QR());
        }
    }

    private void StopRead() {
        camTexture.Stop();
        if (scanning != null)
            StopCoroutine(scanning);

        scanning = null;

        gameObject.SetActive(false);
    }

    public IEnumerator Scanning_QR() {

        if (!camTexture.isPlaying) {
            StopRead();
            yield break;
        }

        while (qr_code_visual.text == "") {
            yield return new WaitForFixedUpdate();
            IBarcodeReader barcodeReader = new BarcodeReader();

            var result = barcodeReader.Decode(camTexture.GetPixels32(), camTexture.width, camTexture.height);

            if (result != null) {
                qr_code_visual.text = result.Text;
                qr_code_visual.caretColor = Color.green;
                StopRead();
            }
        }

        scanning = null;
    }
}
