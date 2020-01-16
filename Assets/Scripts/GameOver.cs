using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TurnBase01_Iwakura {

public class GameOver : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // 終了チェック
        if(Input.GetKey(KeyCode.Escape)){
            UnityEngine.Application.Quit();
        }else 
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SoundManager.Instance.PlaySe("decision4");
            GameSettings.ResetLevel();
            SceneManager.LoadScene("Main");
        }
    }
}

}
