using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogInWindows : MonoBehaviour, IWindows {
    [SerializeField] private WindowsType _type = WindowsType.login;

    private IEnumerator Start() {
        while (!AuthController.Instance)
            yield return new WaitForFixedUpdate();

        AuthController.Instance.InitWindows(this);

        gameObject.SetActive(false);
    }

    public void CloseWindows() {
        gameObject.SetActive(false);
    }

    public WindowsType GetWindowType() {
        return _type;
    }

    public void Init() {
        gameObject.SetActive(true);
    }
}
