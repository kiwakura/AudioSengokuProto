using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TurnBase01_Iwakura {

public class PanelCtrl : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Image>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void TogglePanel()
    {
        GetComponent<Image>().enabled = !GetComponent<Image>().enabled;
    }
    public void On()
    {
        GetComponent<Image>().enabled = true;
    }
    public void Off()
    {
        GetComponent<Image>().enabled = false;
    }

}

}
