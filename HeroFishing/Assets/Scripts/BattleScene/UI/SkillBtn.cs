using Scoz.Func;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HeroFishing.Battle {
    public class SkillButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler {
        public Transform Indicator; // 使用SpriteRenderer的方向指示器
        private Vector2 origin; // 按下按鈕的起始位置
        Camera SceneCam;

        private void Start() {
            SceneCam = GameObject.FindGameObjectWithTag("SceneCam").GetComponent<Camera>();
            Indicator.gameObject.SetActive(false); // 初始時隱藏指示器

        }

        public void OnPointerDown(PointerEventData eventData) {
            var pos = BattleManager.Instance.GetHero(0).transform.position;
            //Indicator.position = new Vector3(pos.x, pos.y + 0.1f, pos.z);
            origin = eventData.position;
            Indicator.gameObject.SetActive(true); // 顯示指示器
        }

        public void OnDrag(PointerEventData eventData) {
            Vector2 direction = eventData.position - origin;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Indicator.rotation = Quaternion.Euler(0, 90 - angle, 0);
        }

        public void OnPointerUp(PointerEventData eventData) {
            Vector2 direction = eventData.position - origin;
            ShootSkill(direction.normalized);
            Indicator.gameObject.SetActive(false); // 釋放技能後，隱藏指示器
        }

        void ShootSkill(Vector2 direction) {
            // 你的技能射擊邏輯
        }
        Vector3 GetTargetPos() {
            Vector3 worldPoint = UIPosition.GetMouseWorldPointOnYZero(-1);
            return worldPoint;
        }
    }
}