using System.Diagnostics;
using System.Windows.Input;
using System.Timers;
using TankGame.Engine;
using TankGame.GameEntities;
using TankGame.Maths;
using TankGame.Input;
using System.Runtime.InteropServices;

namespace TankGame
{
    public partial class TankGameForm : Form
    {
        bool _rotateLeft = false;
        bool _rotateRight = false;
        bool _moveForward = false;
        bool _moveBackward = false;
        bool _turretUp = false;
        bool _turretDown = false;
        bool _turretLeft = false;
        bool _turretRight = false;
        bool _resetCamera = false;
        bool _shiftPressed = false;
        bool _displayBoundingBoxes = false;

        bool _canFire = true;

        int _reloadTime = 500;
        DateTime _canNextFire;

        System.Timers.Timer _timer;

        GameEngine _gameEngine;
        Camera _camera;

        bool _lastFrameTimeValid = false;
        DateTime _lastFrameTime;

        Tank _playerTank = null;
        MouseControl _mouseControl;
        
        public TankGameForm()
        {
            InitializeComponent();
            _gameEngine = new GameEngine();
            _camera = new Camera(100, 512, Width, Height);
            _camera.SetCameraHeight(200);
            _mouseControl = new MouseControl();

            CreateMountains();
            //CreatePyramids();
            CreateTowers();
            CreateTanks();
 
            _timer = new System.Timers.Timer(1.0/50.0);

            _timer.AutoReset = true;
            _timer.Elapsed += Timer_Elapsed;
            _timer.Enabled = true;
        }

        private void TankGameForm_Paint(object sender, PaintEventArgs e)
        {
            if (_lastFrameTimeValid)
            {
                DateTime now = DateTime.UtcNow;
                double durationInMs = ( now - _lastFrameTime).TotalMilliseconds;
                _lastFrameTime = now;
                double framesPerSecond = 1000 / durationInMs;
                
                _gameEngine.SetFrameRate(framesPerSecond);
            }
            else
            {
                _lastFrameTime = DateTime.UtcNow;
                _lastFrameTimeValid = true;
            }
            _gameEngine.GameLoop(e.Graphics, Width, Height,_camera);
        }

        void CreateMountains()
        {
            Random r = new Random();
            for (int i = 0; i < 60; ++i)
            {
                CreateMountain(r.Next(6), Math.PI / 2 + r.NextDouble() * 2 * Math.PI, r.NextDouble() * 3 + 2.0, Color.DarkGreen);
            }
            CreateMountain(1, Math.PI / 2, 3, Color.DarkRed);
            CreateMountain(1, 0, 3, Color.DarkOrange);
        }

        void CreatePyramids()
        {
            for(int i=0;i<10;++i)
            {
                Pyramid r = new Pyramid();
                r.OriginInWorldSpace = new PointFloat3d(0 - i * 200, 0, 50);
                if (i==0) {
                    r.SetColour(Color.Purple);
                }
                else if (i==9)
                {
                    r.SetColour(Color.Pink);
                }
                _gameEngine.AddEntity(r);
            }
        }

        void CreateTowers()
        {
            CreateTower(1200, 1200);
            //Random r = new Random();
            //for (int i = 0; i < 20; ++i)
            //{
            //    CreateTower(r.Next(16000)- 8000, r.Next(16000)- 8000);
            //}
        }

        void CreateTanks()
        {
            _playerTank = CreateTank(300, 300, 0);
            _playerTank.AttachCamera(_camera, new PointFloat3d(0, 175, -50), 0, 0);
            Random r = new Random();
            for (int i = 0; i < 20; ++i)
            {
                CreateTank(r.Next(16000) - 800, r.Next(16000) - 8000, r.NextDouble() * 2 * Math.PI);
            }
        }

        Tank CreateTank(int x, int z, double yRotation)
        {
            Tank t = new Tank();
            t.OriginInWorldSpace = new PointFloat3d(x, 0, z);
            _gameEngine.AddEntity(t);
            return t;
        }

        void CreateTower(int x, int z)
        {
            Tower t = new Tower();
            t.OriginInWorldSpace = new PointFloat3d(x, 0, z);
            _gameEngine.AddEntity(t);
        }

        void CreateMountain(int mountainTypeIndex, double rotationRadians, double scale, Color c)
        {
            int farDistance = 20000;
            PointFloat3d origin = new PointFloat3d(farDistance*Math.Cos(rotationRadians),0,farDistance*Math.Sin(rotationRadians));
            Mountain3d mountain = new Mountain3d(mountainTypeIndex);
            mountain.OriginInWorldSpace = origin;
            mountain.SetColour(c);

            _gameEngine.AddEntity(mountain);
        }

        private void Timer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            this._timer.Stop();
            int dx=0;
            int dy=0;
            if (_mouseControl!=null && _mouseControl.MouseCaptured)
            {
                _mouseControl.GetMouseMovement(out dx, out dy);
            }
            if (!_canFire && DateTime.UtcNow>this._canNextFire)
            {
                this._canFire = true;
            }
            double rotateSpeed = Math.PI / 360;
            double speed = 10;
            if (_shiftPressed)
            {
                speed *= 4;
                rotateSpeed *= 4;
            }
            if (dx!=0)
            {
                _playerTank.AttachedCameraRotateAroundYAxis(rotateSpeed * -dx);
            }

            if (this._rotateLeft)
            {
                _playerTank.Rotate(rotateSpeed);
            }
            if (this._rotateRight)
            {
                _playerTank.Rotate(-rotateSpeed);
            }
            if (this._moveForward)
            {
                _playerTank.MoveForward(speed, _gameEngine.FrameIndex);
            }
            if (this._moveBackward)
            {
                _playerTank.MoveForward(-speed, _gameEngine.FrameIndex);
            }
            if (this._turretUp)
            {
                _playerTank.RotateGunUpOrDown(Math.PI / 180);
            }
            if (this._turretDown)
            {
                _playerTank.RotateGunUpOrDown(-Math.PI / 180);
            }
            if (this._turretLeft)
            {
                _playerTank.RotateTurretRightOrLeft(Math.PI / 180);
            }
            if (this._turretRight)
            {
                _playerTank.RotateTurretRightOrLeft(-Math.PI / 180);
            }

            if (this._resetCamera)
            {
                this._resetCamera = false;
                _camera = new Camera(_camera.ClippingPlaneZ, _camera.ProjectionMultiplier, Width, Height);
                _camera.SetCameraHeight(100);
            }
            Invalidate();
            this._timer.Start();
        }

        private void FireShell()
        {
            Shell shell = this._playerTank.Fire();
            if (shell != null)
            {
                _gameEngine.AddEntity(shell);
                this._canFire = false;
                this._canNextFire = DateTime.UtcNow + new TimeSpan(0, 0, 0, 0, _reloadTime);
            }
            //double shellDistance = 250;
            //int shellX = _cameraLocation.X + (int)(Math.Sin(-_cameraYRotation) * shellDistance);
            //int shellY = _cameraLocation.Y+50;
            //int shellZ = _cameraLocation.Z + (int)(Math.Cos(-_cameraYRotation) * shellDistance);
            //GameEntities.Shell shell = new GameEntities.Shell(new Point3d(shellX,shellY,shellZ));
            //_gameEngine.AddEntity(shell);
        }

        private void TankGameForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left)
            {
                this._rotateLeft = true;
            }
            if (e.KeyCode == Keys.Right)
            {
                this._rotateRight = true;
            }
            if (e.KeyCode == Keys.Up)
            {
                this._moveForward = true;
            }
            if (e.KeyCode == Keys.Down)
            {
                this._moveBackward = true;
            }
            if (e.KeyCode == Keys.ShiftKey)
            {
                this._shiftPressed = true;
            }
            if (e.KeyCode == Keys.A)
            {
                this._turretLeft = true;
            }
            if (e.KeyCode == Keys.D)
            {
                this._turretRight = true;
            }
            if (e.KeyCode == Keys.S)
            {
                this._turretDown = true;
            }
            if (e.KeyCode == Keys.X)
            {
                this._turretUp = true;
            }
            if (e.KeyCode == Keys.Space && this._canFire)
            {
                FireShell();
            }
            if (e.KeyCode == Keys.P)
            {
                _gameEngine.RedrawFrameIndex = 0;
                _gameEngine.ReplayingFrames = true;
            }
            else if (e.KeyCode == Keys.O)
            {
                _gameEngine.ReplayingFrames = false;
            }
            if (e.KeyCode == Keys.U)
            {
                _gameEngine.RedrawLastFrame();
            }
            if (e.KeyCode == Keys.I)
            {
                _gameEngine.RedrawNextFrame();
            }
        }

        private void TankGameForm_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left)
            {
                this._rotateLeft = false;
            }
            if (e.KeyCode == Keys.Right)
            {
                this._rotateRight = false;
            }
            if (e.KeyCode == Keys.Up)
            {
                this._moveForward = false;
            }
            if (e.KeyCode == Keys.Down)
            {
                this._moveBackward = false;
            }

            if (e.KeyCode == Keys.R)
            {
                this._resetCamera = true;
            }
            if (e.KeyCode == Keys.ShiftKey)
            {
                this._shiftPressed = false;
            }

            if (e.KeyCode == Keys.A)
            {
                this._turretLeft = false;
            }
            if (e.KeyCode == Keys.D)
            {
                this._turretRight = false;
            }
            if (e.KeyCode == Keys.S)
            {
                this._turretDown = false;
            }
            if (e.KeyCode == Keys.X)
            {
                this._turretUp = false;
            }
            if (e.KeyCode == Keys.Escape)
            {
                if (this._mouseControl != null)
                {
                    _mouseControl.ReleaseMouse();
                }
            }
            if (e.KeyCode == Keys.B)
            {
                this._displayBoundingBoxes = !this._displayBoundingBoxes;
                this._gameEngine.DrawBoundingBoxes = this._displayBoundingBoxes;
            }
        }

        private void TankGameForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this._mouseControl!=null)
            {
                this._mouseControl.ReleaseMouse();
                this._mouseControl.Dispose();
                this._mouseControl = null;
            }
        }

        private void TankGameForm_MouseClick(object sender, MouseEventArgs e)
        {
            if (this._mouseControl == null)
            {
                this._mouseControl = new MouseControl();
            }
            if (this._mouseControl.MouseCaptured == false)
            {
                this._mouseControl.CaptureMouse();
            }
        }
    }
}