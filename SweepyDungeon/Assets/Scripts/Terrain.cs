
namespace SweepyDungeon
{
	public class Terrain // 텍스트 버전으로 지형 구현시 이용되는 데이터 목록.
	{
		public const int EMPTY = 0;
		public const int GROUND = 1;
		public const int WALL = 2;

		public const int DOOR = 3;
		public const int DOOR_OPEN = 4;
		public const int DOOR_LOCKED = 5;
		public const int DOOR_HIDDEN = 6;

		public const int STAIRS_DOWN = 7;
		public const int STAIRS_UP = 8;

		public const int WATER = 9;
		public const int GENERATOR = 10;	
	}
}