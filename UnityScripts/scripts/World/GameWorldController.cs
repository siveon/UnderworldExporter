﻿using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine.AI;
using UnityEngine.UI;

/// <summary>
/// Game world controller for controlling references and various global activities
/// </summary>

public class GameWorldController : UWEBase
{
    public bool EnableUnderworldGenerator = false;
    public GameObject ceiling;

    public WhatTheHellIsSCD_ARK whatTheHellIsThatFileFor;

    public enum UW1_LevelNos
    {
        EntranceLevel = 0,
        MountainMen = 1,
        Swamp = 2,
        Knights = 3,
        Catacombs = 4,
        Seers = 5,
        Tybal = 6,
        Volcano = 7,
        Ethereal = 8
    };

    public static string[] UW1_LevelNames = new string[]
            {
                "Outcast",
                "Dwarf",
                "Swamp",
                "Knight",
                "Tombs",
                "Seers",
                "Tybal",
                "Abyss",
                "Void"
            };

    public enum UW2_LevelNos
    {
        Britannia0 = 0,
        Britannia1 = 1,
        Britannia2 = 2,
        Britannia3 = 3,
        Britannia4 = 0,
        Prison0 = 8,
        Prison1 = 9,
        Prison2 = 10,
        Prison3 = 11,
        Prison4 = 12,
        Prison5 = 13,
        Prison6 = 14,
        Prison7 = 15,
        Killorn0 = 16,
        Killorn1 = 17,
        Ice0 = 24,
        Ice1 = 25,
        Talorus0 = 32,
        Talorus1 = 33,
        Academy0 = 40,
        Academy1 = 41,
        Academy2 = 42,
        Academy3 = 43,
        Academy4 = 44,
        Academy5 = 45,
        Academy6 = 46,
        Academy7 = 47,
        Tomb0 = 48,
        Tomb1 = 49,
        Tomb2 = 50,
        Tomb3 = 51,
        Pits0 = 56,
        Pits1 = 57,
        Pits2 = 58,
        Ethereal0 = 64,
        Ethereal1 = 65,
        Ethereal2 = 66,
        Ethereal3 = 67,
        Ethereal4 = 68,
        Ethereal5 = 69,
        Ethereal6 = 70,
        Ethereal7 = 71,
        Ethereal8 = 72
    };



    [Header("Controls")]
    public MouseLook MouseX;
    public MouseLook MouseY;


    [Header("World Options")]
    /// <summary>
    /// Enables texture animation effects
    /// </summary>
    public bool EnableTextureAnimation;

    /// <summary>
    /// The grey scale shader. Reference to allow loading of a hidden shader.
    /// </summary>
    public Shader greyScale;

    /// <summary>
    /// The vortex effect shader.  Reference to allow loading of a hidden shader.
    /// </summary>
    public Shader vortex;

    /// <summary>
    /// Is the game at the main menu or should it start at the mainmenu.
    /// </summary>
    public bool AtMainMenu;


    /// <summary>
    /// Enable timer triggers
    /// </summary>
    public bool EnableTimerTriggers = true;

    /// <summary>
    /// The timer execution rate.
    /// </summary>
    public float TimerRate = 1f;


    [Header("Parent Objects")]
    /// <summary>
    /// The level model parent object
    /// </summary>
    public GameObject LevelModel;

    public GameObject TNovaLevelModel;

    /// <summary>
    /// The level model parent object
    /// </summary>
    public GameObject SceneryModel;


    /// <summary>
    /// Gameobject to load the objects at
    /// </summary>
    public GameObject _ObjectMarker;

    /// <summary>
    /// The instance of this class
    /// </summary>
    public static GameWorldController instance;

    /// <summary>
    /// The game object that picked up items are parented to.
    /// </summary>
    public GameObject InventoryMarker;

    [Header("Level")]
    /// <summary>
    /// What level number we are currently on.
    /// </summary>	
    public short LevelNo;

    public static bool LoadingGame = false;
    public static bool NavMeshReady = false;
    public bool[] NavMeshesReady = new bool[4];
    private static string LevelSignature;

    /// <summary>
    /// What level the player starts on in a quick start
    /// </summary>
    public short startLevel = 0;
    /// <summary>
    /// What start position for the player.
    /// </summary>
    public Vector3 StartPos = new Vector3(38f, 4f, 2.7f);

    /// <summary>
    /// Create object reports
    /// </summary>
    public bool CreateReports;
    public bool ShowOnlyInUse;

    [Header("Palettes")]
    /// <summary>
    /// Array of cycled game palettes for animation effects.
    /// </summary>
    public Texture2D[] paletteArray = new Texture2D[8];

    /// <summary>
    /// The index of the palette currently in use
    /// </summary>
    public int paletteIndex = 0;

    /// <summary>
    /// The palette index when going in reverse.
    /// </summary>
    public int paletteIndexReverse = 0;

    /// <summary>
    /// Shared palettes for artwork
    /// </summary>
    public PaletteLoader palLoader;


    [Header("LevelMaps")]
    /// <summary>
    /// The tilemap class for the game
    /// </summary>
    public TileMap[] Tilemaps = new TileMap[9];


    /// <summary>
    /// The auto maps.
    /// </summary>
    public AutoMap[] AutoMaps = new AutoMap[9];

    /// <summary>
    /// The object lists for each level.
    /// </summary>
    public ObjectLoader[] objectList = new ObjectLoader[9];


    /// <summary>
    /// The music controller for the game
    /// </summary>
    private MusicController mus;



    [Header("Property Lists")]
    /// <summary>
    /// The object master class for storing and reading object properties in an external file
    /// </summary>
    public ObjectMasters objectMaster;

    /// <summary>
    /// The critter properties from objects.dat
    /// </summary>
    public Critters critterData;


    /// <summary>
    /// The object dat file
    /// </summary>
    public ObjectDatLoader objDat;

    /// <summary>
    /// The common object properties for uw
    /// </summary>
    public CommonObjectDatLoader commonObject;

    public ObjectPropLoader ShockObjProp;

    /// <summary>
    /// The terrain data from terrain.dat
    /// </summary>
    public TerrainDatLoader terrainData;

    [Header("Paths")]
    public string Lev_Ark_File_Selected = "";//"DATA\\Lev.ark";
    public string SCD_Ark_File_Selected = "";//"DATA\\SCD.ark";
                                             //Game paths
    public string path_uw0;
    public string path_uw1;
    public string path_uw2;
    public string path_shock;
    public string path_tnova;

    [Header("Material Lists")]
    /// <summary>
    /// The material master list for matching the texture list to materials.
    /// </summary>
    public Material[] MaterialMasterList = new Material[260];

    public Material[] SpecialMaterials = new Material[1];

    /// <summary>
    /// Default material for the editor
    /// </summary>
    public Material Jorge;

    /// <summary>
    /// The materials for doors  (doors.gr)
    /// </summary>
    public Material[] MaterialDoors = new Material[13];

    /// <summary>
    /// The materials for tmobj + models (tmobj.gr)
    /// </summary>
    public Material[] MaterialObj = new Material[54];

    /// <summary>
    /// The default model material.
    /// </summary>
    public Material modelMaterial;


    [Header("Nav Meshes")]
    /// <summary>
    /// Generate Nav meshes or not
    /// </summary>
    public bool bGenNavMeshes = true;
    public int GenNavMeshNextFrame = -1;
    public NavMeshSurface NavMeshLand;
    public NavMeshSurface NavMeshWater;
    public NavMeshSurface NavMeshAir;
    public NavMeshSurface NavMeshLava;
    public int MapMeshLayerMask = 0;
    public int DoorLayerMask = 0;


    //public RAIN.Navigation.NavMesh.NavMeshRig NavRigLand;
    //public RAIN.Navigation.NavMesh.NavMeshRig NavRigWater;//To implement for create npc


    [Header("Art Loaders")]
    /// <summary>
    /// The bytloader for bty files
    /// </summary>
    public BytLoader bytloader;
    /// <summary>
    /// The tex loader for textures
    /// </summary>
    public TextureLoader texLoader;
    /// <summary>
    /// The spell icons gr loader
    /// </summary>
    public GRLoader SpellIcons;
    /// <summary>
    /// The object art gr loader
    /// </summary>
    public GRLoader ObjectArt;

    /// <summary>
    /// The door art.
    /// </summary>
    public GRLoader DoorArt;

    /// <summary>
    /// The tm object art.
    /// </summary>
    public GRLoader TmObjArt;

    /// <summary>
    /// The tm flat art.
    /// </summary>
    public GRLoader TmFlatArt;

    /// <summary>
    /// Small animations art.
    /// </summary>
    public GRLoader TmAnimo;

    /// <summary>
    /// The lev ark file data.
    /// </summary>
    private char[] lev_ark_file_data;

    /// <summary>
    /// The female armor
    /// </summary>
    public GRLoader armor_f;

    /// <summary>
    /// The male armor.
    /// </summary>
    public GRLoader armor_m;

    /// <summary>
    /// The cursors art
    /// </summary>
    public GRLoader grCursors;

    /// <summary>
    /// The health & mana flasks.
    /// </summary>
    public GRLoader grFlasks;

    /// <summary>
    /// The option menus
    /// </summary>
    public GRLoader grOptbtns;

    /// <summary>
    /// The Compass 
    /// </summary>
    public GRLoader grCompass;

    /// <summary>
    /// Cutscene data
    /// </summary>
    public CutsLoader cutsLoader;

    public CritLoader[] critsLoader = new CritLoader[64];

    /// <summary>
    /// The weapon animation frames.
    /// </summary>
    public WeaponAnimation weaps;
    //public WeaponAnimationPlayer WeaponAnim;
    public WeaponsLoader weapongr;

    public int difficulty = 1; //1=standard, 0=easy.

    public static bool LoadingObjects = false;

    public struct bablGlobal
    {
        public int ConversationNo;
        public int Size;
        public int[] Globals;
    };

    public bablGlobal[] bGlobals;
    public ConversationVM convVM;

    public static bool WorldReRenderPending = false;
    public static bool ObjectReRenderPending = false;
    public static bool FullReRender = false;



    public KeyBindings keybinds;

    public event_processor events;

    private int startX = -1; private int startY = -1;


    void LoadPath(string _RES)
    {
        string path = "";

        switch (_RES)
        {
            case GAME_UWDEMO: path = GameWorldController.instance.path_uw0; break;
            case GAME_UW1: path = GameWorldController.instance.path_uw1; break;
            case GAME_UW2: path = GameWorldController.instance.path_uw2; break;
            case GAME_SHOCK: path = GameWorldController.instance.path_shock; break;
            case GAME_TNOVA: path = GameWorldController.instance.path_tnova; break;
        }

        Loader.BasePath = path;
        Loader.sep = sep;
        //	if (Loader.BasePath.EndsWith(sep.ToString()) !=true)
        //	{
        //			Loader.BasePath = Loader.BasePath + sep;
        //	}
    }

    /// <summary>
    /// Awake this instance.
    /// </summary>
    /// Should be the very first script to run 
    void Awake()
    {
        instance = this;
        sep = Path.AltDirectorySeparatorChar;
        Lev_Ark_File_Selected = "DATA" + sep + "LEV.ARK";
        SCD_Ark_File_Selected = "DATA" + sep + "SCD.ARK";

        LoadConfigFile();
        //LoadPath();
        return;
    }


    void Start()
    {

        instance = this;
        AtMainMenu = true;
        MapMeshLayerMask = 1 << LevelModel.layer;
        DoorLayerMask = 1 << LayerMask.NameToLayer("Doors");
        //Debug.Log(navmeshsurface.GetComponent<NavMeshSurface>().layerMask.value);
        return;

    }

    void Update()
    {
        PositionDetect();
        /*	if (GenNavMeshNextFrame>1)
			{
				GenNavMeshNextFrame--;
				if (GenNavMeshNextFrame ==1)
				{
					GenNavMeshNextFrame=0;
					if ((bGenNavMeshes) && (!EditorMode))
					{
						GenerateNavMeshes ();
					}	
				}
			}*/
    }

    /*void GenerateNavMeshes ()
	{
		//GenNavMeshNextFrame = -1;
		//GenerateNavmesh(NavRigLand);
		//GenerateNavmesh(NavRigWater);
		GenerateNavmesh (NavMeshLand);//Update nav mesh for the land
		GenerateNavmesh (NavMeshWater);//For water
		GenerateNavmesh (NavMeshAir);//for air
		GenerateNavmesh (NavMeshLava);//for lava
	}*/

    IEnumerator UpdateNavMeshes()
    {
        NavMeshReady = false;
        NavMeshesReady[0] = false;
        NavMeshesReady[1] = false;
        NavMeshesReady[2] = false;
        //NavMeshesReady[3]=false;
        while (LoadingGame)
        {
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(GenerateNavmesh(NavMeshLand, 0));//Update nav mesh for the land
        StartCoroutine(GenerateNavmesh(NavMeshWater, 1));//For water
        StartCoroutine(GenerateNavmesh(NavMeshLava, 2));//for lava
        StartCoroutine(GenerateNavmesh(NavMeshAir, 3));//for air


        while (!(
                    (NavMeshesReady[0]) &&
                    (NavMeshesReady[1]) &&
                    (NavMeshesReady[2]) &&
                    (NavMeshesReady[3])
                )
            )
        {
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(1.5f);
        NavMeshReady = true;
        yield return 0;
    }

    IEnumerator GenerateNavmesh(NavMeshSurface navmeshobj, int index)
    {
        //	if (navmeshobj.GetComponent<NavMeshSurface>()!=null)
        //	{
        //		Destroy(navmeshobj.GetComponent<NavMeshSurface>());
        //}
        //NavMeshSurface navmesh = navmeshobj.AddComponent<NavMeshSurface>();
        //navmesh.layerMask = layer;
        //	navmeshobj.BuildNavMesh();
        //navmeshobj.

        if (navmeshobj.navMeshData == null)
        {
            navmeshobj.BuildNavMesh();
        }
        else
        {
            AsyncOperation task = navmeshobj.UpdateNavMesh(navmeshobj.navMeshData);
            while (!task.isDone)
            {
                yield return new WaitForSeconds(0.1f);
            }
        }
        NavMeshesReady[index] = true;
        yield return 0;
    }

    void LateUpdate()
    {
        if (WorldReRenderPending)
        {
            if ((FullReRender) && (!EditorMode))
            {
                //	currentTileMap().CleanUp(_RES);				
            }
            TileMapRenderer.GenerateLevelFromTileMap(GameWorldController.instance.LevelModel, GameWorldController.instance.SceneryModel, _RES, currentTileMap(), GameWorldController.instance.CurrentObjectList(), !FullReRender);
            if (ObjectReRenderPending)
            {
                ObjectReRenderPending = false;
                ObjectLoader.RenderObjectList(CurrentObjectList(), currentTileMap(), DynamicObjectMarker().gameObject);
            }
            WorldReRenderPending = false;
            FullReRender = false;
            if (!IngameEditor.EditorMode)
            {
                NavMeshLand.UpdateNavMesh(NavMeshLand.navMeshData);
                NavMeshWater.UpdateNavMesh(NavMeshWater.navMeshData);
                //NavMeshAir.UpdateNavMesh(NavMeshAir.navMeshData);
                NavMeshLava.UpdateNavMesh(NavMeshLava.navMeshData);
            }
        }
    }

    /// <summary>
    /// Begins the specified game.
    /// </summary>
    /// <param name="res">Res.</param>
    public void Begin(string res)
    {
        UWHUD.instance.gameSelectUi.SetActive(false);
        LoadPath(res);
        UWEBase._RES = res;//game;
        UWClass._RES = res;//game;
        keybinds.ApplyBindings();//Applies keybinds to certain controls

        switch (res)
        {
            case GAME_TNOVA:
                UWCharacter.Instance.XAxis.enabled = true;
                UWCharacter.Instance.YAxis.enabled = true;
                UWCharacter.Instance.MouseLookEnabled = true;
                UWCharacter.Instance.speedMultiplier = 20;
                break;
            case GAME_SHOCK:
                palLoader = new PaletteLoader("res" + sep + "DATA" + sep + "GAMEPAL.RES", 700);
                //palLoader.Path=Loader.BasePath + "res\\data\\gamepal.res";
                //palLoader.PaletteNo=700;
                //palLoader.LoadPalettes();
                texLoader = new TextureLoader();
                objectMaster = new ObjectMasters();
                ObjectArt = new GRLoader("res" + sep + "DATA" + sep + "OBJART.RES", 1350);
                ShockObjProp = new ObjectPropLoader();
                UWCharacter.Instance.XAxis.enabled = true;
                UWCharacter.Instance.YAxis.enabled = true;
                UWCharacter.Instance.MouseLookEnabled = true;
                UWCharacter.Instance.speedMultiplier = 20;
                break;
            default:
                StartCoroutine(MusicController.instance.Begin());
                objectMaster = new ObjectMasters();
                objDat = new ObjectDatLoader();
                commonObject = new CommonObjectDatLoader();


                palLoader = new PaletteLoader("DATA" + sep + "PALS.DAT", -1);

                //Create palette cycles and store them in the palette array
                PaletteLoader palCycler = new PaletteLoader("DATA" + sep + "PALS.DAT", -1);

                for (int c = 0; c <= 27; c++)
                {
                    switch (_RES)
                    {
                        case GAME_UW2:
                            Palette.cyclePalette(palCycler.Palettes[0], 224, 16);
                            Palette.cyclePalette(palCycler.Palettes[0], 3, 6);
                            break;
                        default:
                            Palette.cyclePalette(palCycler.Palettes[0], 48, 4);
                            Palette.cyclePalette(palCycler.Palettes[0], 16, 7);//Reverse direction.
                            break;
                    }
                    paletteArray[c] = Palette.toImage(palCycler.Palettes[0]);
                }


                bytloader = new BytLoader();

                texLoader = new TextureLoader();
                ObjectArt = new GRLoader(GRLoader.OBJECTS_GR);
                SpellIcons = new GRLoader(GRLoader.SPELLS_GR);
                DoorArt = new GRLoader(GRLoader.DOORS_GR);
                TmObjArt = new GRLoader(GRLoader.TMOBJ_GR);
                TmFlatArt = new GRLoader(GRLoader.TMFLAT_GR);
                TmAnimo = new GRLoader(GRLoader.ANIMO_GR);
                armor_f = new GRLoader(GRLoader.ARMOR_F_GR);
                armor_m = new GRLoader(GRLoader.ARMOR_M_GR);
                grCursors = new GRLoader(GRLoader.CURSORS_GR);
                grFlasks = new GRLoader(GRLoader.FLASKS_GR);
                grOptbtns = new GRLoader(GRLoader.OPTBTNS_GR);
                grCompass = new GRLoader(GRLoader.COMPASS_GR);
                terrainData = new TerrainDatLoader();
                weaps = new WeaponAnimation();
                break;
        }


        switch (_RES)
        {
            case GAME_SHOCK:
            case GAME_TNOVA:
                break;
            case GAME_UW2:
                {
                    if (GameWorldController.instance.startLevel == 0)
                    {//Avatar's bedroom
                        GameWorldController.instance.StartPos = new Vector3(23.43f, 3.95f, 58.29f);
                    }
                    break;
                }
            case GAME_UWDEMO:
                GameWorldController.instance.StartPos = new Vector3(39.06f, 3.96f, 3f); break;
            default:
                {
                    if (GameWorldController.instance.startLevel == 0)
                    {//entrance to the abyss
                        GameWorldController.instance.StartPos = new Vector3(39.06f, 3.96f, 3f);
                    }
                    break;
                }
        }


        switch (res)
        {
            case GAME_TNOVA:
                AtMainMenu = false;
                TileMapRenderer.EnableCollision = false;
                bGenNavMeshes = false;
                UWHUD.instance.gameObject.SetActive(false);
                UWHUD.instance.window.SetFullScreen();
                UWCharacter.Instance.isFlying = true;
                UWCharacter.Instance.playerMotor.enabled = true;
                UWCharacter.Instance.playerCam.backgroundColor = Color.white;
                SwitchTNovaMap("");
                return;
            case GAME_SHOCK:
                TileMapRenderer.EnableCollision = false;
                bGenNavMeshes = false;
                AtMainMenu = false;
                UWCharacter.Instance.isFlying = true;
                UWCharacter.Instance.playerMotor.enabled = true;
                UWHUD.instance.gameObject.SetActive(false);
                UWHUD.instance.window.SetFullScreen();
                SwitchLevel(startLevel);
                return;

            case GAME_UWDEMO:
                //case GAME_UW2:
                //UW Demo does not go to the menu. It will load automatically into the gameworld
                AtMainMenu = false;
                UWCharacter.Instance.transform.position = GameWorldController.instance.StartPos;
                UWHUD.instance.Begin();
                UWCharacter.Instance.Begin();
                UWCharacter.Instance.playerInventory.Begin();
                StringController.instance.LoadStringsPak(Loader.BasePath + "DATA" + sep + "STRINGS.PAK");
                //convVM.LoadCnvArk(Loader.BasePath+"DATA\\cnv.ark");
                break;
            case GAME_UW2:
                UWHUD.instance.Begin();
                UWCharacter.Instance.Begin();
                UWCharacter.Instance.playerInventory.Begin();
                Quest.instance.QuestVariables = new int[250];//UW has a lot more quests. This value needs to be confirmed.
                StringController.instance.LoadStringsPak(Loader.BasePath + "DATA" + sep + "STRINGS.PAK");
                //convVM.LoadCnvArkUW2(Loader.BasePath+"DATA\\cnv.ark");
                break;
            default:
                UWHUD.instance.Begin();
                UWCharacter.Instance.Begin();
                UWCharacter.Instance.playerInventory.Begin();
                StringController.instance.LoadStringsPak(Loader.BasePath + "DATA" + sep + "STRINGS.PAK");
                //convVM.LoadCnvArk(Loader.BasePath+"DATA\\cnv.ark");
                break;
        }

        if (EnableTextureAnimation == true)
        {
            UWHUD.instance.CutsceneFullPanel.SetActive(false);
            InvokeRepeating("UpdateAnimation", 0.2f, 0.2f);
        }

        if (AtMainMenu)
        {
            SwitchLevel(-1);//Turn off all level maps
            UWHUD.instance.CutsceneFullPanel.SetActive(true);
            UWHUD.instance.mainmenu.gameObject.SetActive(true);
            //Freeze player movement and put them at a set location
            UWCharacter.Instance.playerController.enabled = false;
            UWCharacter.Instance.playerMotor.enabled = false;
            UWCharacter.Instance.transform.position = Vector3.zero;

            getMus().InIntro = true;
        }
        else
        {
            UWHUD.instance.CutsceneFullPanel.SetActive(false);
            UWHUD.instance.mainmenu.gameObject.SetActive(false);
            UWHUD.instance.RefreshPanels(UWHUD.HUD_MODE_INVENTORY);
            SwitchLevel(startLevel);
        }
        //PositionDetect();
        //InvokeRepeating("PositionDetect",0.0f,0.02f);
        return;
    }


    /// <summary>
    /// Gets the current level model.
    /// </summary>
    /// <returns>The current level model gameobject</returns>
    public GameObject getCurrentLevelModel()
    {
        //return GameWorldController.instance.WorldModel[LevelNo].transform.FindChild("Level" + LevelNo + "_model").gameObject;
        return LevelModel;
    }

    /// <summary>
    /// Updates the global shader parameter for the colorpalette shaders at set intervals. To enable texture animation
    /// </summary>
    void UpdateAnimation()
    {
        Shader.SetGlobalTexture("_ColorPaletteIn", paletteArray[paletteIndex]);

        if (paletteIndex < paletteArray.GetUpperBound(0))
        {
            paletteIndex++;
        }
        else
        {
            paletteIndex = 0;
        }

        //In Reverse

        Shader.SetGlobalTexture("_ColorPaletteInReverse", paletteArray[paletteIndexReverse]);

        if (paletteIndexReverse > 0)
        {
            paletteIndexReverse--;
        }
        else
        {
            paletteIndexReverse = paletteArray.GetUpperBound(0);
        }
        return;
    }

    /// <summary>
    /// inds a door in the tile pointed to by the two coordinates.
    /// </summary>
    /// <returns>The door.</returns>
    /// <param name="x">The x coordinate.</param>
    /// <param name="y">The y coordinate.</param>
    public static GameObject findDoor(int x, int y)
    {
        return GameObject.Find("door_" + x.ToString("D3") + "_" + y.ToString("D3"));
    }

    /// <summary>
    /// Finds the tile or wall at the specified coordinates.
    /// </summary>
    /// <returns>The tile.</returns>
    /// <param name="x">The x coordinate.</param>
    /// <param name="y">The y coordinate.</param>
    /// <param name="surface">Surface.</param>
    public static GameObject FindTile(int x, int y, int surface)
    {
        string tileName = GetTileName(x, y, surface);
        Transform found = instance.getCurrentLevelModel().transform.Find(tileName);
        if (found != null)
        {
            return found.gameObject;
        }
        Debug.Log("Cannot find " + tileName);
        return null;
    }

    /// <summary>
    /// Gets the gameobject name for the specified tile x,y and surface. Eg Wall_02_03, Tile_22_23
    /// </summary>
    /// <returns>The tile name.</returns>
    /// <param name="x">The x coordinate.</param>
    /// <param name="y">The y coordinate.</param>
    /// <param name="surface">Surface.</param>
    /// Surfaces are 
    public static string GetTileName(int x, int y, int surface)
    {//Assumes we'll only ever need to deal with open/solid tiles with floors and ceilings.
        string tileName;
        string X; string Y;
        X = x.ToString("D2");
        Y = y.ToString("D2");
        switch (surface)
        {
            case TileMap.SURFACE_WALL:  //SURFACE_WALL:
                {
                    tileName = "Wall_" + X + "_" + Y;
                    break;
                }
            case TileMap.SURFACE_CEIL: //SURFACE_CEIL:
                {
                    tileName = "Ceiling_" + X + "_" + Y;
                    break;
                }
            case TileMap.SURFACE_FLOOR:
            case TileMap.SURFACE_SLOPE:
            default:
                {
                    tileName = "Tile_" + X + "_" + Y;
                    break;
                }
        }
        return tileName;
    }

    /// <summary>
    /// Finds a tile in the current level by name
    /// </summary>
    /// <returns>The tile by name.</returns>
    /// <param name="tileName">Tile name.</param>
    public static GameObject FindTileByName(string tileName)
    {
        return instance.getCurrentLevelModel().transform.Find(tileName).gameObject;
    }

    /// <summary>
    /// Returns the transform of the levels object marker where objects are generated on.
    /// </summary>
    /// <returns>The marker.</returns>
    public Transform DynamicObjectMarker()
    {
        return _ObjectMarker.transform;
    }

    /// <summary>
    /// Switches the level to another one. Disables the map and level objects of the old one.
    /// </summary>
    /// <param name="newLevelNo">New level no.</param>
    /// 
    public void SwitchLevel(short newLevelNo)
    {
        if (newLevelNo != -1)
        {
            if (LevelNo == -1)
            {//I'm at the main menu. Load up the file data now.
                critsLoader = new CritLoader[64];//Clear out animations
                InitLevelData();
            }

            if (_RES == GAME_UW2)
            {
                getMus().ChangeTrackListForUW2(newLevelNo);
            }

            //Check loading
            if (Tilemaps[newLevelNo] == null)
            {//Data has not been loaded for this level
                Tilemaps[newLevelNo] = new TileMap(newLevelNo);

                if (UWEBase._RES != UWEBase.GAME_SHOCK)
                {
                    DataLoader.UWBlock lev_ark_block = new DataLoader.UWBlock();
                    DataLoader.UWBlock tex_ark_block = new DataLoader.UWBlock();
                    DataLoader.UWBlock ovl_ark_block = new DataLoader.UWBlock();

                    //Load the tile and object blocks
                    DataLoader.LoadUWBlock(lev_ark_file_data, newLevelNo, 0x7c06, out lev_ark_block);

                    if (_RES == GAME_UW1)
                    {//Load the overlays.
                        DataLoader.LoadUWBlock(lev_ark_file_data, newLevelNo + 9, 0x180, out ovl_ark_block);
                    }

                    //Load the texture maps
                    switch (_RES)
                    {
                        case GAME_UWDEMO:
                            DataLoader.ReadStreamFile(Loader.BasePath + "DATA" + sep + "LEVEL13.TXM", out tex_ark_block.Data);
                            tex_ark_block.DataLen = tex_ark_block.Data.GetUpperBound(0);
                            break;
                        case GAME_UW2:
                            DataLoader.LoadUWBlock(lev_ark_file_data, newLevelNo + 80, -1, out tex_ark_block);
                            break;
                        case GAME_UW1:
                        default:
                            DataLoader.LoadUWBlock(lev_ark_file_data, newLevelNo + 18, 0x7a, out tex_ark_block);
                            break;
                    }

                    if ((lev_ark_block.DataLen > 0) && (tex_ark_block.DataLen > 0))
                    {
                        if (EnableUnderworldGenerator)
                        {
                            Tilemaps[newLevelNo] = UnderworldGenerator.instance.CreateTileMap(newLevelNo);
                        }
                        else
                        {
                            Tilemaps[newLevelNo].BuildTileMapUW(newLevelNo, lev_ark_block, tex_ark_block, ovl_ark_block);
                        }                        
                        
                        objectList[newLevelNo] = new ObjectLoader();
                        objectList[newLevelNo].LoadObjectList(Tilemaps[newLevelNo], lev_ark_block);
                        if (CreateReports)
                        {
                            CreateObjectReport(objectList[newLevelNo].objInfo);
                        }
                        if (EnableUnderworldGenerator)
                        {
                            //Clear all objects for the random generator
                            for (int i = 0; i <= objectList[newLevelNo].objInfo.GetUpperBound(0); i++)
                            {
                                objectList[newLevelNo].objInfo[i].InUseFlag = 0;
                            }
                        }
                    }
                    else
                    {//load an empty level
                     //TODO:
                    }

                    //Original version
                    //	Tilemaps[newLevelNo].BuildTileMapUW(lev_ark_file_data, newLevelNo);
                    //	objectList[newLevelNo]=new ObjectLoader();
                    //	objectList[newLevelNo].LoadObjectList( Tilemaps[newLevelNo],lev_ark_file_data);	


                }
                else
                {
                    Tilemaps[newLevelNo].BuildTileMapShock(lev_ark_file_data, newLevelNo);
                    objectList[newLevelNo] = new ObjectLoader();
                    objectList[newLevelNo].LoadObjectListShock(Tilemaps[newLevelNo], lev_ark_file_data);
                }
                if (UWEBase.EditorMode == false)
                {
                    Tilemaps[newLevelNo].CleanUp(_RES);//I can reduce the tile map complexity after I know about what tiles change due to objects									
                }
                Tilemaps[newLevelNo].CreateRooms();
            }

            if ((UWEBase._RES != UWEBase.GAME_SHOCK) && (LevelNo != -1))
            {
                //Call events for inventory objects on level transition.
                foreach (Transform t in GameWorldController.instance.InventoryMarker.transform)
                {
                    if (t.gameObject.GetComponent<object_base>() != null)
                    {
                        t.gameObject.GetComponent<object_base>().InventoryEventOnLevelExit();
                    }
                }
            }

            if (LevelNo != -1)
            {//Changing from a level that has already loaded
             //Update the positions of all object interactions in the level
             //UpdatePositions();

                if (UWEBase.EditorMode == false)
                {
                    ObjectLoader.UpdateObjectList(GameWorldController.instance.currentTileMap(), GameWorldController.instance.CurrentObjectList());
                }
                //Store the state of the object list with just the objects in objects transform for when I re

            }


            //Get my object info into the tile map.
            LevelNo = newLevelNo;
            switch (UWEBase._RES)
            {
                case GAME_SHOCK:
                    break;
                default:
                    //critsLoader= new CritLoader[64];//Clear out animations
                    if (UWEBase.EditorMode == false)
                    {
                        if (LoadingGame == false)
                        {
                            //Call events for inventory objects on level transition.
                            foreach (Transform t in GameWorldController.instance.InventoryMarker.transform)
                            {
                                if (t.gameObject.GetComponent<object_base>() != null)
                                {
                                    t.gameObject.GetComponent<object_base>().InventoryEventOnLevelEnter();
                                }
                            }
                        }
                    }
                    break;
            }

            TileMapRenderer.GenerateLevelFromTileMap(LevelModel, SceneryModel, _RES, Tilemaps[newLevelNo], objectList[newLevelNo], false);

            if ((startX != -1) && (startY != -1))
            {
                float targetX = (float)startX * 1.2f + 0.6f;
                float targetY = (float)startY * 1.2f + 0.6f;
                float Height = ((float)(GameWorldController.instance.Tilemaps[newLevelNo].GetFloorHeight(startX, startY))) * 0.15f;

                UWCharacter.Instance.transform.position = new Vector3(targetX, Height + 0.1f, targetY);
                //Debug.Log("Spawning at " + UWCharacter.Instance.transform.position);
                UWCharacter.Instance.TeleportPosition = new Vector3(targetX, Height + 0.1f, targetY);
            }
            startX = -1; startY = -1;


            switch (UWEBase._RES)
            {
                case GAME_SHOCK:
                //break;
                default:
                    ObjectLoader.RenderObjectList(objectList[newLevelNo], Tilemaps[newLevelNo], DynamicObjectMarker().gameObject);
                    CleanUpMagicProjectiles();
                    break;
            }


            if ((bGenNavMeshes) && (!EditorMode))
            {
                //if (!GameWorldController.instance.AtMainMenu)
                //{//Force the nav meshes to update in 5 frames time to fix bug with nav meshes of two levels merging.
                //GenNavMeshNextFrame=5;	
                //}

                //GenerateNavmesh(NavRigLand);
                //GenerateNavmesh(NavRigWater);
                //GenerateNavmesh(navmeshsurface,256);
                //GenerateNavMeshes();
                string newSignature = GameWorldController.instance.currentTileMap().getSignature();
                if (newSignature != LevelSignature)
                {
                    //Debug.Log("Generating navmesh");
                    NavMeshReady = false;
                    StartCoroutine(UpdateNavMeshes());
                }
                LevelSignature = newSignature;
            }

            if ((LevelNo == 7) && (UWEBase._RES == UWEBase.GAME_UW1))
            {//Create shrine lava.
                GameObject shrineLava = new GameObject();
                shrineLava.transform.parent = SceneryModel.transform;
                shrineLava.transform.localPosition = new Vector3(-39f, 39.61f, 0.402f);
                shrineLava.transform.localScale = new Vector3(6f, 0.2f, 4.8f);
                shrineLava.AddComponent<ShrineLava>();
                shrineLava.AddComponent<BoxCollider>();
                shrineLava.GetComponent<BoxCollider>().isTrigger = true;
            }
        }
        if ((_RES == GAME_UW2) && (EditorMode == false))
        {
            if (events != null)
            {
                if (!LoadingGame)
                {
                    events.ProcessEvents();
                }
            }
        }
    }

    /// <summary>
    /// Switchs the level and puts the player at the floor level of the new level
    /// </summary>
    /// <param name="newLevelNo">New level no.</param>
    /// <param name="newTileX">New tile x.</param>
    /// <param name="newTileY">New tile y.</param>
    public void SwitchLevel(short newLevelNo, short newTileX, short newTileY)
    {

        //float targetX=(float)newTileX*1.2f + 0.6f;
        //float targetY= (float)newTileY*1.2f + 0.6f;
        //float Height = ((float)(GameWorldController.instance.Tilemaps[newLevelNo].GetFloorHeight(newTileX,newTileY)))*0.15f;

        //UWCharacter.Instance.transform.position=new Vector3(targetX,Height+0.05f,targetY);
        //UWCharacter.Instance.TeleportPosition=new Vector3(targetX,Height+0.05f,targetY);

        startX = newTileX;
        startY = newTileY;
        SwitchLevel(newLevelNo);
    }


    static void CleanUpMagicProjectiles()
    {
        return;
        ObjectLoaderInfo[] objList = GameWorldController.instance.CurrentObjectList().objInfo;
        for (int i = 0; i <= objList.GetUpperBound(0); i++)
        {
            if (objList[i] != null)
            {
                if (objList[i].GetItemType() == ObjectInteraction.A_MAGIC_PROJECTILE)
                {
                    if (objList[i].instance != null)
                    {
                        if (objList[i].instance.GetComponent<MagicProjectile>() != null)
                        {
                            objList[i].instance.GetComponent<MagicProjectile>().DetonateNow = true;
                        }
                    }
                }
            }
        }
    }

    // This will regenerate the navigation mesh when called
    /*	void GenerateNavmesh(RAIN.Navigation.NavMesh.NavMeshRig NavRig)
		{//From Legacy.rivaltheory.com/forums/topics/runtime-navmesh-generation-and-path-finding-tutorial
				int _threadcount=4;
				// Unregister any navigation mesh we may already have (probably none if you are using this)
				NavRig.NavMesh.UnregisterNavigationGraph();
				NavRig.NavMesh.Size = 20;
				//float startTime = Time.time;
				NavRig.NavMesh.StartCreatingContours(_threadcount);
				NavRig.NavMesh.CreateAllContours();
				//float endTime = Time.time;
				//Debug.Log("NavMesh generated in " + (endTime - startTime) + "s");
				NavRig.NavMesh.RegisterNavigationGraph();
				NavRig.Awake();

		}*/






    /// <summary>
    /// Freezes the movement of the specified object if it has a rigid body attached.
    /// </summary>
    /// <param name="myObj">My object.</param>
    public static void FreezeMovement(GameObject myObj)
    {//Stop objects which can move in the 3d world from moving when they are in the inventory or containers.
        Rigidbody rg = myObj.GetComponent<Rigidbody>();
        if (rg != null)
        {
            rg.useGravity = false;
            rg.constraints =
                    RigidbodyConstraints.FreezeRotationX
                    | RigidbodyConstraints.FreezeRotationY
                    | RigidbodyConstraints.FreezeRotationZ
                    | RigidbodyConstraints.FreezePositionX
                    | RigidbodyConstraints.FreezePositionY
                    | RigidbodyConstraints.FreezePositionZ;
        }
    }

    /// <summary>
    /// Unfreeze the movement of the specified object if it has a rigid body attached.
    /// </summary>
    /// <param name="myObj">My object.</param>
    public static void UnFreezeMovement(GameObject myObj)
    {//Allow objects which can move in the 3d world to moving when they are released.
        Rigidbody rg = myObj.GetComponent<Rigidbody>();
        if (rg != null)
        {
            rg.useGravity = true;
            rg.constraints =
                    RigidbodyConstraints.FreezeRotationX
                    | RigidbodyConstraints.FreezeRotationY
                    | RigidbodyConstraints.FreezeRotationZ;
        }
    }

    /// <summary>
    /// Returns the music controller
    /// </summary>
    /// <returns>The mus.</returns>
    public MusicController getMus()
    {
        if (mus == null)
        {
            mus = GameObject.Find("_MusicController").GetComponent<MusicController>();
        }
        return mus;
    }

    /// <summary>
    /// Returns the current tile map
    /// </summary>
    /// <returns>The tile map.</returns>
    public TileMap currentTileMap()
    {
        if (LevelNo == -1)
        {
            return null;
        }
        else
        {
            return Tilemaps[LevelNo];
        }

    }

    public AutoMap currentAutoMap()
    {
        if (LevelNo == -1)
        {
            return null;
        }
        else
        {
            return AutoMaps[LevelNo];
        }
    }

    /// <summary>
    /// Detects where the player currently is an updates their swimming state and auto map as needed.
    /// </summary>
    public void PositionDetect()
    {
        if ((AtMainMenu == true) || (WindowDetect.InMap))
        {
            return;
        }
        if ((_RES != GAME_UW1) && (_RES != GAME_UWDEMO) && (_RES != GAME_UW2))
        {
            return;
        }
        TileMap.visitTileX = (short)(UWCharacter.Instance.transform.position.x / 1.2f);
        TileMap.visitTileY = (short)(UWCharacter.Instance.transform.position.z / 1.2f);
        UWCharacter.Instance.room = currentTileMap().Tiles[TileMap.visitTileX, TileMap.visitTileY].roomRegion;

        if (EditorMode)
        {
            if ((TileMap.visitedTileX != TileMap.visitTileX) || (TileMap.visitedTileY != TileMap.visitTileY))
            {
                if (IngameEditor.FollowMeMode)
                {
                    IngameEditor.UpdateFollowMeMode(TileMap.visitTileX, TileMap.visitTileY);
                }
            }
        }
        //currentTileMap().SetTileVisited(TileMap.visitTileX,TileMap.visitTileY);
        //UWCharacter.Instance.isSwimming=((TileMap.OnWater) && (!UWCharacter.Instance.isWaterWalking) && (!GameWorldController.EditorMode)) ;
        //UWCharacter.Instance.onIce=((TileMap.OnIce) && (!UWCharacter.Instance.isWaterWalking) && (!GameWorldController.EditorMode)) ;

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if
                        (
                                (
                                        (TileMap.visitTileX + x >= 0) && (TileMap.visitTileX + x <= TileMap.TileMapSizeX)
                                )
                                &&
                                (
                                        (TileMap.visitTileY + y >= 0) && (TileMap.visitTileY + y <= TileMap.TileMapSizeY)
                                )
                        )
                {
                    currentAutoMap().MarkTile(TileMap.visitTileX + x, TileMap.visitTileY + y, currentTileMap().Tiles[TileMap.visitTileX + x, TileMap.visitTileY + y].tileType, AutoMap.GetDisplayType(currentTileMap().Tiles[TileMap.visitTileX + x, TileMap.visitTileY + y]));
                }
            }
        }
        TileMap.visitedTileX = TileMap.visitTileX;
        TileMap.visitedTileY = TileMap.visitTileY;
        UWCharacter.Instance.CurrentTerrain = currentTileMap().Tiles[TileMap.visitTileX, TileMap.visitTileY].terrain;
        UWCharacter.Instance.terrainType = TerrainDatLoader.getTerrain(UWCharacter.Instance.CurrentTerrain);
    }

    /// <summary>
    /// Returns the current map object list
    /// </summary>
    /// <returns>The object list.</returns>
    public ObjectLoader CurrentObjectList()
    {
        if (LevelNo == -1)
        {
            return null;
        }
        else
        {
            return objectList[LevelNo];
        }
    }

    /// <summary>
    /// Moves the object to the game world where it will be managed by the objectloader list
    /// </summary>
    /// <param name="obj">Object.</param>
    public static void MoveToWorld(GameObject obj)
    {
        //Debug.Log(obj.name + "is moved to world");
        MoveToWorld(obj.GetComponent<ObjectInteraction>());

    }

    /// <summary>
    /// Moves to world and assigns it to the world object list.
    /// </summary>
    /// <returns>The to world.</returns>
    /// <param name="obj">Object.</param>
    public static ObjectInteraction MoveToWorld(ObjectInteraction obj)
    {
        //Add item to a free slot on the item list and point the instance back to this.
        obj.UpdatePosition();
        //if (obj.transform.parent == GameWorldController.instance.DynamicObjectMarker())
        //{
        //		Debug.Log("Moving to world when object is already in world " + obj.name);
        //}
        obj.transform.parent = GameWorldController.instance.DynamicObjectMarker();
        ObjectLoader.AssignObjectToList(ref obj);
        //	ObjectInteraction.UpdateLinkedList(obj, TileMap.ObjectStorageTile, TileMap.ObjectStorageTile, obj.tileX, obj.tileY);
        //obj.next=0;
        //obj.UpdatePosition();
        //obj.tileX=-1;
        //obj.tileY=-1;//Force an update to linked list.
        obj.GetComponent<object_base>().MoveToWorldEvent();
        if (ConversationVM.InConversation)
        {
            Debug.Log("Use of MoveToWorld in conversation. Review usage to avoid object list corruption! " + obj.name);
            ConversationVM.BuildObjectList();//Reflect changes to object lists
        }

        //obj.name = ObjectLoader.UniqueObjectName(obj.objectloaderinfo);
        return obj;
        //Not needed???
    }

    /// <summary>
    /// Moves to inventory where it will no longer be managed by the objectloader list.
    /// </summary>
    /// <param name="obj">Object.</param>
    public static void MoveToInventory(GameObject obj)
    {
        MoveToInventory(obj.GetComponent<ObjectInteraction>());
    }


    /// <summary>
    /// Moves an object to inventory and removes it from the world map instance
    /// </summary>
    /// <param name="obj">Object.</param>
    public static void MoveToInventory(ObjectInteraction obj)
    {//Break the instance back to the object list
        obj.objectloaderinfo.InUseFlag = 0;//This frees up the slot to be replaced with another item.	
        obj.objectloaderinfo.instance = null;
        if (_RES == GAME_UW2)//Does this need to be done for uw1 as well.
        {
            ObjectLoaderInfo.CleanUp(obj.objectloaderinfo);
        }
        obj.GetComponent<object_base>().MoveToInventoryEvent();
        //ObjectInteraction.UpdateLinkedList(obj, obj.tileX, obj.tileY, TileMap.ObjectStorageTile,TileMap.ObjectStorageTile);
        if (ConversationVM.InConversation)
        {
            ConversationVM.BuildObjectList();//Reflect changes to object lists
        }
    }


    /// <summary>
    /// Updates the positions of all game objects
    /// </summary>
    public void UpdatePositions()
    {
        foreach (Transform t in GameWorldController.instance.DynamicObjectMarker())
        {
            if (t.gameObject.GetComponent<ObjectInteraction>() != null)
            {
                t.gameObject.GetComponent<ObjectInteraction>().UpdatePosition();
            }
        }
    }

    /// <summary>
    /// Writes a lev ark file based on a rebuilding of the data.
    /// </summary>
    /// <param name="slotNo">Slot no.</param>
    /// 320 blocks
    /// 80 level maps - to be decompressed
    /// 80 texture maps - to copy
    /// 80 automaps - to copy for the moment
    /// 80 map notes - top copy for the moment
    public void WriteBackLevArkUW2(int slotNo)
    {
        int NoOfBlocks = 320;
        DataLoader.UWBlock[] blockData = new DataLoader.UWBlock[NoOfBlocks];

        //First update the object list so as to match indices properly	
        ObjectLoader.UpdateObjectList(GameWorldController.instance.currentTileMap(), GameWorldController.instance.CurrentObjectList());

        //First block is always here.
        long AddressToCopyFrom = 0;
        long ReadDataLen = 0;

        //Read in the data for the 80 tile/object maps
        for (int l = 0; l <= GameWorldController.instance.Tilemaps.GetUpperBound(0); l++)
        {
            blockData[l].CompressionFlag = (int)DataLoader.getValAtAddress(lev_ark_file_data, 6 + (l * 4) + (NoOfBlocks * 4), 32);
            blockData[l].DataLen = DataLoader.getValAtAddress(lev_ark_file_data, 6 + (l * 4) + (NoOfBlocks * 8), 32);
            blockData[l].ReservedSpace = DataLoader.getValAtAddress(lev_ark_file_data, 6 + (l * 4) + (NoOfBlocks * 12), 32);
            if (GameWorldController.instance.Tilemaps[l] != null)
            {
                long UnPackDatalen = 0;
                blockData[l].CompressionFlag = DataLoader.UW2_NOCOMPRESSION;
                blockData[l].Data = GameWorldController.instance.Tilemaps[l].TileMapToBytes(lev_ark_file_data, out UnPackDatalen);
                //blockData[l].DataLen=blockData[l].Data.GetUpperBound(0)+1;//32279;//
                blockData[l].DataLen = UnPackDatalen;
                //if (blockData[l].ReservedSpace< blockData[l].DataLen)
                //{
                //Debug.Log("Changing reserved space for block " + l + " to datalen was " + blockData[l].ReservedSpace + " now "  + blockData[l].DataLen );
                //blockData[l].ReservedSpace= blockData[l].DataLen;			
                //}
            }///31752
            else
            {//Copy data from file.
                AddressToCopyFrom = DataLoader.getValAtAddress(lev_ark_file_data, (l * 4) + 6, 32);
                if (AddressToCopyFrom != 0)
                {//Only copy a block with data										
                    blockData[l].Data = CopyData(AddressToCopyFrom, blockData[l].ReservedSpace);//31752
                }
                else
                {
                    blockData[l].DataLen = 0;
                }
            }
        }

        //read in the texture maps
        //TODO: At the moment this is just a straight copy of this information
        for (int l = 0; l <= GameWorldController.instance.Tilemaps.GetUpperBound(0); l++)
        {
            AddressToCopyFrom = DataLoader.getValAtAddress(lev_ark_file_data, (l * 4) + 6 + (80 * 4), 32);
            blockData[l + 80].CompressionFlag = (int)DataLoader.getValAtAddress(lev_ark_file_data, (l * 4) + 6 + (80 * 4) + (NoOfBlocks * 4), 32);
            blockData[l + 80].ReservedSpace = DataLoader.getValAtAddress(lev_ark_file_data, (l * 4) + 6 + (80 * 4) + (NoOfBlocks * 12), 32);
            ReadDataLen = DataLoader.getValAtAddress(lev_ark_file_data, (l * 4) + 6 + (80 * 4) + (NoOfBlocks * 8), 32); //+ (NoOfBlocks*4)				

            if (AddressToCopyFrom != 0)
            {
                blockData[l + 80].Data = CopyData(AddressToCopyFrom, ReadDataLen);
                blockData[l + 80].DataLen = blockData[l + 80].Data.GetUpperBound(0) + 1;
            }
            else
            {
                blockData[l + 80].DataLen = 0;
            }
        }

        //read in the automaps
        for (int l = 0; l <= GameWorldController.instance.Tilemaps.GetUpperBound(0); l++)
        {
            if (GameWorldController.instance.AutoMaps[l] != null)
            {
                blockData[l + 160].CompressionFlag = DataLoader.UW2_NOCOMPRESSION;
                blockData[l + 160].Data = GameWorldController.instance.AutoMaps[l].AutoMapVisitedToBytes();
                blockData[l + 160].DataLen = blockData[l + 160].Data.GetUpperBound(0) + 1;
                blockData[l + 160].ReservedSpace = blockData[l + 160].DataLen;
            }
            else
            {//Just copy the data from the old ark to the new ark
                AddressToCopyFrom = DataLoader.getValAtAddress(lev_ark_file_data, (l * 4) + 6 + (160 * 4), 32);
                blockData[l + 160].CompressionFlag = (int)DataLoader.getValAtAddress(lev_ark_file_data, (l * 4) + 6 + (160 * 4) + (NoOfBlocks * 4), 32);
                ReadDataLen = DataLoader.getValAtAddress(lev_ark_file_data, (l * 4) + 6 + (160 * 4) + (NoOfBlocks * 8), 32); //+ (NoOfBlocks*4)
                blockData[l + 160].ReservedSpace = DataLoader.getValAtAddress(lev_ark_file_data, (l * 4) + 6 + (160 * 4) + (NoOfBlocks * 12), 32);

                if (AddressToCopyFrom != 0)
                {
                    blockData[l + 160].Data = CopyData(AddressToCopyFrom, ReadDataLen);
                    blockData[l + 160].DataLen = blockData[l + 160].Data.GetUpperBound(0) + 1;
                }
                else
                {
                    blockData[l + 160].DataLen = 0;
                }
            }
        }


        //read in the automap notes
        //TODO: At the moment this is just a straight copy of this information
        for (int l = 0; l <= GameWorldController.instance.Tilemaps.GetUpperBound(0); l++)
        {
            if (GameWorldController.instance.AutoMaps[l] != null)
            {
                blockData[l + 240].Data = GameWorldController.instance.AutoMaps[l].AutoMapNotesToBytes();
                if (blockData[l + 240].Data != null)
                {
                    blockData[l + 240].CompressionFlag = DataLoader.UW2_NOCOMPRESSION;
                    blockData[l + 240].DataLen = blockData[l + 240].Data.GetUpperBound(0) + 1;
                    blockData[l + 240].ReservedSpace = blockData[l + 240].DataLen;
                }
                else
                {//just copy
                    AddressToCopyFrom = DataLoader.getValAtAddress(lev_ark_file_data, (l * 4) + 6 + (240 * 4), 32);
                    blockData[l + 240].CompressionFlag = (int)DataLoader.getValAtAddress(lev_ark_file_data, (l * 4) + 6 + (240 * 4) + (NoOfBlocks * 4), 32);
                    ReadDataLen = DataLoader.getValAtAddress(lev_ark_file_data, (l * 4) + 6 + (240 * 4) + (NoOfBlocks * 8), 32); //+ (NoOfBlocks*4)
                    blockData[l + 240].ReservedSpace = DataLoader.getValAtAddress(lev_ark_file_data, (l * 4) + 6 + (240 * 4) + (NoOfBlocks * 12), 32);
                    if (AddressToCopyFrom != 0)
                    {
                        blockData[l + 240].Data = CopyData(AddressToCopyFrom, ReadDataLen);
                        blockData[l + 240].DataLen = blockData[l + 240].Data.GetUpperBound(0) + 1;
                    }
                    else
                    {
                        blockData[l + 240].DataLen = 0;
                    }
                }

            }
            else
            {//just copy.
                AddressToCopyFrom = DataLoader.getValAtAddress(lev_ark_file_data, (l * 4) + 6 + (240 * 4), 32);
                blockData[l + 240].CompressionFlag = (int)DataLoader.getValAtAddress(lev_ark_file_data, (l * 4) + 6 + (240 * 4) + (NoOfBlocks * 4), 32);
                ReadDataLen = DataLoader.getValAtAddress(lev_ark_file_data, (l * 4) + 6 + (240 * 4) + (NoOfBlocks * 8), 32); //+ (NoOfBlocks*4)
                blockData[l + 240].ReservedSpace = DataLoader.getValAtAddress(lev_ark_file_data, (l * 4) + 6 + (240 * 4) + (NoOfBlocks * 12), 32);
                if (AddressToCopyFrom != 0)
                {
                    blockData[l + 240].Data = CopyData(AddressToCopyFrom, ReadDataLen);
                    blockData[l + 240].DataLen = blockData[l + 240].Data.GetUpperBound(0) + 1;
                }
                else
                {
                    blockData[l + 240].DataLen = 0;
                }
            }

        }


        blockData[0].Address = 5126;//This will always be the same.
        long prevAddress = blockData[0].Address;
        long prevSize = Math.Max(blockData[0].ReservedSpace, blockData[0].DataLen);
        //Work out the block addresses.
        for (int i = 1; i < blockData.GetUpperBound(0); i++)
        {
            if (blockData[i].DataLen != 0)//This block has data and needs to be written.
            {
                blockData[i].Address = prevAddress + prevSize;
                prevAddress = blockData[i].Address;
                prevSize = Math.Max(blockData[i].ReservedSpace, blockData[i].DataLen);
            }
            else
            {
                blockData[i].Address = 0;
            }
        }



        //Data is copied. now begin writing the output file
        FileStream file = File.Open(Loader.BasePath + "SAVE" + slotNo + sep + "LEV.ARK", FileMode.Create);
        BinaryWriter writer = new BinaryWriter(file);
        long add_ptr = 0;

        add_ptr += DataLoader.WriteInt8(writer, 0x40);
        add_ptr += DataLoader.WriteInt8(writer, 0x01);
        add_ptr += DataLoader.WriteInt8(writer, 0x0);//1
        add_ptr += DataLoader.WriteInt8(writer, 0x0);//2
        add_ptr += DataLoader.WriteInt8(writer, 0x0);//3
        add_ptr += DataLoader.WriteInt8(writer, 0x0);//4

        //Now write block addresses
        for (int i = 0; i < 320; i++)
        {//write block addresses
            add_ptr += DataLoader.WriteInt32(writer, blockData[i].Address);
        }

        //Now write compression flags
        for (int i = 320; i < 640; i++)
        {//write block compression flags
            add_ptr += DataLoader.WriteInt32(writer, blockData[i - 320].CompressionFlag);
        }

        //Now write data lengths
        for (int i = 960; i < 1280; i++)
        {//write block data lengths
            add_ptr += DataLoader.WriteInt32(writer, blockData[i - 960].DataLen);
        }


        for (int i = 1280; i < 1600; i++)
        {//write block data reservations
            add_ptr += DataLoader.WriteInt32(writer, blockData[i - 1280].ReservedSpace);
        }

        for (long freespace = add_ptr; freespace < blockData[0].Address; freespace++)
        {//write freespace to fill up to the final block.
            add_ptr += DataLoader.WriteInt8(writer, 0);
        }


        //Now be brave and write all my blocks!!!
        for (int i = 0; i <= blockData.GetUpperBound(0); i++)
        {
            if (blockData[i].Data != null)//?
            {
                if (add_ptr < blockData[i].Address)
                {
                    while (add_ptr < blockData[i].Address)
                    {//Fill whitespace until next block address.
                        add_ptr += DataLoader.WriteInt8(writer, 0);
                    }
                }
                else
                {
                    if (add_ptr > blockData[i].Address)
                    {
                        Debug.Log("Writing block " + i + " at " + add_ptr + " should be " + blockData[i].Address);
                    }
                }
                Debug.Log("Writing block " + i + " datalen " + blockData[i].DataLen + " ubound=" + blockData[i].Data.GetUpperBound(0));
                //for (long a =0; a<=blockData[i].Data.GetUpperBound(0); a++)
                int blockUbound = blockData[i].Data.GetUpperBound(0);
                for (long a = 0; a < blockData[i].DataLen; a++)
                {
                    if (a <= blockUbound)
                    {
                        add_ptr += DataLoader.WriteInt8(writer, (long)blockData[i].Data[a]);
                    }
                    else
                    {
                        add_ptr += DataLoader.WriteInt8(writer, 0);
                    }
                }
            }
        }

        file.Close();

        return;



    }

    /// <summary>
    /// Writes a lev ark file based on a rebuilding of the data.
    /// </summary>
    /// <param name="slotNo">Slot no.</param>
    ///<9 blocks level tilemap/master object list>
    ///<9 blocks object animation overlay info>
    ///<9 blocks texture mapping>
    ///<9 blocks automap infos>
    ///<9 blocks map notes>
    ///The remaining 9 x 10 blocks are unused.
    /// 
    public void WriteBackLevArkUW1(int slotNo)
    {
        DataLoader.UWBlock[] blockData = new DataLoader.UWBlock[45];

        //First update the object list so as to match indices properly
        ObjectLoader.UpdateObjectList(GameWorldController.instance.currentTileMap(), GameWorldController.instance.CurrentObjectList());

        //First block is always here.
        long AddressToCopyFrom = 0;

        //Read in the data for the 9 tile/object maps
        for (int l = 0; l <= GameWorldController.instance.Tilemaps.GetUpperBound(0); l++)
        {
            if (GameWorldController.instance.Tilemaps[l] != null)
            {
                long UnPackDatalen = 0;
                blockData[l].Data = GameWorldController.instance.Tilemaps[l].TileMapToBytes(lev_ark_file_data, out UnPackDatalen);
                blockData[l].DataLen = blockData[l].Data.GetUpperBound(0) + 1;
            }///31752
            else
            {
                AddressToCopyFrom = DataLoader.getValAtAddress(lev_ark_file_data, (l * 4) + 2, 32);
                blockData[l].Data = CopyData(AddressToCopyFrom, 31752);//TileMap.TileMapSizeX*TileMap.TileMapSizeY*4  +  256*27 + 768*8);	
                blockData[l].DataLen = blockData[l].Data.GetUpperBound(0) + 1;
            }
        }

        //Read in the data for the animation overlays
        for (int l = 0; l <= GameWorldController.instance.Tilemaps.GetUpperBound(0); l++)
        {
            AddressToCopyFrom = DataLoader.getValAtAddress(lev_ark_file_data, ((l + 9) * 4) + 2, 32);
            blockData[l + 9].Data = CopyData(AddressToCopyFrom, 64 * 6);
            blockData[l + 9].DataLen = blockData[l + 9].Data.GetUpperBound(0) + 1;
        }

        //read in the texture maps
        for (int l = 0; l <= GameWorldController.instance.Tilemaps.GetUpperBound(0); l++)
        {
            if (GameWorldController.instance.Tilemaps[l] != null)
            {
                blockData[l + 18].Data = GameWorldController.instance.Tilemaps[l].TextureMapToBytes();
                blockData[l + 18].DataLen = blockData[l + 18].Data.GetUpperBound(0) + 1;
            }
            else
            {
                AddressToCopyFrom = DataLoader.getValAtAddress(lev_ark_file_data, ((l + 18) * 4) + 2, 32);
                blockData[l + 18].Data = CopyData(AddressToCopyFrom, 0x7a);
                blockData[l + 18].DataLen = blockData[l + 18].Data.GetUpperBound(0) + 1;
            }
        }

        //read in the auto maps
        for (int l = 0; l <= GameWorldController.instance.AutoMaps.GetUpperBound(0); l++)
        {
            blockData[l + 27].Data = GameWorldController.instance.AutoMaps[l].AutoMapVisitedToBytes();
            if (blockData[l + 27].Data != null)
            {
                blockData[l + 27].DataLen = blockData[l + 27].Data.GetUpperBound(0) + 1;
            }
            else
            {
                blockData[l + 27].DataLen = 0;
            }
        }


        //read in the auto maps notes
        for (int l = 0; l <= GameWorldController.instance.AutoMaps.GetUpperBound(0); l++)
        {
            blockData[l + 36].Data = GameWorldController.instance.AutoMaps[l].AutoMapNotesToBytes();
            if (blockData[l + 36].Data != null)
            {
                blockData[l + 36].DataLen = blockData[l + 36].Data.GetUpperBound(0) + 1;
            }
            else
            {
                blockData[l + 36].DataLen = 0;
            }
        }

        blockData[0].Address = 542;
        long prevAddress = blockData[0].Address;
        //Work out the block addresses.
        for (int i = 1; i < blockData.GetUpperBound(0); i++)
        {//This algorithm is probably wrong but only works because all blocks are in use!
            if (blockData[i - 1].DataLen != 0)
            {
                blockData[i].Address = prevAddress + blockData[i - 1].DataLen;
                prevAddress = blockData[i].Address;
            }
            else
            {
                blockData[i].Address = 0;
            }
        }


        FileStream file = File.Open(Loader.BasePath + "SAVE" + slotNo + sep + "LEV.ARK", FileMode.Create);
        BinaryWriter writer = new BinaryWriter(file);
        long add_ptr = 0;
        add_ptr += DataLoader.WriteInt8(writer, 0x87);
        add_ptr += DataLoader.WriteInt8(writer, 0);
        for (int i = 0; i <= blockData.GetUpperBound(0); i++)
        {//write block addresses
            add_ptr += DataLoader.WriteInt32(writer, blockData[i].Address);
        }

        for (long freespace = add_ptr; freespace < blockData[0].Address; freespace++)
        {//write freespace
            add_ptr += DataLoader.WriteInt8(writer, 0);
        }

        //Now be brave and write all my blocks!!!
        for (int i = 0; i <= blockData.GetUpperBound(0); i++)
        {
            if (blockData[i].Data != null)
            {
                for (long a = 0; a <= blockData[i].Data.GetUpperBound(0); a++)
                {
                    add_ptr += DataLoader.WriteInt8(writer, (long)blockData[i].Data[a]);
                }
            }
        }

        file.Close();

        return;
    }

    /// <summary>
    /// Inits the level data maps and textures.
    /// </summary>
    void InitLevelData()
    {
        // Path to lev.ark file to load
        string Lev_Ark_File;

        switch (_RES)
        {
            case GAME_SHOCK:
                Tilemaps = new TileMap[15];
                objectList = new ObjectLoader[15];
                break;
            case GAME_UWDEMO:
                Tilemaps = new TileMap[1];
                objectList = new ObjectLoader[1];
                AutoMaps = new AutoMap[1];
                break;
            case GAME_UW2:
                Tilemaps = new TileMap[80];//Not all are in use.
                objectList = new ObjectLoader[80];
                AutoMaps = new AutoMap[80];
                break;
            case GAME_UW1:
            default:
                Tilemaps = new TileMap[9];
                objectList = new ObjectLoader[9];
                AutoMaps = new AutoMap[9];
                break;
        }



        switch (UWEBase._RES)
        {
            case UWEBase.GAME_SHOCK:
                MaterialMasterList = new Material[273];
                break;
            case UWEBase.GAME_UWDEMO:
                MaterialMasterList = new Material[58];
                break;
            case UWEBase.GAME_UW2:
                MaterialMasterList = new Material[256];//For each texture in UW2
                break;
            case UWEBase.GAME_UW1:
            default:
                MaterialMasterList = new Material[260];//For each texture in UW1
                break;
        }

        //Load up my map materials
        for (int i = 0; i <= MaterialMasterList.GetUpperBound(0); i++)
        {
            MaterialMasterList[i] = (Material)Resources.Load(_RES + "/Materials/textures/" + _RES + "_" + i.ToString("d3"));
            switch (MaterialMasterList[i].shader.name.ToUpper())
            {
                case "COLOURREPLACEMENT":
                case "COLOURREPLACEMENTREVERSE":
                    MaterialMasterList[i].mainTexture = texLoader.LoadImageAt(i, 1);//load a greyscale texture for use with the shader.
                    break;
                case "LEGACY SHADERS/BUMPED DIFFUSE":
                    {
                        Texture2D loadedTexture = texLoader.LoadImageAt(i, 2);//Get normal map for mod directory
                        MaterialMasterList[i].mainTexture = texLoader.LoadImageAt(i, 0);
                        if (loadedTexture != null)
                        {
                            MaterialMasterList[i].SetTexture("_BumpMap", TextureLoader.NormalMap(loadedTexture, TextureLoader.BumpMapStrength));
                        }
                    }
                    break;
                default:
                    //Debug.Log(i + " is " + MaterialMasterList[i].shader.name);
                    MaterialMasterList[i].mainTexture = texLoader.LoadImageAt(i, 0);
                    break;
            }
        }
        if (_RES == GAME_UW1)
        {
            SpecialMaterials[0] = (Material)Resources.Load(_RES + "/Materials/textures/" + _RES + "_224_maze");
            SpecialMaterials[0].mainTexture = texLoader.LoadImageAt(224);
        }
        MaterialObj = new Material[TmObjArt.NoOfFileImages()];

        //Load the materials for the TMOBJ file
        for (int i = 0; i <= MaterialObj.GetUpperBound(0); i++)
        {
            MaterialObj[i] = (Material)Resources.Load(_RES + "/Materials/tmobj/tmobj_" + i.ToString("d2"));
            if (MaterialObj[i] != null)
            {
                MaterialObj[i].mainTexture = TmObjArt.LoadImageAt(i);
            }
        }

        switch (_RES)
        {
            case GAME_SHOCK:
                break;

            default:
                //Load up my door texture
                for (int i = 0; i <= MaterialDoors.GetUpperBound(0); i++)
                {
                    MaterialDoors[i] = (Material)Resources.Load(_RES + "/Materials/doors/doors_" + i.ToString("d2") + "_material");
                    MaterialDoors[i].mainTexture = DoorArt.LoadImageAt(i);
                }
                break;

        }

        //Load up my tile maps
        //First read in my lev_ark file
        switch (UWEBase._RES)
        {
            case GAME_SHOCK:
                Lev_Ark_File = "RES" + sep + "DATA" + sep + "ARCHIVE.DAT";
                break;
            case UWEBase.GAME_UWDEMO:
                Lev_Ark_File = "DATA" + sep + "LEVEL13.ST";
                break;
            case UWEBase.GAME_UW2:
            case UWEBase.GAME_UW1:
            default:
                Lev_Ark_File = Lev_Ark_File_Selected; //"DATA\\lev.ark";//Eventually this will be a save game.
                break;
        }

        if (!DataLoader.ReadStreamFile(Loader.BasePath + Lev_Ark_File, out lev_ark_file_data))
        {
            Debug.Log(Loader.BasePath + Lev_Ark_File + "File not loaded");
            Application.Quit();
        }

        //Load up auto map data
        switch (_RES)
        {
            case GAME_UWDEMO:
                AutoMaps[0] = new AutoMap();
                AutoMaps[0].InitAutoMapDemo();
                break;
            case GAME_UW1:
                for (int i = 0; i <= AutoMaps.GetUpperBound(0); i++)
                {
                    AutoMaps[i] = new AutoMap();
                    AutoMaps[i].InitAutoMapUW1(i, lev_ark_file_data);
                }
                break;
            case GAME_UW2:
                for (int i = 0; i <= AutoMaps.GetUpperBound(0); i++)
                {
                    AutoMaps[i] = new AutoMap();
                    AutoMaps[i].InitAutoMapUW2(i, lev_ark_file_data);
                }
                break;
        }

        switch (_RES)
        {
            case GAME_UW2:
                events = new event_processor();
                if (whatTheHellIsThatFileFor != null)
                {
                    whatTheHellIsThatFileFor.DumpScdArkInfo(SCD_Ark_File_Selected);
                }
                break;
        }
    }


    /// <summary>
    /// Inits the B globals.
    /// </summary>
    /// <param name="SlotNo">Slot no.</param>
    public void InitBGlobals(int SlotNo)
    {
        char[] bglob_data;
        if (SlotNo == 0)
        {//Init from BABGLOBS.DAT. Initialise the data.
            if (DataLoader.ReadStreamFile(Loader.BasePath + "DATA" + sep + "BABGLOBS.DAT", out bglob_data))
            {
                int NoOfSlots = bglob_data.GetUpperBound(0) / 4;
                int add_ptr = 0;
                bGlobals = new bablGlobal[NoOfSlots + 1];
                for (int i = 0; i <= NoOfSlots; i++)
                {
                    bGlobals[i].ConversationNo = (int)DataLoader.getValAtAddress(bglob_data, add_ptr, 16);
                    bGlobals[i].Size = (int)DataLoader.getValAtAddress(bglob_data, add_ptr + 2, 16);
                    bGlobals[i].Globals = new int[bGlobals[i].Size];
                    add_ptr = add_ptr + 4;
                }
            }
        }
        else
        {
            int NoOfSlots = 0;//Assumes the same no of slots that is in the babglobs is in bglobals.
            if (DataLoader.ReadStreamFile(Loader.BasePath + "DATA" + sep + "BABGLOBS.DAT", out bglob_data))
            {
                NoOfSlots = bglob_data.GetUpperBound(0) / 4;
                NoOfSlots++;
            }
            if (DataLoader.ReadStreamFile(Loader.BasePath + "SAVE" + SlotNo + sep + "BGLOBALS.DAT", out bglob_data))
            {
                //int NoOfSlots = bglob_data.GetUpperBound(0)/4;
                int add_ptr = 0;
                bGlobals = new bablGlobal[NoOfSlots];
                for (int i = 0; i < NoOfSlots; i++)
                {

                    bGlobals[i].ConversationNo = (int)DataLoader.getValAtAddress(bglob_data, add_ptr, 16);
                    bGlobals[i].Size = (int)DataLoader.getValAtAddress(bglob_data, add_ptr + 2, 16);
                    bGlobals[i].Globals = new int[bGlobals[i].Size];
                    add_ptr += 4;
                    for (int g = 0; g < bGlobals[i].Size; g++)
                    {
                        bGlobals[i].Globals[g] = (int)DataLoader.getValAtAddress(bglob_data, add_ptr, 16);
                        if (bGlobals[i].Globals[g] == 65535)
                        {
                            bGlobals[i].Globals[g] = 0;
                        }
                        add_ptr = add_ptr + 2;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Writes the BGlobals data to file
    /// </summary>
    /// <param name="SlotNo">Slot no.</param>
    public void WriteBGlobals(int SlotNo)
    {
        int fileSize = 0;
        for (int c = 0; c <= bGlobals.GetUpperBound(0); c++)
        {
            fileSize += 4;  //No and size
            fileSize += bGlobals[c].Size * 2;
        }
        //Create an output byte array
        Byte[] output = new byte[fileSize];
        int add_ptr = 0;
        for (int c = 0; c <= bGlobals.GetUpperBound(0); c++)
        {
            //Write Slot No
            output[add_ptr] = (byte)(bGlobals[c].ConversationNo & 0xff);
            output[add_ptr + 1] = (byte)((bGlobals[c].ConversationNo >> 8) & 0xff);
            //Write Size
            output[add_ptr + 2] = (byte)(bGlobals[c].Size & 0xff);
            output[add_ptr + 3] = (byte)((bGlobals[c].Size >> 8) & 0xff);
            add_ptr = add_ptr + 4;
            for (int g = 0; g <= bGlobals[c].Globals.GetUpperBound(0); g++)
            {
                output[add_ptr] = (byte)(bGlobals[c].Globals[g] & 0xff);
                output[add_ptr + 1] = (byte)((bGlobals[c].Globals[g] >> 8) & 0xff);
                add_ptr += 2;
            }
        }
        File.WriteAllBytes(Loader.BasePath + "SAVE" + SlotNo + sep + "BGLOBALS.DAT", output);

    }

    /// <summary>
    /// Copies the data from the cached lev ark file to a new array;
    /// </summary>
    /// <returns>The data.</returns>
    /// <param name="address">Address.</param>
    /// <param name="length">Length.</param>
    public char[] CopyData(long address, long length)
    {
        char[] DataToCopy = new char[length];

        for (int i = 0; i <= DataToCopy.GetUpperBound(0); i++)
        {
            DataToCopy[i] = lev_ark_file_data[address + i];
        }
        return DataToCopy;
    }

    /// <summary>
    /// Switchs to a Terra nova map.
    /// </summary>
    /// <param name="levelFileName">Level file name.</param>
    public void SwitchTNovaMap(string levelFileName)
    {
        string path;
        if (levelFileName == "")
        {
            path = NovaLevelSelect.MapSelected;
        }
        else
        {
            path = levelFileName;//Loader.BasePath + "MAPS\\roadmap.res";		
        }

        char[] archive_ark;
        if (DataLoader.ReadStreamFile(path, out archive_ark))
        {
            DataLoader.Chunk lev_ark;
            if (!DataLoader.LoadChunk(archive_ark, 86, out lev_ark))
            {
                return;
            }
            UWCharacter.Instance.playerCam.GetComponent<Light>().range = 200f;
            UWCharacter.Instance.playerCam.farClipPlane = 3000f;
            UWCharacter.Instance.playerCam.renderingPath = RenderingPath.DeferredShading;
            TileMapRenderer.RenderTNovaMap(TNovaLevelModel.transform, lev_ark.data);

        }
    }



    /// <summary>
    /// Loads the config file.
    /// </summary>
    /// <returns><c>true</c>, if config file was loaded, <c>false</c> otherwise.</returns>
    bool LoadConfigFile()
    {
        string fileName = Application.dataPath + sep + ".." + sep + "config.ini";
        if (File.Exists(fileName))
        {
            string line;
            StreamReader fileReader = new StreamReader(fileName, Encoding.Default);
            //string PreviousKey="";
            //string PreviousValue="";
            using (fileReader)
            {
                // While there's lines left in the text file, do this:
                do
                {
                    line = fileReader.ReadLine();
                    if (line != null)
                    {
                        if (line.Length > 1)
                        {
                            if ((line.Substring(1, 1) != ";") && (line.Contains("=")))//Is not a commment and contains a param
                            {
                                string[] entries = line.Split('=');
                                //int val = 0;
                                //string pathfound="";
                                KeyCode keyCodeToUse;
                                KeyBindings.instance.chartoKeycode.TryGetValue(entries[1].ToLower(), out keyCodeToUse);

                                switch (entries[0].ToUpper())
                                {
                                    case "MOUSEX"://Mouse sensitivity X
                                        {
                                            float val = 15f;
                                            if (float.TryParse(entries[1], out val))
                                            {
                                                MouseX.sensitivityX = val;
                                            }
                                            break;
                                        }
                                    case "MOUSEY"://Mouse sensitivity Y
                                        {
                                            float val = 15f;
                                            if (float.TryParse(entries[1], out val))
                                            {
                                                MouseY.sensitivityY = val;
                                            }
                                            break;
                                        }
                                    case "PATH_UW0":
                                        {
                                            path_uw0 = entries[1];
                                            break;
                                        }
                                    case "PATH_UW1":
                                        {
                                            path_uw1 = entries[1];
                                            break;
                                        }
                                    case "PATH_UW2":
                                        {
                                            path_uw2 = entries[1];
                                            break;
                                        }
                                    case "PATH_SHOCK":
                                        {
                                            path_shock = entries[1];
                                            break;
                                        }
                                    case "PATH_TNOVA":
                                        {
                                            path_tnova = entries[1];
                                            break;
                                        }

                                    case "FLYUP":
                                        KeyBindings.instance.FlyUp = keyCodeToUse; break;
                                    case "FLYDOWN":
                                        KeyBindings.instance.FlyDown = keyCodeToUse; break;
                                    case "TOGGLEMOUSELOOK":
                                        KeyBindings.instance.ToggleMouseLook = keyCodeToUse; break;
                                    case "TOGGLEFULLSCREEN":
                                        KeyBindings.instance.ToggleFullScreen = keyCodeToUse; break;
                                    case "INTERACTIONOPTIONS":
                                        KeyBindings.instance.InteractionOptions = keyCodeToUse; break;
                                    case "INTERACTIONTALK":
                                        KeyBindings.instance.InteractionTalk = keyCodeToUse; break;
                                    case "INTERACTIONPICKUP":
                                        KeyBindings.instance.InteractionPickup = keyCodeToUse; break;
                                    case "INTERACTIONLOOK":
                                        KeyBindings.instance.InteractionLook = keyCodeToUse; break;
                                    case "INTERACTIONATTACK":
                                        KeyBindings.instance.InteractionAttack = keyCodeToUse; break;
                                    case "INTERACTIONUSE":
                                        KeyBindings.instance.InteractionUse = keyCodeToUse; break;
                                    case "CASTSPELL":
                                        KeyBindings.instance.CastSpell = keyCodeToUse; break;
                                    case "TRACKSKILL":
                                        KeyBindings.instance.TrackSkill = keyCodeToUse; break;


                                    case "DEFAULTLIGHTLEVEL":
                                        {
                                            float lightlevel = 16f;
                                            if (float.TryParse(entries[1], out lightlevel))
                                            {
                                                LightSource.BaseBrightness = lightlevel;
                                            }
                                            break;
                                        }

                                    case "FOV":
                                        {
                                            float fov = 75f;
                                            if (float.TryParse(entries[1], out fov))
                                            {
                                                Camera.main.fieldOfView = fov;
                                            }
                                            break;
                                        }
                                    case "INFINITEMANA":
                                        {
                                            Magic.InfiniteMana = (entries[1] == "1");
                                            break;
                                        }

                                    case "GODMODE":
                                        {
                                            UWCharacter.Invincible = (entries[1] == "1");
                                            break;
                                        }

                                    case "CONTEXTUIENABLED":
                                        {
                                            WindowDetectUW.ContextUIEnabled = (entries[1] == "1");
                                            break;
                                        }

                                    case "UW1_SOUNDBANK":
                                        {
                                            MusicController.UW1Path = entries[1];
                                            break;
                                        }
                                    case "UW2_SOUNDBANK":
                                        {
                                            MusicController.UW2Path = entries[1];
                                            break;
                                        }
                                    case "GENREPORT":
                                        {
                                            CreateReports = (entries[1] == "1");
                                            break;
                                        }
                                    case "SHOWINUSE"://only show inuse objects in reports
                                        {
                                            ShowOnlyInUse = (entries[1] == "1");
                                            break;
                                        }
                                }
                            }
                        }

                    }
                }
                while (line != null);
                fileReader.Close();
                return true;
            }
        }
        else
        {
            return false;
        }
    }



    void CreateObjectReport(ObjectLoaderInfo[] objList)
    {
        StreamWriter writer = new StreamWriter(Application.dataPath + "//..//_objectreport.xml");
        writer.WriteLine("<ObjectReport>");
        for (int o = 0; o <= objList.GetUpperBound(0); o++)
        {
            if (((objList[o].InUseFlag == 0) && (!ShowOnlyInUse)) || (objList[o].InUseFlag == 1))
            {
                writer.WriteLine("\t<Object>");
                writer.WriteLine("\t\t<ObjectName>" + ObjectLoader.UniqueObjectNameEditor(objList[o]) + "</ObjectName>");
                writer.WriteLine("\t\t<Index>" + o + "</Index>");
                writer.WriteLine("\t\t<Address>" + objList[o].address + "</Address>");
                writer.WriteLine("\t\t<StaticProperties>");
                writer.WriteLine("\t\t\t<ItemID>" + objList[o].item_id + "</ItemID>");
                writer.WriteLine("\t\t\t<InUse>" + objList[o].InUseFlag + "</InUse>");
                writer.WriteLine("\t\t\t<Flags>" + objList[o].flags + "</Flags>");
                writer.WriteLine("\t\t\t<Enchant>" + objList[o].enchantment + "</Enchant>");
                writer.WriteLine("\t\t\t<DoorDir>" + objList[o].doordir + "</DoorDir>");
                writer.WriteLine("\t\t\t<Invis>" + objList[o].invis + "</Invis>");
                writer.WriteLine("\t\t\t<IsQuant>" + objList[o].is_quant + "</IsQuant>");
                writer.WriteLine("\t\t\t<Texture>" + objList[o].texture + "</Texture>");
                writer.WriteLine("\t\t\t<Position>");
                writer.WriteLine("\t\t\t\t<tileX>" + objList[o].tileX + "</tileX>");
                writer.WriteLine("\t\t\t\t<tileY>" + objList[o].tileY + "</tileY>");
                writer.WriteLine("\t\t\t\t<xpos>" + objList[o].x + "</xpos>");
                writer.WriteLine("\t\t\t\t<ypos>" + objList[o].y + "</ypos>");
                writer.WriteLine("\t\t\t\t<zpos>" + objList[o].zpos + "</zpos>");
                writer.WriteLine("\t\t\t</Position>");
                writer.WriteLine("\t\t\t<Quality>" + objList[o].quality + "</Quality>");
                writer.WriteLine("\t\t\t<Next>" + objList[o].next + "</Next>");
                writer.WriteLine("\t\t\t<Owner>" + objList[o].owner + "</Owner>");
                writer.WriteLine("\t\t\t<Link>" + objList[o].link + "</Link>");
                writer.WriteLine("\t\t</StaticProperties>");
                if (o < 256)
                {//mobile info
                    writer.WriteLine("\t\t<MobileProperties>");
                    writer.WriteLine("\t\t\t<npc_hp>" + objList[o].npc_hp + "</npc_hp>");
                    writer.WriteLine("\t\t\t<ProjectileHeadingMinor>" + objList[o].ProjectileHeadingMinor + "</ProjectileHeadingMinor>");
                    writer.WriteLine("\t\t\t<ProjectileHeadingMajor>" + objList[o].ProjectileHeadingMajor + "</ProjectileHeadingMajor>");
                    writer.WriteLine("\t\t\t<MobileUnk01>" + objList[o].MobileUnk01 + "</MobileUnk01>");
                    writer.WriteLine("\t\t\t<npc_goal>" + objList[o].npc_goal + "</npc_goal>");
                    writer.WriteLine("\t\t\t<npc_gtarg>" + objList[o].npc_gtarg + "</npc_gtarg>");
                    writer.WriteLine("\t\t\t<MobileUnk02>" + objList[o].MobileUnk02 + "</MobileUnk02>");
                    writer.WriteLine("\t\t\t<npc_level>" + objList[o].npc_level + "</npc_level>");
                    writer.WriteLine("\t\t\t<MobileUnk03>" + objList[o].MobileUnk03 + "</MobileUnk03>");
                    writer.WriteLine("\t\t\t<MobileUnk04>" + objList[o].MobileUnk04 + "</MobileUnk04>");
                    writer.WriteLine("\t\t\t<npc_talkedto>" + objList[o].npc_talkedto + "</npc_talkedto>");
                    writer.WriteLine("\t\t\t<npc_attitude>" + objList[o].npc_attitude + "</npc_attitude>");
                    writer.WriteLine("\t\t\t<MobileUnk05>" + objList[o].MobileUnk05 + "</MobileUnk05>");
                    writer.WriteLine("\t\t\t<npc_height>" + objList[o].npc_height + "</npc_height>");
                    writer.WriteLine("\t\t\t<MobileUnk06>" + objList[o].MobileUnk06 + "</MobileUnk06>");
                    writer.WriteLine("\t\t\t<MobileUnk07>" + objList[o].MobileUnk07 + "</MobileUnk07>");
                    writer.WriteLine("\t\t\t<MobileUnk08>" + objList[o].MobileUnk08 + "</MobileUnk08>");
                    writer.WriteLine("\t\t\t<MobileUnk09>" + objList[o].MobileUnk09 + "</MobileUnk09>");
                    writer.WriteLine("\t\t\t<Projectile_Speed>" + objList[o].Projectile_Speed + "</Projectile_Speed>");
                    writer.WriteLine("\t\t\t<Projectile_Pitch>" + objList[o].Projectile_Pitch + "</Projectile_Pitch>");
                    writer.WriteLine("\t\t\t<Projectile_Sign>" + objList[o].Projectile_Sign + "</Projectile_Sign>");
                    writer.WriteLine("\t\t\t<npc_voidanim>" + objList[o].npc_voidanim + "</npc_voidanim>");
                    writer.WriteLine("\t\t\t<MobileUnk11>" + objList[o].MobileUnk11 + "</MobileUnk11>");
                    writer.WriteLine("\t\t\t<MobileUnk12>" + objList[o].MobileUnk12 + "</MobileUnk12>");
                    writer.WriteLine("\t\t\t<npc_yhome>" + objList[o].npc_yhome + "</npc_yhome>");
                    writer.WriteLine("\t\t\t<npc_xhome>" + objList[o].npc_xhome + "</npc_xhome>");
                    writer.WriteLine("\t\t\t<npc_heading>" + objList[o].npc_heading + "</npc_heading>");
                    writer.WriteLine("\t\t\t<MobileUnk13>" + objList[o].MobileUnk13 + "</MobileUnk13>");
                    writer.WriteLine("\t\t\t<npc_hunger>" + objList[o].npc_hunger + "</npc_hunger>");
                    writer.WriteLine("\t\t\t<MobileUnk14>" + objList[o].MobileUnk14 + "</MobileUnk14>");
                    writer.WriteLine("\t\t\t<npc_whoami>" + objList[o].npc_whoami + "</npc_whoami>");
                    writer.WriteLine("\t\t</MobileProperties>");
                }
                writer.WriteLine("\t</Object>");
            }

        }

        writer.WriteLine("</ObjectReport>");
        writer.Close();
    }

}