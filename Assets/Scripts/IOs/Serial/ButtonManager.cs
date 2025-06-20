using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class ButtonManager : MonoBehaviour
{
    [DllImport("mai2io.dll")]

    private int button;

    private static extern void io_send_button(uint index, bool value);
    void OnCollisionEnter(Collision collision)
    {
        io_send_button((uint)button, true);
    }

    void OnCollisionExit(Collision collision)
    {
        io_send_button((uint)button, false);
    }
}
