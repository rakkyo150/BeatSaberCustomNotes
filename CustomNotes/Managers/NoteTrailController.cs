using System.IO;
using System.Reflection;
using CustomNotes.Overrides;
using CustomNotes.Settings.Utilities;
using UnityEngine;
using Zenject;

namespace CustomNotes.Managers
{
    // ②CustamNotesから独立したい

    public class NoteTrailController : MonoBehaviour, INoteControllerDidInitEvent, INoteControllerNoteWasCutEvent, INoteControllerNoteWasMissedEvent, INoteControllerNoteDidDissolveEvent
    {
        private PluginConfig _pluginConfig;
        private IDifficultyBeatmap _difficultyBeatmap;
        private GameNoteController _gameNoteController;
        private TrailRenderer _trailRenderer;
        private CustomNoteColorNoteVisuals _customNoteColorNoteVisuals;

        private float _trailTime;
        private Shader shader;
        private Material mt;

        // ③なぜかインジェクトしないとInit動かなかった
        // https://virtualcast.jp/blog/2020/04/zenject_initialize_order_trap/
        // 初期化処理の実行順の問題？
        [Inject]
        internal void Init(PluginConfig pluginConfig, IDifficultyBeatmap difficultyBeatmap)
        {
            _pluginConfig = pluginConfig;
            _difficultyBeatmap = difficultyBeatmap;
            _trailTime = _pluginConfig.TrailLength / _difficultyBeatmap.noteJumpMovementSpeed;
            _gameNoteController = GetComponent<GameNoteController>();
            _customNoteColorNoteVisuals = GetComponent<CustomNoteColorNoteVisuals>();

            LoadShader();
            mt = new Material(this.shader);

            _gameNoteController.didInitEvent.Add(this);
            _gameNoteController.noteWasCutEvent.Add(this);
            _gameNoteController.noteWasMissedEvent.Add(this);
            _gameNoteController.noteDidDissolveEvent.Add(this);
            _customNoteColorNoteVisuals.didInitEvent += Color_DidInit;
        }

        protected void OnDestroy()
        {
            if (_gameNoteController != null)
            {
                _gameNoteController.didInitEvent.Remove(this);
                _gameNoteController.noteWasCutEvent.Remove(this);
                _gameNoteController.noteWasMissedEvent.Remove(this);
                _gameNoteController.noteDidDissolveEvent.Remove(this);
                _customNoteColorNoteVisuals.didInitEvent -= Color_DidInit;
            }
        }

        public void HandleNoteControllerDidInit(NoteControllerBase noteControllerBase)
        {
            switch (noteControllerBase.noteData.colorType)
            {
                case ColorType.ColorA:
                case ColorType.ColorB:
                    _trailRenderer = gameObject.AddComponent<TrailRenderer>();
                    _trailRenderer.material = mt;
                    _trailRenderer.time = _trailTime;
                    _trailRenderer.widthMultiplier = _pluginConfig.TrailWidth;

                    break;
                default:
                    break;
            }
        }

        public void HandleNoteControllerNoteWasMissed(NoteController nc)
        {
            switch (nc.noteData.colorType)
            {
                case ColorType.ColorA:
                case ColorType.ColorB:
                    GameObject.Destroy(this.gameObject.GetComponent<TrailRenderer>());
                    break;
                default:
                    break;
            }
        }

        public void HandleNoteControllerNoteWasCut(NoteController nc, in NoteCutInfo _)
        {
            HandleNoteControllerNoteWasMissed(nc);
        }

        public void HandleNoteControllerNoteDidDissolve(NoteController noteController)
        {
            HandleNoteControllerNoteWasMissed(noteController);
        }

        private void Color_DidInit(ColorNoteVisuals visuals, NoteControllerBase noteController)
        {
            if (_pluginConfig.ColoredTrail)
            {
                SetColor((visuals as CustomNoteColorNoteVisuals).noteColor);
            }
        }

        public void SetColor(Color color)
        {
            _trailRenderer.material.color = color;
        }

        private void LoadShader()
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            // ResourcesにはシェーダそのものではなくUnityでAssetBundle化したものが必要
            Stream st = asm.GetManifestResourceStream("CustomNotes.Resources.Shaders.sh_custom_unlit");



            AssetBundle assetBundle = AssetBundle.LoadFromStream(st);
            this.shader = assetBundle.LoadAsset<Shader>("sh_custom_unlit");
            assetBundle.Unload(false);


            if (this.shader == null)
            {
                Logger.log.Debug("Failed to load AssetBundle");
            }
        }
    }
}
