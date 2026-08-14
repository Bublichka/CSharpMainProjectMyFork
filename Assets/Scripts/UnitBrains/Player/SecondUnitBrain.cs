using GluonGui.Dialog;
using Model;
using Model.Runtime.Projectiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Utilities;
using static UnityEngine.GraphicsBuffer;

namespace UnitBrains.Player
{
    public class SecondUnitBrain : DefaultPlayerUnitBrain
    {
        public override string TargetUnitName => "Cobra Commando";
        private const float OverheatTemperature = 3f;
        private const float OverheatCooldown = 2f;
        private float _temperature = 0f;
        private float _cooldownTime = 0f;
        private bool _overheated;
        public List<Vector2Int> _targetNoRangeAttack = new();

        protected override void GenerateProjectiles(Vector2Int forTarget, List<BaseProjectile> intoList)
        {
            float overheatTemperature = OverheatTemperature;
            ///////////////////////////////////////
            // Homework 1.3 (1st block, 3rd module)
            ///////////////////////////////////////

            if (GetTemperature() >= overheatTemperature)
                return;
            else
            {
                for (int a = 0; a <= GetTemperature(); ++a)
            {
                var projectile = CreateProjectile(forTarget);
                AddProjectileToList(projectile, intoList);
            }
                IncreaseTemperature();
            }
        }

        ///////////////////////////////////////
        public override Vector2Int GetNextStep()
        {   
            if (_targetNoRangeAttack.Count == 0)
                return unit.Pos;
          
            Vector2Int target = _targetNoRangeAttack[0];

            if (IsTargetInRange(target))
                return unit.Pos;

            Vector2Int positionUnitPlayer = unit.Pos.CalcNextStepTowards(target);
            return positionUnitPlayer;
        }
        protected override List<Vector2Int> SelectTargets()//Метод выбора цели.
        {
            List<Vector2Int> result = new();//Список результатов.

            _targetNoRangeAttack.Clear();

            List<Vector2Int> allTargets = GetAllTargets().ToList();

            if (allTargets.Count == 0)
            {
                Vector2Int Base = runtimeModel.RoMap.Bases[
                IsPlayerUnitBrain ? RuntimeModel.BotPlayerId : RuntimeModel.PlayerId];
                allTargets.Add(Base);
            }
               
            Vector2Int closestTarget = allTargets[0];

            float minDistance = DistanceToOwnBase(closestTarget);
            foreach (var target in allTargets)
            {
                float distance = DistanceToOwnBase(closestTarget);
                
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestTarget = target;
                }
            }

            if (IsTargetInRange(closestTarget))
            {
                result.Add(closestTarget);
            }
            else
            {
                _targetNoRangeAttack.Add(closestTarget);
            }
                
            return result;
        }


        ///////////////////////////////////////
        public override void Update(float deltaTime, float time)
        {
            if (_overheated)
            {              
                _cooldownTime += Time.deltaTime;
                float t = _cooldownTime / (OverheatCooldown/10);
                _temperature = Mathf.Lerp(OverheatTemperature, 0, t);
                if (t >= 1)
                {
                    _cooldownTime = 0;
                    _overheated = false;
                }
            }
        }

        private int GetTemperature()
        {
            if(_overheated) return (int) OverheatTemperature;
            else return (int)_temperature;
        }

        private void IncreaseTemperature()
        {
            _temperature += 1f;
            if (_temperature >= OverheatTemperature) _overheated = true;
        }
    }
}

//if (== 0)
//{
//    return unit.Pos;
//}
//if (> 0)
//{
//    Vector2Int positionUnitPlayer = unit.Pos;
//    Vector2Int nextPositionPlayer = _targetNoRangeAttack;
//    positionUnitPlayer = positionUnitPlayer.CalcNextStepTowards(nextPositionPlayer);

//    return nextPositionPlayer;
//}

//List<Vector2Int> _targetNoRangeAttack = new List<Vector2Int>();//Список для целей вне зоны атаки.

//var EnemyBase = runtimeModel.RoMap.Bases[
//    IsPlayerUnitBrain ? RuntimeModel.BotPlayerId : RuntimeModel.PlayerId];//Получаем базу и определяем чья она.


//float minDistance = float.MaxValue;//Код для опеределения ближайшей цели.
//if (result.Count == 0)
//{
//    return result;
//}
//Vector2Int closestTarget = result[0];
//foreach (Vector2Int Target in result)
//{
//    if (DistanceToOwnBase(Target) < minDistance)
//    {
//        closestTarget = Target;
//        minDistance = DistanceToOwnBase(Target);
//    }
//}
//result.Clear();
//result.Add(closestTarget);

//return result;