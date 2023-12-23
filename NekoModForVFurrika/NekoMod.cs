using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using MelonLoader;
using HarmonyLib;
using UnityEngine.UI;
using UnityEngine.Events;

namespace NekoModForVFurrika
{
    public class NekoMod : MelonMod
    {
        public static NekoMod Instance;

        private List<Transform> TitleHeads = new List<Transform>();
        private bool TitleSpinMode = false;

        private GameObject NekoMenu;
        private bool NekoMenuGenerated = false;

        // private bool isLeaderboardDisabled = false;

        private List<Sprite> CustomHeads = new List<Sprite>();
        private Sprite EvolutionBG;
        private List<Vector3> EvolutionHeadPositions = new List<Vector3>();

        public override void OnInitializeMelon()
        {
            Instance = this;
            LoadCustomHeads();
            
            // Setting up all the custom heads positions on the in-game Evolution panel
            EvolutionBG = GetCustomSprite($"{Application.streamingAssetsPath}/HUD/Evolution.png");
            EvolutionHeadPositions.Add(new Vector3(-2.0697f, 3.0048f, 0));
            EvolutionHeadPositions.Add(new Vector3(0, 2.9455f, 0));
            EvolutionHeadPositions.Add(new Vector3(2.0509f, 2.9745f, 0));
            EvolutionHeadPositions.Add(new Vector3(-2.0473f, 0.68f, 0));
            EvolutionHeadPositions.Add(new Vector3(0, 0.8255f, 0));
            EvolutionHeadPositions.Add(new Vector3(2.1109f, 0.78f, 0));
            EvolutionHeadPositions.Add(new Vector3(-2.1527f, -1.6327f, 0));
            EvolutionHeadPositions.Add(new Vector3(0, -1.5091f, 0));
            EvolutionHeadPositions.Add(new Vector3(1.9909f, -1.4655f, 0));
            EvolutionHeadPositions.Add(new Vector3(-1.8836f, -3.9927f, 0));
            EvolutionHeadPositions.Add(new Vector3(1.2455f, -3.8345f, 0));
        }

        private void LoadCustomHeads()
        {
            for (int i = 0; i < 11; i++)
            {
                CustomHeads.Add(GetCustomSprite($"{Application.streamingAssetsPath}/CustomHeads/{i}.png"));
            }
        }

        private void PatchCustomTitleHeads()
        {
            // Replacing all the heads on the title screen by custom sprites
            foreach (var head in TitleHeads)
            {
                int sprite = 0;
                if (head.name.Contains("ANDRE")) sprite = 1;
                if (head.name.Contains("starboi")) sprite = 2;
                if (head.name.Contains("skai")) sprite = 3;
                if (head.name.Contains("mixi")) sprite = 4;
                if (head.name.Contains("allie")) sprite = 5;
                if (head.name.Contains("dooper")) sprite = 6;
                if (head.name.Contains("chai")) sprite = 7;
                if (head.name.Contains("JT")) sprite = 8;
                if (head.name.Contains("chester")) sprite = 9;
                if (head.name.Contains("dingo")) sprite = 10;

                if (CustomHeads[sprite] != null) head.GetComponent<SpriteRenderer>().sprite = CustomHeads[sprite];
            }
        }

        private void CreateModMenu()
        {
            // Pretty messy way to create the mod menu, but I kinda rushed it to keep a style consistency with the base game.
            if (NekoMenu == null)
            {
                var MenuRoot = new GameObject("NekoModMenu");
                MenuRoot.transform.SetParent(GameManager.Instance.GetComponentInChildren<CanvasScaler>().transform);
                NekoMenu = MenuRoot.gameObject;
                MenuRoot.transform.localPosition = Vector3.zero;
                MenuRoot.transform.localScale = Vector3.one;
                var bg = MenuRoot.gameObject.AddComponent<Image>();
                bg.color = new Color(0, 0, 0, .99f);
                bg.GetComponent<RectTransform>().sizeDelta = new Vector2(2000, 2000);
            }

            var templateButton = GameObject.Find("Canvas/PLAY");
            
            var toggleNekoMenuEvent = new UnityEvent();
            toggleNekoMenuEvent.AddListener(() => { ToggleNekoMenu(); });
            var nekoMenuButton = CreateNewButton(templateButton, "Neko Mod\n<size=50%>v1.1</size>", toggleNekoMenuEvent, true);
            nekoMenuButton.GetComponent<RectTransform>().anchoredPosition += new Vector2(0, 150);

            if (!NekoMenuGenerated)
            {
                var closeMenuButton = CreateNewButton(templateButton, "Close", toggleNekoMenuEvent);
                closeMenuButton.transform.SetParent(NekoMenu.transform);
                closeMenuButton.GetComponent<RectTransform>().anchoredPosition += new Vector2(-725, -250);

                var aboutMod = CreateNewButton(templateButton, "About the mod\n\n<size=50%>This mod has been developed by\nRaphael Neko using MelonLoader.</size>", null);
                aboutMod.GetComponent<Button>().enabled = false;
                aboutMod.transform.SetParent(NekoMenu.transform);
                var aboutRT = aboutMod.GetComponent<RectTransform>();
                aboutRT.sizeDelta = new Vector2(1050, 350);
                aboutRT.anchoredPosition = new Vector2(0, 250);


                var howToEvent = new UnityEvent();
                howToEvent.AddListener(() => { Application.OpenURL("https://raphaelneko.com/vfurrika-mod#tutorial"); });
                var howTo = CreateNewButton(templateButton, "How to set up custom heads", howToEvent, true);
                howTo.transform.SetParent(NekoMenu.transform);
                howTo.GetComponent<RectTransform>().sizeDelta = new Vector2(1050, 100);

                var openCustomFolderEvent = new UnityEvent();
                openCustomFolderEvent.AddListener(() => { Application.OpenURL($"{Application.streamingAssetsPath}/CustomHeads/"); });
                var openCustomFolder = CreateNewButton(templateButton, "Open custom folder", openCustomFolderEvent, true);
                openCustomFolder.transform.SetParent(NekoMenu.transform);
                openCustomFolder.GetComponent<RectTransform>().sizeDelta = new Vector2(1050, 75);
                openCustomFolder.GetComponent<RectTransform>().anchoredPosition += new Vector2(0, -110);

                NekoMenu.SetActive(false);
                NekoMenuGenerated = true;
            }
        }

        private GameObject CreateNewButton(GameObject buttonTemplate, string buttonText, UnityEvent onClick, bool autoSize = false)
        {
            var newButton = GameObject.Instantiate(buttonTemplate, buttonTemplate.transform.parent);
            var text = newButton.GetComponentInChildren<Text>();
            text.text = buttonText;
            text.lineSpacing = .4f;
            text.resizeTextForBestFit = autoSize;
            text.supportRichText = true;
            newButton.GetComponent<Button>().onClick = new Button.ButtonClickedEvent();
            newButton.GetComponent<Button>().onClick.AddListener(()=> { onClick.Invoke(); });
            newButton.gameObject.name = buttonText.ToUpper();
            return newButton;
        }

        private void ToggleNekoMenu()
        {
            if (NekoMenu == null) return;
            NekoMenu.SetActive(!NekoMenu.activeSelf);
        }


        public static Sprite GetLoadedHead(int id)
        {
            return Instance.CustomHeads[id];
        }

        private Sprite GetCustomSprite(string path)
        {
            if (!System.IO.File.Exists(path)) return null;
            byte[] pngBytes = System.IO.File.ReadAllBytes(path);
            Texture2D tex = new Texture2D(2, 2);
            ImageConversion.LoadImage(tex, pngBytes);
            Sprite fromTex = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(.5f, .5f), 100);
            LoggerInstance.Msg($"Custom sprite loaded at: {path}");
            return fromTex;
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            // LoggerInstance.Msg(System.ConsoleColor.DarkGray, $"Scene {sceneName} with build index {buildIndex} has been loaded!");

            #region TITLE SCREEN

            if (buildIndex == 0)
            {
                var TitleHeadsParent = GameObject.Find("Heads");
                foreach (var head in TitleHeadsParent.GetComponentsInChildren<Transform>())
                {
                    if (head.name != "Heads") TitleHeads.Add(head);
                }
                PatchCustomTitleHeads();
                CreateModMenu();
            }
            else
            {
                TitleHeads.Clear();
            }

            #endregion


            #region GAME

            if (buildIndex == 1)
            {
                var EvolutionPanel = GameObject.Find("UI/Evolution").transform;
                EvolutionPanel.GetComponent<SpriteRenderer>().sprite = EvolutionBG;
                for (int i = 0; i < 11; i++)
                {
                    var head = new GameObject($"Head{i}").transform;
                    head.SetParent(EvolutionPanel);
                    head.localPosition = EvolutionHeadPositions[i];
                    var customSprite = GetLoadedHead(i);
                    if (customSprite == null) customSprite = GetCustomSprite($"{Application.streamingAssetsPath}/HUD/OG Heads/{i}.png");
                    var spriteRenderer = head.gameObject.AddComponent<SpriteRenderer>();
                    spriteRenderer.sprite = customSprite;
                    spriteRenderer.sortingOrder = 1;
                    head.localScale = Vector3.one * (i == 10 ? .25f : .2f);
                }
            }

            #endregion


            #region LEADERBOARD
            /*
            if (buildIndex == 2)
            {
                
            }
            */

            #endregion
        }

        /*
        public override void OnUpdate()
        {
            if (SceneManager.GetActiveScene().buildIndex == 0 && TitleHeads.Count > 0 && TitleSpinMode) TitleHeadsSpin();
        }
        */
        

        private void TitleHeadsSpin()
        {
            foreach (var head in TitleHeads) head.Rotate(Vector3.forward * (1500 * Time.deltaTime));
        }
    }

    #region Existing Script Patches

    [HarmonyPatch(typeof(BubbleObject), "SetCurrentFruit")]
    public static class PatchPawFruit
    {
        private static void Postfix(BubbleObject __instance)
        {
            var img = __instance.GetComponentInChildren<FruitController>().GetComponent<SpriteRenderer>();
            int sprite = -1;
            if (img.sprite.name.Contains("LUIROI")) sprite = 0;
            else if (img.sprite.name.Contains("ANDRE")) sprite = 1;
            else if (img.sprite.name.Contains("Star")) sprite = 2;
            else if (img.sprite.name.Contains("Skai")) sprite = 3;
            else if (img.sprite.name.Contains("MIXI")) sprite = 4;
            else if (img.sprite.name.Contains("S3A")) sprite = 5;
            else if (img.sprite.name.Contains("Dooper")) sprite = 6;
            else if (img.sprite.name.Contains("Chai")) sprite = 7;
            else if (img.sprite.name.Contains("JT")) sprite = 8;
            else if (img.sprite.name.Contains("Chester")) sprite = 9;
            else if (img.sprite.name.Contains("dingo")) sprite = 10;

            if (sprite == -1) return;
            var customSprite = NekoMod.GetLoadedHead(sprite);
            if (customSprite != null) img.sprite = customSprite;
        }
    }
    
    [HarmonyPatch(typeof(BubbleObject), "SetNextFruit")]
    public static class PatchNextFruitUI
    {
        private static void Postfix(BubbleObject __instance)
        {
            var img = GameObject.Find("SCORE/Next Fruit Sprite").GetComponent<Image>();
            int sprite = -1;
            if (img.sprite.name.Contains("LUIROI")) sprite = 0;
            else if (img.sprite.name.Contains("ANDRE")) sprite = 1;
            else if (img.sprite.name.Contains("Star")) sprite = 2;
            else if (img.sprite.name.Contains("Skai")) sprite = 3;
            else if (img.sprite.name.Contains("MIXI")) sprite = 4;
            else if (img.sprite.name.Contains("S3A")) sprite = 5;
            else if (img.sprite.name.Contains("Dooper")) sprite = 6;
            else if (img.sprite.name.Contains("Chai")) sprite = 7;
            else if (img.sprite.name.Contains("JT")) sprite = 8;
            else if (img.sprite.name.Contains("Chester")) sprite = 9;
            else if (img.sprite.name.Contains("dingo")) sprite = 10;

            if (sprite == -1) return;
            var customSprite = NekoMod.GetLoadedHead(sprite);
            if (customSprite != null) img.sprite = customSprite;
        }
    }

    [HarmonyPatch(typeof(FruitController), "SpawnFruit")]
    public static class PatchNewFruits
    {
        private static void Postfix(FruitController __instance)
        {
            var img = __instance.GetComponent<SpriteRenderer>();
            int sprite = -1;
            if (img.sprite.name.Contains("LUIROI")) sprite = 0;
            else if (img.sprite.name.Contains("ANDRE")) sprite = 1;
            else if (img.sprite.name.Contains("Star")) sprite = 2;
            else if (img.sprite.name.Contains("Skai")) sprite = 3;
            else if (img.sprite.name.Contains("MIXI")) sprite = 4;
            else if (img.sprite.name.Contains("S3A")) sprite = 5;
            else if (img.sprite.name.Contains("Dooper")) sprite = 6;
            else if (img.sprite.name.Contains("Chai")) sprite = 7;
            else if (img.sprite.name.Contains("JT")) sprite = 8;
            else if (img.sprite.name.Contains("Chester")) sprite = 9;
            else if (img.sprite.name.Contains("dingo")) sprite = 10;

            if (sprite == -1) return;
            var customSprite = NekoMod.GetLoadedHead(sprite);
            if (customSprite != null) img.sprite = customSprite;
        }
    }

    #endregion
}