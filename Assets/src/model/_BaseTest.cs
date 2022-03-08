// public class Model<T> where T : Model<T> {
//   public Controller<T> gameObject;

//   public Model() {

//   }
// }

// public abstract class Controller<T> : MonoBehaviour where T : Model<T> {
//   T model;

//   public virtual void Init(T model) {
//     this.model = model;
//     model.gameObject = this;
//   }
// }

// public class GameModel : Model<GameModel> {
//   public static GameModel main;

//   public Circuit circuit;
//   public Level level;

//   public GameModel() : base() {
//   }
// }

// public class LevelBootstrap : MonoBehaviour {
//   void Start() {
//     GameModel.main = new GameModel();
//   }
// }

// public class GameModelController : Controller<GameModel> {
// }

// public class Level : Model<Level> {

// }