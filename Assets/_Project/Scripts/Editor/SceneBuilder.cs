#if UNITY_EDITOR
using System.IO;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using SpaceInvaders.Core;
using SpaceInvaders.Enemies;
using SpaceInvaders.Player;
using SpaceInvaders.Projectiles;
using SpaceInvaders.UI;

namespace SpaceInvaders.EditorTools
{
    // Tools > Space Invaders > Build Everything
    public static class SceneBuilder
    {
        private const string Root = "Assets/_Project";
        private const string PrefabsDir = Root + "/Prefabs";
        private const string SpritesDir = Root + "/Sprites";
        private const string ScenesDir = Root + "/Scenes";
        private const string ScenePath = ScenesDir + "/Main.unity";

        private const string PlayerLayer = "Player";
        private const string EnemyLayer = "Enemy";

        [MenuItem("Tools/Space Invaders/Build Everything")]
        public static void BuildEverything()
        {
            EnsureFolders();
            EnsureLayer(PlayerLayer);
            EnsureLayer(EnemyLayer);

            Sprite square = CreateSquareSprite();

            Projectile playerBullet = CreateBulletPrefab("PlayerBullet", square, Color.white, 12f, LayerMask.GetMask(EnemyLayer));
            Projectile enemyBullet = CreateBulletPrefab("EnemyBullet", square, Color.magenta, 5f, LayerMask.GetMask(PlayerLayer));

            Enemy[] enemies =
            {
                CreateEnemyPrefab("Enemy_Red", square, Color.red, 10, 1, enemyBullet),
                CreateEnemyPrefab("Enemy_Orange", square, new Color(1f, 0.6f, 0f), 10, 1, enemyBullet),
                CreateEnemyPrefab("Enemy_Yellow", square, Color.yellow, 10, 1, enemyBullet),
                CreateEnemyPrefab("Enemy_Cyan", square, Color.cyan, 10, 1, enemyBullet),
            };

            Image healthIcon = CreateHealthIconPrefab();

            BuildScene(square, playerBullet, enemies, healthIcon);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Space Invaders: сцена Main собрана. Нажмите Play.");
        }

        // ---------- папки, слои, спрайт ----------

        private static void EnsureFolders()
        {
            foreach (string dir in new[] { PrefabsDir, SpritesDir, ScenesDir })
                Directory.CreateDirectory(dir);

            AssetDatabase.Refresh();
        }

        private static void EnsureLayer(string layerName)
        {
            if (LayerMask.NameToLayer(layerName) != -1) return;

            var tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty layers = tagManager.FindProperty("layers");

            for (int i = 8; i < layers.arraySize; i++)
            {
                SerializedProperty slot = layers.GetArrayElementAtIndex(i);
                if (string.IsNullOrEmpty(slot.stringValue))
                {
                    slot.stringValue = layerName;
                    tagManager.ApplyModifiedProperties();
                    return;
                }
            }

            Debug.LogError($"Нет свободного слоя для {layerName}");
        }

        private static Sprite CreateSquareSprite()
        {
            string path = SpritesDir + "/Square.png";

            if (!File.Exists(path))
            {
                var texture = new Texture2D(4, 4);
                var pixels = new Color[16];
                for (int i = 0; i < pixels.Length; i++) pixels[i] = Color.white;
                texture.SetPixels(pixels);
                texture.Apply();

                File.WriteAllBytes(path, texture.EncodeToPNG());
                Object.DestroyImmediate(texture);
                AssetDatabase.ImportAsset(path);

                var importer = (TextureImporter)AssetImporter.GetAtPath(path);
                importer.textureType = TextureImporterType.Sprite;
                importer.spritePixelsPerUnit = 4f;
                importer.filterMode = FilterMode.Point;
                importer.SaveAndReimport();
            }

            return AssetDatabase.LoadAssetAtPath<Sprite>(path);
        }

        // ---------- префабы ----------

        private static GameObject NewSpriteObject(string name, Sprite sprite, Color color, Vector2 size, int layer)
        {
            var go = new GameObject(name);
            go.layer = layer;
            go.transform.localScale = new Vector3(size.x, size.y, 1f);

            var renderer = go.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.color = color;

            var body = go.AddComponent<Rigidbody2D>();
            body.bodyType = RigidbodyType2D.Kinematic;

            var collider = go.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;

            return go;
        }

        private static T SavePrefab<T>(GameObject go, string name) where T : Component
        {
            string path = $"{PrefabsDir}/{name}.prefab";
            GameObject saved = PrefabUtility.SaveAsPrefabAsset(go, path);
            Object.DestroyImmediate(go);
            return saved.GetComponent<T>();
        }

        private static Projectile CreateBulletPrefab(string name, Sprite sprite, Color color, float speed, LayerMask targets)
        {
            GameObject go = NewSpriteObject(name, sprite, color, new Vector2(0.15f, 0.4f), 0);
            var projectile = go.AddComponent<Projectile>();

            var so = new SerializedObject(projectile);
            so.FindProperty("speed").floatValue = speed;
            so.FindProperty("damage").intValue = 1;
            so.FindProperty("targetLayers").intValue = targets.value;
            so.ApplyModifiedPropertiesWithoutUndo();

            return SavePrefab<Projectile>(go, name);
        }

        private static Enemy CreateEnemyPrefab(string name, Sprite sprite, Color color, int score, int hits, Projectile bullet)
        {
            GameObject go = NewSpriteObject(name, sprite, color, new Vector2(0.8f, 0.5f), LayerMask.NameToLayer(EnemyLayer));

            var health = go.AddComponent<Health>();
            var hso = new SerializedObject(health);
            hso.FindProperty("maxHealth").intValue = hits;
            hso.ApplyModifiedPropertiesWithoutUndo();

            var enemy = go.AddComponent<Enemy>();
            var eso = new SerializedObject(enemy);
            eso.FindProperty("scoreReward").intValue = score;
            eso.FindProperty("bulletPrefab").objectReferenceValue = bullet;
            eso.ApplyModifiedPropertiesWithoutUndo();

            return SavePrefab<Enemy>(go, name);
        }

        private static Image CreateHealthIconPrefab()
        {
            var go = new GameObject("HealthIcon", typeof(RectTransform), typeof(Image));
            go.GetComponent<Image>().color = Color.green;
            go.GetComponent<RectTransform>().sizeDelta = new Vector2(40f, 40f);

            return SavePrefab<Image>(go, "HealthIcon");
        }

        // ---------- сцена ----------

        private static void BuildScene(Sprite square, Projectile playerBullet, Enemy[] enemyPrefabs, Image healthIcon)
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            // камера
            var cameraGo = new GameObject("Main Camera") { tag = "MainCamera" };
            var camera = cameraGo.AddComponent<Camera>();
            cameraGo.AddComponent<AudioListener>();
            camera.orthographic = true;
            camera.orthographicSize = 6f;
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = Color.black;
            cameraGo.transform.position = new Vector3(0f, 0f, -10f);

            // системные объекты
            var systems = new GameObject("Systems");
            var input = new GameObject("Input").AddComponent<InputReader>();
            var game = new GameObject("Game").AddComponent<GameController>();
            var grid = new GameObject("EnemyGrid").AddComponent<EnemyGrid>();
            var enemyShooter = new GameObject("EnemyShooter").AddComponent<EnemyShooter>();
            input.transform.SetParent(systems.transform);
            game.transform.SetParent(systems.transform);
            grid.transform.SetParent(systems.transform);
            enemyShooter.transform.SetParent(systems.transform);

            var gridSo = new SerializedObject(grid);
            SerializedProperty rowPrefabs = gridSo.FindProperty("rowPrefabs");
            rowPrefabs.arraySize = enemyPrefabs.Length;
            for (int i = 0; i < enemyPrefabs.Length; i++)
                rowPrefabs.GetArrayElementAtIndex(i).objectReferenceValue = enemyPrefabs[i];
            gridSo.ApplyModifiedPropertiesWithoutUndo();

            // игрок
            GameObject playerGo = NewSpriteObject("Player", square, Color.green, new Vector2(1.2f, 0.5f), LayerMask.NameToLayer(PlayerLayer));
            playerGo.transform.position = new Vector3(0f, -4.5f, 0f);

            var playerHealthCore = playerGo.AddComponent<Health>();
            var phSo = new SerializedObject(playerHealthCore);
            phSo.FindProperty("maxHealth").intValue = 3;
            phSo.ApplyModifiedPropertiesWithoutUndo();

            var playerHealth = playerGo.AddComponent<PlayerHealth>();
            var movement = playerGo.AddComponent<PlayerMovement>();
            var shooter = playerGo.AddComponent<PlayerShooter>();

            var muzzle = new GameObject("Muzzle").transform;
            muzzle.SetParent(playerGo.transform);
            muzzle.localPosition = new Vector3(0f, 0.8f, 0f);

            var shooterSo = new SerializedObject(shooter);
            shooterSo.FindProperty("bulletPrefab").objectReferenceValue = playerBullet;
            shooterSo.FindProperty("muzzle").objectReferenceValue = muzzle;
            shooterSo.ApplyModifiedPropertiesWithoutUndo();

            // UI
            var canvasGo = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            var canvas = canvasGo.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasGo.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);

            // счёт: левый верхний угол
            TMP_Text scoreText = CreateText("ScoreText", canvasGo.transform, "Score: 0", 48, TextAlignmentOptions.TopLeft);
            Anchor(scoreText.rectTransform, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(30f, -20f), new Vector2(600f, 80f));
            var scoreView = scoreText.gameObject.AddComponent<ScoreView>();
            SetRef(scoreView, "label", scoreText);

            // здоровье: низ экрана по центру
            var healthGo = new GameObject("HealthContainer", typeof(RectTransform), typeof(HorizontalLayoutGroup));
            healthGo.transform.SetParent(canvasGo.transform, false);
            var healthRect = healthGo.GetComponent<RectTransform>();
            Anchor(healthRect, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 30f), new Vector2(400f, 60f));
            var layout = healthGo.GetComponent<HorizontalLayoutGroup>();
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.spacing = 12f;
            layout.childControlWidth = false;
            layout.childControlHeight = false;
            var healthView = healthGo.AddComponent<HealthView>();
            SetRef(healthView, "iconPrefab", healthIcon);
            SetRef(healthView, "container", healthGo.transform);

            // плашка конца игры
            var panelGo = new GameObject("EndPanel", typeof(RectTransform), typeof(Image));
            panelGo.transform.SetParent(canvasGo.transform, false);
            var panelRect = panelGo.GetComponent<RectTransform>();
            panelRect.anchorMin = Vector2.zero;
            panelRect.anchorMax = Vector2.one;
            panelRect.offsetMin = Vector2.zero;
            panelRect.offsetMax = Vector2.zero;
            panelGo.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.75f);

            TMP_Text title = CreateText("Title", panelGo.transform, "Поражение", 96, TextAlignmentOptions.Center);
            Anchor(title.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 90f), new Vector2(1400f, 150f));
            TMP_Text details = CreateText("Details", panelGo.transform, "Счёт: 0", 56, TextAlignmentOptions.Center);
            Anchor(details.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, -70f), new Vector2(1400f, 220f));

            var endScreen = new GameObject("EndScreen").AddComponent<EndScreenView>();
            endScreen.transform.SetParent(canvasGo.transform, false);
            SetRef(endScreen, "panel", panelGo);
            SetRef(endScreen, "title", title);
            SetRef(endScreen, "details", details);

            // bootstrapper
            var bootstrapper = new GameObject("Bootstrapper").AddComponent<Bootstrapper>();
            var bso = new SerializedObject(bootstrapper);
            bso.FindProperty("mainCamera").objectReferenceValue = camera;
            bso.FindProperty("input").objectReferenceValue = input;
            bso.FindProperty("game").objectReferenceValue = game;
            bso.FindProperty("playerMovement").objectReferenceValue = movement;
            bso.FindProperty("playerShooter").objectReferenceValue = shooter;
            bso.FindProperty("playerHealth").objectReferenceValue = playerHealth;
            bso.FindProperty("enemyGrid").objectReferenceValue = grid;
            bso.FindProperty("enemyShooter").objectReferenceValue = enemyShooter;
            bso.FindProperty("scoreView").objectReferenceValue = scoreView;
            bso.FindProperty("healthView").objectReferenceValue = healthView;
            bso.FindProperty("endScreen").objectReferenceValue = endScreen;
            bso.ApplyModifiedPropertiesWithoutUndo();

            EditorSceneManager.SaveScene(scene, ScenePath);

            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };
        }

        // ---------- помощники UI ----------

        private static TMP_Text CreateText(string name, Transform parent, string text, float size, TextAlignmentOptions alignment)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(TextMeshProUGUI));
            go.transform.SetParent(parent, false);

            var tmp = go.GetComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = size;
            tmp.alignment = alignment;
            tmp.color = Color.white;
            return tmp;
        }

        private static void Anchor(RectTransform rect, Vector2 min, Vector2 max, Vector2 position, Vector2 size)
        {
            rect.anchorMin = min;
            rect.anchorMax = max;
            rect.pivot = new Vector2(min.x, min.y);
            rect.anchoredPosition = position;
            rect.sizeDelta = size;
        }

        private static void SetRef(Object target, string field, Object value)
        {
            var so = new SerializedObject(target);
            so.FindProperty(field).objectReferenceValue = value;
            so.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
#endif
