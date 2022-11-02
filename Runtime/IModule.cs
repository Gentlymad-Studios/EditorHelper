#if UNITY_EDITOR
namespace EditorHelper {
    /// <summary>
    /// Interface that serves as a contract for Editor UI modules
    /// </summary>
    public interface IModule {
        void SetupStyles();
        void Draw();
        void Cleanup();
    }
}
#endif
