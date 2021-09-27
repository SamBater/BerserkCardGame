using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Multiplay.Model
{
    public class Weapon : Card
    {
        public override Kind TypeKind => Kind.Weapon;

        public Weapon(Card card) : base(card)
        {

        }

        public Weapon(int id, string name, int cost, string description, string functionDes, string imgPath) : base(id, name, cost, description, functionDes, imgPath)
        {
        }
    }
}
