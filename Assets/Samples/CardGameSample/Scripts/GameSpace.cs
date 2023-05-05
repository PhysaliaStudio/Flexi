using System.Collections.Generic;
using UnityEngine;

namespace Physalia.Flexi.Samples.CardGame
{
    public class GameSpace : MonoBehaviour
    {
        [SerializeField]
        private Transform playersRoot;
        [SerializeField]
        private Transform enemiesRoot;
        [SerializeField]
        private UnitAvatar unitAvatarPrefab;

        [Header("Arranmgement")]
        [SerializeField]
        private float unitWidth = 1f;
        [SerializeField]
        private float unitGap = 0.2f;

        private readonly Dictionary<Unit, UnitAvatar> avatarTable = new();
        private readonly List<UnitAvatar> players = new();
        private readonly List<UnitAvatar> enemies = new();

        public UnitAvatar GetAvatar(Unit unit)
        {
            bool success = avatarTable.TryGetValue(unit, out UnitAvatar avatar);
            if (success)
            {
                return avatar;
            }

            return null;
        }

        public void CreateAvatar(Unit unit)
        {
            Transform root;
            List<UnitAvatar> list;
            if (unit.UnitType == UnitType.PLAYER)
            {
                root = playersRoot;
                list = players;
            }
            else
            {
                root = enemiesRoot;
                list = enemies;
            }

            UnitAvatar avatar = Instantiate(unitAvatarPrefab, root);
            UnitAvatarAnimation animationPrefab = unit.Data.AvatarPrefab;
            if (animationPrefab != null)
            {
                UnitAvatarAnimation animation = Instantiate(animationPrefab, avatar.transform);
                avatar.SetAnimation(animation);
            }

            avatar.SetUnit(unit);
            avatarTable.Add(unit, avatar);
            list.Add(avatar);
            Reposition(list);
        }

        public void RemoveAvatar(Unit unit)
        {
            bool success = avatarTable.TryGetValue(unit, out UnitAvatar avatar);
            if (!success)
            {
                return;
            }

            List<UnitAvatar> list;
            if (unit.UnitType == UnitType.PLAYER)
            {
                list = players;
            }
            else
            {
                list = enemies;
            }

            avatarTable.Remove(unit);
            list.Remove(avatar);
            Reposition(list);

            Destroy(avatar.gameObject);
        }

        private void Reposition(List<UnitAvatar> avatarList)
        {
            if (avatarList.Count == 0)
            {
                return;
            }

            if (avatarList.Count == 1)
            {
                avatarList[0].transform.localPosition = new Vector3(0f, 0f, 0f);
                return;
            }

            float totalWidth = unitWidth * avatarList.Count;
            totalWidth += unitGap * (avatarList.Count - 1);

            float leftMost = totalWidth * -0.5f;
            float current = leftMost;
            for (var i = 0; i < avatarList.Count; i++)
            {
                current += unitWidth * 0.5f;
                avatarList[i].transform.localPosition = new Vector3(current, 0f, 0f);

                current += unitWidth * 0.5f;
                current += unitGap;
            }
        }
    }
}
