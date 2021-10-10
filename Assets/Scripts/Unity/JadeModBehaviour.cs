using System;
using System.Collections.Generic;
using R1Engine;
using UnityEngine;

public class JadeModBehaviour : MonoBehaviour {
    public ObjectType CurrentType;


	private void Start() {
        if(!Prefabs.ContainsKey(CurrentType)) ChangeObjectType(true);
        gameObject.name = $"[{WorldPrefab.Key:X8}] {gameObject.name}";
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
            gao.transform.localRotation = Quaternion.LookRotation(gao.transform.localPosition - cam.transform.position, Vector3.up);
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
        [ObjectType.Enemy_Rabbid] = new Prefab(0x11000000),
        [ObjectType.Mount_Warthog] = new Prefab(0x11000003),
        [ObjectType.Mount_Bat] = new Prefab(0x11000006),
        [ObjectType.Mount_SpiderLarge] = new Prefab(0x11000009),
        [ObjectType.Mount_SpiderSmall] = new Prefab(0x1100000C),
        [ObjectType.Mount_SaucerBlack] = new Prefab(0x1100000F),
        [ObjectType.Mount_SaucerYellow] = new Prefab(0x11000012),

        [ObjectType.Entry] = new Prefab(0x11000015),
        [ObjectType.Enemy_RabbidGun] = new Prefab(0x11000018),
        [ObjectType.Enemy_RabbidGrey] = new Prefab(0x1100001B),
        [ObjectType.Enemy_KongBunny] = new Prefab(0x1100001E),
        [ObjectType.Enemy_NurgleDemon] = new Prefab(0x11000021),
        [ObjectType.Enemy_RabbidDancer] = new Prefab(0x11000024),

        [ObjectType.Mount_TRex] = new Prefab(0x11000030),
        [ObjectType.Mount_KingKong] = new Prefab(0x11000033),
        [ObjectType.Mount_Eagle] = new Prefab(0x11000036),
        [ObjectType.Mount_BatKingKong] = new Prefab(0x11000039),
        [ObjectType.Mount_Pig] = new Prefab(0x1100003C), //new Prefab(0x3A003214),

        [ObjectType.Mount_SheepWhite] = new Prefab(0x1100003F), //new Prefab(0x3A003214),
        [ObjectType.Mount_SheepBrown] = new Prefab(0x11000042), //new Prefab(0x3A003214),
        [ObjectType.Mount_Cow] = new Prefab(0x11000045), //new Prefab(0x3A003214),
        [ObjectType.Mount_Plum] = new Prefab(0x11000048), //new Prefab(0x3A003214),

        [ObjectType.Mount_SpiderMid] = new Prefab(0x1100004B),
        [ObjectType.Mount_SpiderHuge] = new Prefab(0x1100004E),
        [ObjectType.Object_Cage] = new Prefab(0x11000051), //new Prefab(0x4D008B6C),

        // Include waypoints
        // [ObjectType.NPC_Globox] = new Prefab(0x1100004E),//new Prefab(0x0B012E14),

        // Include Col objects
        //[ObjectType.Enemy_Bipod] = new Prefab(0x0B01426E),
        //[ObjectType.Enemy_BipodBeach] = new Prefab(0x0D008E20),
        //[ObjectType.Enemy_BipodBig] = new Prefab(0x0B012880),

        // Include mutiple objects
        //[ObjectType.Mount_SaucerBig] = new Prefab(0x1100000F),

    };
    public Prefab WorldPrefab => new Prefab(0x2C001DCC);

    public enum ObjectType {
        Entry,

        Enemy_Rabbid,
        Enemy_RabbidGun,
        Enemy_RabbidGrey,
        Enemy_KongBunny,
        Enemy_NurgleDemon, // Use RRR2 Rayman
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
        Mount_SpiderLarge,
        Mount_SpiderMid,
        Mount_SpiderSmall,
        Mount_SpiderHuge,
        Mount_SaucerBlack,
        Mount_SaucerYellow,
        //Mount_SaucerBig,

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