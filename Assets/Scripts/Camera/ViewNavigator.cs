using Emc2.Scripts.Enums;
using System.Collections.Generic;

namespace Emc2.Scripts.Camera
{
    public sealed class ViewNavigator
    {
        private readonly IReadOnlyList<EAtelierView> _order;

        public ViewNavigator(IReadOnlyList<EAtelierView> order)
        {
            _order = order;
        }

        public EAtelierView? GetAdjacent(EAtelierView current, int direction)
        {
            int idx = -1;
            for (int i = 0; i < _order.Count; i++)
            {
                if (_order[i] == current) { idx = i; break; }
            }
            
            if (idx < 0) 
            {
                return null;
            }

            int next = idx + direction;
            if (next < 0 || next >= _order.Count) 
            {
                return null;
            }

            return _order[next];
        }
    }
}
