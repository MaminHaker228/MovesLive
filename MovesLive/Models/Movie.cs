using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MovesLive.Models
{
    public class Movie : INotifyPropertyChanged
    {
        private int _id;
        private string _title;
        private string _description;
        private string _category;
        private int _year;
        private double _rating;
        private int _duration;
        private string _posterPath;
        private string _streamingUrl;
        private string _localSourcePath;

        public int Id
        {
            get => _id;
            set
            {
                _id = value;
                OnPropertyChanged();
            }
        }

        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged();
            }
        }

        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                OnPropertyChanged();
            }
        }

        public string Category
        {
            get => _category;
            set
            {
                _category = value;
                OnPropertyChanged();
            }
        }

        public int Year
        {
            get => _year;
            set
            {
                _year = value;
                OnPropertyChanged();
            }
        }

        public double Rating
        {
            get => _rating;
            set
            {
                _rating = value;
                OnPropertyChanged();
            }
        }

        public int Duration
        {
            get => _duration;
            set
            {
                _duration = value;
                OnPropertyChanged();
            }
        }

        public string PosterPath
        {
            get => _posterPath;
            set
            {
                _posterPath = value;
                OnPropertyChanged();
            }
        }

        public string StreamingUrl
        {
            get => _streamingUrl;
            set
            {
                _streamingUrl = value;
                OnPropertyChanged();
            }
        }

        public string LocalSourcePath
        {
            get => _localSourcePath;
            set
            {
                _localSourcePath = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}