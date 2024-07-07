using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts
{
	public class ZoomHandler : MonoBehaviour
	{
		[SerializeField] private float MIN_FOV;
		[SerializeField] private float MAX_FOV;
		[SerializeField] private float MIN_SIZE;
		[SerializeField] private float MAX_SIZE;
		[SerializeField] private float SENSITIVITY;
		[SerializeField] private Camera orthographicCamera;
		private Camera perspectiveCamera;
		private InputAction zoomAction;
		private float fov;
		private float size;

		private void OnEnable()
		{
			perspectiveCamera = Camera.main;
			perspectiveCamera.enabled = true;
			orthographicCamera.enabled = false;
			fov = perspectiveCamera.fieldOfView;
			size = orthographicCamera.orthographicSize;
		}

		public void Initialize(InputAction zoomAction)
		{
			this.zoomAction = zoomAction;
			this.zoomAction.Enable();
		}

		/// <summary>
		/// Zoom in and out with the mouse scroll wheel.  Start with the prespective camera.  Once the FOV value is greater
		/// than the maximum allowed FOV, switch to the orthographic camera, and allow its size to be controlled between its 
		/// minimum and maximum allowed values.
		/// </summary>
		private void Update()
		{
			var scroll = zoomAction.ReadValue<float>();

			// Scroll the perspective camera between the allowed fov values
			if (perspectiveCamera.enabled)
			{
				fov -= scroll * SENSITIVITY;
				fov = Mathf.Clamp(fov, MIN_FOV, MAX_FOV);
				if (fov == MAX_FOV)
				{
					perspectiveCamera.enabled = false;
					orthographicCamera.enabled = true;
					SwapTags();
				}
				else
				{
					perspectiveCamera.fieldOfView = fov;
				}
			}

			// Scroll the orthographic camera between the allowed size values
			if (orthographicCamera.enabled)
			{
				size -= scroll * SENSITIVITY;
				size = Mathf.Clamp(size, MIN_SIZE, MAX_SIZE);
				if (size == MIN_SIZE)
				{
					perspectiveCamera.enabled = true;
					orthographicCamera.enabled = false;
					SwapTags();
				} 
				else
				{
					orthographicCamera.orthographicSize = size;
				}
			}
		}

		private void SwapTags()
		{
			string temp = perspectiveCamera.tag;
			perspectiveCamera.tag = orthographicCamera.tag;
			orthographicCamera.tag = temp;
		}
	}
}
