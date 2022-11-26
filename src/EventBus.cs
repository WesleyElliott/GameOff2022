using Godot;

public partial class EventBus : Node {

    [Signal]
    public delegate void WarehouseCapacityEventHandler(int packageAmount);

    [Signal]
    public delegate void PlayerPackageUpdatedEventHandler(int packageCount);

    [Signal]
    public delegate void GameTimerTickEventHandler(int newTime);

    [Signal]
    public delegate void ShowNotificationEventHandler(DropOff dropOff);

    [Signal]
    public delegate void HideNotificationEventHandler(DropOff dropOff);

    [Signal]
    public delegate void PackageExpiredEventHandler(DropOff dropOff);
}