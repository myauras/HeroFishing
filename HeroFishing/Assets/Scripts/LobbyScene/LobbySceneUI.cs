using UnityEngine;
using Scoz.Func;
using System;


namespace HeroFishing.Main {

    public class LobbySceneUI : BaseUI {

        public enum LobbyUIs {
            Lobby,//�w�]����
            Map,//�a�Ϥ���
        }
        public LobbyUIs CurUI { get; private set; } = LobbyUIs.Lobby;
        public BaseUI LastPopupUI { get; private set; }//�����W�����u�X����(�������ɭn�����W�����u�X����)

        static bool FirstEnterLobby = true;//�Ĥ@���i�J�j�U��|�]�w�^false �ΨӧP�_�O�_�Ĥ@���i�J�j�U�Ӱ��P�_


        //�i�C���N�n��l�ƪ�UI��o��(�|�W�[���������ɪ�Ū���ɶ�)
        [SerializeField] MapUI MyMapUI;


        public static LobbySceneUI Instance { get; private set; }


        ////�i�C��������l�ơA����n�ήɤ~��l�ƪ�UI��o��
        //[SerializeField] AssetReference BattleUIAsset;
        ////�Უ�ͪ�UI���h
        //[SerializeField] Transform BattleUIParent;
        ////�Უ�ͪ�UI����
        //BattleUI MyBattleUI;

        public override void Init() {
            base.Init();
            MyMapUI.Init();
            SwitchUI(LobbyUIs.Lobby);
            Instance = this;
        }

        public void SwitchUI(LobbyUIs _ui, Action _cb = null) {

            if (LastPopupUI != null)
                LastPopupUI.SetActive(false);//�����u�X����
            //PlayerInfoUI.GetInstance<PlayerInfoUI>()?.SetActive(false);//�Ҧ������w�]�����|�}�Ҹ�T�ɭ�

            switch (_ui) {
                case LobbyUIs.Lobby://���Ӧb��L�����ɡA�i�H�ǤJLobby�������u�X��������ܦ^�w�]����
                    MyMapUI.SetActive(false);
                    _cb?.Invoke();
                    LastPopupUI = null;
                    break;
                case LobbyUIs.Map:
                    MyMapUI.SetActive(true);
                    _cb?.Invoke();
                    LastPopupUI = MyMapUI;
                    break;

                    //case AdventureUIs.Battle:
                    //    MyCreateRoleUI.SetActive(false);
                    //    MyBattleUI?.SetActive(true);
                    //    //�P�_�O�_�w�g���J�L��UI�A�Y�٨S���L�N��Ū�����ö}�l���J
                    //    if (MyBattleUI != null) {
                    //        MyBattleUI.SetBattle();
                    //        _cb?.Invoke();
                    //    } else {
                    //        LoadAssets(_ui, _cb);//Ū������
                    //    }
                    //    LastPopupUI = MyBattleUI;

                    //    break;
            }
        }

        public override void RefreshText() {
        }

        //void LoadAssets(AdventureUIs _ui, Action _cb) {
        //    switch (_ui) {
        //        case AdventureUIs.Battle:
        //            PopupUI.ShowLoading(StringData.GetUIString("WaitForLoadingUI"));
        //            //��l��UI
        //            AddressablesLoader.GetPrefabByRef(BattleUIAsset, (prefab, handle) => {
        //                PopupUI.HideLoading();
        //                GameObject go = Instantiate(prefab);
        //                go.transform.SetParent(BattleUIParent);

        //                RectTransform rect = go.GetComponent<RectTransform>();
        //                go.transform.localPosition = prefab.transform.localPosition;
        //                go.transform.localScale = prefab.transform.localScale;
        //                rect.offsetMin = Vector2.zero;//Left�BBottom
        //                rect.offsetMax = Vector2.zero;//Right�BTop

        //                MyBattleUI = go.GetComponent<BattleUI>();
        //                MyBattleUI.Init();
        //                MyBattleUI.SetBattle();
        //                _cb?.Invoke();
        //            }, () => { WriteLog.LogError("���JBattleUIAsset����"); });
        //            break;
        //    }
        //}

        public void OnMapBtnClick() {
            SwitchUI(LobbyUIs.Map);
        }



    }
}