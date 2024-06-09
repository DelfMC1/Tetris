using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace TetrisGame
{
    public partial class MainWindow : Window
    {
        private const int Rows = 20;
        private const int Columns = 10;
        private const int BlockSize = 30;
        private readonly DispatcherTimer _gameTickTimer;
        private readonly DispatcherTimer _gameTimeTimer;
        private readonly int[,] _gameGrid = new int[Rows, Columns];
        private readonly List<Rectangle> _currentTetromino = new List<Rectangle>();
        private readonly List<Rectangle> _shadowTetromino = new List<Rectangle>();
        private readonly Random _random = new Random();
        private int _currentTetrominoType;
        private int _nextTetrominoType;
        private int _holdTetrominoType = -1;
        private Point _currentPosition;
        private int _currentRotationState;
        private int _score;
        private DateTime _startTime;
        private bool _canHold = true;
        private bool _gameOver = false;
        private List<(string Name, int Score)> _leaderboard = new List<(string Name, int Score)>();

        public MainWindow()
        {
            InitializeComponent();
            _gameTickTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(500) };
            _gameTickTimer.Tick += GameTick;
            _gameTimeTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _gameTimeTimer.Tick += UpdateGameTime;
        }

        // Inicializuje a spustí novou hru
        private void StartNewGame()
        {
            Array.Clear(_gameGrid, 0, _gameGrid.Length);
            _score = 0;
            UpdateScore();
            _startTime = DateTime.Now;
            _gameTickTimer.Start();
            _gameTimeTimer.Start();
            _gameOver = false;
            _nextTetrominoType = _random.Next(1, 8);
            SpawnNewTetromino();
            UpdateNextTetromino();
            UpdateHoldTetromino();
        }

        // Zpracovává události tikání hry, pohybuje tetrominem dolů
        private void GameTick(object sender, EventArgs e)
        {
            if (!_gameOver)
            {
                MoveTetromino(0, 1);
            }
        }

        // Vytvoří nový tetromino
        private void SpawnNewTetromino()
        {
            _currentTetrominoType = _nextTetrominoType;
            _nextTetrominoType = _random.Next(1, 8);
            _currentPosition = new Point(4, 0);
            _currentRotationState = 0;
            _currentTetromino.Clear();

            if (!IsValidMove(_currentPosition, _currentRotationState))
            {
                GameOver();
                return;
            }

            DrawGameGrid();
            CreateTetromino(_currentTetrominoType, GameCanvas, _currentPosition);
            DrawShadow();
            UpdateNextTetromino();
        }

        // Vytvoří tetromino na plátně
        private void CreateTetromino(int tetrominoType, Canvas canvas, Point position)
        {
            double blockSize = canvas == GameCanvas ? BlockSize : canvas.Width / 4;
            double offsetX = canvas == GameCanvas ? 0 : (canvas.Width - 2 * blockSize) / 2;
            double offsetY = canvas == GameCanvas ? 0 : (canvas.Height - 2 * blockSize) / 2;

            switch (tetrominoType)
            {
                case 1: // I
                    CreateBlock(canvas, position.X, position.Y - 1, Brushes.Cyan, blockSize, offsetX, offsetY);
                    CreateBlock(canvas, position.X, position.Y, Brushes.Cyan, blockSize, offsetX, offsetY);
                    CreateBlock(canvas, position.X, position.Y + 1, Brushes.Cyan, blockSize, offsetX, offsetY);
                    CreateBlock(canvas, position.X, position.Y + 2, Brushes.Cyan, blockSize, offsetX, offsetY);
                    break;
                case 2: // J
                    CreateBlock(canvas, position.X - 1, position.Y - 1, Brushes.Blue, blockSize, offsetX, offsetY);
                    CreateBlock(canvas, position.X, position.Y - 1, Brushes.Blue, blockSize, offsetX, offsetY);
                    CreateBlock(canvas, position.X, position.Y, Brushes.Blue, blockSize, offsetX, offsetY);
                    CreateBlock(canvas, position.X, position.Y + 1, Brushes.Blue, blockSize, offsetX, offsetY);
                    break;
                case 3: // L
                    CreateBlock(canvas, position.X + 1, position.Y - 1, Brushes.Orange, blockSize, offsetX, offsetY);
                    CreateBlock(canvas, position.X, position.Y - 1, Brushes.Orange, blockSize, offsetX, offsetY);
                    CreateBlock(canvas, position.X, position.Y, Brushes.Orange, blockSize, offsetX, offsetY);
                    CreateBlock(canvas, position.X, position.Y + 1, Brushes.Orange, blockSize, offsetX, offsetY);
                    break;
                case 4: // O
                    CreateBlock(canvas, position.X, position.Y, Brushes.Yellow, blockSize, offsetX, offsetY);
                    CreateBlock(canvas, position.X, position.Y + 1, Brushes.Yellow, blockSize, offsetX, offsetY);
                    CreateBlock(canvas, position.X + 1, position.Y, Brushes.Yellow, blockSize, offsetX, offsetY);
                    CreateBlock(canvas, position.X + 1, position.Y + 1, Brushes.Yellow, blockSize, offsetX, offsetY);
                    break;
                case 5: // S
                    CreateBlock(canvas, position.X, position.Y, Brushes.Green, blockSize, offsetX, offsetY);
                    CreateBlock(canvas, position.X, position.Y + 1, Brushes.Green, blockSize, offsetX, offsetY);
                    CreateBlock(canvas, position.X + 1, position.Y - 1, Brushes.Green, blockSize, offsetX, offsetY);
                    CreateBlock(canvas, position.X + 1, position.Y, Brushes.Green, blockSize, offsetX, offsetY);
                    break;
                case 6: // T
                    CreateBlock(canvas, position.X, position.Y, Brushes.Purple, blockSize, offsetX, offsetY);
                    CreateBlock(canvas, position.X, position.Y - 1, Brushes.Purple, blockSize, offsetX, offsetY);
                    CreateBlock(canvas, position.X, position.Y + 1, Brushes.Purple, blockSize, offsetX, offsetY);
                    CreateBlock(canvas, position.X + 1, position.Y, Brushes.Purple, blockSize, offsetX, offsetY);
                    break;
                case 7: // Z
                    CreateBlock(canvas, position.X, position.Y - 1, Brushes.Red, blockSize, offsetX, offsetY);
                    CreateBlock(canvas, position.X, position.Y, Brushes.Red, blockSize, offsetX, offsetY);
                    CreateBlock(canvas, position.X + 1, position.Y, Brushes.Red, blockSize, offsetX, offsetY);
                    CreateBlock(canvas, position.X + 1, position.Y + 1, Brushes.Red, blockSize, offsetX, offsetY);
                    break;
            }
        }

        // Vytvoří blok na plátně
        private void CreateBlock(Canvas canvas, double x, double y, Brush color, double blockSize, double offsetX, double offsetY)
        {
            var block = new Rectangle
            {
                Width = blockSize,
                Height = blockSize,
                Fill = color
            };
            canvas.Children.Add(block);
            Canvas.SetLeft(block, offsetX + x * blockSize);
            Canvas.SetTop(block, offsetY + y * blockSize);
            if (canvas == GameCanvas)
            {
                _currentTetromino.Add(block);
            }
        }

        // Vytvoří stínový blok na plátně
        private void CreateShadowBlock(Canvas canvas, double x, double y, double blockSize, double offsetX, double offsetY)
        {
            var block = new Rectangle
            {
                Width = blockSize,
                Height = blockSize,
                Fill = Brushes.Gray,
                Opacity = 0.5
            };
            canvas.Children.Add(block);
            Canvas.SetLeft(block, offsetX + x * blockSize);
            Canvas.SetTop(block, offsetY + y * blockSize);
            _shadowTetromino.Add(block);
        }

        // Pohne tetrominem
        private void MoveTetromino(int dx, int dy)
        {
            var newPosition = new Point(_currentPosition.X + dx, _currentPosition.Y + dy);

            if (IsValidMove(newPosition, _currentRotationState))
            {
                _currentPosition = newPosition;
                foreach (var block in _currentTetromino)
                {
                    var left = Canvas.GetLeft(block) + dx * BlockSize;
                    var top = Canvas.GetTop(block) + dy * BlockSize;
                    Canvas.SetLeft(block, left);
                    Canvas.SetTop(block, top);
                }
                DrawShadow();
            }
            else if (dy == 1)
            {
                PlaceTetromino();
                CheckForCompletedRows();
                SpawnNewTetromino();
                _canHold = true;  // Povolit držení po umístění tetromina
            }
        }

        // Otočí tetromino
        private void RotateTetromino()
        {
            int newRotationState = (_currentRotationState + 1) % 4;
            var newBlockPositions = GetBlockPositions(newRotationState);

            // Zkontroluje, jestli otočení nezpůsobí, že tetromino vyjde mimo plátno, a upraví pozici, pokud je to nutné
            while (!IsValidMove(_currentPosition, newRotationState))
            {
                if (_currentPosition.X < Columns / 2)
                {
                    _currentPosition.X++;
                }
                else
                {
                    _currentPosition.X--;
                }
            }

            if (IsValidMove(_currentPosition, newRotationState))
            {
                _currentRotationState = newRotationState;

                // Vymaže aktuální bloky tetromina
                foreach (var block in _currentTetromino)
                {
                    GameCanvas.Children.Remove(block);
                }
                _currentTetromino.Clear();

                // Vytvoří nové bloky tetromina s aktualizovaným stavem otočení
                foreach (var blockPosition in newBlockPositions)
                {
                    CreateBlock(GameCanvas, _currentPosition.X + blockPosition.X, _currentPosition.Y + blockPosition.Y, GetColor(_currentTetrominoType), BlockSize, 0, 0);
                }
                DrawShadow();
            }
        }

        // Zkontroluje, zda je pohyb platný
        private bool IsValidMove(Point newPosition, int newRotationState)
        {
            var newBlockPositions = GetBlockPositions(newRotationState);

            foreach (var blockPosition in newBlockPositions)
            {
                var x = (int)(newPosition.X + blockPosition.X);
                var y = (int)(newPosition.Y + blockPosition.Y);
                if (x < 0 || x >= Columns || y >= Rows || (y >= 0 && _gameGrid[y, x] != 0))
                {
                    return false;
                }
            }
            return true;
        }

        // Vrátí pozice bloků pro tetromino
        private List<Point> GetBlockPositions(int rotationState)
        {
            var blockPositions = new List<Point>();
            switch (_currentTetrominoType)
            {
                case 1: // I
                    if (rotationState % 2 == 0)
                    {
                        blockPositions.Add(new Point(0, -1));
                        blockPositions.Add(new Point(0, 0));
                        blockPositions.Add(new Point(0, 1));
                        blockPositions.Add(new Point(0, 2));
                    }
                    else
                    {
                        blockPositions.Add(new Point(-1, 0));
                        blockPositions.Add(new Point(0, 0));
                        blockPositions.Add(new Point(1, 0));
                        blockPositions.Add(new Point(2, 0));
                    }
                    break;
                case 2: // J
                    if (rotationState == 0)
                    {
                        blockPositions.Add(new Point(-1, -1));
                        blockPositions.Add(new Point(0, -1));
                        blockPositions.Add(new Point(0, 0));
                        blockPositions.Add(new Point(0, 1));
                    }
                    else if (rotationState == 1)
                    {
                        blockPositions.Add(new Point(-1, 0));
                        blockPositions.Add(new Point(0, 0));
                        blockPositions.Add(new Point(1, 0));
                        blockPositions.Add(new Point(1, 1));
                    }
                    else if (rotationState == 2)
                    {
                        blockPositions.Add(new Point(0, -1));
                        blockPositions.Add(new Point(0, 0));
                        blockPositions.Add(new Point(0, 1));
                        blockPositions.Add(new Point(1, 1));
                    }
                    else
                    {
                        blockPositions.Add(new Point(-1, -1));
                        blockPositions.Add(new Point(-1, 0));
                        blockPositions.Add(new Point(0, 0));
                        blockPositions.Add(new Point(1, 0));
                    }
                    break;
                case 3: // L
                    if (rotationState == 0)
                    {
                        blockPositions.Add(new Point(1, -1));
                        blockPositions.Add(new Point(0, -1));
                        blockPositions.Add(new Point(0, 0));
                        blockPositions.Add(new Point(0, 1));
                    }
                    else if (rotationState == 1)
                    {
                        blockPositions.Add(new Point(-1, 0));
                        blockPositions.Add(new Point(0, 0));
                        blockPositions.Add(new Point(1, 0));
                        blockPositions.Add(new Point(1, -1));
                    }
                    else if (rotationState == 2)
                    {
                        blockPositions.Add(new Point(0, -1));
                        blockPositions.Add(new Point(0, 0));
                        blockPositions.Add(new Point(0, 1));
                        blockPositions.Add(new Point(-1, 1));
                    }
                    else
                    {
                        blockPositions.Add(new Point(-1, 0));
                        blockPositions.Add(new Point(0, 0));
                        blockPositions.Add(new Point(1, 0));
                        blockPositions.Add(new Point(-1, 1));
                    }
                    break;
                case 4: // O
                    blockPositions.Add(new Point(0, 0));
                    blockPositions.Add(new Point(0, 1));
                    blockPositions.Add(new Point(1, 0));
                    blockPositions.Add(new Point(1, 1));
                    break;
                case 5: // S
                    if (rotationState % 2 == 0)
                    {
                        blockPositions.Add(new Point(0, 0));
                        blockPositions.Add(new Point(0, 1));
                        blockPositions.Add(new Point(1, -1));
                        blockPositions.Add(new Point(1, 0));
                    }
                    else
                    {
                        blockPositions.Add(new Point(-1, 0));
                        blockPositions.Add(new Point(0, 0));
                        blockPositions.Add(new Point(0, 1));
                        blockPositions.Add(new Point(1, 1));
                    }
                    break;
                case 6: // T
                    if (rotationState == 0)
                    {
                        blockPositions.Add(new Point(0, 0));
                        blockPositions.Add(new Point(0, -1));
                        blockPositions.Add(new Point(0, 1));
                        blockPositions.Add(new Point(1, 0));
                    }
                    else if (rotationState == 1)
                    {
                        blockPositions.Add(new Point(-1, 0));
                        blockPositions.Add(new Point(0, 0));
                        blockPositions.Add(new Point(1, 0));
                        blockPositions.Add(new Point(0, 1));
                    }
                    else if (rotationState == 2)
                    {
                        blockPositions.Add(new Point(0, -1));
                        blockPositions.Add(new Point(0, 0));
                        blockPositions.Add(new Point(0, 1));
                        blockPositions.Add(new Point(-1, 0));
                    }
                    else
                    {
                        blockPositions.Add(new Point(-1, 0));
                        blockPositions.Add(new Point(0, 0));
                        blockPositions.Add(new Point(1, 0));
                        blockPositions.Add(new Point(0, -1));
                    }
                    break;
                case 7: // Z
                    if (rotationState % 2 == 0)
                    {
                        blockPositions.Add(new Point(0, -1));
                        blockPositions.Add(new Point(0, 0));
                        blockPositions.Add(new Point(1, 0));
                        blockPositions.Add(new Point(1, 1));
                    }
                    else
                    {
                        blockPositions.Add(new Point(-1, 0));
                        blockPositions.Add(new Point(0, 0));
                        blockPositions.Add(new Point(0, 1));
                        blockPositions.Add(new Point(1, 1));
                    }
                    break;
            }
            return blockPositions;
        }

        // Vykreslí stín tetromina
        private void DrawShadow()
        {
            // Vymaže existující stínové bloky
            foreach (var block in _shadowTetromino)
            {
                GameCanvas.Children.Remove(block);
            }
            _shadowTetromino.Clear();

            // Vypočítá pozici stínu
            var shadowPosition = _currentPosition;
            while (IsValidMove(new Point(shadowPosition.X, shadowPosition.Y + 1), _currentRotationState))
            {
                shadowPosition.Y++;
            }

            // Vykreslí stínové bloky
            var blockPositions = GetBlockPositions(_currentRotationState);
            foreach (var blockPosition in blockPositions)
            {
                CreateShadowBlock(GameCanvas, shadowPosition.X + blockPosition.X, shadowPosition.Y + blockPosition.Y, BlockSize, 0, 0);
            }
        }

        // Umístí tetromino na mřížku
        private void PlaceTetromino()
        {
            foreach (var block in _currentTetromino)
            {
                var x = (int)(Canvas.GetLeft(block) / BlockSize);
                var y = (int)(Canvas.GetTop(block) / BlockSize);
                _gameGrid[y, x] = _currentTetrominoType;
                GameCanvas.Children.Remove(block);
            }
            _currentTetromino.Clear();
        }

        // Zkontroluje, zda jsou řady kompletní, a odstraní je
        private void CheckForCompletedRows()
        {
            for (int y = 0; y < Rows; y++)
            {
                bool isComplete = true;
                for (int x = 0; x < Columns; x++)
                {
                    if (_gameGrid[y, x] == 0)
                    {
                        isComplete = false;
                        break;
                    }
                }

                if (isComplete)
                {
                    RemoveRow(y);
                    _score += 100;
                    UpdateScore();
                }
            }
        }

        // Odstraní řadu a přesune bloky dolů
        private void RemoveRow(int row)
        {
            for (int y = row; y > 0; y--)
            {
                for (int x = 0; x < Columns; x++)
                {
                    _gameGrid[y, x] = _gameGrid[y - 1, x];
                }
            }

            for (int x = 0; x < Columns; x++)
            {
                _gameGrid[0, x] = 0;
            }

            DrawGameGrid();
        }

        // Vykreslí herní mřížku
        private void DrawGameGrid()
        {
            GameCanvas.Children.Clear();
            for (int y = 0; y < Rows; y++)
            {
                for (int x = 0; x < Columns; x++)
                {
                    if (_gameGrid[y, x] != 0)
                    {
                        var block = new Rectangle
                        {
                            Width = BlockSize,
                            Height = BlockSize,
                            Fill = GetColor(_gameGrid[y, x])
                        };
                        GameCanvas.Children.Add(block);
                        Canvas.SetLeft(block, x * BlockSize);
                        Canvas.SetTop(block, y * BlockSize);
                    }
                }
            }
        }

        // Vrátí barvu bloku na základě typu tetromina
        private Brush GetColor(int blockType)
        {
            return blockType switch
            {
                1 => Brushes.Cyan,
                2 => Brushes.Blue,
                3 => Brushes.Orange,
                4 => Brushes.Yellow,
                5 => Brushes.Green,
                6 => Brushes.Purple,
                7 => Brushes.Red,
                _ => Brushes.White,
            };
        }

        // Vyčistí herní plátno
        private void ClearGameCanvas()
        {
            GameCanvas.Children.Clear();
        }

        // Nastaví stav hry na konec
        private void GameOver()
        {
            _gameOver = true;
            _gameTickTimer.Stop();
            _gameTimeTimer.Stop();
            GameCanvasGrid.Visibility = Visibility.Collapsed;
            GameOverMenu.Visibility = Visibility.Visible;
        }

        // Zpracovává stisky kláves
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (_gameOver) return;

            switch (e.Key)
            {
                case Key.Left:
                    MoveTetromino(-1, 0);
                    break;
                case Key.Right:
                    MoveTetromino(1, 0);
                    break;
                case Key.Down:
                    MoveTetromino(0, 1);
                    break;
                case Key.Up:
                    RotateTetromino();
                    break;
                case Key.C:
                    HoldTetromino();
                    break;
            }
        }

        // Aktualizuje skóre
        private void UpdateScore()
        {
            ScoreTextBlock.Text = _score.ToString();
        }

        // Aktualizuje čas hry
        private void UpdateGameTime(object sender, EventArgs e)
        {
            var elapsedTime = DateTime.Now - _startTime;
            TimeTextBlock.Text = elapsedTime.ToString(@"mm\:ss");
        }

        // Aktualizuje zobrazení následujícího tetromina
        private void UpdateNextTetromino()
        {
            NextCanvas.Children.Clear();
            CreateTetromino(_nextTetrominoType, NextCanvas, new Point(1, 1));
        }

        // Aktualizuje zobrazení drženého tetromina
        private void UpdateHoldTetromino()
        {
            HoldCanvas.Children.Clear();
            if (_holdTetrominoType != -1)
            {
                CreateTetromino(_holdTetrominoType, HoldCanvas, new Point(1, 1));
            }
        }

        // Zpracovává držení tetromina
        private void HoldTetromino()
        {
            if (!_canHold)
                return;

            _canHold = false;

            if (_holdTetrominoType == -1)
            {
                _holdTetrominoType = _currentTetrominoType;
                SpawnNewTetromino();
            }
            else
            {
                int temp = _currentTetrominoType;
                _currentTetrominoType = _holdTetrominoType;
                _holdTetrominoType = temp;
                _currentPosition = new Point(4, 0);
                _currentRotationState = 0;
                _currentTetromino.Clear();
                DrawGameGrid();
                CreateTetromino(_currentTetrominoType, GameCanvas, _currentPosition);
            }

            UpdateHoldTetromino();
        }

        // Zpracovává kliknutí na tlačítko Play
        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            MainMenu.Visibility = Visibility.Collapsed;
            GameCanvasGrid.Visibility = Visibility.Visible;
            StartNewGame();
        }

        // Zpracovává kliknutí na tlačítko Play Again
        private void PlayAgainButton_Click(object sender, RoutedEventArgs e)
        {
            GameOverMenu.Visibility = Visibility.Collapsed;
            GameCanvasGrid.Visibility = Visibility.Visible;
            StartNewGame();
        }
    }
}