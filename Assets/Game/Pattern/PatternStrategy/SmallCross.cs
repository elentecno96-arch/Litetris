using Game.Bulb.LightCube;
using Game.Pattern.Interface;
using System.Collections.Generic;
using UnityEngine;
using Game.Manager;

namespace Game.Pattern.Strategy
{
    public class SmallCross : IPatternStrategy
    {
        public List<LightCube> GetTargetCubes(int centerX, int centerY, int range)
        {
            List<LightCube> targets = new List<LightCube>();
            var board = BoardManager.Instance;

            AddIfValid(targets, board, centerX, centerY);
            AddIfValid(targets, board, centerX + 1, centerY);
            AddIfValid(targets, board, centerX - 1, centerY);
            AddIfValid(targets, board, centerX, centerY + 1);
            AddIfValid(targets, board, centerX, centerY - 1);

            return targets;
        }

        private void AddIfValid(List<LightCube> list, BoardManager board, int x, int y)
        {
            if (x >= 0 && x < board.BoardSize && y >= 0 && y < board.BoardSize)
            {
                var cube = board.GetCube(x, y);
                if (cube != null) list.Add(cube);
            }
        }
    }
}