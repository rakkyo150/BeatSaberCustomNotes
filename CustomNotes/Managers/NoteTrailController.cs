using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;
using CustomNotes.Settings.Utilities;
using SiraUtil.Interfaces;
using CustomNotes.Overrides;

namespace CustomNotes.Managers
{
    // ②CustamNotesから独立したい
    
    public class NoteTrailController:MonoBehaviour,IColorable, INoteControllerDidInitEvent,INoteControllerNoteWasCutEvent, INoteControllerNoteWasMissedEvent, INoteControllerNoteDidDissolveEvent
    {
        private PluginConfig _pluginConfig;
        private GameNoteController _gameNoteController;
        private TrailRenderer _trailRenderer;
        private CustomNoteColorNoteVisuals _customNoteColorNoteVisuals;

        private Material mt= new Material(Shader.Find("Legacy Shaders/Diffuse"));

        public Color Color => _customNoteColorNoteVisuals != null ? _customNoteColorNoteVisuals.noteColor : Color.white;


        // ③なぜかインジェクトしないとInit動かなかった
        // https://virtualcast.jp/blog/2020/04/zenject_initialize_order_trap/
        // 初期化処理の実行順の問題？
        [Inject]
        internal void Init(PluginConfig pluginConfig)
        {
            _pluginConfig = pluginConfig;
            _gameNoteController = GetComponent<GameNoteController>();
            _customNoteColorNoteVisuals = GetComponent<CustomNoteColorNoteVisuals>();

            _gameNoteController.didInitEvent.Add(this);
            _gameNoteController.noteWasCutEvent.Add(this);
            _gameNoteController.noteWasMissedEvent.Add(this);
            _gameNoteController.noteDidDissolveEvent.Add(this);
        }

        protected void OnDestroy()
        {
            if (_gameNoteController != null)
            {
                _gameNoteController.didInitEvent.Remove(this);
                _gameNoteController.noteWasCutEvent.Remove(this);
                _gameNoteController.noteWasMissedEvent.Remove(this);
                _gameNoteController.noteDidDissolveEvent.Remove(this);
            }
        }

        public void HandleNoteControllerDidInit(NoteControllerBase noteControllerBase)
        {
            switch (noteControllerBase.noteData.colorType)
            {
                case ColorType.ColorA:
                case ColorType.ColorB:
                    _trailRenderer = gameObject.AddComponent<TrailRenderer>();
                    _trailRenderer.time = 0.3f;
                    _trailRenderer.widthMultiplier = 0.05f;
                    Logger.log.Debug("add trail");
                    
                    // ①なぜか色変わらない
                    // マテリアルが悪い？？
                    SetColor(Color);
                    Logger.log.Debug("mt color"+mt.color.ToString());
                    _trailRenderer.material = mt;
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
            Logger.log.Debug("cutcut");
        }

        public void HandleNoteControllerNoteDidDissolve(NoteController noteController)
        {
            HandleNoteControllerNoteWasMissed(noteController);
        }

        public void SetColor(Color color)
        {
            mt.color = color;
        }
    }
}
