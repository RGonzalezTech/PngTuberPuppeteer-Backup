using System;
using System.Linq;
using System.Windows.Media.Imaging;

namespace PngTuber.Pupper
{
    public class ExpressionViewModel : ViewModelBase
    {
        private ExpressionSetModel expressionSet;
        public ExpressionViewModel(ExpressionSetModel expressionSet)
        {
            this.expressionSet = expressionSet;
            this.ReloadAllImages();
        }

        public ExpressionViewModel()
        {
            this.expressionSet = new ExpressionSetModel()
            {
                Name = "Untitled"
            };
        }

        private BitmapImage eyesOpenMouthOpen;
        public BitmapImage EyesOpenMouthOpen
        {
            get
            {
                return this.eyesOpenMouthOpen;
            }
            private set
            {
                if (this.eyesOpenMouthOpen == value)
                {
                    return;
                }

                this.eyesOpenMouthOpen = value;
                this.NotifyPropertyChanged();
            }
        }
        private BitmapImage eyesOpenMouthClose;
        public BitmapImage EyesOpenMouthClose
        {
            get
            {
                return this.eyesOpenMouthClose;
            }
            private set
            {
                if (this.eyesOpenMouthClose == value)
                {
                    return;
                }

                this.eyesOpenMouthClose = value;
                this.NotifyPropertyChanged();
            }
        }
        private BitmapImage eyesCloseMouthOpen;
        public BitmapImage EyesCloseMouthOpen
        {
            get
            {
                return this.eyesCloseMouthOpen;
            }
            private set
            {
                if (this.eyesCloseMouthOpen == value)
                {
                    return;
                }

                this.eyesCloseMouthOpen = value;
                this.NotifyPropertyChanged();
            }
        }
        private BitmapImage eyesCloseMouthClose;
        public BitmapImage EyesCloseMouthClose
        {
            get
            {
                return this.eyesCloseMouthClose;
            }
            private set
            {
                if (this.eyesCloseMouthClose == value)
                {
                    return;
                }

                this.eyesCloseMouthClose = value;
                this.NotifyPropertyChanged();
            }
        }

        private BitmapImage LoadImage(string bitmapPath)
        {
            try
            {
                var uri = new Uri(bitmapPath);
                var image = new BitmapImage(uri);
                image.Freeze();

                return image;
            }
            catch
            {
                return null;
            }
        }

        public string EyesOpenMouthOpenPath
        {
            get
            {
                return this.expressionSet.EyesOpenMouthOpenPath;
            }
            set
            {
                if (this.expressionSet.EyesOpenMouthOpenPath == value)
                {
                    return;
                }

                this.expressionSet.EyesOpenMouthOpenPath = value;
                this.NotifyPropertyChanged();

                this.EyesOpenMouthOpen = this.LoadImage(value);
            }
        }

        public string EyesOpenMouthClosePath
        {
            get
            {
                return this.expressionSet.EyesOpenMouthClosePath;
            }
            set
            {
                if (this.expressionSet.EyesOpenMouthClosePath == value)
                {
                    return;
                }

                this.expressionSet.EyesOpenMouthClosePath = value;
                this.NotifyPropertyChanged();

                this.EyesOpenMouthClose = this.LoadImage(value);
            }
        }

        public string EyesCloseMouthOpenPath
        {
            get
            {
                return this.expressionSet.EyesCloseMouthOpenPath;
            }
            set
            {
                if (this.expressionSet.EyesCloseMouthOpenPath == value)
                {
                    return;
                }

                this.expressionSet.EyesCloseMouthOpenPath = value;
                this.NotifyPropertyChanged();

                this.EyesCloseMouthOpen = this.LoadImage(value);
            }
        }

        public string EyesCloseMouthClosePath
        {
            get
            {
                return this.expressionSet.EyesCloseMouthClosePath;
            }
            set
            {
                if (this.expressionSet.EyesCloseMouthClosePath == value)
                {
                    return;
                }

                this.expressionSet.EyesCloseMouthClosePath = value;
                this.NotifyPropertyChanged();

                this.EyesCloseMouthClose = this.LoadImage(value);
            }
        }

        public string Name
        {
            get
            {
                return this.expressionSet.Name;
            }
            set
            {
                if (this.expressionSet.Name == value)
                {
                    return;
                }

                this.expressionSet.Name = value;
                this.NotifyPropertyChanged();
            }
        }

        public void ReloadAllImages()
        {
            this.EyesOpenMouthOpen = this.LoadImage(this.EyesOpenMouthOpenPath);
            this.EyesOpenMouthClose = this.LoadImage(this.EyesOpenMouthClosePath);
            this.EyesCloseMouthOpen = this.LoadImage(this.EyesCloseMouthOpenPath);
            this.EyesCloseMouthClose = this.LoadImage(this.EyesCloseMouthClosePath);
        }
    }
}
