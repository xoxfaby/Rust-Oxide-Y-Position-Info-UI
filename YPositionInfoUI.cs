using System;
using UnityEngine;
using Rust;
using Oxide.Game.Rust.Cui;
using Oxide.Core.Libraries.Covalence;
using Oxide.Game.Rust.Libraries.Covalence;

namespace Oxide.Plugins
{
    [Info("Y-Position Info UI", "XoXFaby", "1.0.1")]
    [Description("Helps you more easily see your Y position.")]
    class YPositionInfoUI : CovalencePlugin
    {
        class PlayerInfo : MonoBehaviour
        {
            public BaseEntity yReferenceEntity;
            public float nextUIRefresh = 0;
            public bool showUI;
        }
        private CuiElementContainer container = new CuiElementContainer();
        private CuiElementContainer backgroundContainer = new CuiElementContainer();
        private CuiLabel posLabel = new CuiLabel();
        private CuiPanel backgroundPanel = new CuiPanel();
        private String posJson;

        const String labelName = "YUILabelName";
        const String backgroundName = "YUIBackgroundName";
        const String AnchorMin = "0.5 0.15";
        const String AnchorMax = "0.5 0.15";
        const String OffsetMin = "-50 -15";
        const String OffsetMax = "50 15";

        void InitBasePlayer(BasePlayer player)
        {
            PlayerInfo info = player.GetComponent<PlayerInfo>();
            if (info == null)
            {
                info = player.gameObject.AddComponent<PlayerInfo>();
                info.showUI = (bool)Config["ShowUIByDefault"];
            }
            CuiHelper.DestroyUi(player, labelName);
            CuiHelper.DestroyUi(player, backgroundName);
            if (info.showUI)
            {
                CuiHelper.AddUi(player, backgroundContainer);
            }
        }

        private void Init()
        {
            posLabel.Text.Text = "REPLACEME";
            posLabel.Text.FontSize = 24;
            posLabel.Text.Align = TextAnchor.MiddleCenter;
            posLabel.RectTransform.AnchorMin = AnchorMin;
            posLabel.RectTransform.AnchorMax = AnchorMax;
            posLabel.RectTransform.OffsetMin = OffsetMin;
            posLabel.RectTransform.OffsetMax = OffsetMax;
            container.Add(posLabel, "Hud", labelName);
            posJson = CuiHelper.ToJson(container);

            backgroundPanel.Image.Color = "0 0 0 0.8";
            backgroundPanel.RectTransform.AnchorMin = AnchorMin;
            backgroundPanel.RectTransform.AnchorMax = AnchorMax;
            backgroundPanel.RectTransform.OffsetMin = OffsetMin;
            backgroundPanel.RectTransform.OffsetMax = OffsetMax;
            backgroundContainer.Add(backgroundPanel, "Hud", backgroundName);

            foreach (BasePlayer player in BasePlayer.allPlayerList)
            {
                InitBasePlayer(player);
            }
        }

        protected override void LoadDefaultConfig()
        {
            UnityEngine.Debug.Log("Creating default configuration file");
            Config["ShowUIByDefault"] = true;
            Config["UIRefreshRateMilliSeconds"] = 0;
            Config["CommandOnlyMode"] = false;
        }

        void OnPlayerConnected(BasePlayer player)
        {
            InitBasePlayer(player);
        }

        private object OnPlayerTick(BasePlayer player, PlayerTick msg, bool wasPlayerStalled)
        {
            PlayerInfo info = player.GetComponent<PlayerInfo>();
            if (!info.showUI) return null;

            if ((int)Config["UIRefreshRateMilliSeconds"] > 0)
            {
                if (Time.time < info.nextUIRefresh)
                {
                    return null;
                }
                else
                {
                    float UIRefreshRate = (int)Config["UIRefreshRateMilliSeconds"] / 1000f;
                    info.nextUIRefresh = Time.time + UIRefreshRate;
                }
            }
            CuiHelper.DestroyUi(player, labelName);
            String yString;
            if (info.yReferenceEntity != null)
            {
                yString = $"{player.gameObject.transform.position.y - (info.yReferenceEntity.transform.position.y + 0.1f):+0.0000;-0.0000}";
            }
            else
            {
                yString = $"{player.transform.position.y:0.0000}";
            }
            String newJson = posJson.Replace("REPLACEME", yString);
            CuiHelper.AddUi(player, newJson);
            return null;
        }

        void setYReferenceEntity(BasePlayer player)
        {
            BaseEntity heldEntity = player.GetHeldEntity();
            if (heldEntity == null)
            {
                UnityEngine.RaycastHit hitInfo;
                GamePhysics.Trace(player.eyes.HeadRay(), 0f, out hitInfo, 50, Physics.AllLayers);
                BaseEntity baseEntity = hitInfo.collider?.ToBaseEntity();
                PlayerInfo info = player.GetComponent<PlayerInfo>();
                if (baseEntity != null)
                {
                    info.yReferenceEntity = baseEntity;
                }
                else
                {
                    info.yReferenceEntity = null;
                }
            }
        }

        void OnPlayerInput(BasePlayer player, InputState input)
        {
            if (!(bool)Config["CommandOnlyMode"]) return;

            if (input.IsDown(BUTTON.RELOAD) && !input.WasDown(BUTTON.RELOAD))
            {
                setYReferenceEntity(player);
            }
        }

        [Command("toggleyui")]
        private void ToggleYUICommand(RustPlayer player, string command, string[] args)
        {
            BasePlayer basePlayer = (BasePlayer)player.Object;
            PlayerInfo info = basePlayer.GetComponent<PlayerInfo>();
            info.showUI = !(bool)info.showUI;
            if (info.showUI)
            {
                CuiHelper.AddUi(basePlayer, backgroundContainer);
            }
            else
            {
                CuiHelper.DestroyUi(basePlayer, labelName);
                CuiHelper.DestroyUi(basePlayer, backgroundName);
            }
        }

        [Command("setyentity")]
        private void SetYEntity(RustPlayer player, string command, string[] args)
        {
            BasePlayer basePlayer = (BasePlayer)player.Object;
            setYReferenceEntity(basePlayer);
        }
    }
}