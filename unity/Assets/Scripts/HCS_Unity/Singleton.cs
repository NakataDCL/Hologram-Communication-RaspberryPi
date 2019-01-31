using System;
using System.Collections;
using UnityEngine;

public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour {
	protected static T instance;
	public static T Instance {
		get {
			if (instance == null) {
				instance = (T) FindObjectOfType (typeof (T));

				if (instance == null) {
					instance = (new GameObject ()).AddComponent<T> ();
					instance.name = "(singleton)" + typeof (T);
				}
			}
			return instance;
		}
	}

	protected virtual void Awake () {
		if (instance != null) {
			Destroy (gameObject);
			return;
		}
		instance = this as T;
		DontDestroyOnLoad (gameObject);

		Initialize ();
	}

	protected virtual void Initialize () { }
}