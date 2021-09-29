using System;
using R1Engine;
using UnityEngine;

public class JadeModBehaviour : MonoBehaviour {

	private void Start() {
		
	}

	// Update is called once per frame
	void Update()
    {
        if (Controller.LoadState != Controller.State.Finished) 
            return;

        if (Input.GetKeyDown(KeyCode.KeypadMultiply)) {
            GameObject gao = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            gao.layer = LayerMask.NameToLayer("3D Collision");
            gao.transform.SetParent(transform, false);
            gao.transform.localPosition = Controller.obj.levelEventController.editor.cam.camera3D.transform.TransformPoint(Vector3.forward * 5f);
            //gao.transform.localPosition = Controller.obj.levelEventController.editor.cam.camera3D.transform.localPosition;
        }
    }

    public enum ObjectType {
        Entry,

        Enemy_Rabbid,
        Enemy_RabbidGun,
        Enemy_RabbidGrey,
        Enemy_KongBunny,
        Enemy_Doppelganger, // Use RRR2 Rayman
        Enemy_RabbidDancer,
        Enemy_Bipod,
        Enemy_BipodBeach,
        Enemy_BipodBig,

        Mount_Warthog,
        Mount_TRex,
        Mount_KingKong,
        Mount_Bat,
        Mount_Eagle,
        Mount_BatKingKong,
        Mount_Pig,
        Mount_SheepWhite,
        Mount_SheepBrown,
        Mount_Plum,
        Mount_Spider,
        Mount_SpiderSmall,
        Mount_SaucerBlack,
        Mount_SaucerYellow,

        NPC_Globox,

        Object_Cage,
        Object_Box,

        Weapon_PlungerGun,
        Weapon_Uzi,
        Weapon_Grenade,
        Weapon_Barrel,
        Weapon_BeachBall,
        Weapon_WoodenClub,
    }
}