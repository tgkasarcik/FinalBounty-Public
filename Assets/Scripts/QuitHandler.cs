using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class QuitHandler
{
	public QuitHandler(InputAction quitAction)
	{
		quitAction.performed += QuitAction_performed;
		quitAction.Enable();
	}

	private void QuitAction_performed(InputAction.CallbackContext obj)
	{
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#endif
		Application.Quit();
	}
}
