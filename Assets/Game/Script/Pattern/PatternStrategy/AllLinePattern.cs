using Game.Bulb.LightCube;
using Game.Script.Manager;
using Game.Pattern.Interface;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Pattern.PatternStrategy
{
    public class AllLinePattern : IPatternStrategy
    {
        private bool isVertical;
        public AllLinePattern(bool isVertical)
        {
            this.isVertical = isVertical;
        }

        public List<LightCube> GetTargetCubes(int centerX, int centerY, int range)
        {
            //이제 여기에서 연산 부분을 삭제하고 보드에서 리스트를 받아오도록 변경
            if (isVertical)
            {
                //세로줄 요청
                return BoardManager.Instance.GetColumn(centerX);
            }
            else
            {
                //가로줄 요청
                return BoardManager.Instance.GetRow(centerY);
            }
            //List<LightCube> targets = new List<LightCube>();
            //int boardSize = 9;

            //for (int i = 0; i < boardSize; i++)
            //{
            //    int x = isVertical ? centerX : i;
            //    int y = isVertical ? i : centerY;

            //    var cube = BoardManager.Instance.GetCube(x, y);
            //    if (cube != null) targets.Add(cube);
            //}
            //return targets;
        }
    }
}
