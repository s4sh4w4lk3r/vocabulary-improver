using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace wpf.Model.Local
{
    /// <summary>
    /// Represents the rating value and allows values only from 0 to 10 inclusive.
    /// </summary>
    public class Rating
    {
        public int Value { get; private set; }
        protected Rating(int rating)
        {
            if (rating < 0 || rating > 10) throw new VIException($"The value must be from 0 to 10 inclusive. Value = {rating}");
            Value = rating;
        }
        public void Increase()
        {
            if (Value >= 10) return;
            Value++;
        } // Увеличивает рейтинг.
        public void Reduce()
        {
            if (Value <= 0) return;
            Value--;
        } // Уменьшает рейтинг.
    }
}
