using System;
using System.Collections.Generic;
using R1Engine;
using UnityEngine;

public class JadeModBehaviour : MonoBehaviour {
    public ObjectType CurrentType;


	private void Start() {
        if(!Prefabs.ContainsKey(CurrentType)) ChangeObjectType(true);
		print($"Current object type: {CurrentType}");
	}

	// Update is called once per frame
	void Update()
    {
        if (Controller.LoadState != Controller.State.Finished) 
            return;

        if (Input.GetKeyDown(KeyCode.Keypad4)) ChangeObjectType(false, print: true);
        if (Input.GetKeyDown(KeyCode.Keypad6)) ChangeObjectType(true, print: true);
        if (Input.GetKeyDown(KeyCode.Keypad5)) {
            GameObject gao = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            gao.name = $"[{Prefabs[CurrentType].Key:X8}] {CurrentType}";
            UnityEngine.Random.InitState((int)CurrentType);
            gao.GetComponent<MeshRenderer>().material.color = UnityEngine.Random.ColorHSV(0, 1, 0.2f, 1f, 0.8f, 1.0f);
            gao.layer = LayerMask.NameToLayer("3D Collision");
            gao.transform.SetParent(transform, false);
            var cam = Controller.obj.levelEventController.editor.cam.camera3D;
            gao.transform.localPosition = cam.transform.TransformPoint(Vector3.forward * 5f);
            gao.transform.localRotation = Quaternion.LookRotation(cam.transform.position - gao.transform.localPosition, Vector3.up);
        }
    }

    public void ChangeObjectType(bool increase, bool print = false) {
        Array a = Enum.GetValues(typeof(ObjectType));
        int i = 0;
        for (i = 0; i < a.Length; i++) {
            if ((ObjectType)a.GetValue(i) == CurrentType)
                break;
        }
        bool change = true;
        while (change) {
            if (increase) {
                i = (i + 1) % a.Length;
            } else {
                i = (i + a.Length - 1) % a.Length;
            }
            var newType = (ObjectType)i;
            if (Prefabs.ContainsKey(newType)) {
                change = false;
                CurrentType = newType;
            }
        }
        if(print)
            Debug.Log($"Current object type: {CurrentType}");
    }

    public class Prefab {
        public uint Key { get; set; }
        public Prefab(uint key) {
            Key = key;
        }
    }

    public Dictionary<ObjectType, Prefab> Prefabs = new Dictionary<ObjectType, Prefab>() {
        [ObjectType.Enemy_Rabbid] = new Prefab(0x77000000),
        [ObjectType.Mount_Warthog] = new Prefab(0x77000001),
        [ObjectType.Mount_Bat] = new Prefab(0x77000002),
        [ObjectType.Mount_SaucerBlack] = new Prefab(0x77000003),
        [ObjectType.Mount_SaucerYellow] = new Prefab(0x77000004),
        [ObjectType.Mount_Spider] = new Prefab(0x77000005),
        [ObjectType.Mount_SpiderSmall] = new Prefab(0x77000006),
    };

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
        Mount_Cow,
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