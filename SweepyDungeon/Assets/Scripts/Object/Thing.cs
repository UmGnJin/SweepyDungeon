using System.Collections.Generic;
using UnityEngine;

namespace SweepyDungeon.Object
{
    public abstract class Thing : MonoBehaviour
    {
        protected int hp;
        public int maxhp = 100;
        protected int stamina;
        public int maxstamina = 100;
        public int power;

        public bool exhausted = false;
        protected int block;
        protected int vision_distance;
        public int isTurn;  //1 �̻��� ��� �� ��ü�� ���̴�, 0�� ��� �ܼ��� �� ��ü�� ���� �ƴ� ���̸�, ������ ��� ���� ���� ������ ���� ������ ���̴�

        
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
        }
        public void be_hit(int val)
        {
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
            HpChange(-val);
        }
        public void be_poisoned(int val)
        {
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
            /*if(this.gameObject.tag == "Player")
                Dungeon.dungeon.Plr.isturn_start = true;*/
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
        
    }
}
