using Game.Bulb.LightCube;
using Game.Script.Manager;
using Game.Pattern.Interface;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Pattern.PatternStrategy
{
    public class LinePattern : IPatternStrategy
    {
        public List<LightCube> GetTargetCubes(int centerX, int centerY, int range)
        {
            bool isHorizontal = Random.value > 0.5f;

            //루프 없이 보드 매니저가 미리 분류해둔 리스트를 즉시 반환
            if (isHorizontal)
            {
                //y축 위치(centerY)를 기준으로 가로 한 줄 배달
                return BoardManager.Instance.GetRow(centerY);
            }
            else
            {
                //x축 위치(centerX)를 기준으로 세로 한 줄 배달
                return BoardManager.Instance.GetColumn(centerX);
            }
            //List<LightCube> targets = new List<LightCube>();
            //bool isHorizontal = Random.value > 0.5f;
            //for (int i = 0; i < 13; i++)
            //{
            //    if (isHorizontal) targets.Add(BoardManager.Instance.GetCube(i, centerY));
            //    else targets.Add(BoardManager.Instance.GetCube(centerX, i));
            //}
            //return targets;
        }
    }
}
