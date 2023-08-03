using AoC18.Common;

namespace AoC18.Day13
{
    class MineCart
    {
        public Coord2D currentPosition;
        public Coord2D direction;
        public bool crashed = false;
        public int lastTurn = 2;   // 0 = left, 1 = straight, 2 = right

        public MineCart(Coord2D position, Coord2D direction)
        {
            currentPosition = position;
            this.direction = direction;
        }

        public Coord2D TurnLeft(Coord2D direction)
            =>  new Coord2D(direction.y, -direction.x);

        public Coord2D TurnRight(Coord2D direction)
            =>  new Coord2D(-direction.y, direction.x);


        public void Move(Dictionary<Coord2D, char> railMap)
        {
            currentPosition += direction;
            var element = railMap[currentPosition];
            if (element == '+')
            {
                direction = lastTurn switch
                {
                    2 => TurnLeft(direction),
                    0 => direction,
                    1 => TurnRight(direction),
                    _ => throw new Exception("Invalid lastTurn value")
                };

                lastTurn = (lastTurn + 1) % 3;
            }
            else if (element == '/')
                direction = new Coord2D(-direction.y, -direction.x);
            else if (element == '\\')
                direction = new Coord2D(direction.y, direction.x);
        }
    }

    class MineCartRunner
    {
        Dictionary<Coord2D, char> RailMap = new();
        List<MineCart> MineCarts = new();

        public void ParseInput(List<string> lines)
        {
            string cartChars = "<>^v";

            for(int row=0; row<lines.Count; row++) 
                for(int col = 0; col<lines[row].Length;col++)
                { 
                    var element = lines[row][col];
                    if (cartChars.Contains(element))
                    {
                        var position = new Coord2D(col, row);
                        var direction = new Coord2D(element == '<' ? -1 : element == '>' ? 1 : 0, 
                                                    element == '^' ? -1 : element == 'v' ? 1 : 0);

                        MineCarts.Add( new MineCart(position, direction));
                        element = element == '<' || element == '>' ? '-' : '|';
                    }
                    RailMap[new Coord2D(col, row)] = element;
                }
        }
      
        Coord2D RunCarts(int part = 1)
        {
            while (true)
            {
                MineCarts = MineCarts.OrderBy(c => c.currentPosition.y).ThenBy(c => c.currentPosition.x).ToList();

                foreach (var cart in MineCarts)
                {
                    cart.Move(RailMap);
                    if (MineCarts.Count(c => c.currentPosition == cart.currentPosition) > 1)
                    {
                        if (part == 1)
                            return cart.currentPosition;
                        else
                            MineCarts.Where(c => c.currentPosition == cart.currentPosition).ToList()
                                     .ForEach(c => c.crashed = true);
                    }
                }

                MineCarts.RemoveAll(c => c.crashed);
                
                if (MineCarts.Count == 1)
                    return MineCarts[0].currentPosition;
            }
        }

        public string Solve(int part = 1)
            => RunCarts(part).ToString();
    }
}
