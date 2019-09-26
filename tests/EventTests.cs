using System;
using System.Text;
using System.Threading;

namespace Orikivo
{

    // referenced from C# docs
    public class TestEventArgs : EventArgs
    {
        public TestEventArgs(string s)
        {
            message = s;
        }
        private string message;
        public string Message
        {
            get { return message; }
            set { message = value; }
        }
    }

    public class TestPublisher
    {
        public event EventHandler<TestEventArgs> MessageRead;
        
        public void ReadMessage(string message)
        {
            CallMessageRead(new TestEventArgs(message));
        }
        protected virtual void CallMessageRead(TestEventArgs e)
        {
            EventHandler<TestEventArgs> handler = MessageRead;

            if (handler != null)
            {
                e.Message += " (This is where the event args can be handled.)";
                handler(this, e);
            }

        }
    }

    public class TestSubscriber
    {
        private string _id;
        public TestSubscriber(string id, TestPublisher publisher)
        {
            _id = id;
            publisher.MessageRead += OnMessageRead;
        }

        public void OnMessageRead(object caller, TestEventArgs e)
        {
            Console.WriteLine($"{_id}: read message: {e.Message}");
        }
    }


    public class EventTests
    {
        public static void TestEvent(string message)
        {
            TestPublisher pub = new TestPublisher();
            TestSubscriber sub0 = new TestSubscriber("0", pub);
            TestSubscriber sub1 = new TestSubscriber("1", pub);

            pub.ReadMessage(message);
            Console.WriteLine("Event test complete.");
        }
    }
}
