using System.Collections.Generic;
using UnityEngine;

namespace SeganX
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private string prefabPath = "Menus/";
        [SerializeField] private Canvas canvas = null;

        private List<System.Type> typeStack = new List<System.Type>();
        private List<GameState> stateStack = new List<GameState>();

        public System.Action<GameState> OnBackButton = new System.Action<GameState>(x => { });
        public System.Action<GameState> OnOpenState = new System.Action<GameState>(x => { });

        public bool IsEmpty => CurrentState == null && CurrentPopup == null && typeStack.Count < 1;
        public Canvas Canvas => canvas;
        public GameState CurrentPopup => stateStack.Count > 0 ? stateStack[0] : null;
        public GameState CurrentState { get; private set; } = null;

        public T OpenState<T>(bool resetStack = false) where T : GameState
        {
            // load prefab
            T prefab = Resources.Load<T>(prefabPath + typeof(T).Name);
            if (prefab == null)
            {
                Debug.LogError("game could not find " + typeof(T).Name);
                return null;
            }

            // close current state
            if (CurrentState != null)
            {
                var delay = CurrentState.PreClose();
                Destroy(CurrentState.gameObject, delay);
            }

            // update type stack
            if (resetStack) typeStack.Clear();
            if (typeStack.Count < 1 || typeStack[0] != typeof(T))
                typeStack.Insert(0, typeof(T));

            // instantiate new state from prefab
            CurrentState = Instantiate<GameState>(prefab);
            CurrentState.name = prefab.name;

            AttachState(CurrentState);

            OnOpenState(CurrentState);
            return CurrentState as T;
        }

        private GameState CloseCurrentState()
        {
            if (typeStack.Count < 2) return CurrentState;

            typeStack.RemoveAt(0);
            var delay = CurrentState.PreClose();
            Destroy(CurrentState.gameObject, delay);

            var state = Resources.Load<GameState>(prefabPath + typeStack[0].Name);
            CurrentState = Instantiate(state) as GameState;
            CurrentState.name = state.name;
            AttachState(CurrentState);

            OnOpenState(CurrentState);

            return CurrentState;
        }

        public T OpenPopup<T>(GameObject prefab) where T : GameState
        {
            if (prefab == null) return null;
            T res = Instantiate<GameObject>(prefab).GetComponent<T>();
            res.name = prefab.name;
            stateStack.Insert(0, res);
            Resources.UnloadUnusedAssets();
            AttachState(res);
            OnOpenState(res);
            return res;
        }

        public T OpenPopup<T>() where T : GameState
        {
            T popup = Resources.Load<T>(prefabPath + typeof(T).Name);
            if (popup == null)
            {
                Debug.LogError("game could not find " + typeof(T).Name);
                return null;
            }
            return OpenPopup<T>(popup.gameObject);
        }

        public bool ClosePopup(GameState popup)
        {
            if (popup != null && stateStack.Remove(popup))
            {
                var delay = popup.PreClose();
                Destroy(popup.gameObject, delay);
                return true;
            }
            return false;
        }

        //  close current popup window and return the remains opened popup
        public int ClosePopup(bool closeAll = false)
        {
            if (stateStack.Count < 1) return 0;
            ClosePopup(stateStack[0]);
            return closeAll ? ClosePopup(closeAll) : stateStack.Count;
        }

        public GameManager Back(GameState gameState)
        {
            if (ClosePopup(gameState))
            {
                OnBackButton(CurrentPopup != null ? CurrentPopup : CurrentState);
            }
            else if (CurrentState == gameState)
            {
                CloseCurrentState();
                OnBackButton(CurrentPopup != null ? CurrentPopup : CurrentState);
            }
            return this;
        }

        private void AttachState(GameState panel)
        {
            if (panel == null) return;

            if (canvas.worldCamera == null)
                canvas.worldCamera = Camera.main;

            if (panel.transform is RectTransform)
            {
                var panelcanvas = panel.GetComponent<Canvas>();
                if (panelcanvas == null)
                    panel.transform.SetParent(canvas.transform, false);
                else if (panelcanvas.worldCamera == null)
                    panelcanvas.worldCamera = canvas.worldCamera;
            }
        }

        protected virtual void LateUpdate()
        {
            //  handle escape key
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (stateStack.Count > 0)
                    stateStack[0].Back();
                else if (CurrentState != null)
                    CurrentState.Back();
            }
        }

        protected virtual void Reset()
        {
            var validPath = Application.dataPath + "/Resources/" + prefabPath;
            if (System.IO.Directory.Exists(validPath) == false)
                System.IO.Directory.CreateDirectory(validPath);
        }

        //////////////////////////////////////////////////////
        /// STATIC MEMBERS
        //////////////////////////////////////////////////////
        private static GameManager game = default;
        internal static GameManager Game
        {
            get
            {
                if (game == null)
                {
                    game = FindObjectOfType<GameManager>();
                    if (game == null)
                    {
                        game = new GameObject().AddComponent<GameManager>();
                        game.gameObject.name = "Game";
                        DontDestroyOnLoad(game);
                    }
                }
                return game;
            }
        }
    }
}
