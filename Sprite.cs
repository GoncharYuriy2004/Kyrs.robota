using System.Windows;
using System.Windows.Media.Imaging;

namespace WPF
{
    internal class Sprite
    {
        public Rect rect_;
        public BitmapImage? bitmap_;
        public double speed_;

        public Sprite(Rect rect_, BitmapImage? bitmap_, double speed_)
        {
            this.rect_ = rect_;
            this.bitmap_ = bitmap_;
            this.speed_ = speed_;
        }
    }
}
