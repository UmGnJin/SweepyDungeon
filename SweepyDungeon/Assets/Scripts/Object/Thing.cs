using System.Collections.Generic;
using UnityEngine;

namespace ArcanaDungeon.Object
{
    public abstract class Thing : MonoBehaviour
    {
        /*protected int hp;
        public int maxhp = 100;
        protected int stamina;
        public int maxstamina = 100;
        public int power;

        public bool exhausted = false;
        protected int block;
        protected int vision_distance;
        public int isTurn;  //1 �̻��� ��� �� ��ü�� ���̴�, 0�� ��� �ܼ��� �� ��ü�� ���� �ƴ� ���̸�, ������ ��� ���� ���� ������ ���� ������ ���̴�

        public List<int> route_pos = new List<int>();  //������������ �̵� ���, �̵��� �׻� route_pos[0]���� �̵��ؼ� ����ȴ�

        private Dictionary<int, int> condition;  //�����̻� �� ���� ǥ��, key�� �����̻� �����̸� value�� ���ӽð�, key�� ���� ȿ�� : 0=���� / 1=���� / 2=�޷� / 3=�ߵ� / 4 = Ǯ����(�����ʿ�) / 5 = ��ȭ

        new public string name;

        public Thing()
        {
            condition = new Dictionary<int, int>();
            this.vision_distance = 6;
            this.hp = 0;
            this.stamina = 0;
        }

        public abstract void Spawn();

        //hp ���� �Լ�
        public int GetHp()
        {
            return this.hp;
        }

        public void HpChange(int val)
        {
            //new void HpChange(val){
            //  //ü���� 1 ���� ������ �� 1 ����
            //  super.HpChange(val);
            if (val < 0)
            {
                BlockChange(val);
                if (block + val < 0) { val += block; } else { val = 0; }
            }

            if (val > 0)
            {
                this.hp = Mathf.Clamp(this.hp + val, 0, this.maxhp);
            }
            else
            {

                this.hp += val;
                if (this.hp <= 0)
                {
                    this.die();
                }
            }
            UI.uicanvas.GaugeChange();  //�÷��̾� ü��/���¹̳�/�� ����, player�� ���� �ϴµ� �ݺ��Ǵ� �κ��� �� ���� �׳� ���⿡ ��������
        }
        public void be_hit(int val)
        {
            UI.uicanvas.blood(transform.position);
            HpChange(-val);
            if (this.condition.ContainsKey(5))
            {
                if (this.condition[5] > 0)
                {
                    this.HpChange(this.condition[5] / 10); // this.condition[5]�� ������ ���� ���� ��ȭ ����
                }
            }
        }
        public void be_fired(int val)
        {
            UI.uicanvas.fire(transform.position);
            HpChange(-val);
        }
        public void be_poisoned(int val)
        {
            UI.uicanvas.poison(transform.position);
            HpChange(-val);
        }

        //stamina ���� �Լ�
        public int GetStamina()
        {
            return stamina;
        }

        public void StaminaChange(int val)
        {
            if (val > 0)
            {
                if (this.stamina + val > this.maxstamina)
                {
                    this.stamina = this.maxstamina;
                }
                else
                {
                    this.stamina += val;
                }
            }
            else
            {
                this.stamina += val;
                if (this.stamina < 0)
                {
                    this.stamina = 0;
                }
            }
            UI.uicanvas.GaugeChange();
        }

        //block ���� �Լ�
        public int GetBlock()
        {
            return this.block;
        }

        public void BlockChange(int val)
        {
            if (val > 0)
            {
                this.block += val;
            }
            else
            {
                if (this.block + val < 0)
                {
                    this.block = 0;
                }
                else
                {
                    this.block += val;
                }
            }
        }

        //���̵� ���� �Լ�, ���� �ʿ� ����̴� �����̰� ���� �� ������ ��
        public void move() { }

        public void route_BFS(int dest_x, int dest_y)    //���� �켱 Ž������ ������������ ��θ� route_pos�� �������ִ� �Լ�
        {
            //route_BFS������ ��ǥ�� x+y*(�� �ʺ�)�� ��Ÿ����. BFS �˰����� �ִ��� �����ϰ� �����ϱ� ���� �ε����ϰ� ��ǥ�� int ���� 1���� ��Ÿ�� ���̴�
            int destination = dest_x + dest_y * Dungeon.dungeon.currentlevel.width;
            Queue<int> checking = new Queue<int>();
            int[] prev = new int[Dungeon.dungeon.currentlevel.length];
            int[] dir = new int[] { -1, Dungeon.dungeon.currentlevel.width, 1, -Dungeon.dungeon.currentlevel.width, -1 + Dungeon.dungeon.currentlevel.width, 1 + Dungeon.dungeon.currentlevel.width, 1 - Dungeon.dungeon.currentlevel.width, -1 - Dungeon.dungeon.currentlevel.width };

            checking.Enqueue((int)(transform.position.x + transform.position.y * Dungeon.dungeon.currentlevel.width));
            int temp, temp2;
            while (checking.Count > 0)
            {
                //�ֺ� ��ǥ ���� �� Ȯ���ؾ� �ϴ� �� : �ڱ� �ڽ��� cur_pos�� �ƴѰ�, passable�ΰ�?, level�� length ���� �̳��� �����ΰ�, prev[i]==null (�̹� üũ�� ĭ)�ΰ�, �ٸ� ���Ϳ� ��ġ�� �ʴ°�
                for (int ii = 0; ii < 8; ii++)
                {
                    temp = checking.Peek() + dir[ii];
                    //Debug.Log((temp % Dungeon.dungeon.currentlevel.width) + " / " + (temp / Dungeon.dungeon.currentlevel.width));
                    if ((transform.position.x + transform.position.y * Dungeon.dungeon.currentlevel.width != temp) &
                            ((Terrain.thing_tag[Dungeon.dungeon.currentlevel.map[temp % Dungeon.dungeon.currentlevel.width, temp / Dungeon.dungeon.currentlevel.width]] & Terrain.passable) != 0) &
                            (temp > 0 & temp < Dungeon.dungeon.currentlevel.length) &
                            (prev[temp] == 0) &
                            (Dungeon.dungeon.find_enemy(temp % Dungeon.dungeon.currentlevel.width, temp / Dungeon.dungeon.currentlevel.width) == null))
                    {
                        checking.Enqueue(temp);
                        prev[temp] = checking.Peek();
                    }
                }

                //Plr_pos[0]�̶� ���� ��ǥ���� Ȯ��, ������ prev �迭 �� Ÿ��ö󰡸鼭 route_pos�� ����
                if (checking.Peek() == destination)
                {
                    temp2 = checking.Peek();
                    route_pos.Clear();
                    while (prev[temp2] != 0)
                    {
                        route_pos.Insert(0, temp2);
                        temp2 = prev[temp2];
                    }
                    break;
                }
                checking.Dequeue();
            }

            return;
        }

        //�����̻� ó�� ���� �Լ�
        public void condition_process() // ��ȣ�� �� �����̻� �̸� �� ȿ�� ����ٶ�.
        {
            if (this.condition.ContainsKey(0))
            {    //���� - ������ ���� ���ظ� ���� �ϵ��� �޴´�. �ߵ��� ���� �ʱ� ��ġ�� ���ƾ� �Ѵ�. ���� ���� �� ��� �����Ǿ�� �Ѵ�.
                if (this.condition[0] > 0)
                {
                    be_fired(10);
                    this.condition[0] -= 1;
                }
            }
            if (this.condition.ContainsKey(1))
            {    //���� - 1�ϵ��� �ൿ�� �� ����.(���ѽ��� ������ ��å�� �ʿ��Ҽ� ����)
                if (this.condition[1] > 0)
                {
                    this.isTurn -= 1;
                    this.condition[1] -= 1;
                }
            }
            if (this.condition.ContainsKey(2))
            {    //�޷�
                if (this.condition[2] > 0)
                {
                    StaminaChange(15);
                    this.condition[2] -= 1;
                }
            }
            if (this.condition.ContainsKey(3))
            {    //�ߵ� - ��ø�� �����̻�. ��ø Ƚ���� ���� ���� ���ظ� �ް�, ��ø�� 1 �����Ѵ�. �̷��� ��ø�� 0�� �� ���, �ߵ��� �����ȴ�.
                if (this.condition[3] > 0)
                {
                    be_poisoned(condition[3]);
                    this.condition[3] -= 1;
                }
            }
            if (this.condition.ContainsKey(5))
            {    // ��ȭ�� ���� �ڸ��� ���� �ϼ� 10���� ���� ���� ��������
                if (this.condition[5] > 0)
                {
                    if(this.condition[5] % 10 == 9)
                    {
                        this.condition[5] = 0;
                    }
                    if (this.condition[5] % 10 > 3) // 14 
                    {
                        this.condition[5] = (this.condition[5] / 10) * 10 + 3; // 12
                    }
                    else if (this.condition[5] % 10 > 0)
                    {
                        this.condition[5] -= 1; // 11 10
                    }
                    else
                        this.condition[5] = 0; // 0
                    Debug.Log("�� ��ȭ �� " + this.condition[5]);
                }
            }
            List<int> temp = new List<int>(this.condition.Keys);
            foreach (int i in temp)
            {
                if (this.condition[i] <= 0)
                {
                    this.condition.Remove(i);
                }
            }
        }

        public void condition_add(int key, int val)
        { // key�� �����̻� ��ȣ, val�� �����̻� ��ġ. �ߵ� 2�� ���, key�� 3, val�� 2
            if (condition.ContainsKey(key))
            {
                condition[key] += val;
            }
            else
            {
                condition.Add(key, val);
            }
        }

        public Dictionary<int, int> GetCondition()
        {
            return this.condition;
        }



        public abstract void die();//�ڳ��߿� �ڱ��ڽ��� map[]���� �����ϴ� ������ �־����

        public void Turnend()
        {
            if(this.gameObject.tag == "Player")
                Dungeon.dungeon.Plr.isturn_start = true;
            this.condition_process();
            this.StaminaChange(5);
            this.isTurn -= 1;
        }

        public List<float[]> range_check(float dest_x, float dest_y)
        {
            //�� ������ ���� ��ǥ���� (dest_x,dest_y)���� �´�� �簢�� ��ǥ���� ���ؿ�
            List<float[]> result = new List<float[]>();
            //��ǥ ������ ���� ���̰� ���� �������̶�� x��ǥ�� �ٲ㰡�� result�� ����
            if (dest_y - transform.position.y == 0)
            {
                int temp_var = dest_x - transform.position.x > 0 ? 1 : -1;
                for (float i = transform.position.x; i * temp_var <= dest_x * temp_var; i += temp_var)
                {
                    result.Add(new float[2] { i, dest_y });
                }
                //��ǥ ������ ���� ���̰� ���� �������̶�� y��ǥ�� �ٲ㰡�� result�� ����
            }
            else if (dest_x - transform.position.x == 0)
            {
                int temp_var = dest_y - transform.position.y > 0 ? 1 : -1;
                for (float i = transform.position.y; i * temp_var <= dest_y * temp_var; i += temp_var)
                {
                    result.Add(new float[2] { dest_x, i });
                }
                //�������� �ƴ϶�� 
            }
            else
            {
                float x_cur = transform.position.x; float y_cur = transform.position.y; //���� Ȯ�� ���� ��ǥ, Ÿ�� ���� ���� float���� ������ ��
                float slope = (dest_x - x_cur) / (dest_y - y_cur);
                int x_gap = dest_x - x_cur > 0 ? 1 : -1;
                int y_gap = dest_y - y_cur > 0 ? 1 : -1;
                float y_changing_at_x = x_cur + y_gap * slope / 2 + x_gap * 0.5f; //y��ǥ�� ���� ���� x��ǥ��, �Ҽ��� �Ʒ��� 0.0 ~ 0.5 ��� �Ʒ� for���� ���ǹ����� �ش� ĭ�� ���簢���� �������� �ش� ĭ�� ��ǥ�����ٴ� �۴�, ���� ������ y_changin_x�� 0.5f�� ���� ������ �� ���� ���´�
                //ù y��ǥ 0.5 ��ȭ�ϴ� ����
                for (; x_cur * x_gap < y_changing_at_x * x_gap; x_cur += x_gap)
                {
                    result.Add(new float[2] { x_cur, y_cur });
                }
                y_cur += y_gap;
                //�߰�
                for (; y_cur * y_gap < dest_y * y_gap; y_cur += y_gap)
                {
                    if (y_changing_at_x % 1 != 0)
                    {
                        result.Add(new float[2] { x_cur - x_gap, y_cur });
                    }
                    y_changing_at_x += y_gap * slope;
                    for (; x_cur * x_gap < y_changing_at_x * x_gap; x_cur += x_gap)
                    {
                        result.Add(new float[2] { x_cur, y_cur });
                    }
                }
                //������ y��ǥ�� 0.5 ��ȭ�ϴ� ����
                if (y_changing_at_x % 1 != 0)
                {
                    result.Add(new float[2] { x_cur - x_gap, y_cur });
                }
                for (; x_cur * x_gap <= dest_x * x_gap; x_cur += x_gap)
                {
                    result.Add(new float[2] { x_cur, y_cur });
                }
            }
            return result;
        }
        public Thing range_attack(int dest_x, int dest_y, bool by_player)   //���Ÿ� ����, �ں��� ���̿� �ΰ� ���� �� ������ �Ұ����ϵ��� ���� �ʿ�
        {
            //�� ������ ���� ��ǥ���� (dest_x,dest_y)���� �´�� �簢�� ��ǥ���� ���ؿ�
            List<float[]> result = new List<float[]>();
            //��ǥ ������ ���� ���̰� ���� �������̶�� x��ǥ�� �ٲ㰡�� result�� ����
            if (dest_y - transform.position.y == 0)
            {
                int temp_var = dest_x - transform.position.x > 0 ? 1 : -1;
                for (float i = transform.position.x; i * temp_var <= dest_x * temp_var; i += temp_var)
                {
                    result.Add(new float[2] { i, dest_y });
                }
            }
            //��ǥ ������ ���� ���̰� ���� �������̶�� y��ǥ�� �ٲ㰡�� result�� ����
            else if (dest_x - transform.position.x == 0)
            {
                int temp_var = dest_y - transform.position.y > 0 ? 1 : -1;
                for (float i = transform.position.y; i * temp_var <= dest_y * temp_var; i += temp_var)
                {
                    result.Add(new float[2] { dest_x, i });
                }
            }
            //�������� �ƴ϶�� 
            else
            {
                float x_cur = transform.position.x; float y_cur = transform.position.y; //���� Ȯ�� ���� ��ǥ, Ÿ�� ���� ���� float���� ������ ��
                float slope = (dest_x - x_cur) / (dest_y - y_cur);
                int x_gap = dest_x - x_cur > 0 ? 1 : -1;
                int y_gap = dest_y - y_cur > 0 ? 1 : -1;
                float y_changing_at_x = x_cur + y_gap * slope / 2 + x_gap * 0.5f; //y��ǥ�� ���� ���� x��ǥ��, �Ҽ��� �Ʒ��� 0.0 ~ 0.5 ��� �Ʒ� for���� ���ǹ����� �ش� ĭ�� ���簢���� �������� �ش� ĭ�� ��ǥ�����ٴ� �۴�, ���� ������ y_changin_x�� 0.5f�� ���� ������ �� ���� ���´�
                //ù y��ǥ 0.5 ��ȭ�ϴ� ����
                for (; x_cur * x_gap < y_changing_at_x * x_gap; x_cur += x_gap)
                {
                    result.Add(new float[2] { x_cur, y_cur });
                }
                y_cur += y_gap;
                //�߰�
                for (; y_cur * y_gap < dest_y * y_gap; y_cur += y_gap)
                {
                    if (y_changing_at_x % 1 != 0)
                    {
                        result.Add(new float[2] { x_cur - x_gap, y_cur });
                    }
                    y_changing_at_x += y_gap * slope;
                    for (; x_cur * x_gap < y_changing_at_x * x_gap; x_cur += x_gap)
                    {
                        result.Add(new float[2] { x_cur, y_cur });
                    }
                }
                //������ y��ǥ�� 0.5 ��ȭ�ϴ� ����
                if (y_changing_at_x % 1 != 0)
                {
                    result.Add(new float[2] { x_cur - x_gap, y_cur });
                }
                for (; x_cur * x_gap <= dest_x * x_gap; x_cur += x_gap)
                {
                    result.Add(new float[2] { x_cur, y_cur });
                }
            }

            //���� ����� Thing�� ã�� ��ȯ�Ѵ�
            Thing closest = null;
            int closest_distance = 999;
            foreach (float[] r in result)
            {
                //���� üũ
                foreach (GameObject t in Dungeon.dungeon.enemies[Dungeon.dungeon.currentlevel.floor - 1])
                {
                    if (t != this.gameObject & r[0] == t.transform.position.x & r[1] == t.transform.position.y & Dungeon.distance_cal(transform, t.transform) < closest_distance)
                    {
                        closest = t.GetComponent<Enemy>();
                        closest_distance = Dungeon.distance_cal(transform, t.transform);
                    }
                }
                //�÷��̾� üũ
                if (!by_player & r[0] == Dungeon.dungeon.Plr.transform.position.x & r[1] == Dungeon.dungeon.Plr.transform.position.y & Dungeon.distance_cal(transform, Dungeon.dungeon.Plr.transform) < closest_distance)
                {
                    closest = Dungeon.dungeon.Plr;
                    closest_distance = Dungeon.distance_cal(transform, Dungeon.dungeon.Plr.transform);
                }
                //passable üũ
                if ((Dungeon.dungeon.currentlevel.map[(int)r[0], (int)r[1]] & Terrain.passable) == 0)
                {
                    closest = null;
                }
            }

            if (closest != null) { UI.uicanvas.range_shot(this.gameObject, closest.gameObject); }
            return closest;
        }*/
    }
}
