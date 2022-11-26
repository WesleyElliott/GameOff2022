using Godot;

struct GameState {

    public GameState() {
        _gameTime = 15;
        Money = 0;
        Paused = false;
        GameOver = false;
    }

    private int _gameTime;

    public int GameTime {
        get {
            return _gameTime;
        }
        set {
            _gameTime = value;
            if (_gameTime < 0) {
                _gameTime = 0;
            }
        }
    }
    public int Money;
    public bool Paused;
    public bool GameOver;
}

public partial class GameController : Node {

    [Export]
    public Color SafeColor { get; private set; }

    [Export]
    public Color WarningColor { get; private set; }

    [Export]
    public Color CriticalColor { get; private set; }

    [Export]
    public GameOver GameOver { get; private set; }

    [Export]
    public Pause PauseMenu { get; private set; }

    private Timer gameTimer;
    private GameState gameState = new GameState();

    public override void _EnterTree() {
        this.EventBus().PackageDelivered += OnPackageDelivered;
        this.EventBus().PackageExpired += OnPackageExpired;
        this.EventBus().PauseChanged += OnPauseChanged;
    }

    public override void _ExitTree() {
        this.EventBus().PackageDelivered -= OnPackageDelivered;
        this.EventBus().PackageExpired -= OnPackageExpired;
        this.EventBus().PauseChanged -= OnPauseChanged;
    }

    public override void _Ready() {
        gameTimer = GetNode<Timer>("GameTimer");
        OnTick();
    }

    public override void _UnhandledKeyInput(InputEvent @event) {
        if (@event is InputEventKey eventKey) {
            if (eventKey.Pressed && eventKey.Keycode == Key.Escape) {
                OnPauseChanged(!gameState.Paused);
            }
        }
    }

    public void OnTick() {
        gameTimer.Start();
        this.EventBus().EmitSignal(EventBus.SignalName.GameTimerTick, gameState.GameTime);

        if (gameState.GameTime == 0 && !gameState.GameOver) {
            HandleGameOver();
        }

        gameState.GameTime -= 1;
    }

    private void OnPackageDelivered(DropOff dropOff, bool anyoneHome) {
        gameState.Money += 20;
        this.EventBus().EmitSignal(EventBus.SignalName.MoneyChanged, gameState.Money);
        if (!anyoneHome) {
            gameState.GameTime += 10;
            gameState.Money += 10;
            this.EventBus().EmitSignal(EventBus.SignalName.MoneyChanged, gameState.Money);
            this.EventBus().EmitSignal(EventBus.SignalName.GameTimerTick, gameState.GameTime);
        }
    }

    private void OnPackageExpired(DropOff dropOff) {
        gameState.GameTime -= 10;
        this.EventBus().EmitSignal(EventBus.SignalName.GameTimerTick, gameState.GameTime);
    }

    private void OnPauseChanged(bool newState) {
        gameState.Paused = newState;
        GetTree().Paused = newState;

        if (gameState.Paused) {
            GD.Print("[GameController] PAUSED");
            PauseMenu.Render();
            PauseMenu.Show();
            gameTimer.Stop();
        } else {
            GD.Print("[GameController] UNPAUSED");
            PauseMenu.Hide();
            gameTimer.Start();
        }
    }

    private void HandleGameOver() {
        GD.Print("[GameController] GAME OVER");
        this.EventBus().EmitSignal(EventBus.SignalName.GameEnd);
        gameState.GameOver = true;
        GameOver.Visible = true;
        GameOver.Render(gameState.Money);

        gameTimer.Stop();
    }
}