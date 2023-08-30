using System.Collections.Generic;
using UnityEngine;
using Scoz.Func;
using UnityEngine.UI;
using Service.Realms;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;

namespace HeroFishing.Main {
    public class StartSceneUI : BaseUI {
        [SerializeField] Toggle TermsOfUseToggle;
        [SerializeField] GameObject GuestLoginBtn;//登入按鈕
        [SerializeField] GameObject ThirdpartBtns;//三方登入按鈕
        [SerializeField] GameObject AppleLoginGO;//蘋果登入
        [SerializeField] GameObject LogutoutGO;//登出
        [SerializeField] GameObject DeleteACGO;//刪除帳戶(蘋果要求)
        [SerializeField] GameObject BackToLobbyGO;//返回大廳按鈕
        [SerializeField] GameObject TestTool;//編輯器登入會顯示的物件
        [SerializeField] Text MiddleText;//等待登入文字
        [SerializeField] RectTransform QuestReportButton; // 問題回報的按鍵

        public static StartSceneUI Instance { get; private set; }

        public override void Init() {
            base.Init();
            Instance = this;
        }
        public override void RefreshText() {
        }

        private void Start() {
            SetMiddleText(StringJsonData.GetUIString("LoginType"));
            AppleLoginGO.SetActive(false);

            //Apple登入要打開
            //#if UNITY_IOS
            //            AppleLoginGO.SetActive(true);
            //#endif

        }

        public enum Condition {
            HideAll,//隱藏所有按鈕
            NotLogin,//還沒登入就顯示登入按鈕
            OfflineMode,//離線模式
            BackFromLobby_ShowLogoutBtn,//從大廳返回主介面 且 已經是登入狀態，會顯示登出按鈕與返回大廳按鈕
            BackFromLobby_ShowLoginBtn,//從大廳返回主介面 且 已經是登入狀態，會顯示登出按鈕與返回大廳按鈕
        }
        public void ShowUI(Condition _condition) {
            SetActive(true);
            switch (_condition) {
                case Condition.OfflineMode:
                    ShowLoginUI(false);
                    break;
                case Condition.HideAll:
                    SetActive(false);
                    break;
                case Condition.NotLogin:
                    ShowLoginUI(true);
                    break;
                case Condition.BackFromLobby_ShowLogoutBtn:
                    ShowLoginUI(false);
                    break;
                case Condition.BackFromLobby_ShowLoginBtn:
                    ShowLoginUI(true);
                    break;
            }
        }

        /// <summary>
        /// true:顯示登入按鈕並隱藏登出按鈕
        /// false:顯示登出按鈕並隱藏登入按鈕
        /// </summary>
        void ShowLoginUI(bool _show) {
            GuestLoginBtn.SetActive(_show);
            ThirdpartBtns.SetActive(_show);
            BackToLobbyGO.SetActive(!_show);
            LogutoutGO.SetActive(!_show);
            DeleteACGO.SetActive(!_show);
        }
        /// <summary>
        /// 登入按鈕按下
        /// </summary>
        public void OnSignupClick(string _authentication) {

            if (!TermsOfUseToggle.isOn) {//沒有勾選同意使用者條款的話會跳彈窗並返回
                PopupUI_Local.ShowClickCancel(StringJsonData.GetUIString("NeedToAgreeTersOfUse"), null);
                return;
            }

            //錯誤的登入類型就返回
            AuthType authType = AuthType.Guest;
            if (!MyEnum.TryParseEnum(_authentication, out authType)) { WriteLog.LogError("錯誤的登入類型: " + _authentication); return; }

            switch (authType) {
                case AuthType.Guest:

                    if (RealmManager.MyApp.CurrentUser == null) {//玩家還沒登入
                        PopupUI_Local.ShowLoading(string.Format("Loading"));

                        UniTask.Void(async () => {
                            await RealmManager.AnonymousSignup();//Realm訪客註冊
                            StartSceneManager.Instance.ShowInfo();//顯示下方文字
                            CompleteRegistrationEvent(authType);// 通知分析註冊完成事件
                            await InitPlayerData(authType);//初始化玩家資料


                        });

                    } else {//如果本來就有登入，代表是從大廳退回主畫面的，此時讓玩家登出並重新登入
                        PopupUI_Local.ShowConfirmCancel(StringJsonData.GetUIString("OverrideGuestAccountCheck"), () => {

                            PopupUI_Local.ShowLoading(string.Format("Loading"));
                            UniTask.Void(async () => {
                                await RealmManager.Signout();//Realm登出
                                await RealmManager.AnonymousSignup();//Realm訪客註冊
                                StartSceneManager.Instance.ShowInfo();//顯示下方文字
                                CompleteRegistrationEvent(authType);// 通知分析註冊完成事件
                                await InitPlayerData(authType);//初始化玩家資料

                            });

                        }, null);
                    }
                    break;
                    //                case AuthType.Facebook:
                    //                    if (FirebaseManager.MyUser == null) {//玩家還沒登入Firebase，開始進行三方登入
                    //                        FBAuth();
                    //                    } else {//如果玩家本來就有登入Firebase帳戶，代表是從大廳退回主畫面的，此時要登出後再進行三方登入
                    //                        PopupUI_Local.ShowConfirmCancel(StringData.GetUIString("OverrideGuestAccountCheck"), () => {
                    //                            StartCoroutine(FirebaseManager.Logout(() => {//登出
                    //                                FBAuth();
                    //                            }));
                    //                        }, null);
                    //                    }
                    //                    break;
                    //                case AuthType.Apple:
                    //                    if (FirebaseManager.MyUser == null) {//玩家還沒登入Firebase，開始進行三方登入
                    //                        AppleAuth();
                    //                    } else {//如果玩家本來就有登入Firebase帳戶，代表是從大廳退回主畫面的，此時要登出後再進行三方登入
                    //                        PopupUI_Local.ShowConfirmCancel(StringData.GetUIString("OverrideGuestAccountCheck"), () => {
                    //                            StartCoroutine(FirebaseManager.Logout(() => {//登出
                    //                                AppleAuth();
                    //                            }));
                    //                        }, null);
                    //                    }
                    //                    break;
                    //                case AuthType.Google:
                    //#if UNITY_EDITOR
                    //                    PopupUI_Local.ShowClickCancel("Editor不能使用Google登入", null);
                    //#else
                    //                                if (FirebaseManager.MyUser == null) {//玩家還沒登入Firebase，開始進行三方登入
                    //                                    GoogleAuth();
                    //                                } else {//如果玩家本來就有登入Firebase帳戶，代表是從大廳退回主畫面的，此時要登出後再進行三方登入
                    //                                    PopupUI_Local.ShowConfirmCancel(StringData.GetUIString("OverrideGuestAccountCheck"), () => {
                    //                                        StartCoroutine(FirebaseManager.Logout(() => {//登出
                    //                                            GoogleAuth();
                    //                                        }));
                    //                                    }, null);
                    //                                }
                    //#endif
                    //                    break;

            }
        }
        /// <summary>
        /// 初始化玩家資料
        /// </summary>
        async Task InitPlayerData(AuthType _authType) {
            WriteLog.LogColorFormat("User: {0} Auth: {1} ", WriteLog.LogType.Realm, RealmManager.MyApp.CurrentUser.Id, "代處理");
            ShowUI(Condition.HideAll);
            WriteLog.LogColor("尚無此玩家資料，開始初始化玩家資料", WriteLog.LogType.Realm);
            var replyData = await RealmManager.CallAtlasFunc(RealmManager.AtlasFunc.InitPlayerData, new Dictionary<string, object> {
                { "AuthType", AuthType.Guest.ToString() }
            });
            Debug.LogError("finish call func");
            RealmManager.GetDatas();
            //FirebaseManager.SignUp(_authType, data => {//初始化玩家帳號完成後跑這裡
            //    FirebaseManager.LoadDatas(() => {
            //        StartManager.Instance.SetVersionText();//顯示下方文字
            //        StartDownloadingAssetAndGoNextScene();
            //    });
            //});
        }
        /// <summary>
        /// 登出帳戶，按下後會登出並顯示回需要登入狀態
        /// </summary>
        public void Logout() {
            //PopupUI_Local.ShowConfirmCancel(StringData.GetUIString("LogoutAccountCheck"), GameSettingData.GetInt(GameSetting.LogoutCowndownSecs), () => {
            //    StartCoroutine(FirebaseManager.Logout(() => {//登出
            //        ShowUI(Condietion.BackFromLobby_ShowLoginBtn);
            //    }));
            //}, null);
        }
        /// <summary>
        /// 移除帳戶，按下後會解除所有平台綁定並登出並顯示回需要登入狀態
        /// </summary>
        public void DeleteAccount() {
            PopupUI_Local.ShowConfirmCancel(StringJsonData.GetUIString("DeleteAccountCheck"), GameSettingJsonData.GetInt(GameSetting.LogoutCowndownSecs), () => {
                UnlinkAllPlatfromsAndLogout();
            }, null);
        }
        /// <summary>
        /// 遞迴解綁所有平台並登出帳戶
        /// </summary>
        void UnlinkAllPlatfromsAndLogout() {
            //if (FirebaseManager.IsLinkingAnyThirdPart) {
            //    if (FirebaseManager.IsLinkingThrdPart(ThirdPartLink.Facebook)) {//還沒綁定就進行綁定
            //        PopupUI_Local.ShowLoading(StringData.GetUIString("UnLinkingFB"));
            //        FirebaseManager.UnlinkFacebook(result => {
            //            PopupUI_Local.HideLoading();
            //            UnlinkAllPlatfromsAndLogout();
            //        });
            //        return;
            //    }
            //    if (FirebaseManager.IsLinkingThrdPart(ThirdPartLink.Apple)) {//還沒綁定就進行綁定
            //        PopupUI_Local.ShowLoading(StringData.GetUIString("UnLinkingApple"));
            //        FirebaseManager.UnlinkApple(result => {
            //            PopupUI_Local.HideLoading();
            //            UnlinkAllPlatfromsAndLogout();
            //        });
            //        return;
            //    }
            //    if (FirebaseManager.IsLinkingThrdPart(ThirdPartLink.Google)) {//還沒綁定就進行綁定
            //        PopupUI_Local.ShowLoading(StringData.GetUIString("UnLinkingGoogle"));
            //        FirebaseManager.UnlinkGoogle(result => {
            //            PopupUI_Local.HideLoading();
            //            UnlinkAllPlatfromsAndLogout();
            //        });
            //        return;
            //    }
            //} else {
            //    PopupUI_Local.ShowLoading(StringData.GetUIString("Loading"));
            //    StartCoroutine(FirebaseManager.Logout(() => {//登出
            //        PopupUI_Local.HideLoading();
            //        ShowUI(Condietion.BackFromLobby_ShowLoginBtn);
            //        PlayerPrefs.DeleteAll();//清除所有本機資料
            //    }));
            //}
        }
        void FBAuth() {
            //PopupUI_Local.ShowLoading(string.Format("Loading"));
            //FirebaseManager.SignInWithFacebook(result => {
            //    PopupUI_Local.HideLoading();
            //    if (!result) {
            //        PopupUI_Local.ShowClickCancel(StringData.GetUIString("SigninFail"), null);
            //    } else {
            //        OnThirdPartAuthFinished(AuthType.Facebook);
            //    }
            //});
        }

        void AppleAuth() {
            //PopupUI_Local.ShowLoading(string.Format("Loading"));
            //FirebaseManager.SignInWithApple(result => {
            //    PopupUI_Local.HideLoading();
            //    if (!result) {
            //        PopupUI_Local.ShowClickCancel(StringData.GetUIString("SigninFail"), null);
            //    } else {
            //        OnThirdPartAuthFinished(AuthType.Apple);
            //    }
            //});
        }
        void GoogleAuth() {
            //PopupUI_Local.ShowLoading(string.Format("Loading"));
            //FirebaseManager.SignInWithGoogle(result => {
            //    PopupUI_Local.HideLoading();
            //    if (!result) {
            //        PopupUI_Local.ShowClickCancel(StringData.GetUIString("SigninFail"), null);
            //    } else {
            //        OnThirdPartAuthFinished(AuthType.Google);
            //    }
            //});
        }
        /// <summary>
        /// 三方登入驗證完成跑這裡
        /// </summary>
        void OnThirdPartAuthFinished(AuthType _authType) {
            // 通知分析註冊完成事件
            CompleteRegistrationEvent(_authType);

            //FirebaseManager.GetDataByDocID(ColEnum.Player, FirebaseManager.MyUser.UserId, (colName, data) => {
            //    CheckIfNeedInitializePlayerData(_authType, data);
            //});


        }


        public void SetMiddleText(string _str) {
            MiddleText.text = _str;
        }
        public void StartDownloadingAssetAndGoNextScene() {
            ShowUI(Condition.HideAll);
            StartSceneManager.Instance.StartDownloadingAssetAndGoNextScene();//進入下一個場景
        }

        /// <summary>
        /// 完成分析相關的註冊事件
        /// </summary>
        private void CompleteRegistrationEvent(AuthType authType) {

#if APPSFLYER
            // 設定玩家UID
            AppsFlyerManager.Inst.SetCustomerUserId(FirebaseManager.MyUser.UserId);
            // AppsFlyer紀錄玩家登入
            AppsFlyerManager.Inst.CompleteRegistration(FirebaseManager.MyUser.UserId, authType.ToString(), FirebaseManager.LanguageCode);
#endif

#if !UNITY_EDITOR && FIREBASE_ANALYTICS
            FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
            // 設定Firebase UserId
            FirebaseAnalytics.SetUserId(FirebaseManager.MyUser.UserId);
            // 註冊登入來源
            FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventSignUp,
            new Parameter[] {
            new Parameter(FirebaseAnalytics.ParameterMethod, authType.ToString())}
            );

            // 記錄登入事件
            Parameter[] RegistrationParameters = {
                          new Parameter("userId", FirebaseManager.MyUser.UserId),
                          new Parameter("authType", authType.ToString()),
#if UNITY_IOS
                          new Parameter("platform", "Apple"),
#elif UNITY_ANDROID
                          new Parameter("platform", "Google"),
#endif
                          new Parameter("languageCode", FirebaseManager.LanguageCode)
                        };
            // 紀錄完成註冊事件
            FirebaseAnalytics.LogEvent("COMPLETE_REGISTRATION", RegistrationParameters);
#endif
        }

        /// <summary>
        /// 使用者條款
        /// </summary>
        public void OnTermsOfUseClick() {
            //PopupUI_Local.ShowLoading(StringData.GetUIString("Loading"));
            //FirebaseManager.GetDataByDocID(ColEnum.GameSetting, "BackendURL", (col, dic) => {
            //    PopupUI_Local.HideLoading();
            //    string backendAddress = dic[BackendURLType.BackendAddress.ToString()].ToString();
            //    string userContractURL = dic[BackendURLType.UserContractURL.ToString()].ToString();
            //    string showUrl = string.Concat(backendAddress, userContractURL);
            //    WriteLog.Log($"[OnTermsOfUseClick] showUrl={showUrl}");
            //    Rect rect = new Rect(0, 0, Screen.width, Screen.height);
            //    WebViewManager.Inst.ShowWebview(showUrl, rect);
            //});
        }

        /// <summary>
        /// 隱私權條款
        /// </summary>
        public void OnProtectionPolicyClick() {
            //PopupUI_Local.ShowLoading(StringData.GetUIString("Loading"));
            //FirebaseManager.GetDataByDocID(ColEnum.GameSetting, "BackendURL", (col, dic) => {
            //    PopupUI_Local.HideLoading();
            //    string backendAddress = dic[BackendURLType.BackendAddress.ToString()].ToString();
            //    string protectionPolicyURL = dic[BackendURLType.ProtectionPolicyURL.ToString()].ToString();
            //    string showUrl = string.Concat(backendAddress, protectionPolicyURL);
            //    WriteLog.Log($"[OnProtectionPolicyClick] showUrl={showUrl}");
            //    Rect rect = new Rect(0, 0, Screen.width, Screen.height);
            //    WebViewManager.Inst.ShowWebview(showUrl, rect);
            //});
        }

        public void OnClearBundleClick() {
            AddressableManage.Instance.ReDownload();
        }

        public void OnQuestReportButtonClick() {

            //PopupUI_Local.ShowLoading(StringData.GetUIString("Loading"));
            //string version = Application.version;
            //FirebaseManager.GetDataByDocID(ColEnum.GameSetting, "BackendURL", (col, dic) => {
            //    PopupUI_Local.HideLoading();
            //    string backendAddress = dic[BackendURLType.BackendAddress.ToString()].ToString();
            //    string customerServiceURL = dic[BackendURLType.CustomerServiceURL.ToString()].ToString();
            //    string uid = FirebaseManager.MyUser?.UserId ?? "";
            //    string addUserURL = string.Format(customerServiceURL, version, uid);
            //    string showURL = string.Concat(backendAddress, addUserURL);
            //    WriteLog.Log($"[OnQuestReportButtonClick] showURL = {showURL}, version={version}, uid={uid}");
            //    Rect rect = new Rect(0, 0, Screen.width, Screen.height);
            //    WebViewManager.Inst.ShowWebview(showURL, rect);
            //});

        }


    }
}