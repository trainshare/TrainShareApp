using System.ComponentModel;

namespace TrainShareApp.Model
{
    public abstract class EntityBase : INotifyPropertyChanged, INotifyPropertyChanging
    {
        /// <summary>
        /// Fires if a property has changed its value
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Fires if a property is about to change its value
        /// </summary>
        public event PropertyChangingEventHandler PropertyChanging;

        /// <summary>
        /// Notify that a property has changed
        /// </summary>
        /// <param name="propertyName">The name of the property</param>
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Notify that a property is about to change
        /// </summary>
        /// <param name="propertyName">The name of the property</param>
        protected void OnPropertyChanging(string propertyName)
        {
            if (PropertyChanging != null)
            {
                PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
            }
        }
    }
}