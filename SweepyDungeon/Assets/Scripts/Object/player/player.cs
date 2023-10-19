using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using ArcanaDungeon;
using Terrain = ArcanaDungeon.Terrain;



namespace ArcanaDungeon.Object
{
    public class player : Thing
    {
        public player me = null;


        public float MovePower = 0.2f;
        public int MoveTimerLimit = 5;
        public SpriteRenderer spriteRenderer;
        public Animator anim;
        public Rigidbody2D rigid;
        public Transform tr;
        public Vector3 movement;

        public Vector2 PlayerPos = new Vector2(0, 0);
        public Vector2 MoveVector = new Vector2(0, 0);
        public Vector2 MousePos = new Vector2(0, 0);
        Camera cam;

        public int MoveTimer = 0;
        int Mou_x = 0;
        int Mou_y = 0;
        bool isMouseMove = false;
        bool canDraw = true;
        int drawCount = 0;

        public bool[,] FOV;

        public bool isturn_start;

        // Start is called before the first frame update
        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            anim = GetComponent<Animator>();
            rigid = GetComponent<Rigidbody2D>();
            tr = GetComponent<Transform>();
            cam = GameObject.Find("Main Camera").GetComponent<Camera>();
            if (me == null)
                me = this;

            maxhp = 100;
            //maxhp = 999;
            maxstamina = 100;
            HpChange(maxhp);
            StaminaChange(maxstamina);
            isturn_start = true;
        }
        

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                StaminaChange(10);
                Turnend();//�ϳѱ��
            }
            if (isTurn > 0)
            {
                if (isturn_start) {
                    isturn_start = false;
                    vision_marker();
                }
                if (MoveTimer <= 0)
                {
                    Get_MouseInput(); //���콺 �Է�
                    
                }
                if (Input.GetButton("Horizontal"))
                    spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;
            }
            PlayerPos = new Vector2(Mathf.Round(transform.position.x - 1), Mathf.Round(transform.position.y));
            vision_marker();//���� 2���� ���߿� ���� ����� �� �Լ��� �ϼ��Ǹ� �� ������ �Űܾ� ��
            
        }

        
        private void FixedUpdate()
        {//�Է¹޴°�
            
            if (MoveTimer > 0)
                MoveTimer--;
            if (transform.position.x == Mou_x && transform.position.y == Mou_y )
            {
                isMouseMove = false;
                
            }
          
            else if(MoveTimer <= 0 && isMouseMove == true)
            {
                
                try //���� ������ �̵��ϸ� �迭 �ε��� ������ ����ٴ� ������ ���, �ƹ����� ���� �̵��ϸ鼭 route_pos�� ������ ����� ������ ���δ�
                {
                    route_pos.RemoveAt(0);
                    MoveTimer = MoveTimerLimit;
                    this.Turnend();
                }
                catch (Exception e) { Debug.Log(e); }
            }
           

        }
        private void Get_MouseInput()
        {

            
            if (Input.GetMouseButtonDown(0) & !EventSystem.current.IsPointerOverGameObject())
            {
                MousePos = Input.mousePosition;
                MousePos = cam.ScreenToWorldPoint(MousePos);
                isMouseMove = true;



                //Debug.Log("SDF");
                Mou_x = Mathf.RoundToInt(MousePos.x);
                Mou_y = Mathf.RoundToInt(MousePos.y);
                route_BFS(Mou_x, Mou_y);
                
                //Debug.Log("x = " + Mou_x +"("+ MousePos.x + ") y = " + Mou_y +"(" + MousePos.y + ")");
            }
        }
        public override void Spawn()
        {
            /*for (int i = 0; i < Dungeon.dungeon.currentlevel.width; i++)
            {
                for (int j = 0; j < Dungeon.dungeon.currentlevel.height; j++)
                {
                    if (Dungeon.dungeon.currentlevel.map[i, j] == Terrain.STAIRS_UP)
                    {
                        PlayerPos = new Vector2(i, j + 1);
                        transform.position = new Vector2(i, j + 1);
                        return;
                    }
                    else
                        continue;
                }
            }*/
            Debug.Log("Cannot find Upstairs.");
        }//�ʿ� ���� ��, ��� �ڸ��� ����
        public void Spawn(Vector2 pos)
        {
            PlayerPos = pos;
            transform.position = pos;
            vision_marker();
        }// Ư�� ��ǥ�� ��ȯ
        
        private void vision_marker()
        {
            FOV = new bool[Dungeon.dungeon.currentlevel.width, Dungeon.dungeon.currentlevel.height];
            util.Visionchecker.vision_check((int)Mathf.Round(transform.position.x), (int)Mathf.Round(transform.position.y), this.vision_distance, FOV);


            //�������� RGB���� 0~1 ������ ��Ÿ���� �� �⺻������
            for (int i = 0; i < Dungeon.dungeon.currentlevel.width; i++)
            {
                for (int j = 0; j < Dungeon.dungeon.currentlevel.height; j++)
                {
                    if (FOV[i, j])
                    {
                        Dungeon.dungeon.currentlevel.temp_gameobjects[i, j].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
                        Enemy temp_enem = Dungeon.dungeon.find_enemy(i, j);
                        if ( temp_enem != null) {
                            temp_enem.gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
                            temp_enem.status_update();
                        }
                    }
                    else
                    {
                        Dungeon.dungeon.currentlevel.temp_gameobjects[i, j].GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 0.5f, 1);
                        Enemy temp_enem = Dungeon.dungeon.find_enemy(i, j);
                        if (temp_enem != null)
                        {
                            temp_enem.gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
                            temp_enem.status_hide();
                        }
                    }
                }
            }
        }

        public new void BlockChange(int val)
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
            UI.uicanvas.GaugeChange();
        }
        public override void die()
        {
            UI.uicanvas.Gameover();
        }
    }
}