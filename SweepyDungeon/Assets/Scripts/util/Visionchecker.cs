using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArcanaDungeon;

//http://www.roguebasin.com/index.php?title=FOV_using_recursive_shadowcasting <<< 매우 중요하다 필독
namespace ArcanaDungeon.util
{
    public class Visionchecker : MonoBehaviour
    {

        public static int[][] rounding;

        private static void temp_Visionchecker()
        {   //★이 함수는 단 1번만 실행되야 하는데 static으로만 사용할 클래스다 보니 생성자를 사용할 수가 없다, 나중에 방법을 찾자
            rounding = new int[15][];
            for (int i = 1; i <= 14; i++)
            {
                rounding[i] = new int[i + 1];
                for (int j = 1; j <= i; j++)
                {
                    rounding[i][j] = (int)Math.Min(j, Math.Floor(i * Math.Cos(Math.Asin((double)j / (double)i))));
                }
            }
        }

        //x와 y는 캐릭터의 pos를 levelr의 width로 나눠서 구한다
        public static void vision_check(int x, int y, int distance, bool[,] FOV)
        {
            temp_Visionchecker();//★사실 이건 매번 실행되는 게 아니라 1번만 실행되야 한다

            //플레이어 위치는 항상 시야가 밝혀진다
            FOV[x, y] = true;

            //scanOctant는 8등분된 원만큼 시야를 탐색한다
            scanOctant(FOV, distance, x, y, 1, 0, 1, 1, 1, false); //오른쪽 아래, 왼쪽
            scanOctant(FOV, distance, x, y, 1, 0, 1, 1, 1, true);    //오른쪽 아래, 오른쪽

            scanOctant(FOV, distance, x, y, 1, 0, 1, 1, -1, true);   //오른쪽 위, 오른쪽
            scanOctant(FOV, distance, x, y, 1, 0, 1, 1, -1, false);    //오른쪽 위, 왼쪽

            scanOctant(FOV, distance, x, y, 1, 0, 1, -1, -1, false);   //왼쪽 위, 오른쪽
            scanOctant(FOV, distance, x, y, 1, 0, 1, -1, -1, true);  //왼쪽 위, 왼쪽

            scanOctant(FOV, distance, x, y, 1, 0, 1, -1, 1, true);   //왼쪽 아래, 왼쪽 
            scanOctant(FOV, distance, x, y, 1, 0, 1, -1, 1, false);    //왼쪽 아래, 오른쪽
        }

        //row는 플레이어로부터 얼마나 떨어진 곳부터 스캔할지
        //lSlope, rSlope는 각각 스캔을 시작하고 끝낼 경계선의 기울기 역수
        //x_mirro와 y_mirror는 각각 x좌표와 y좌표의 부호가 바뀜을 의미하며, xy_mirror는 y=x 혹은 y=-x에 대해 대칭을 의미한다
        private static void scanOctant(bool[,] FOV, int distance, int x, int y,
            int row, double lSlope, double rSlope, int x_mirror, int y_mirror, bool xy_mirror)
        {
            bool still_blocking = false;
            int start, end, col, cur_x, cur_y = 0;

            //스캔은 가까운 곳에서 먼 곳 순서대로 진행한다, 어느 row부터 시작할지는 매개변수로 전달될 것이다
            for (; row <= distance; row++)
            {
                //start와 end는 이번 scanOctant에서 스캔을 시작할 열과 끝낼 열을 나타낸다.
                if (lSlope == 0)
                {
                    start = 0;
                }
                else
                {
                    start = (int)Math.Ceiling(row * lSlope);
                }

                if (rSlope == 1)
                {
                    end = rounding[distance][row];
                }
                else
                {
                    end = Math.Min(rounding[distance][row], (int)Math.Floor(row * rSlope));
                }

                //먼저 플레이어 좌표를 넣고 거기에서 스캔이 완료된 row를 건너뛰어서 이번 스캔을 시작할 지점을 찾는다
                //cur = y*Dungeon.dungeon.currentlevel.width+x
                cur_x = x;
                cur_y = y;

                //xy_mirror == true라면 y=x 혹은 y=-x에 대해 대칭시킨 것이다
                if (xy_mirror)
                {
                    //cur += x_mirror * row + y_mirror * start * Dungeon.dungeon.currentlevel.width;
                    cur_x += x_mirror * row;
                    cur_y += y_mirror * start;
                }
                else
                {
                    //cur += x_mirror * start + y_mirror * row * Dungeon.dungeon.currentlevel.width;
                    cur_x += x_mirror * start;
                    cur_y += y_mirror * row;
                }

                for (col = start; col <= end; col++)
                {
                    //어찌됐건 scanOctant가 스캔한 지역은 시야에 보인다, 스캔되지 않은 지역은 null로 남아서 구분된다
                    FOV[cur_x, cur_y] = true;

                    //장애물과 만나면 장애물의 왼쪽 지역을 스캔하는 새로운 scanOctant를 재귀실행한다, 장애물의 오른쪽은 원래 실행되던 scanOctant가 계속한다
                    if (Dungeon.dungeon.currentlevel.vision_blockings[cur_x, cur_y])
                    {
                        if (!still_blocking)
                        {
                            still_blocking = true;

                            //장애물의 왼쪽을 스캔하기 때문에 가장 왼쪽 칸을 스캔 중이었다면 의미없다
                            if (col != start)
                            {
                                //장애물의 왼쪽을 스캔할려면 장애물의 4개 꼭지점 중에 플레이어에게서 row는 멀고 col은 가까운 쪽을 새로운 경계선으로 봐야 한다, 따라서 rSlope 계산에 row는 +1을 한다
                                scanOctant(FOV, distance, x, y, row + 1, lSlope, (double)col / (double)(row + 1), x_mirror, y_mirror, xy_mirror);
                            }
                        }
                    }
                    else
                    {
                        if (still_blocking)
                        {
                            still_blocking = false;
                            //장애물의 오른쪽을 스캔하므로 왼쪽 경계선을 조정한다
                            lSlope = col / row;
                        }
                    }

                    //다음 스캔할 행,열로 좌표를 바꾼다, 물론 어느 축에 대해 대칭된 상태라면 좌표 이동 또한 조금 달라진다
                    if (xy_mirror)
                    {
                        cur_y += y_mirror;
                    }
                    else
                    {
                        cur_x += x_mirror;
                    }

                }

                //만약 row의 오른쪽 끝이 장애물이라면 장애물의 오른쪽을 스캔하는 scanOctant가 정지한다
                if (still_blocking) { break; }

            }
        }
    }
}