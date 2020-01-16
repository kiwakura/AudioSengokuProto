using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurnBase01_Iwakura {

//public class Knight : MonoBehaviour
public class Knight : UnitBase
{
    // Start is called before the first frame update
    override protected void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    override protected void Update()
    {
        base.Update();
#if false        
        float move_speed = 0.2f;
        transform.Translate(move_speed, 0, 0);
#endif
    }

#if false
    private void OnTriggerEnter2D(Collider2D collision){
        if(collision.gameObject.name == "Wall2D"){
            Destroy(gameObject);
        }
    }
#endif
}

}
