using UnityEngine;
using UnityEditor;

namespace Ballistics
{
    [CustomEditor(typeof(WeaponController))]
    public class WeaponControllerEditor : Editor
    {
        WeaponController TargetWeaponController;

        private void OnEnable()
        {
            TargetWeaponController = (WeaponController)target;

            if (TargetWeaponController.TargetWeapon == null)
            {
                TargetWeaponController.TargetWeapon = TargetWeaponController.GetComponent<Weapon>();
            }
            if (TargetWeaponController.myMagazineController == null)
            {
                TargetWeaponController.myMagazineController = TargetWeaponController.GetComponent<MagazineController>();
            }
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("Weapon Controller Editor", EditorStyles.largeLabel);
            EditorGUILayout.Separator();

            EditorGUILayout.LabelField("通用设置", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;

            TargetWeaponController.TargetWeapon = (Weapon)EditorGUILayout.ObjectField("控制武器", TargetWeaponController.TargetWeapon, typeof(Weapon), true);
            TargetWeaponController.sound = (AudioSource)EditorGUILayout.ObjectField("音效控制", TargetWeaponController.sound, typeof(AudioSource), true);
            TargetWeaponController.particle = (ParticleSystem)EditorGUILayout.ObjectField("枪口特效", TargetWeaponController.particle, typeof(ParticleSystem), true);
            TargetWeaponController.ScopePos = (Transform)EditorGUILayout.ObjectField("狙击视野位置", TargetWeaponController.ScopePos, typeof(Transform), true);
            TargetWeaponController.ShootDelay = EditorGUILayout.FloatField("开火间隔", TargetWeaponController.ShootDelay);
            TargetWeaponController.RecoilAmount = EditorGUILayout.FloatField("后坐力系数", TargetWeaponController.RecoilAmount);
            TargetWeaponController.AttackTime = EditorGUILayout.FloatField("后坐力枪口上翘时间", TargetWeaponController.AttackTime);
            TargetWeaponController.HoldTime = EditorGUILayout.FloatField("枪口上翘保持时间", TargetWeaponController.HoldTime);
            TargetWeaponController.ReleaseTime = EditorGUILayout.FloatField("枪口上翘恢复时间", TargetWeaponController.ReleaseTime);

            EditorGUILayout.Space();
            EditorGUI.indentLevel--;
            EditorGUILayout.LabelField("射击模式", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            TargetWeaponController.WeaponType = (ShootingType)EditorGUILayout.EnumPopup("模式", TargetWeaponController.WeaponType);
            EditorGUILayout.Separator();

            if (TargetWeaponController.WeaponType == ShootingType.Burst)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.LabelField("散弹模式", EditorStyles.label);
                EditorGUI.indentLevel++;
                TargetWeaponController.BulletsPerShot = EditorGUILayout.IntField("发射数量", TargetWeaponController.BulletsPerShot);
                TargetWeaponController.BurstSpreadAngle = EditorGUILayout.FloatField("散射角度", TargetWeaponController.BurstSpreadAngle);
                EditorGUI.indentLevel -= 2;
            }

            if (TargetWeaponController.WeaponType == ShootingType.Salves)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.LabelField("单次连发模式", EditorStyles.label);
                EditorGUI.indentLevel++;
                TargetWeaponController.BulletsPerShot = EditorGUILayout.IntField("连发数量", TargetWeaponController.BulletsPerShot);
                TargetWeaponController.SalveBulletShootDelay = Mathf.Clamp(EditorGUILayout.FloatField("连发间隔", TargetWeaponController.SalveBulletShootDelay), 0, TargetWeaponController.ShootDelay / TargetWeaponController.BulletsPerShot);
                EditorGUI.indentLevel -= 2;
            }

            EditorGUILayout.Space();

            TargetWeaponController.myMagazineController = (MagazineController)EditorGUILayout.ObjectField("弹夹控制器", TargetWeaponController.myMagazineController, typeof(MagazineController), true);
            if (GUI.changed) EditorUtility.SetDirty(TargetWeaponController);
        }
    }
}