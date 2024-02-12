using PotyogosAmoba.Model.Model;
using PotyogosAmoba.Model.Persistance;
using Moq;
namespace AmobaTest
{
    [TestClass]
    public class UnitTest1: IDisposable
    {
        private PotyogosAmobaModel _model = null!;
        private PotyogosAmobaTable _mockedTable = null!;
        private Mock<IPotyogosAmobaDataAccess> _mock = null!;
        [TestInitialize]
        public void Initialize()
        {
            _mockedTable = new PotyogosAmobaTable(10);
            _mockedTable.SetSign(9, 3, 1);
            _mockedTable.SetSign(9, 4, 1);
            _mockedTable.SetSign(9, 4, 2);

            _mock = new Mock<IPotyogosAmobaDataAccess>();
            _mock.Setup(mock => mock.LoadAsync(It.IsAny<String>()))
                .Returns(() => Task.FromResult(_mockedTable));
            _model = new PotyogosAmobaModel(10, _mock.Object);
            _model.GameAdvanced += new EventHandler<PotyogosAmobaEventArgs>(Model_GameAdvanced);
            _model.GameOver += new EventHandler<PotyogosAmobaEventArgs>(Model_GameOver);
        }

        [TestMethod]
        public void New10TableTest()
        {
            _model = new PotyogosAmobaModel(10, _mock.Object);
            Assert.AreEqual(0, _model.Table.StepNumber);
            Assert.AreEqual(10, _model.Table.Size);
            Assert.AreEqual(TimeSpan.Zero, _model.Table.OTime);
            Assert.AreEqual(TimeSpan.Zero, _model.Table.XTime);
            Int32 emptyFields = 0;
            for (Int32 i = 0; i < 10; i++)
                for (Int32 j = 0; j < 10; j++)
                    if (_model.GameTable[i, j] == 0)
                        emptyFields++;
            Assert.AreEqual(100, emptyFields);

        }

        [TestMethod]
        public void New20TableTest()
        {
            _model = new PotyogosAmobaModel(20, _mock.Object);
            Assert.AreEqual(0, _model.Table.StepNumber);
            Assert.AreEqual(20, _model.Table.Size);
            Assert.AreEqual(TimeSpan.Zero, _model.Table.OTime);
            Assert.AreEqual(TimeSpan.Zero, _model.Table.XTime);
            Int32 emptyFields = 0;
            for (Int32 i = 0; i < 20; i++)
                for (Int32 j = 0; j < 20; j++)
                    if (_model.GameTable[i, j] == 0)
                        emptyFields++;
            Assert.AreEqual(400, emptyFields);
        }

        [TestMethod]
        public void New30TableTest()
        {
            _model = new PotyogosAmobaModel(30, _mock.Object);
            Assert.AreEqual(0, _model.Table.StepNumber);
            Assert.AreEqual(30, _model.Table.Size);
            Assert.AreEqual(TimeSpan.Zero, _model.Table.OTime);
            Assert.AreEqual(TimeSpan.Zero, _model.Table.XTime);
            Int32 emptyFields = 0;
            for (Int32 i = 0; i < 30; i++)
                for (Int32 j = 0; j < 30; j++)
                    if (_model.GameTable[i, j] == 0)
                        emptyFields++;
            Assert.AreEqual(900, emptyFields);
        }

        [TestMethod]
        public void StepGameTest()
        {
            _model = new PotyogosAmobaModel(10, _mock.Object);
            Int32 x = 0;
            Int32 y = 0;
            Int32 currentPlayer = 1;
            Assert.AreEqual((PotyogosAmobaTable.Player)currentPlayer, _model.Table.CurrentPlayer);
            _model.StepGame(x, y);
            currentPlayer = 2;
            Assert.AreEqual((PotyogosAmobaTable.Player)currentPlayer, _model.Table.CurrentPlayer);
            Assert.AreEqual(TimeSpan.Zero, _model.Table.OTime);
            Assert.AreEqual(TimeSpan.Zero, _model.Table.XTime);
            Assert.AreEqual(1, _model.Table.StepNumber);
            Assert.AreEqual((PotyogosAmobaTable.Player)1, _model.GameTable[9, 0]);


        }

        [TestMethod]
        public void GameWonTest()
        {
            _model = new PotyogosAmobaModel(10, _mock.Object);
            for(int i= 0; i < 8; i++) 
            {
                
                if(_model.Table.CurrentPlayer == PotyogosAmobaTable.Player.PlayerO)
                {
                    _model.StepGame(9, 9);
                    
                }
                else
                {
                    _model.StepGame(i, i);
                }
            }
            Assert.IsTrue(_model.IsGameOver);
            Assert.AreEqual(PotyogosAmobaTable.Player.PlayerO, _model.GameTable[9,9]);
            Assert.AreEqual(PotyogosAmobaTable.Player.PlayerO, _model.GameTable[8,9]);
            Assert.AreEqual(PotyogosAmobaTable.Player.PlayerO, _model.GameTable[7,9]);
            Assert.AreEqual(PotyogosAmobaTable.Player.PlayerO, _model.GameTable[6,9]);

        }



        [TestMethod]
        public async Task LoadTest()
        {
            _model = new PotyogosAmobaModel(10, _mock.Object);

            await _model.LoadGameAsync(String.Empty);

            for (int i = 0; i < _model.Table.Size ; ++i)
            {
                for (int j = 0; j < _model.Table.Size; ++j)
                {
                    Assert.AreEqual(_mockedTable.Table[i,j], _model.GameTable[i, j]);
                }
            }

            _mock.Verify(dataAccess => dataAccess.LoadAsync(String.Empty), Times.Once());
        }


        private void Model_GameOver(object? sender, PotyogosAmobaEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Model_GameAdvanced(object? sender, PotyogosAmobaEventArgs e)
        {
            Assert.IsTrue(_model.Table.XTime >= TimeSpan.Zero);
            Assert.IsTrue( _model.Table.OTime >= TimeSpan.Zero);

            
        }

        public void Dispose()
        {
            if (_model != null)
            {
                _model.Dispose();
                GC.SuppressFinalize(this);

            }
        }
    }
}