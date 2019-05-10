using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NM_Tool.CardHelper
{
    interface ICard
    {
        string ReadCardId();
        string ReadDefaltCardId();
        void IniCard(string card_id);
        void IniNewCard(string card_id);

    }
}
