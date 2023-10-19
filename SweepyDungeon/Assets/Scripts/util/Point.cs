using System;

namespace ArcanaDungeon.util
{
    public class Point //벡터2 대체용 2차원좌표 클래스. 존재하는 이유는 벡터2에서 비교가 아닌 방법으로 x나 y값을 직접 추출할수 없어서인것으로 기억.
                       //원본은 녹픽던 코드에서 찾아볼 수 있음.
    {
        public int x, y;

        public Point() { }// 기본생성자특) 아무것도안함
        public Point(Point p)//깊은복사 생성자
        {
            x = p.x;
            y = p.y;
        }
        public Point(int x, int y)//보통은 이렇게 생성한다
        {
            this.x = x;
            this.y = y;
        }

        public Point Set(int x, int y)//값 수정
        {
            this.x = x;
            this.y = y;
            return this;
        }
        public Point Set(Point p)//값 수정2. 이것도 사실상 깊은복사.
        {
            x = p.x;
            y = p.y;
            return this;
        }
        public Point Clone()//이 점에 대한 깊은복사 
        {
            return new Point(this);
        }
        public Point Scale(int n)//가로 세로 모두 n배
        {
            x *= n;
            y *= n;
            return this;
        }
        public Point Offset(int x, int y)//값 +-하기
        {
            this.x += x;
            this.y += y;
            return this;
        }
        public Point Offset(Point p)//값 +-하기. 그런데 다른 점 좌표를 쓰는.
        {
            x += p.x;
            y += p.y;
            return this;
        }
        public float Distance(Point p)//이 점과 p와의 거리.
        {
            return (float)Math.Sqrt(Math.Pow(p.x - x, 2) + Math.Pow(p.y - y, 2));
        }
        public override bool Equals(object obj)//기존 equals 대체. Point형인지 확인 후 좌표비교.
        {
            if (obj is Point)
            {
                Point p = (Point)obj;
                return p.x == x && p.y == y;
            }
            else
                return false;
        }
    }
}