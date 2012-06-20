using System.Collections;

namespace TrainShareApp.ViewModels
{
    public class HistoryControlViewModel
    {
        public string From { get; set; }
        public string To { get; set; }
        public IEnumerable Checkins { get; set; }
        public bool Expanded { get; set; }

        public void Expand()
        {
            Expanded = true;
        }

        public void Collapse()
        {
            Expanded = false;
        }
    }
}