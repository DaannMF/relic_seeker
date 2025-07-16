interface IControllable {
    void OutlineEntity();
    void ControlEntity(PlayerController controller);
    void OnStartLooking();
    void OnStopLooking();
    void OnInteract(PlayerController controller);
}