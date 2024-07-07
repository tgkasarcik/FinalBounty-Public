using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerObject;

public class ControledShot : MonoBehaviour
{
   public float projectileSpeed = 1f;
   private Rigidbody projectileRb;

   public IPlayer Player;

   //makes bullets fly towards the mouse cursor
   void Start()
   {
        projectileRb = this.gameObject.GetComponent<Rigidbody>();
   }
   void FixedUpdate()
   {    
        if(Player != null && Player.playerObj != null)
        {
          Vector3 MousePos;
          if(Player.bossBattle)
          {
               MousePos = GetMousePosBossBattle();
          }
          else
          {
               MousePos = GetMousePos();
          }

          projectileRb.AddForce(MousePos * projectileSpeed * 1.5f, ForceMode.Acceleration);
          this.transform.rotation = Quaternion.LookRotation(projectileRb.velocity, this.transform.up);
        }
   }

   private Vector3 GetMousePos()
     {
          //gets direction from 
          Vector3 mousePos = Input.mousePosition;
          Vector3 cameraToPlayerObj = Camera.main.transform.position - Player.playerObj.transform.position;

          Vector3 MousePosition = Camera.main.ScreenToWorldPoint(
               new Vector3(mousePos.x, mousePos.y, Camera.main.nearClipPlane + cameraToPlayerObj.magnitude));
               MousePosition = (MousePosition - this.transform.position).normalized;
          return MousePosition;
     }

   private Vector3 GetMousePosBossBattle()
     {
          //gets direction from 
          Vector3 mousePos = Input.mousePosition;
          // Vector3 cameraToPlayerObj = Camera.main.transform.position - PlayerObj.transform.position;
          // Vector3 point = new Vector3(mousePos.x, mousePos.y, Camera.main.nearClipPlane + cameraToPlayerObj.magnitude);
          // Debug.Log(point.x + ", " + point.y + ", " + point.z);
          RaycastHit hit;
          Vector3 shootDir = Vector3.zero;
          Ray MousePosition = Camera.main.ScreenPointToRay(mousePos);
          if(Physics.Raycast(MousePosition, out hit))
          {
               shootDir = (hit.point - this.transform.position).normalized;
          }

          return shootDir;
     }
}
