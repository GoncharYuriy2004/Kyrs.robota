using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace WPF
{
    internal class MainViewModel : BaseViewModel
    {
        private MainModel model;
        private DispatcherTimer timer;
        public MainViewModel()
        {
            model = new MainModel();
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(15);
            timer.Tick += Update;
            timer.Start();
        }

        private void Update(object? sender, EventArgs? e)
        {
            model.Update();
            readyFrame = model.bufferBitmap_;
        }



        public RenderTargetBitmap readyFrame
        {
            get { return model.bufferBitmap_; }

            set { 
                model.bufferBitmap_ = value; 
                OnPropertyChanged(nameof(readyFrame));
            }
        }
        
        //commands
        private RelayCommand? pressLeft_;
        private RelayCommand? pressRight_;
        private RelayCommand? pressUp_;
        private RelayCommand? pressDown_;
        private RelayCommand? pressSpace_;
        private RelayCommand? pressEsc_;

        public ICommand PressEscCommand
        {
            get
            {
                if (pressEsc_ == null)
                {
                    pressEsc_ = new RelayCommand(param => Application.Current.Shutdown());
                }
                return pressEsc_;
            }
        }

        public ICommand PressSpaceCommand
        {
            get
            {
                if (pressSpace_ == null)
                {
                    pressSpace_ = new RelayCommand(param => model.PressSpace());
                }
                return pressSpace_;
            }
        }


        public ICommand PressLeftCommand
        {
            get
            {
                if (pressLeft_ == null)
                {
                    pressLeft_ = new RelayCommand(param => model.PressLeft());
                }
                return pressLeft_;
            }
        }
        public ICommand PressRightCommand
        {
            get
            {
                if (pressRight_ == null)
                {
                    pressRight_ = new RelayCommand(param => model.PressRight());
                }
                return pressRight_;
            }
        }

        public ICommand PressUpCommand
        {
            get
            {
                if (pressUp_ == null)
                {
                    pressUp_ = new RelayCommand(param => model.PressUp());
                }
                return pressUp_;
            }
        }

        public ICommand PressDownCommand
        {
            get
            {
                if (pressDown_ == null)
                {
                    pressDown_ = new RelayCommand(param => model.PressDown());
                }
                return pressDown_;
            }
        }


    }
}
