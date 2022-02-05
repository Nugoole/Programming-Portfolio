using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALT.ResultViewer
{
    public class DateRangeCollection : ObservableCollection<DateValue>
    {
        private static object lockObject = new object();
        public TimeSpan TimeSpanForShow { get; set; }

        public DateRangeCollection():base()
        {
           
        }

        public new void Add(DateValue dateValue)
        {
            
            lock (lockObject)
            {
                if(this.Any(x=>x.Time.Equals(dateValue.Time)))
                {
                    this.Where(x => x.Time.Equals(dateValue.Time)).First().Value += dateValue.Value;
                    return;
                }
                    

                base.Add(dateValue);

                Refresh();
            }
        }


        
        public void Refresh()
        {
            if(TimeSpanForShow != TimeSpan.Zero)
            {
                var timeNow = DateTime.Now;

                List<DateValue> valuesForRemove = new List<DateValue>();

                foreach (DateValue val in this)
                {
                    if (val.Time < (timeNow - TimeSpanForShow))
                        valuesForRemove.Add(val);
                    else
                        break;
                }

                valuesForRemove.ForEach(x => Remove(x));
            }
        }
    }
}
