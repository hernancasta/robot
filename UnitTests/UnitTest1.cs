using NUnit.Framework;
using Shared.Command.Movement;

namespace UnitTests
{
    public class Tests
    {
        Shared.Serialization.SystemTextJsonSerializer serializer;

        [SetUp]
        public void Setup()
        {
            serializer = new Shared.Serialization.SystemTextJsonSerializer(); 
        }

        [Test]
        public void TestSerialization()
        {
            MotorCommand m = new MotorCommand();

            m.MovementType = MovementType.Position;

            var bytes = serializer.SerializeBytes(m);
            var check = serializer.DeserializeBytes<MotorCommand>(bytes);

            Assert.AreEqual(MovementType.Position, check.MovementType);
        }

        [Test]
        public void TestSerialization2()
        {
            MotorCommand m = new MotorCommand();

            m.MovementType = MovementType.Position;

            var bytes = serializer.Serialize(m);
            var check = serializer.Deserialize<MotorCommand>(bytes);

            Assert.AreEqual(MovementType.Position, check.MovementType);

        }
    }
}