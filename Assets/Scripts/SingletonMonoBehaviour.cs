//
// http://waken.hatenablog.com/?page=1471566151
//
using UnityEngine;

public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : SingletonMonoBehaviour<T>
{
	private static T instance;
	public static T Instance {
		get {
            if ( instance == null ) {
                instance = ( T )FindObjectOfType( typeof( T ) );
                if ( instance == null ) {
                    Debug.LogError ( typeof( T ) + "is nothing" );
                }
            }
			return instance;
		}
	}

	protected virtual void doAwake()
	{
		CheckInstance();
	}
	
	protected bool CheckInstance()
	{
		if( instance == null)
		{
			instance = (T)this;
			return true;
		}else if( Instance == this )
		{
			return true;
		}
		
		Destroy(this);
		return false;
	}

	void OnDestroy () {
		instance = null;
	}
}