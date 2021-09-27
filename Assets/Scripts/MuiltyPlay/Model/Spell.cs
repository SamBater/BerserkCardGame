using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Multiplay.Model
{
    public class Spell : Card
    {
        public Spell(Card card) : base(card)
        {

        }

        public Spell(int id, string name, int cost, string description, string functionDes, string imgPath) : base(id, name, cost, description, functionDes, imgPath)
        {

        }

        public override Kind TypeKind => Kind.Spell;
    }
}
