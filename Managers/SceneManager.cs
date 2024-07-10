using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 김도명_TEXTRPG
{
    public class SceneManager
    {
        // 현재 씬
        public Scene CurrentScene { get; protected set; }
        public Scene PrevScene { get; protected set; }

        private Dictionary<string, Scene> Scenes = new Dictionary<string, Scene>();
        private Dictionary<string, ActionOption> Options = new Dictionary<string, ActionOption>();



        public void Initialize()
        {
            // #1. 씬 정보 초기화.
            DirectoryInfo directoryInfo = new DirectoryInfo(Directory.GetParent(Environment.CurrentDirectory).Parent.FullName + "/Scene");
            foreach (FileInfo info in directoryInfo.GetFiles())
            {
                string sceneName = Path.GetFileNameWithoutExtension(info.FullName);
                Type type = Type.GetType($"{this.GetType().Namespace}.{sceneName}");
                if (type != null)
                {
                    Scene scene = Activator.CreateInstance(type) as Scene;
                    if (scene != null)
                    {
                        Scenes.Add(sceneName, scene);
                    }
                }
            }

            // #2. 선택지 정보 초기화.

            Options.Add("NewGame", new ActionOption("NewGame", "새로하기", () => EnterScene<CreateCharacterScene>()));
            Options.Add("SaveGame", new ActionOption("SaveGame", "저장하기", () => {
                Managers.Game.Data.CurrentSceneName = "Main";
                Managers.Game.Data.SceneMetaData = 0;
                EnterScene<SaveScene>();
            }));
            Options.Add("Quit", new ActionOption("Quit", "게임종료", () => { Renderer.Down(); Environment.Exit(0); }));
            Options.Add("Inventory", new ActionOption("Inventory", "인벤토리", () => EnterScene<EquipmentScene>()));
            Options.Add("ShowInfo", new ActionOption("ShowInfo", "상태보기", () => EnterScene<CharacterInfoScene>()));
            Options.Add("DungeonEnter", new ActionOption("DungeonEnter", $"던전입장", () => EnterScene<BattleScene>()));
            Options.Add("Quest", new ActionOption("Quest", "의뢰소", () => EnterScene<QuestScene>()));
            Options.Add("Shop", new ActionOption("Shop", "상점", () => EnterScene<ShopScene>()));
            Options.Add("Rest", new ActionOption("Rest", "여관", () => EnterScene<RestScene>()));
            Options.Add("Main", new ActionOption("Main", "메인으로", () => EnterScene<MainScene>()));
            Options.Add("Casino", new ActionOption("Casino", "도박장", () => EnterScene<CasinoScene>()));
            Options.Add("Back", new ActionOption("Back", "뒤로가기", () => EnterScene<Scene>(PrevScene.GetType().Name)));

            #region 월드맵
            Options.Add("WorldMap", new ActionOption("WorldMap", "월드맵", () => EnterScene<WorldMapScene>()));
            Options.Add("Dungeon1", new ActionOption("Dungeon1", "고대의 숲 d(적정레벨 1 ~ 4)w", () =>
            {
                BattleScene.DungeonName = GameTable.BattleSceneNames[0];
                BattleScene.Stage = 0;
                EnterScene<BattleScene>();
            }
            ));
            Options.Add("Dungeon2", new ActionOption("Dungeon2", "그림자 성채 d(적정레벨 5 ~ 8)w", () =>
            {
                BattleScene.DungeonName = GameTable.BattleSceneNames[1];
                BattleScene.Stage = 5;
                EnterScene<BattleScene>();
            }
            ));
            Options.Add("Dungeon3", new ActionOption("Dungeon3", "불타는 폐허 d(적정레벨 9 ~ 12)w", () =>
            {
                BattleScene.DungeonName = GameTable.BattleSceneNames[2];
                BattleScene.Stage = 10;
                EnterScene<BattleScene>();
            }
            ));
            Options.Add("Dungeon4", new ActionOption("Dungeon4", "서리 동굴 d(적정레벨 13 ~ 16)w", () =>
            {
                BattleScene.DungeonName = GameTable.BattleSceneNames[3];
                BattleScene.Stage = 15;
                EnterScene<BattleScene>();
            }
            ));
            Options.Add("Dungeon5", new ActionOption("Dungeon5", "어둠의 성역 d(적정레벨 17 ~ 20)w", () =>
            {
                BattleScene.DungeonName = GameTable.BattleSceneNames[4];
                BattleScene.Stage = 20;
                EnterScene<BattleScene>();
            }
            ));

            #endregion
        }

        #region ActionOption

        public ActionOption GetOption(string key) => Options[key];

        #endregion


        #region Scene

        /// <summary>
        /// 씬을 불러옵니다.
        /// </summary>
        /// <typeparam name="T">씬 타입을 결정합니다.</typeparam>
        /// <param name="sceneKey"></param>
        /// <returns></returns>
        public Scene GetScene<T>(string sceneKey = null) where T : Scene
        {
            if (string.IsNullOrEmpty(sceneKey)) sceneKey = typeof(T).Name;
            if (!Scenes.TryGetValue(sceneKey, out Scene scene)) return null;
            return scene;
        }

        /// <summary>
        /// 씬에 진입합니다.
        /// </summary>
        /// <typeparam name="T">씬 타입을 결정합니다.</typeparam>
        /// <param name="sceneKey"></param>
        public void EnterScene<T>(string sceneKey = null) where T : Scene
        {
            Renderer.Down();

            // #1. Scene 불러오기.
            if (string.IsNullOrEmpty(sceneKey)) sceneKey = typeof(T).Name;
            if (!Scenes.TryGetValue(sceneKey, out Scene scene)) return;
            if (scene == null || scene == CurrentScene) return;

            // #2. 이전 씬 설정.
            SetPrevScene();

            // #3. 현재 씬 진입.
            CurrentScene = scene;
            scene.EnterScene();
            scene.NextScene();
        }


        private void SetPrevScene()
        {
            PrevScene = CurrentScene;
        }

        #endregion
    }



    public class ActionOption
    {
        public string Key { get; private set; }
        public string Description { get; private set; }
        public Action Action { get; private set; }
        public ActionOption(string key, string description, Action action)
        {
            Key = key;
            Description = description;
            Action = action;
        }

        public void Execute() => Action?.Invoke();
    }

    public enum Command
    {
        Nothing,
        Tab,
        MoveTop,
        MoveBottom,
        MoveLeft,
        MoveRight,
        Interact,
        Exit,
        F12,
    }


}
