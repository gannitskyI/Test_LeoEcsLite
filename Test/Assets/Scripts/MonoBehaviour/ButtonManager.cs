using UnityEngine;
using UnityEngine.UI;
public class ButtonManager : MonoBehaviour, IButtonManager
{
    public Button TakeButton { get; private set; }

    void Awake()
    {
        TakeButton = GameObject.Find("TakeButton").GetComponent<Button>();
    }
}