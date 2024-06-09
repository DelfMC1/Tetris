MainWindow Třída
MainWindow()

To je konstruktor. Nastaví časovače pro hru a čas hry. 
StartNewGame()

Tato metoda začne novou hru. Vymaže puvodni hru, nastaví skóre na 0 a začne časovače. 
GameTick(object sender, EventArgs e)

Tato metoda se spustí pokaždé, když se zvysuje cas. Pohne tetrominem dolů, pokud hra neskončila.
SpawnNewTetromino()

Tato metoda vytvoří nové tetromino. Pokud se tetromino nemůže pohybovat, znamená to, že hra skončila.
CreateTetromino(int tetrominoType, Canvas canvas, Point position)

Tato metoda kreslí tetromino na plochu. Každý typ tetromina má jinou barvu a tvar.
CreateBlock(Canvas canvas, double x, double y, Brush color, double blockSize, double offsetX, double offsetY)

Tato metoda kreslí jeden blok na plochu.
CreateShadowBlock(Canvas canvas, double x, double y, double blockSize, double offsetX, double offsetY)

Tato metoda ukáže, kam tetromino dopadne.
MoveTetromino(int dx, int dy)

Tato metoda pohne tetrominem podle hodnot dx a dy.
RotateTetromino()

Tato metoda otočí tetromino.
IsValidMove(Point newPosition, int newRotationState)

Tato metoda kontroluje, jestli se tetromino může pohnout do nové pozice a otočit.
GetBlockPositions(int rotationState)

Tato metoda vrací pozice bloků tetromina podle jeho otočení.
DrawShadow()

Tato metoda kreslí stín tetromina na plochu.
PlaceTetromino()

Tato metoda umístí tetromino plochu kam patri.
CheckForCompletedRows()

Tato metoda kontroluje, jestli jsou nějaké řady kompletní, a pokud ano, odstraní je a přidá skore.
RemoveRow(int row)

Tato metoda odstraní řadu a všechny bloky nad ní posune dolů.
DrawGameGrid()

Tato metoda kreslí celou herní mřížku.
GetColor(int blockType)

Tato metoda vrací barvu bloku na základě typu tetromina.
ClearGameCanvas()

Tato metoda vymaže celé herní plátno.
GameOver()

Tato metoda ukončí hru, zastaví časovače a zobrazí menu Game Over.
Window_KeyDown(object sender, KeyEventArgs e)

Tato metoda zpracovává stisky kláves pro pohyb a otáčení tetromina, když je hra aktivní.
UpdateScore()

Tato metoda aktualizuje zobrazení skóre.
UpdateGameTime(object sender, EventArgs e)

Tato metoda aktualizuje zobrazení uplynulého času hry.
UpdateNextTetromino()

Tato metoda aktualizuje zobrazení následujícího tetromina.
UpdateHoldTetromino()

Tato metoda aktualizuje zobrazení schovaneho tetromina.
HoldTetromino()

Tato metoda zpracovává funkci držení tetromina, aby se mohlo použít později.
PlayButton_Click(object sender, RoutedEventArgs e)

Tato metoda se spustí, když se klikne na tlačítko Play. Skryje hlavní menu a začne novou hru.
PlayAgainButton_Click(object sender, RoutedEventArgs e)

Tato metoda se spustí, když se klikne na tlačítko Play Again. Skryje menu Game Over a začne novou hru.
