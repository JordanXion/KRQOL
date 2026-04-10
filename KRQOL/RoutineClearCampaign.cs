using NGame2.NUI.NComponent2;
using NGame2.NUI.NWindow;
using NGame2.NUI.NWindow.NPayShop;
using NGame2.NUI.NWorldMap;
using NGame2.NUI.NWorldMap.NMapLayout;
using NShared.NEvent;
using System.Collections;
using UnityEngine;

namespace KRQOL
{
    internal class RoutineClearCampaign : Routine
    {
        internal override string Name => "Campaign Clear";

        internal RoutineClearCampaign(Plugin plugin) : base(plugin) { }

        private Coroutine _navigationCoroutine;
        private Coroutine _battleLoopCoroutine;

        internal override void Start()
        {
            base.Start();
            _navigationCoroutine = _plugin.StartCoroutine(GoToCurrentCampaignLevel());
            _battleLoopCoroutine = _plugin.StartCoroutine(RunBattleLoop());
        }

        internal override void Stop() 
        { 
            base.Stop();
            if (_navigationCoroutine != null)
            {
                _plugin.StopCoroutine(_navigationCoroutine);
                _navigationCoroutine = null;
            }
            if (_battleLoopCoroutine != null)
            {
                _plugin.StopCoroutine(_battleLoopCoroutine);
                _battleLoopCoroutine = null;
            }
        }

        private IEnumerator GoToCurrentCampaignLevel()
        {
            Log("Navigating to current campaign level...");

            if (!OpenMenuPortal()) yield break;
            yield return new WaitForSeconds(1.0f);

            if (!PortalMenuClickMoveLatelyBattle()) yield break;
            yield return new WaitForSeconds(1.0f);

            if (!PortalMenuClickAcceptMove()) yield break;
            yield return new WaitForSeconds(1.0f);

            Transform nextNode = FindNextMapNode();
            if (!MoveToMapNode(nextNode)) yield break;
            yield return new WaitForSeconds(1.0f);

            if (!ClickPrepareBattle()) yield break;
            yield return new WaitForSeconds(1.0f);

            if (!ClickStartBattle()) yield break;
            yield return new WaitForSeconds(1.0f);

            Log("Finished navigating to current campaign level.");
        }

        private IEnumerator RunBattleLoop()
        {
            bool success = false;
            while (IsRunning)
            {
                success = true;
                if (!ClickPrepareBattle())
                {
                    Log("Waiting for prepare battle button...");
                    success = false;
                }

                yield return new WaitForSeconds(1.0f);

                if (!ClickStartBattle())
                {
                    Log("Waiting for start battle button... Probably shouldn't see this");
                    success = false;
                }

                if (success)
                {
                    Log("Started battle, waiting 30s before next action...");
                    yield return new WaitForSeconds(30.0f);
                }
                else
                {
                    Log("Did not start battle, waiting 5s before retrying...");
                    yield return new WaitForSeconds(5.0f);
                }
            }
        }

        private bool OpenMenuPortal() => FindAndClick<LobbyRightMenu>(m => m.OnClickPortalButton());

        private bool PortalMenuClickMoveLatelyBattle() => FindAndClick<PortalChapterComponent>(m => m.OnClickMoveLatelyBattleNode());

        private bool PortalMenuClickAcceptMove() => FindAndClick<PayShopMessage3ButtonPopup>(m => m.OnClickButton1());

        private bool ClickPrepareBattle() => FindAndClick<LobbyNodeFunctionMenu>(m => m.OnClickButton());

        private bool ClickReadyForBattle() => FindAndClick<BattleDungeonInfo>(m => m.OnButtonReadyBattle());

        private bool ClickStartBattle() => FindAndClick<BattlePartySetting>(m => m.OnClickStartBattle());

        private Transform FindNextMapNode()
        {
            Log("Finding next map node...");
            var mapLayout = GameObject.FindAnyObjectByType<MapLayout>();
            if (mapLayout == null)
            {
                Log("MapLayout not found.");
                return null;
            }

            Transform nodeRoot = mapLayout.NodeRoot;
            Transform lastActiveChild = null;
            for (int i = 0; i < nodeRoot.childCount; i++)
            {
                Transform child = nodeRoot.GetChild(i);
                if (!child.gameObject.activeInHierarchy) continue;
                if (child.GetComponent<MapNode>() == null) continue;
                if (!child.name.EndsWith("Field")) continue;
                lastActiveChild = child;
            }

            Log($"Found last active node: {lastActiveChild?.name}");
            return lastActiveChild;
        }

        private bool MoveToMapNode(Transform node)
        {
            Log($"Moving to map node {node.name}");
            if (node == null)
            {
                Log("No active map node found, stopping routine.");
                return OnFailure();
            }
            var mapNode = node.GetComponent<MapNode>();
            if (mapNode == null)
            {
                Log("MapNode component not found on node.");
                return OnFailure();
            }
            Log($"Clicking node: {mapNode.DungeonIndex} CanVisit={mapNode.CanVisit} IsCompleted={mapNode.IsCompleted}");
            mapNode.OnClick();
            return true;
        }


    }
}
