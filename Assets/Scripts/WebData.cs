using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebData
{

}

public enum WindowsType { login, registration, qr_read, prize_demo };

public interface IWindows {
    [SerializeField] static WindowsType w_type;

    public WindowsType GetWindowType();

    public void Init();

    public void CloseWindows();
}