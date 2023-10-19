using System;

namespace ArcanaDungeon.util
{
    public class Rect // 유니티 Rext로 대체 불가능한 이유 : Intersect()기능없음, 여기 기능 추가하기에는 어차피 코드 추가해야하는게 같고, 코드 수정량 많을 것으로 예상되니 그대로 감.
                      // 유니티 라이브러리에 Rect 클래스가 있으므로 사용시 using Rect = ArcanaDungeon.util.Rect;를 넣어주도록 하자.
    {
        public int x, y, xMax, yMax;//절대좌표상 x, y가 좌측상단. Max는 우측 / 하단.

        public Rect() : this(0, 0, 0 ,0)//매개변수 없는 생성자는 0, 0, 0, 0을 자동할당
        {

        }
        public Rect(Rect r)//깊은복사 생성자
        {
            x = r.x;
            y = r.y;
            xMax = r.xMax;
            yMax = r.yMax;
        }
        public Rect(int x, int y, int xMax, int yMax)//절대좌표 지정으로 생성. 보통 이 방법으로 생성.
        {
            this.x = x;
            this.y = y;
            this.xMax = xMax;
            this.yMax = yMax;
        }
        public void Set(int x, int y, int xMax, int yMax)//절대좌표 변경.
        {
            this.x = x;
            this.y = y;
            this.xMax = xMax;
            this.yMax = yMax;
        }
        public void SetPosition(int x, int y)//해당 좌표로 Rect를 이동. 좌표에 맞게 Max좌표가 변경됨.
        {
            int dx = this.x - x;
            int dy = this.y - y;
            this.x = x;
            this.y = y;
            xMax -= dx;
            yMax -= dy;
        }
        public void MovePosition(int x, int y)//해당 값만큼 그 방향으로 이동. Max좌표 역시 이동.
        {
            this.x += x;
            this.y += y;
            xMax += x;
            yMax += y;
        }

        public virtual int Width() { return xMax - x; } //너비
        public virtual int Height() { return yMax - y; } //높이

        public Rect Intersect(Rect r1)//자기 자신과 대상을 비교하도록 만들어진 Intersect. 두 도형간 겹치는 부분을 Rect로 만들어 제공한다.
                                      //정확히는 x, y좌표는 큰 값을, Max좌표는 작은 값을 받아 새 Rect를 만든다. 그렇기 때문에, 겹치지 않는 두 Rect간에는 너비가 높이가 0이나 음수인 경우가 발생할 수 있다.
                                      //이를 바탕으로 둘이 겹치는지 아닌지, 겹치면 얼마나인지, 거리는 어느 정도인지 등을 확인 가능하다.
        {
            Rect r = new Rect();
            r.x = Math.Max(r1.x, this.x);
            r.y = Math.Max(r1.y, this.y);
            r.xMax = Math.Min(r1.xMax, this.xMax);
            r.yMax = Math.Min(r1.yMax, this.yMax);

            return r;
        }
    }
}