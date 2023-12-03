using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WPF
{
    internal class MainModel
    {
        private Typeface FONT = new Typeface(new FontFamily("Arial"), FontStyles.Normal, FontWeights.Bold, FontStretches.Normal);
        private int width = 1920;
        private int height = 1080;
        private int road_tile_width = 716;
        private int road_tile_height = 512;
        private int cars_ysdpeed = 13;
        private int car_yspeed = 10;
        private int car_xspeed=25;
        private bool gameOver = false;
        private bool pause = false;


        private int score;
        private Point car_position;
        private Rect car_rect;
        private Rect car_allowed_bounds;
        private List<Sprite> road_tiles;
        private List<Sprite> cars;
        private List<Sprite> bonuses;

        private BitmapImage[] carImages;
        private BitmapImage[] bonusImages;
        private BitmapImage bgImage;
        private BitmapImage carImage;
        private DrawingVisual drawingVisual;
        private Random random;

        
        public RenderTargetBitmap bufferBitmap_;

        public MainModel()
        {
            score = 0;
            random = new Random();
            bufferBitmap_ = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
            drawingVisual = new DrawingVisual();

            carImages = new BitmapImage[4] { 
                (BitmapImage)Application.Current.Resources["car_green"],
                (BitmapImage)Application.Current.Resources["car_orange"],
                (BitmapImage)Application.Current.Resources["car_red"],
                (BitmapImage)Application.Current.Resources["car_white"]
            };

            bonusImages = new BitmapImage[4]
            {
                (BitmapImage)Application.Current.Resources["apple"],
                (BitmapImage)Application.Current.Resources["star"],
                (BitmapImage)Application.Current.Resources["life"],
                (BitmapImage)Application.Current.Resources["diamond"]
            };
            
            bgImage = (BitmapImage)Application.Current.Resources["background"];
            carImage = (BitmapImage)Application.Current.Resources["car"];

            
            car_allowed_bounds = new Rect((width-road_tile_width+175) / 2, 0,
                                          road_tile_width-carImage.Width-175,height-carImage.Height);

            InitSprites();
        }

        private void InitSprites()
        {
            car_position = new Point((width - carImage.Width) / 2, (height - carImage.Height) / 2);
            //плитки дороги
            road_tiles = new List<Sprite>();
            for (int i = 1; i <= 8; i++)
            {
                BitmapImage selected;
                if (i % 2 == 0)
                {
                    selected = (BitmapImage)Application.Current.Resources["road1_1"];
                }
                else
                {
                    selected = (BitmapImage)Application.Current.Resources["road1_2"];
                }
                road_tiles.Add(new Sprite(new Rect((width - road_tile_width) / 2, height - (road_tile_height * i), road_tile_width, road_tile_height), selected, car_yspeed));
            }
            for (int i = 9; i <= 16; i++)
            {
                BitmapImage selected;
                if (i % 2 == 0)
                {
                    selected = (BitmapImage)Application.Current.Resources["road2_1"];
                }
                else
                {
                    selected = (BitmapImage)Application.Current.Resources["road2_2"];
                }
                road_tiles.Add(new Sprite(new Rect((width - road_tile_width) / 2, height - (road_tile_height * i), road_tile_width, road_tile_height), selected, car_yspeed));
            }

            //машини суперники
            cars = new List<Sprite>();
            for (int i = 0; i <= 16; i++)
            {
                int choice = random.Next(0, 4);
                var rect = new Rect()
                {
                    X = car_allowed_bounds.X + (car_allowed_bounds.Width - carImages[choice].Width) * random.NextDouble(),
                    Y = -carImages[choice].Height * i * 2 - random.Next(300, 600),
                    Width = carImages[choice].Width,
                    Height = carImages[choice].Height
                };

                cars.Add(new Sprite(rect, carImages[choice], cars_ysdpeed));
            }

            //бонуси
            bonuses = new List<Sprite>();
            for (int i = 0; i <= 15; i++)
            {
                int choice = random.Next(0, 4);
                var rect = new Rect()
                {
                    X = car_allowed_bounds.X + (car_allowed_bounds.Width - bonusImages[choice].Width) * random.NextDouble(),
                    Y = -random.Next(300, 600) * i * 2,
                    Width = bonusImages[choice].Width,
                    Height = bonusImages[choice].Height
                };

                bonuses.Add(new Sprite(rect, bonusImages[choice], car_yspeed));
            }
        }

        [Obsolete]
        private FormattedText CreateFormatedText(string text, int size, Brush brush)
        {
            FormattedText formattedText = new FormattedText(text,
                                                            System.Globalization.CultureInfo.CurrentCulture,
                                                            FlowDirection.LeftToRight,
                                                            FONT,
                                                            size,
                                                            brush);
            return formattedText;
        }

        private void Rendering()
        {   
            
            using (DrawingContext drawingContext = drawingVisual.RenderOpen())
            {
                drawingContext.DrawImage(bgImage, new Rect(0, 0, width, height));

                //малюємо дорогу
                foreach (var sprite in road_tiles)
                {
                    if(sprite.rect_.Y > - road_tile_height && sprite.rect_.Y < height)
                    {
                        drawingContext.DrawImage(sprite.bitmap_, sprite.rect_);
                    }
                }

                //малюємо суперників
                foreach (var sprite in cars)
                {
                    if (sprite.rect_.Y > -sprite.bitmap_.Height && sprite.rect_.Y < height)
                    {
                        drawingContext.DrawImage(sprite.bitmap_, sprite.rect_);
                    }
                }

                //малюємо бонуси
                foreach (var sprite in bonuses)
                {
                    if (sprite.rect_.Y > -sprite.bitmap_.Height && sprite.rect_.Y < height)
                    {
                        drawingContext.DrawImage(sprite.bitmap_, sprite.rect_);
                    }
                }

                // Виведення тексту
                drawingContext.DrawText(CreateFormatedText($"Очки : {score}\nSpace - пауза\nEsc - вихід",54, Brushes.White), 
                                        new Point(50, 50));


                //наша машина
                drawingContext.DrawImage(carImage, car_rect);
                
                //повідомлення програшу
                if (gameOver)
                {
                    var formated_text = CreateFormatedText("Нажаль ви програли!\n  Пробіл до рестарту", 100, Brushes.Yellow);
                    Point text_pos = new((width-formated_text.Width)/2, (height - formated_text.Height) / 2);
                    drawingContext.DrawText(formated_text,text_pos);
                }

                //повідомлення програшу
                else if (pause)
                {
                    var formated_text = CreateFormatedText("ПАУЗА", 120, Brushes.Yellow);
                    Point text_pos = new((width - formated_text.Width) / 2, (height - formated_text.Height) / 2);
                    drawingContext.DrawText(formated_text, text_pos);
                }
            }

            bufferBitmap_.Render(drawingVisual);
        }

        public void Update()
        {
            if (!gameOver && !pause)
            {
                car_rect = new Rect(car_position.X, car_position.Y, carImage.Width, carImage.Height);

                //рухаємо дорогу
                foreach (var sprite in road_tiles)
                {
                    sprite.rect_.Y += sprite.speed_;
                }
                if (road_tiles[0].rect_.Y>=height) 
                { 
                    var rect = road_tiles[road_tiles.Count-1].rect_;
                    rect.Y -= road_tile_height;
                    var sprite = new Sprite(rect, road_tiles[0].bitmap_, road_tiles[0].speed_);
                    road_tiles.RemoveAt(0);
                    road_tiles.Add(sprite);
                }

                //рухаємо машини
                foreach (var sprite in cars)
                {
                    sprite.rect_.Y += sprite.speed_;
                }
                if (cars[0].rect_.Y >= height)
                {
                    var rect = cars[cars.Count - 1].rect_;
                    rect.Y -= random.Next(300, 600) + rect.Height ;
                    rect.X = car_allowed_bounds.X + (car_allowed_bounds.Width - cars[0].bitmap_.Width) * random.NextDouble();

                    var sprite = new Sprite(rect, cars[0].bitmap_, cars[0].speed_);
                    cars.RemoveAt(0);
                    cars.Add(sprite);
                }

                //рухаємо бонуси
                foreach (var sprite in bonuses)
                {
                    sprite.rect_.Y += sprite.speed_;
                }
                if (bonuses[0].rect_.Y >= height)
                {
                    var rect = bonuses[bonuses.Count - 1].rect_;
                    rect.Y -= random.Next(300, 600) * 3 + rect.Height;
                    rect.X = car_allowed_bounds.X + (car_allowed_bounds.Width - bonuses[0].bitmap_.Width) * random.NextDouble();

                    var sprite = new Sprite(rect, bonuses[0].bitmap_, car_yspeed);
                    bonuses.RemoveAt(0);
                    bonuses.Add(sprite);
                }



                //зіткнення нашої машини з іншими машинами
                foreach ( var sprite in cars)
                {
                    if (sprite.rect_.IntersectsWith(car_rect))
                    {
                        Console.WriteLine("BOOM!!!");
                        gameOver = true;
                    }
                    
                    
                    foreach (var bonus in bonuses)
                    {
                        if (sprite.rect_.IntersectsWith(bonus.rect_))
                        {
                            bonus.rect_.Y = bonus.rect_.Y - random.Next(1000, 1500);
                            bonus.rect_.X = car_allowed_bounds.X + (car_allowed_bounds.Width - bonus.rect_.Width) * random.NextDouble();
                        }

                        if (car_rect.IntersectsWith(bonus.rect_))
                        {
                            bonus.rect_.Y = bonus.rect_.Y - random.Next(1000, 1500);
                            bonus.rect_.X = car_allowed_bounds.X + (car_allowed_bounds.Width - bonus.rect_.Width) * random.NextDouble();
                            score += 50;
                        }

                    }
                }



            }

            //відмальовуємо сцену
            Rendering();
            GC.Collect();
            //GC.WaitForPendingFinalizers();
            Console.WriteLine("Update!");
        }


        public void PressLeft()
        {
            car_position.X -= car_xspeed;
            if (!car_allowed_bounds.Contains(car_position))
            {
                car_position.X += car_xspeed;
            }
            Console.WriteLine("LEFT");
        }
        public void PressRight()
        {
            car_position.X += car_xspeed;
            if (!car_allowed_bounds.Contains(car_position))
            {
                car_position.X -= car_xspeed;
            }
            Console.WriteLine("RIGHT");
        }

        public void PressUp()
        {
            car_position.Y -= car_xspeed;
            if (!car_allowed_bounds.Contains(car_position))
            {
                car_position.Y += car_xspeed;
            }

            Console.WriteLine("UP");
        }

        public void PressDown()
        {
            car_position.Y += car_xspeed;
            if (!car_allowed_bounds.Contains(car_position))
            {
                car_position.Y -= car_xspeed;
            }

            Console.WriteLine("DOWN");
        }

        public void PressSpace()
        {
            Console.WriteLine("SPACE");
            if(gameOver && !pause)
            {
                score = 0;
                InitSprites();
                gameOver = false;
            }
            else if(!gameOver && !pause)
            {
                pause = true;
            }
            else if(!gameOver && pause) 
            {
                pause = false;               
            }
        }


    }
}
